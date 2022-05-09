using UnityEngine;
using ARWindow.PlayerManagement;
using ARWindow.Configuration.WindowConfigurationManagement;
using Injecter;

namespace ARWindow.Core
{
    public class Core : MonoBehaviour
    {
        [SerializeField, InterfaceType(typeof(IFaceDataProvider))] private MonoBehaviour faceDataProvider;
        [SerializeField] private Camera renderCamera;
        [SerializeField] private Transform windowCenter;

        [Inject] private readonly WindowConfiguration windowConfiguration;

        private IFaceDataProvider FaceDataProvider => faceDataProvider as IFaceDataProvider;

        private void Start()
        {
            renderCamera.aspect = windowConfiguration.Width / windowConfiguration.Height;
        }

        private void Update()
        {
            var eyePosition = FaceDataProvider.GetFacePosition();
            var centre = windowCenter.position;

            renderCamera.transform.position = centre + eyePosition;
            renderCamera.transform.LookAt(windowCenter);

            var left = centre;
            left.x -= windowConfiguration.Width / 2f;
            var right = centre;
            right.x += windowConfiguration.Width / 2f;

            var vleft = (left - eyePosition).normalized;
            var vright = (right - eyePosition).normalized;

            Debug.DrawRay(eyePosition, vleft * 1000, Color.green);
            Debug.DrawRay(eyePosition, vright * 1000, Color.blue);

            renderCamera.fieldOfView = Vector3.Angle(vleft, vright);
        }
    }
}