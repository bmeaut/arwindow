using System.Drawing;
using UnityEngine;

namespace ARWindow.PlayerManagement
{
    public abstract class IFaceDataProvider : MonoBehaviour
    {
        public abstract Vector3 GetFacePosition();
        public abstract Rectangle GetFaceRect();
    }
}