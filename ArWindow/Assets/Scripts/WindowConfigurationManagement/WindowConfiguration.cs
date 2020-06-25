using System;
using UnityEngine;

namespace WindowConfigurationManagement
{
    public class WindowConfiguration
    {
        private static readonly Lazy<WindowConfiguration> lazy = new Lazy<WindowConfiguration>(() => new WindowConfiguration());
        public static WindowConfiguration Instance => lazy.Value;

        public float Width { get; }
        public float Height { get; }

        private float playerCameraAngleInDegree, playerCameraYPos, playerCameraXPos;
        private float targetCameraAngleInDegree, targetCameraYPos, targetCameraXPos;
        

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

        private WindowConfiguration()
        {
            playerCameraAngleInDegree = 90;
            playerCameraXPos = 0;
            playerCameraYPos = 5;
            Width = 20;
            Height = 15;
        }
    }
}
