using System.Drawing;

namespace ARWindow.ImageProcessing
{
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