using UnityEngine;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ARWindow.ImageCapture
{
    public abstract class IImageCapture : MonoBehaviour
    {
        public abstract Image<Bgr, byte> ImageFrame { get; }
    }
}