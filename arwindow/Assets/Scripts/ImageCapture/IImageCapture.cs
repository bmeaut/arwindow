using Emgu.CV;
using Emgu.CV.Structure;

namespace ARWindow.ImageCapture
{
    public interface IImageCapture
    {
        Image<Bgr, byte> ImageFrame { get; }
    }
}