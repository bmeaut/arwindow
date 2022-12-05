using ARWindow.PlayerManagement;
using Emgu.CV.Util;
using System.Drawing;
using UnityEngine;

namespace Assets.Scripts.PlayerManagement
{
    public interface IFaceLandmarkProvider
    {
        VectorOfVectorOfPointF GetLandmarks();
        Vector3 GetFacePosition();
        Rectangle GetFaceRect();
    }
}
