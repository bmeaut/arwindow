using ARWindow.Configuration.WindowConfigurationManagement;
using Assets.Scripts.PlayerManagement;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Injecter;
using UnityEngine;

namespace Assets.Scripts.ObjectMovement
{
    class FaceLandmarkDetectionMovement : MonoBehaviour
    {
        Rigidbody rb;

        [SerializeField, InterfaceType(typeof(IFaceLandmarkProvider))] private MonoBehaviour faceLandmarkProvider;
        [Inject] private readonly WindowConfiguration windowConfiguration;
        private IFaceLandmarkProvider FaceLandmarkProvider => faceLandmarkProvider as IFaceLandmarkProvider;

        private Vector3 destination;
        private Vector3 startPos;
        private VectorOfPoint3D32F face3D;
        private Matrix<double> cameraMat, distCoeffs;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            var focalLengthMm = Camera.main.focalLength;
            cameraMat = new Matrix<double>(3, 3)
            {
                [0, 0] = focalLengthMm,
                [0, 1] = 0,
                [0, 2] = 640 / 2,
                [1, 0] = 0,
                [1, 1] = focalLengthMm,
                [1, 2] = 360 / 2,
                [2, 0] = 1,
                [2, 1] = 0,
                [2, 2] = 1
            };
            //cameraMat = Mat.Zeros(3, 3, Emgu.CV.CvEnum.DepthType.Cv64F, 1);
            //distCoeffs = Mat.Zeros(4, 1, Emgu.CV.CvEnum.DepthType.Cv64F, 1);
            distCoeffs = new Matrix<double>(4, 1)
            {
                [0, 0] = 0,
                [1, 0] = 0,
                [2, 0] = 0,
                [3, 0] = 0
            };
            startPos = transform.position;
            face3D = new VectorOfPoint3D32F(new MCvPoint3D32f[] {
                new MCvPoint3D32f(0, 0, 0),             // Nose
                new MCvPoint3D32f(0, -330, -65),        // Chin
                new MCvPoint3D32f(-225, 170, -135),     // Left eye corner
                new MCvPoint3D32f(225, 170, 135),       // Right eye corner
                new MCvPoint3D32f(-150, -150, -125),    // Left mouth corner
                new MCvPoint3D32f(150, -150, -125)      // Right mouth corner
            });
        }

        void Update()
        {
            var landmarks = FaceLandmarkProvider.GetLandmarks()[0];
            using var importantLandmarks = new VectorOfPointF(new System.Drawing.PointF[] {
                landmarks[30],
                landmarks[8],
                landmarks[36],
                landmarks[45],
                landmarks[48],
                landmarks[54]
            });
            using var rotMat = new Mat();
            using var transMat = new Mat();
            using var rotVec = new Matrix<double>(3, 1);
            using var transVec = new Matrix<double>(3, 1);
            CvInvoke.SolvePnP(face3D, importantLandmarks, cameraMat, distCoeffs, rotMat, transMat);
            rotMat.ConvertTo(rotVec, Emgu.CV.CvEnum.DepthType.Cv64F);
            transMat.ConvertTo(transVec, Emgu.CV.CvEnum.DepthType.Cv64F);
            rb.position = new Vector3(0, 0, 5);
            rb.rotation = Quaternion.Euler((float)rotVec[0, 0], (float)rotVec[1, 0], (float)rotVec[2, 0]);
        }
    }
}
