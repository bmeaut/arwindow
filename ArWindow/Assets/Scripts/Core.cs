using PlayerManagement;
using UnityEngine;
using WindowConfigurationManagement;

public class Core : MonoBehaviour
{
    [SerializeField] private IPlayerManager playerManager;
    [SerializeField] private Camera renderCamera;
    [SerializeField] private Transform windowCenter;

    private void Start()
    {
        renderCamera.aspect = WindowConfiguration.Instance.Width / WindowConfiguration.Instance.Height;
    }

    private void Update()
    {
        var playerData = playerManager.GetPlayerData();
        var centre = windowCenter.position;

        renderCamera.transform.position = centre + playerData.EyePosition;
        renderCamera.transform.LookAt(windowCenter);

        var left = centre;
        left.x -= WindowConfiguration.Instance.Width / 2f;
        var right = centre;
        right.x += WindowConfiguration.Instance.Width / 2f;

        var vleft = (left - playerData.EyePosition).normalized;
        var vright = (right - playerData.EyePosition).normalized;
        renderCamera.fieldOfView = Mathf.Acos(Vector3.Dot(vleft, vright)) / Mathf.PI * 180f;

    }
}
