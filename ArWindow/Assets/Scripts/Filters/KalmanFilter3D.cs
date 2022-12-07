using UnityEngine;

namespace Assets.Scripts.Filters
{
    public struct KalmanFilter3D
    {
        private KalmanFilter1D fx, fy, fz;

        public KalmanFilter3D(double A, double H, double Q, double R, Vector3 initial_P, Vector3 initial_x)
        {
            fx = new KalmanFilter1D(A, H, Q, R, initial_P.x, initial_x.x);
            fy = new KalmanFilter1D(A, H, Q, R, initial_P.y, initial_x.y);
            fz = new KalmanFilter1D(A, H, Q, R, initial_P.z, initial_x.z);
        }

        public KalmanFilter3D(double A, double H, double Q, double R, double initial_P, double initial_x)
        {
            fx = new KalmanFilter1D(A, H, Q, R, initial_P, initial_x);
            fy = new KalmanFilter1D(A, H, Q, R, initial_P, initial_x);
            fz = new KalmanFilter1D(A, H, Q, R, initial_P, initial_x);
        }

        public Vector3 Output(Vector3 input)
        {
            return new Vector3(
                (float)fx.Output(input.x),
                (float)fy.Output(input.y),
                (float)fz.Output(input.z)
            );
        }
    }
}
