using UnityEngine;
using Configuration.WindowConfigurationManagement;
using ImageProcessing;
using Injecter;

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
        public FaceDetection faceDetection;

        [Inject] private readonly WindowConfiguration windowConfiguration;

        private ConfigurationManager(){ }

        // Start is called before the first frame update
        void Start()
        {
            windowConfiguration.UpdateConfiguration();

            // We _could_ call this from FaceDetection's Start(), but it's here just for consistency
            faceDetection.UpdateConfiguration();
            //...
        }
    }
}
