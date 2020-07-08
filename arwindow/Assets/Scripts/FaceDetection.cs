using UnityEngine;
using UnityEngine.UI;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using PlayerManagement;
using WindowConfigurationManagement;
using System.Threading;

namespace ImageProcessing
{
    public class FaceDetection : IPlayerManager
    {
        #region Private fields
        [SerializeField] private RawImage imageBox = null;

        [SerializeField, Tooltip("Should face recognition use webcamera or prerecorded video footage?")]
        private bool useCamera = true;
        [SerializeField, Tooltip("If video footage is used for face recognition, specify its access path.")]
        private string videoPath = "";

        private static readonly string CASCADE_PATH = @"Assets/Plugins/EmguCV/haarcascade_frontalface_default.xml";

        private VideoCapture capture;
        private CascadeClassifier cc;

        private Size imageSize;
        private const float z_dist = 5.0f; //Placeholder until we get actual depth data
        private PointF faceRectCenter = new PointF(0, 0);
        private Vector3 facePos => RemapToCameraCoords(faceRectCenter);
        private PlayerData playerData = new PlayerData { EyePosition = Vector3.zero };
        #endregion

        // OnEnable is called just after the object is enabled
        void OnEnable()
        {
            capture = useCamera ? new VideoCapture() : new VideoCapture(videoPath);
            cc = new CascadeClassifier(CASCADE_PATH);
        }

        // Update is called once per frame
        void Update()
        {
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

            playerData.EyePosition = facePos;
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
                     20);
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
            cameraCoords.y = WindowConfiguration.Instance.Height * ((point.Y / imageSize.Height) - 0.5f);

            return cameraCoords;
        }

        // Release resources when this object is not in use
        void OnDisable()
        {
            cc.Dispose();
            capture.Dispose();
        }
    }
}