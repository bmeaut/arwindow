using System.Drawing;
using UnityEngine;

namespace ARWindow.PlayerManagement
{
    public class StaticFaceProvider : MonoBehaviour, IFaceDataProvider
    {
        public Vector3 GetFacePosition() => transform.position;

        public Rectangle GetFaceRect() => default;
    }
}


