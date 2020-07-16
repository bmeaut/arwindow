using PlayerManagement;
using UnityEngine;
using Configuration.WindowConfigurationManagement;
using Injecter;

public class Core : MonoBehaviour
{
    [SerializeField] private IPlayerManager playerManager;
    [SerializeField] private Camera renderCamera;
    [SerializeField] private Transform windowCenter;

    [Inject] private readonly WindowConfiguration windowConfiguration;

    private void Start()
    {
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

    void OnDrawGizmos()
    {
        // Visualize window borders in Unity editor
        var window = windowConfiguration;
        Gizmos.DrawWireCube(windowCenter.position, new Vector3(window.Width, window.Height, 0.01f));
    }
}
