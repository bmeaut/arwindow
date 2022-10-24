using ARWindow.Configuration.WindowConfigurationManagement;
using ARWindow.PlayerManagement;
using Injecter;
using UnityEngine;

namespace ARWindow.ARObjects
{
    public class FaceDetectionMovement : MonoBehaviour
    {
        Rigidbody rb;

        [Tooltip("Enables or disables smoothened (kinematic) movement.\nCan result in smoother tracking with an appropriate \"speed\" value, but can cause object to lag behind.")]
        public bool smooth = false;
        [Tooltip("How fast should the object move towards the detected face.\nOnly used if \"smooth\" is set to true. Values higher than ~300.0 can lead to instability.")]
        public float speed = 250.0f;

        [SerializeField, InterfaceType(typeof(IFaceDataProvider))] private MonoBehaviour faceDataProvider;
        [Inject] private readonly WindowConfiguration windowConfiguration;
        private IFaceDataProvider FaceDataProvider => faceDataProvider as IFaceDataProvider;

        private float focalLengthMm;

        private Vector3 destination;
        private Vector3 startPos;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            focalLengthMm = Camera.main.focalLength;
            startPos = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if (smooth)
                rb.velocity /= 10f;

            var pos = FaceDataProvider.GetFacePosition();
            var screenHeight = FaceDataProvider.GetFaceRect().Height;
            if (screenHeight != default)
            {
                float depth = EstimateDepth(screenHeight);
                float halfwidth, halfheight;
                halfwidth = windowConfiguration.Width / 2f;
                halfheight = windowConfiguration.Height / 2f;
                destination = new Vector3()
                {
                    x = pos.x * (2 * depth + halfwidth) / halfwidth * -1f, //TODO: Why is this (* -1.0) mirroring necessary?
                    y = pos.y * (2 * depth + halfheight) / halfheight,
                    z = depth * -1,
                };
            }
        }

        void FixedUpdate()
        {
            if (smooth)
                rb.AddForce((destination - transform.position).normalized * speed);
            else
                rb.position = destination;
        }

        private float EstimateDepth(float screenHeightPx)
        {
            const float faceHeightMm = 150.0f;                              // TODO: Get face height from config
            return focalLengthMm * faceHeightMm * 0.1f / screenHeightPx;    // TODO: Find out why this has to be multiplied by 0.1 to get realistic values
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            float mod = 0.2f * -destination.z + 1f;
            var box = new Vector3()
            {
                x = windowConfiguration?.Width * mod ?? 20f,
                y = windowConfiguration?.Height * mod ?? 10f,
                z = 0.5f
            };
            Gizmos.DrawWireCube(new Vector3(startPos.x, startPos.y, destination.z), box);
        }
    }
}