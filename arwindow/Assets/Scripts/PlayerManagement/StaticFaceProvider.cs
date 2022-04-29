using System.Drawing;
using UnityEngine;

namespace ARWindow.PlayerManagement
{
    public class StaticFaceProvider : IFaceDataProvider
    {
        public override Vector3 GetFacePosition() => transform.position;

        public override Rectangle GetFaceRect() => default;
    }
}


