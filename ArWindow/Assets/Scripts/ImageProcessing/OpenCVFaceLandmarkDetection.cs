using ARWindow.ImageCapture;
using ARWindow.PlayerManagement;
using Assets.Scripts.PlayerManagement;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;
using UnityEngine;

namespace Assets.Scripts.ImageProcessing
{
    public class OpenCVFaceLandmarkDetection : MonoBehaviour, IFaceLandmarkProvider
    {
        [SerializeField, InterfaceType(typeof(IFaceDataProvider))] private MonoBehaviour faceDataProvider;
        [SerializeField, InterfaceType(typeof(IImageCapture))] private MonoBehaviour _imageCapture;
        private IFaceDataProvider FaceDataProvider => faceDataProvider as IFaceDataProvider;
        private IImageCapture ImageCapture => _imageCapture as IImageCapture;


        private static readonly string LBF_MODEL_PATH = @"Assets/Resources/lbfmodel.yaml";
        private FacemarkLBF _facemark;

        public Vector3 GetFacePosition() => FaceDataProvider?.GetFacePosition() ?? new Vector3();
        public Rectangle GetFaceRect() => FaceDataProvider?.GetFaceRect() ?? new Rectangle();

        public void Awake()
        {
            _facemark = new FacemarkLBF(new FacemarkLBFParams());
            _facemark.LoadModel(LBF_MODEL_PATH);
        }

        public VectorOfVectorOfPointF GetLandmarks()
        {
            using var img = ImageCapture.ImageFrame?.Clone();
            var landmarks = new VectorOfVectorOfPointF();
            if (img != null)
            {
                var gray = img.Convert<Gray, byte>();
                gray._EqualizeHist();
                var facePos = new VectorOfRect(new Rectangle[] { FaceDataProvider.GetFaceRect() });
                _facemark.Fit(gray, facePos, landmarks);
            }
            return landmarks;
        }
    }
}
