using UnityEngine;
using UnityEngine.UI;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using PlayerManagement;
using Configuration.WindowConfigurationManagement;
using System.Threading;
using Serialization;

namespace ImageProcessing
{
    public class FaceDetection : IPlayerManager
    {
        #region Private fields
        [SerializeField] private RawImage imageBox = null;

        [SerializeField, Tooltip("Should face recognition use webcamera or prerecorded video footage? (Video path is specified in LocalSettings.json)")]
        private bool useCamera = true;
        private string videoPath = "";
        private const string CONFIG_PATH = "Assets/Config/LocalSettings.json";

        private static readonly string CASCADE_PATH = @"Assets/Plugins/EmguCV/haarcascade_frontalface_default.xml";

        private VideoCapture capture;
        private CascadeClassifier cc;

        private Size imageSize;
        private const float z_dist = 5.0f; //Placeholder until we get actual depth data
        private PointF faceRectCenter = new PointF(0, 0);
        private Vector3 FacePos => RemapToCameraCoords(faceRectCenter);
        private PlayerData playerData = new PlayerData { EyePosition = new Vector3(0,0,5) };
        #endregion

        // OnEnable is called just after the object is enabled
        void OnEnable()
        {
            if (useCamera)
            {
                capture = new VideoCapture();
            }
            else if (videoPath != "")
            {
                capture = new VideoCapture(videoPath);
            }
            else return;

            cc = new CascadeClassifier(CASCADE_PATH);
        }

        // Update is called once per frame
        void Update()
        {
            if (capture == null || cc == null) return;

            using (Image<Bgr, byte> img = capture.QueryFrame().ToImage<Bgr, byte>())
            {
                imageSize = img.Size;

                var faces = cc.DetectMultiScale(img, 1.1, 10);
                foreach (Rectangle face in faces)
                {
                    faceRectCenter = GetRectCenter(face);
                    DrawFaceMarkers(img, face);
                }

                Texture2D texture = new Texture2D(img.Width, img.Height);
                texture.LoadImage(img.ToJpegData());

                if (imageBox != null && imageBox.isActiveAndEnabled)
                {
                    imageBox.texture = texture;
                }
            }

            if (!useCamera)
            {
                Thread.Sleep((int)(1000 / capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps)));
                if (capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.PosFrames) >= capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameCount))
                {
                    capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.PosFrames, 0);
                }
            }

            playerData.EyePosition = FacePos;
        }

        public override PlayerData GetPlayerData()
        {
            return playerData;
        }

        private void DrawFaceMarkers(Image<Bgr, byte> img, Rectangle face)
        {
            img.Draw(face, new Bgr(System.Drawing.Color.Red), 4);
            img.Draw(new Rectangle((int)faceRectCenter.X, (int)faceRectCenter.Y, 1, 1),
                     new Bgr(System.Drawing.Color.Magenta),
                     5);
        }

        private static PointF GetRectCenter(Rectangle rect)
        {
            return new PointF(rect.Left + rect.Width / 2.0f,
                             rect.Top + rect.Height / 2.0f);
        }

        private Vector3 RemapToCameraCoords(PointF point)
        {
            var cameraCoords = new Vector3(0, 0, z_dist);
            cameraCoords.x = WindowConfiguration.Instance.Width * ((point.X / imageSize.Width) - 0.5f);
            cameraCoords.y = -1.0f * WindowConfiguration.Instance.Height * ((point.Y / imageSize.Height) - 0.5f);

            return cameraCoords;
        }

        public void UpdateConfiguration()
        {
            this.enabled = false;

            var config = ConfigSerializer.ReadJsonFile(CONFIG_PATH);

            videoPath = config.Value<string>("videoPath");

            //Call OnEnabled() so that the VideoCapture initializes properly
            this.enabled = true;
        }

        // Release resources when this object is not in use
        void OnDisable()
        {
            if (cc != null) cc.Dispose();
            if (capture != null) capture.Dispose();
        }
    }
}