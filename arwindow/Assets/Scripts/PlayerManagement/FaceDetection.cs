using UnityEngine;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using ARWindow.Configuration.WindowConfigurationManagement;
using ARWindow.ImageCapture;
using ARWindow.PlayerManagement;

namespace ARWindow.ImageProcessing
{
    public class FaceDetection : IPlayerManager
    {
        #region Properties and private fields
        [SerializeField] private WindowConfiguration window;
        [SerializeField] private IImageCapture imageCapture;

        private static readonly string CASCADE_PATH = @"Assets/Resources/haarcascade_frontalface_default.xml";

        private CascadeClassifier cc;

        private Size imageSize;
        private const float z_dist = 5.0f; //Placeholder until we get actual depth data
        private PointF faceRectCenter = new PointF(0, 0);
        private Vector3 FacePos => RemapToCameraCoords(faceRectCenter);
        private PlayerData playerData = new PlayerData { EyePosition = new Vector3(0, 0, 5) };
        public Rectangle detectedFace { get; private set; }
        #endregion

        void Awake()
        {
            if (window == null) window = FindObjectOfType<WindowConfiguration>();
        }

        // OnEnable is called just after the object is enabled
        void OnEnable()
        {
            cc = new CascadeClassifier(CASCADE_PATH);
        }

        // Update is called once per frame
        void Update()
        {
            if (cc == null) return;

            using (Image<Bgr, byte> img = imageCapture.ImageFrame)
            {
                if (img == null) return;

                imageSize = img.Size;

                var faces = cc.DetectMultiScale(img, 1.1, 10);
                if (faces.Length == 0)
                {
                    detectedFace = default;
                    return;
                }
                detectedFace = faces[0];
            }

            faceRectCenter = GetRectCenter(detectedFace);
            playerData.EyePosition = FacePos;
        }

        public override PlayerData GetPlayerData()
        {
            return playerData;
        }

        private static PointF GetRectCenter(Rectangle rect)
        {
            return new PointF(rect.Left + rect.Width / 2.0f,
                             rect.Top + rect.Height / 2.0f);
        }

        private Vector3 RemapToCameraCoords(PointF point)
        {
            var cameraCoords = new Vector3(0, 0, z_dist);
            cameraCoords.x = window.Width * ((point.X / imageSize.Width) - 0.5f);
            cameraCoords.y = -1.0f * window.Height * ((point.Y / imageSize.Height) - 0.5f);

            return cameraCoords;
        }

        // Release resources when this object is not in use
        void OnDisable()
        {
            if (cc != null)
            {
                cc.Dispose();
                cc = null;
            }
        }
    }
}