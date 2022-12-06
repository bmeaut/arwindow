using ARWindow.Configuration.WindowConfigurationManagement;
using ARWindow.ImageCapture;
using ARWindow.PlayerManagement;
using Emgu.CV;
using Emgu.CV.Structure;
using Injecter;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UltraFaceDotNet;
using UnityEngine;

namespace ARWindow.ImageProcessing
{
    public class UltraFaceDetection : MonoBehaviour, IFaceDataProvider
    {
        [Inject] private WindowConfiguration window;
        [SerializeField, InterfaceType(typeof(IImageCapture))] private MonoBehaviour _imageCapture;
        [SerializeField] private float _confidenceThreshold = 0.7f;
        private const float z_dist = 5.0f; //Placeholder until we get actual depth data
        private UltraFace _ultra;
        private static readonly string BIN_PATH = @"Assets/Resources/RFB-320.bin";
        private static readonly string PARAM_PATH = @"Assets/Resources/RFB-320.param";
        private IImageCapture ImageCapture => _imageCapture as IImageCapture;

        private RectangleF _detectedFace;
        private Vector3 _facePosition = new Vector3(0, 0, z_dist);

        public Vector3 GetFacePosition() => _facePosition;
        public Rectangle GetFaceRect() => Rectangle.Round(_detectedFace);

        private void Awake()
        {
            _ultra = UltraFace.Create(new UltraFaceParameter
            {
                BinFilePath = BIN_PATH,
                ParamFilePath = PARAM_PATH,
                InputLength = 240,
                InputWidth = 320,
                ScoreThreshold = _confidenceThreshold,
                TopK = 1 //one head
            });
        }

        // Update is called once per frame
        void Update()
        {
            using (Image<Bgr, byte> img = ImageCapture.ImageFrame?.Clone())
            {
                if (img == null) return;

                var faces = Detect(img.Mat);

                if (faces.Count() == 0)
                {
                    _detectedFace = default;
                    return;
                }

                _detectedFace = faces.OrderByDescending(f => f.confidence).First().rect;
                _facePosition = RemapToCameraCoords(GetRectCenter(_detectedFace), img.Size);
            }
        }

        public IEnumerable<DetectedFace> Detect(Mat matBGR)
        {
            var ultraimg = NcnnDotNet.Mat.FromPixelsResize(matBGR.DataPointer, NcnnDotNet.PixelType.Bgr2Rgb, matBGR.Cols, matBGR.Rows, 320, 240);
            var result = _ultra.Detect(ultraimg);
            List<DetectedFace> faces = new List<DetectedFace>();
            foreach (var face in result)
            {
                //resize rectangle to original resolution
                float x1 = face.X1 / 320 * matBGR.Width;
                float y1 = face.Y1 / 240 * matBGR.Height;
                float x2 = face.X2 / 320 * matBGR.Width;
                float y2 = face.Y2 / 240 * matBGR.Height;
                faces.Add(new DetectedFace(face.Score, new RectangleF(x1, y1, x2 - x1, y2 - y1)));
            }
            return faces;
        }

        private static PointF GetRectCenter(RectangleF rect)
        {
            return new PointF(rect.Left + rect.Width / 2.0f,
                             rect.Top + rect.Height / 2.0f);
        }

        private Vector3 RemapToCameraCoords(PointF point, Size imageSize)
        {
            var cameraCoords = new Vector3(0, 0, z_dist);
            cameraCoords.x = window.Width * ((point.X / imageSize.Width) - 0.5f);
            cameraCoords.y = -1.0f * window.Height * ((point.Y / imageSize.Height) - 0.5f);

            return cameraCoords;
        }
    }
}