using Serialization;
using System;
using UnityEngine;
using Newtonsoft.Json;

namespace Configuration.WindowConfigurationManagement
{
    public class WindowConfiguration
    {
        private const string WINDOW_CONFIG_PATH = "Assets/Config/WindowConfiguration.json";

        public float Width { get; private set; } = 20;
        public float Height { get; private set; } = 15;

        [JsonProperty] private float playerCameraAngleInDegree = 90;
        [JsonProperty] private float playerCameraXPos = 0;
        [JsonProperty] private float playerCameraYPos = 5;
        private float targetCameraAngleInDegree, targetCameraYPos, targetCameraXPos;

        private WindowConfiguration() { }


        public Vector3 PlayerCameraPointToWindowCenteredPoint(Vector3 point)
        {
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

        public void UpdateConfiguration()
        {
            var config = ConfigSerializer.ReadJsonFile(WINDOW_CONFIG_PATH);

            playerCameraAngleInDegree = config.Value<float>("playerCameraAngleInDegree");
            playerCameraXPos = config.Value<float>("playerCameraXPos");
            playerCameraYPos = config.Value<float>("playerCameraYPos");
            Width = config.Value<float>("Width");
            Height = config.Value<float>("Height");
        }
    }
}
