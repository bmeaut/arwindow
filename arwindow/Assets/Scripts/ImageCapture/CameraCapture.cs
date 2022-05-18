using ARWindow.Serialization;
using Emgu.CV;
using Emgu.CV.Structure;
using UnityEngine;

namespace ARWindow.ImageCapture
{
    public class CameraCapture : MonoBehaviour, IImageCapture
    {
        private const string CONFIG_PATH = "Assets/Config/LocalSettings.json";
        private int cameraId = 1;
        private VideoCapture capture;

        public Image<Bgr, byte> ImageFrame => capture?.QueryFrame().ToImage<Bgr, byte>();

        private void Awake()
        {
            var config = ConfigSerializer.ReadJsonFile(CONFIG_PATH);
            cameraId = config.Value<int>("cameraId");
        }

        private void OnEnable()
        {
            capture = new VideoCapture(cameraId);
        }

        private void OnDisable()
        {
            if (capture != null)
            {
                capture.Dispose();
                capture = null;
            }
        }
    }
}