using Emgu.CV;
using Emgu.CV.Structure;
using ImageCapture;
using ImageProcessing;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Debug
{
    public class FaceDetectionDebugImage : MonoBehaviour
    {
        [SerializeField] private IImageCapture imageCapture;
        [SerializeField] private FaceDetection faceDetection; //TODO: erre valami generikusabb megoldást találni
        [SerializeField] private RawImage imageBox;
        //debug texture to display detected face rects in the corner
        Texture2D texture;

        private void Awake()
        {
            if (imageBox == null) imageBox = FindObjectOfType<RawImage>();
        }

        // Update is called once per frame
        void Update()
        {

            using (Image<Bgr, byte> img = imageCapture.ImageFrame?.Clone())
            {
                if (img == null) return;

                if (faceDetection.detectedFace != default)
                    DrawFaceMarkers(img, faceDetection.detectedFace);

                if (texture is null)
                    texture = new Texture2D(img.Width, img.Height);
                texture.LoadImage(img.ToJpegData());

                if (imageBox != null && imageBox.isActiveAndEnabled)
                {
                    imageBox.texture = texture;
                }
            }
        }

        private void DrawFaceMarkers(Image<Bgr, byte> img, Rectangle face)
        {
            img.Draw(face, new Bgr(System.Drawing.Color.Red), 4);
            img.Draw(new Rectangle((int)(face.Left + face.Width / 2.0f), (int)(face.Top + face.Height / 2.0f), 1, 1),
                     new Bgr(System.Drawing.Color.Magenta),
                     3);
        }
    }
}