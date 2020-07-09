using UnityEngine;
using Configuration.WindowConfigurationManagement;

namespace Configuration
{
    /// <summary>
    /// Handles the loading of configuration files for several components of the application.
    /// TODO: this class should probably not be aware of the components it loads config for,
    /// but those components may not access this class before runtime so events are suboptimal 
    /// -- Dependency Injection?
    /// </summary>
    public class ConfigurationManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            WindowConfiguration.Instance.UpdateConfiguration();
            //Todo: video path, ...
        }
    }
}
