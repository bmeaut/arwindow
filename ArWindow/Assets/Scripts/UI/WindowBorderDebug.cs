using ARWindow.Configuration.WindowConfigurationManagement;
using Injecter;
using UnityEngine;

namespace ARWindow.UI.Debug
{
    public class WindowBorderDebug : MonoBehaviour
    {
        [Inject] private readonly WindowConfiguration _windowConfiguration;
        [SerializeField] private float _defaultWidth = 20;
        [SerializeField] private float _defaultHeight = 15;

        private float Width => _windowConfiguration?.Width ?? _defaultWidth;
        private float Height => _windowConfiguration?.Height ?? _defaultHeight;

        void OnDrawGizmos()
        {
            // Visualize window borders in Unity editor
            Gizmos.DrawWireCube(transform.position, new Vector3(Width, Height, 0.01f));
        }
    }
}