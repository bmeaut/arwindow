using PlayerManagement;
using UnityEngine;
using Configuration.WindowConfigurationManagement;

public class Core : MonoBehaviour
{
    [SerializeField, InterfaceType(typeof(IPlayerManager))] private MonoBehaviour playerManager;
    [SerializeField] private Camera renderCamera;
    [SerializeField] private Transform windowCenter;

    private WindowConfiguration windowConfiguration;
    private IPlayerManager PlayerManager => playerManager as IPlayerManager;

    private void Start()
    {
        windowConfiguration = windowCenter.GetComponent<WindowConfiguration>();
        renderCamera.aspect = windowConfiguration.Width / windowConfiguration.Height;
    }

    private void Update()
    {
        var playerData = PlayerManager.GetPlayerData();
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
