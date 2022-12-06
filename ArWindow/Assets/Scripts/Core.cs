using ARWindow.Configuration.WindowConfigurationManagement;
using ARWindow.PlayerManagement;
using Injecter;
using UnityEngine;

namespace ARWindow.Core
{
    public class Core : MonoBehaviour
    {
        [SerializeField, InterfaceType(typeof(IFaceDataProvider))] private MonoBehaviour faceDataProvider;
        [SerializeField] private Camera renderCamera;
        [SerializeField] private Transform windowCenter;
        [SerializeField] private bool newAlgorithm = true;

        [Inject] private readonly WindowConfiguration windowConfiguration;

        // Screen corners, origo is window center
        private Vector3 pa = Vector3.zero;
        private Vector3 pb = Vector3.zero;
        private Vector3 pc = Vector3.zero;

        // Orthonormal basis for the screen
        private Vector3 vr = Vector3.zero;
        private Vector3 vu = Vector3.zero;
        private Vector3 vn = Vector3.zero;

        private IFaceDataProvider FaceDataProvider => faceDataProvider as IFaceDataProvider;

        private void Start()
        {
            renderCamera.aspect = windowConfiguration.Width / windowConfiguration.Height;

            float windowLeft = -windowConfiguration.Width / 2.0f;
            float windowRight = windowConfiguration.Width / 2.0f;
            float windowTop = windowConfiguration.Height / 2.0f;
            float windowBottom = -windowConfiguration.Height / 2.0f;

            // Screen corners, origo is window center
            pa = new Vector3(windowLeft, windowBottom, 0.0f);
            pb = new Vector3(windowRight, windowBottom, 0.0f);
            pc = new Vector3(windowLeft, windowTop, 0.0f);

            //orthonormal basis for the screen
            vr = Vector3.Normalize(pb - pa); // right
            vu = Vector3.Normalize(pc - pa); // up
            vn = Vector3.Normalize(Vector3.Cross(vr, vu)); // screen normal
        }

        private void Update()
        {
            var eyePosition = FaceDataProvider.GetFacePosition();

            // At the beginning for some milliseconds, unity won't get the kinect capture and gives errors,
            // so we check, that when eyeposition is zero vector, we don't do anything (kinect not initalized).
            if (eyePosition == Vector3.zero)
                return;

            // levetitett frustrum merete, nearPlane legyen a leheto legnagyobb, farPlane a leheto legkisebb!
            float nearPlane = 0.3f;
            float farPlane = 1000.0f;

            // Screen corner vectors (from eye to corners)
            Vector3 va = pa - eyePosition;
            Vector3 vb = pb - eyePosition;
            Vector3 vc = pc - eyePosition;

            //distance from eye to screen-space origin
            float d = -Vector3.Dot(vn, va); //assert d is positive

            float left = Vector3.Dot(vr, va) * nearPlane / d;
            float right = Vector3.Dot(vr, vb) * nearPlane / d;
            float bottom = Vector3.Dot(vu, va) * nearPlane / d;
            float top = Vector3.Dot(vu, vc) * nearPlane / d;

            if (newAlgorithm)
            {
                Matrix4x4 P = Matrix4x4.Frustum(left, right, bottom, top, nearPlane, farPlane);
                Matrix4x4 T = Matrix4x4.Translate(-eyePosition);
                //Matrix4x4 M = CreateMMatrix(vr, vu, vn);
                //Matrix4x4 R = Matrix4x4.Rotate(Quaternion.Inverse(transform.rotation) * windowCenter.transform.rotation);

                // Originally: M * R * T. But in our system, we don't need the rotation matrix and the M basis change matrix is an identity matrix.
                // WorldToCameraMatrix corrects shadow calculations as well, so we had to change the directional light to point to the subject
                renderCamera.worldToCameraMatrix = T;

                renderCamera.projectionMatrix = P;
            }
            else
            {
                // TODO: test, masik projektben ezt hasznaltuk de ez most nincs: Matrix4x4.CreatePerspectiveOffCenter(left, right, bottom, top, nearPlane, farPlane)
                var P = PerspectiveOffCenter(left, right, bottom, top, nearPlane, farPlane);
                //var P = Matrix4x4.Frustum(left, right, bottom, top, nearPlane, farPlane);
                //var viewMatrix = Matrix4x4.LookAt(eyePosition, eyePosition - vn, vu); // mintha a user merőlegesen nézne a screenre

                renderCamera.transform.position = windowCenter.position + eyePosition;
                renderCamera.transform.LookAt(windowCenter);
                //renderCamera.worldToCameraMatrix = viewMatrix; // TODO: eyyel valamiert nem mukodott
                renderCamera.projectionMatrix = P;//eredeti: P*M*T
            }
        }

        Matrix4x4 CreateMMatrix(Vector3 DirRight, Vector3 DirUp, Vector3 DirNormal)
        {
            Matrix4x4 m = Matrix4x4.zero;
            m[0, 0] = DirRight.x;
            m[0, 1] = DirRight.y;
            m[0, 2] = DirRight.z;

            m[1, 0] = DirUp.x;
            m[1, 1] = DirUp.y;
            m[1, 2] = DirUp.z;

            m[2, 0] = DirNormal.x;
            m[2, 1] = DirNormal.y;
            m[2, 2] = DirNormal.z;

            m[3, 3] = 1.0f;
            return m;
        }

        // https://medium.com/try-creative-tech/off-axis-projection-in-unity-1572d826541e
        // http://160592857366.free.fr/joe/ebooks/ShareData/Generalized%20Perspective%20Projection.pdf
        // TODO: We don't actually need this, because Matrix4x4.Frustrum function does exactly the same thing
        //left-handed, not right-handed as wpf but unity uses left so i guess its good :)
        /// <summary>
        /// Set an off-center projection, where perspective's vanishing
        /// point is not necessarily in the center of the screen.
        /// left/right/top/bottom define near plane size, i.e.
        /// how offset are corners of camera's near plane.
        /// Tweak the values and you can see camera's frustum change.
        /// </summary>
        Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
        {
            float x = (2.0f * near) / (right - left);
            float y = (2.0f * near) / (top - bottom);
            float a = (right + left) / (right - left);
            float b = (top + bottom) / (top - bottom);
            float c = -(far + near) / (far - near);
            float d = -(2.0f * far * near) / (far - near);
            float e = -1.0f;

            Matrix4x4 m = new Matrix4x4();
            m.m00 = x; m.m01 = 0; m.m02 = a; m.m03 = 0;
            m.m10 = 0; m.m11 = y; m.m12 = b; m.m13 = 0;
            m.m20 = 0; m.m21 = 0; m.m22 = c; m.m23 = d;
            m.m30 = 0; m.m31 = 0; m.m32 = e; m.m33 = 0;
            return m;
        }
    }
}