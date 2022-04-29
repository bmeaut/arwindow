using UnityEngine;
using ARWindow.PlayerManagement;
using ARWindow.Configuration.WindowConfigurationManagement;

namespace ARWindow.Core
{
    public class Core : MonoBehaviour
    {
        [SerializeField] private IFaceDataProvider faceDataProvider;
        [SerializeField] private Camera renderCamera;
        [SerializeField] private Transform windowCenter;

        private WindowConfiguration windowConfiguration;

        private void Start()
        {
            windowConfiguration = windowCenter.GetComponent<WindowConfiguration>();
            renderCamera.aspect = windowConfiguration.Width / windowConfiguration.Height;
        }

        private void Update()
        {
            var eyePosition = faceDataProvider.GetFacePosition();
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