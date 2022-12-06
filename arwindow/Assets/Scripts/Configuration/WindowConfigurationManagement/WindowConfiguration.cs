using System;
using UnityEngine;
using Newtonsoft.Json;
using ARWindow.Serialization;

namespace ARWindow.Configuration.WindowConfigurationManagement
{
    public class WindowConfiguration
    {
        private const string WINDOW_CONFIG_PATH = "Assets/Config/WindowConfiguration.json";
        public float Width { get; private set; } = 70.5f;
        public float Height { get; private set; } = 39.65f;

        [JsonProperty] public float playerCameraAngleInDegree = 90;
        [JsonProperty] public float playerCameraXPos = 0;
        [JsonProperty] public float playerCameraYPos = 5;
        [JsonProperty] public float windowAngleInDegree = 10.5f;
        [JsonProperty] public float kinectDistanceWindowTop = 14;
        private float targetCameraAngleInDegree, targetCameraYPos, targetCameraXPos;

        public WindowConfiguration()
        {
            UpdateConfiguration();
        }

        public Vector3 PlayerCameraPointToWindowCenteredPoint(Vector3 point)
        {
            point = TransformKinectCoordinatesToStraightKinectCoordinates(point);

            var angleInRadian = playerCameraAngleInDegree * Math.PI / 180.0;
            var a = point.y * Math.Cos(angleInRadian);
            var b = point.z * Math.Sin(angleInRadian);
            var c = b / Math.Tan(angleInRadian);
            var d = playerCameraYPos - c;
            var e = a * Math.Tan(angleInRadian);

            return new Vector3
            {
                x = point.x + playerCameraXPos,
                y = (float)(d + e),
                z = (float)(a + b)
            };
        }

        public Vector3 TargetCameraPointToWindowCenteredPoint(Vector3 point)
        {
            var angleInRadian = targetCameraAngleInDegree * Math.PI / 180.0;
            var a = point.y * Math.Cos(angleInRadian);
            var b = point.z * Math.Sin(angleInRadian);
            var c = b / Math.Tan(angleInRadian);
            var d = targetCameraYPos - c;
            var e = a * Math.Tan(angleInRadian);

            return new Vector3
            {
                x = -(point.x + targetCameraXPos),
                y = (float)(d + e),
                z = -(float)(a + b)
            };
        }

        private Vector3 TransformKinectCoordinatesToStraightKinectCoordinates(Vector3 pointInKinectCoordinate)
        {
            double alpha = windowAngleInDegree * Math.PI / 180.0;

            var v1 = new Vector3(0, Height, 0);
            var v2 = new Vector3(0, (float)Math.Cos(alpha) * Height, (float)Math.Sin(alpha) * Height);

            var converted = new Vector3();
            converted.x = pointInKinectCoordinate.x;
            converted.y = pointInKinectCoordinate.y * (float)Math.Cos(alpha) - pointInKinectCoordinate.z * (float)Math.Cos(Math.PI / 2 - alpha);
            converted.z = pointInKinectCoordinate.z * (float)Math.Cos(alpha) + pointInKinectCoordinate.y * (float)Math.Cos(Math.PI / 2 - alpha);

            return v2 - v1 + converted;
        }

        private void UpdateConfiguration()
        {
            var config = ConfigSerializer.ReadJsonFile(WINDOW_CONFIG_PATH);

            playerCameraAngleInDegree = config.Value<float>("playerCameraAngleInDegree");
            playerCameraXPos = config.Value<float>("playerCameraXPos");

            windowAngleInDegree = config.Value<float>("windowAngleInDegree");
            kinectDistanceWindowTop = config.Value<float>("kinectDistanceWindowTop");

            Width = config.Value<float>("Width");
            Height = config.Value<float>("Height");

            // This should be around 14 + 40 / 2 = 34 cm
            playerCameraYPos = kinectDistanceWindowTop + Height / 2.0f;
        }
    }
}
