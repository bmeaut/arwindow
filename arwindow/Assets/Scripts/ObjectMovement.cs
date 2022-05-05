using ARWindow.Configuration.WindowConfigurationManagement;
using Injecter;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ARWindow.ARObjects
{
    public class ObjectMovement : MonoBehaviour
    {
        public float speed = 1.0f;
        public float triggerDistance = 5f;

        Rigidbody rb;
        private Vector3 startPos;
        private Vector3 destination;

        public Vector2 boundsX;
        public Vector2 boundsY;
        public Vector2 boundsZ;

        public float maxRotationDegrees = 30f;
        public float rotationSpeed = 1f;

        [Inject] private readonly WindowConfiguration windowConfiguration;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();

            startPos = transform.position;
            float windowWidth = windowConfiguration.Width;
            float windowHeight = windowConfiguration.Height;
            boundsX = new Vector2(startPos.x - windowWidth, startPos.x + windowWidth);
            boundsY = new Vector2(startPos.y - windowHeight, startPos.y + windowHeight);
            boundsZ = new Vector2(startPos.z - 10, startPos.z + 10);

            FindNewDestination();
        }

        // Update is called once per frame
        void Update()
        {
            if(Vector3.Distance(transform.position, destination) <= triggerDistance)
            {
                FindNewDestination();
            }
        }

        void FixedUpdate()
        {
            rb.AddForce((destination - transform.position).normalized * speed);

            rb.MoveRotation(Quaternion.AngleAxis(Mathf.Sin(Time.time * rotationSpeed) * maxRotationDegrees, Vector3.up));
        }

        private void FindNewDestination()
        {
            rb.velocity /= 10;

            destination = new Vector3(Random.Range(boundsX.x, boundsX.y),
                                      Random.Range(boundsY.x, boundsY.y),
                                      Random.Range(boundsZ.x, boundsZ.y));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(startPos, new Vector3(boundsX.y - boundsX.x,
                                                      boundsY.y - boundsY.x,
                                                      boundsZ.y - boundsZ.x));
        }
    }
}