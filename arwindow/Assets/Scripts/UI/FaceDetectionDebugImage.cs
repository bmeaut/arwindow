using ARWindow.ImageCapture;
using ARWindow.PlayerManagement;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

namespace ARWindow.UI.Debug
{
    public class FaceDetectionDebugImage : MonoBehaviour
    {
        [SerializeField, InterfaceType(typeof(IImageCapture))] private MonoBehaviour imageCapture;
        [SerializeField, InterfaceType(typeof(IFaceDataProvider))] private MonoBehaviour faceDetection;
        [SerializeField] private RawImage imageBox;

        private IFaceDataProvider FaceDetection => faceDetection as IFaceDataProvider;
        private IImageCapture ImageCapture => imageCapture as IImageCapture;

        //debug texture to display detected face rects in the corner
        Texture2D texture;

        private void Awake()
        {
            if (imageBox == null) imageBox = FindObjectOfType<RawImage>();
        }

        // Update is called once per frame
        void Update()
        {

            using (Image<Bgr, byte> img = ImageCapture.ImageFrame?.Clone())
            {
                if (img == null) return;

                if (FaceDetection.GetFaceRect() != default)
                    DrawFaceMarkers(img, FaceDetection.GetFaceRect());

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