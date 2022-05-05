using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UltraFaceDotNet;
using Emgu.CV;
using Emgu.CV.Structure;
using ARWindow.PlayerManagement;
using ARWindow.ImageCapture;

namespace ARWindow.ImageProcessing
{
    public class UltraFaceDetection : IFaceDataProvider
    {
        [SerializeField] private IImageCapture _imageCapture;
        [SerializeField] private float _confidenceThreshold = 0.7f;
        private UltraFace _ultra;
        private static readonly string BIN_PATH = @"Assets/Resources/RFB-320.bin";
        private static readonly string PARAM_PATH = @"Assets/Resources/RFB-320.param";

        private RectangleF _detectedFace;

        public override Vector3 GetFacePosition()
        {
            return new Vector3(0, 0, 5); //TODO
        }

        public override Rectangle GetFaceRect() => Rectangle.Round(_detectedFace);

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
            using (Image<Bgr, byte> img = _imageCapture.ImageFrame?.Clone())
            {
                if (img == null) return;

                var faces = Detect(img.Mat);

                if (faces.Count() == 0)
                {
                    _detectedFace = default;
                    return;
                }

                _detectedFace = faces.OrderByDescending(f => f.confidence).First().rect;
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


        public struct DetectedFace
        {
            public float confidence;
            public RectangleF rect;

            public DetectedFace(float confidence, RectangleF rect)
            {
                this.confidence = confidence;
                this.rect = rect;
            }
        }
    }
}