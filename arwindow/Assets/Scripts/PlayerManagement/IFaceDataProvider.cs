using System.Drawing;
using UnityEngine;

namespace ARWindow.PlayerManagement
{
    public interface IFaceDataProvider
    {
        Vector3 GetFacePosition();
        Rectangle GetFaceRect();
    }
}