using ARWindow.Serialization;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using UnityEngine;

namespace ARWindow.ImageCapture
{
    public class VideoFileCapture : MonoBehaviour, IImageCapture
    {
        private const string CONFIG_PATH = "Assets/Config/LocalSettings.json";
        private string videoPath = "";
        private VideoCapture capture;
        private int videoFrameCount; //Number of frames in video file
        private int videoCaptureFps;

        public Image<Bgr, byte> ImageFrame => _imgFrame;
        private Image<Bgr, byte> _imgFrame;

        private void Awake()
        {
            var config = ConfigSerializer.ReadJsonFile(CONFIG_PATH);
            videoPath = config.Value<string>("videoPath");
        }

        private void OnEnable()
        {
            if (!string.IsNullOrEmpty(videoPath))
            {
                capture = new VideoCapture(videoPath);
                videoFrameCount = (int)capture.GetCaptureProperty(CapProp.FrameCount);
                videoCaptureFps = (int)capture.GetCaptureProperty(CapProp.Fps);
            }
            else
            {
                Debug.LogWarning("Video file access path not specified!");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (capture == null) return;

            //seek to correct video frame
            var vframe = (Time.time * videoCaptureFps) % videoFrameCount;
            capture.SetCaptureProperty(CapProp.PosFrames, vframe);

            _imgFrame = capture.QueryFrame().ToImage<Bgr, byte>();
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