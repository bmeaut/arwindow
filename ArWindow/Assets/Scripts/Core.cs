using UnityEngine;
using ARWindow.PlayerManagement;
using ARWindow.Configuration.WindowConfigurationManagement;

namespace ARWindow.Core
{
    public class Core : MonoBehaviour
    {
        [SerializeField] private IPlayerManager playerManager;
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
            var playerData = playerManager.GetPlayerData();
            var centre = windowCenter.position;

            renderCamera.transform.position = centre + playerData.EyePosition;
            renderCamera.transform.LookAt(windowCenter);

            var left = centre;
            left.x -= windowConfiguration.Width / 2f;
            var right = centre;
            right.x += windowConfiguration.Width / 2f;

            var vleft = (left - playerData.EyePosition).normalized;
            var vright = (right - playerData.EyePosition).normalized;

            Debug.DrawRay(playerData.EyePosition, vleft * 1000, Color.green);
            Debug.DrawRay(playerData.EyePosition, vright * 1000, Color.blue);

            renderCamera.fieldOfView = Vector3.Angle(vleft, vright);
        }
    }
}