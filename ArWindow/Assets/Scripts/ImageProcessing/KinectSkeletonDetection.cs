﻿using ARWindow.Configuration.WindowConfigurationManagement;
using ARWindow.PlayerManagement;
using Assets.Scripts.Filters;
using Injecter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using UnityEngine;
using Windows.Kinect;

namespace Assets.Scripts.ImageProcessing
{
    public class KinectSkeletonDetection : MonoBehaviour, IFaceDataProvider
    {
        [SerializeField] private bool isKalmanFilterOn = false;
        [SerializeField] private double KeepStillThreshold = 1.0; // cm, eucledian distance
        [SerializeField] private bool trackClosestBody = true;

        [Inject] private readonly WindowConfiguration _windowConfiguration;

        private KinectSensor KinectSensor { get; set; }
        private MultiSourceFrameReader MultiSourceFrameReader { get; set; }

        private AutoResetEvent dataAvailableEvent = new AutoResetEvent(false);
        private volatile bool processingData = false;
        private List<CameraSpacePoint> headPositionsJoint = new List<CameraSpacePoint>();
        private int bodyCount;
        private Body[] bodies = null;

        private Vector3 HeadPosition = Vector3.zero;

        private KalmanFilter3D filter = new KalmanFilter3D(1, 1, 7, 7, 0, 0);

        private Thread KinectDataProcessingThread;

        public Vector3 GetFacePosition() => HeadPosition;
        public Rectangle GetFaceRect() => GetViewerFaceRect(HeadPosition);

        private Dictionary<ulong, CameraSpacePoint> headPositionJointPerBody = new Dictionary<ulong, CameraSpacePoint>();
        private ulong currentBodyID = 0;

        // Start is called before the first frame update
        void Start()
        {
            InitKinect();
        }

        private void InitKinect()
        {
            KinectDataProcessingThread = new Thread(new ThreadStart(ProcessData));
            KinectDataProcessingThread.Start();

            KinectSensor = KinectSensor.GetDefault();

            FrameSourceTypes frameSourceTypes = FrameSourceTypes.Body;

            frameSourceTypes |= FrameSourceTypes.Color; // New driver miatt
            //Vigyázni kell arra, hogy ezzel a többit ki kell kapcsolni, mert túl sokáig tart processzelni
            MultiSourceFrameReader = KinectSensor.OpenMultiSourceFrameReader(frameSourceTypes);
            MultiSourceFrameReader.MultiSourceFrameArrived += OnMultiFrameArrived;

            bodyCount = KinectSensor.BodyFrameSource.BodyCount;
            bodies = new Body[bodyCount];
            KinectSensor.Open();
        }

        private void ProcessData()
        {
            while (true)
            {
                dataAvailableEvent.WaitOne();
                processingData = true;

                Vector3? headPosition = null;

                if (headPositionsJoint.Count != 0)
                {
                    if(trackClosestBody)
                    {
                        // We don't need the square root, because we don't need the actual distance from the camera,
                        // only the nearest point.
                        var closest = headPositionsJoint.OrderBy(p => p.X * p.X + p.Y * p.Y + p.Z * p.Z).First();
                        headPosition = _windowConfiguration.PlayerCameraPointToWindowCenteredPoint(
                            ConvertCameraSpacePointToVector3D(closest));
                    }
                    else
                    {
                        if(headPositionJointPerBody.TryGetValue(currentBodyID, out var value))
                        {
                            headPosition = _windowConfiguration.PlayerCameraPointToWindowCenteredPoint(
                                ConvertCameraSpacePointToVector3D(value));
                        }
                        else
                        {
                            var newBody = headPositionJointPerBody.First();
                            currentBodyID = newBody.Key;
                            headPosition = _windowConfiguration.PlayerCameraPointToWindowCenteredPoint(
                                ConvertCameraSpacePointToVector3D(newBody.Value));
                        }
                    }
                }

                if (headPosition.HasValue)
                {
                    if (isKalmanFilterOn)
                        HeadPosition = Track(headPosition.Value);
                    else HeadPosition = headPosition.Value;
                }
                processingData = false;
            }
        }

        private Vector3 Track(Vector3 headPosition)
        {
            if (Vector3.Distance(HeadPosition, headPosition) < KeepStillThreshold)
            {
                return HeadPosition;
            }
            return filter.Output(headPosition);
        }

        private void OnMultiFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            dataAvailableEvent.Reset();
            if (processingData) return;

            bool dataAvailable = false;
            MultiSourceFrame msf = e.FrameReference.AcquireFrame();
            if (msf != null)
                using (BodyFrame bodyFrame = msf.BodyFrameReference.AcquireFrame())
                {
                    headPositionJointPerBody.Clear();
                    headPositionsJoint.Clear();
                    bodyFrame.GetAndRefreshBodyData(bodies);
                    foreach (var body in bodies)
                    {
                        if (body.IsTracked)
                        {
                            if (currentBodyID == 0)
                                currentBodyID = body.TrackingId;

                            headPositionJointPerBody.Add(body.TrackingId, body.Joints[JointType.Head].Position);
                            headPositionsJoint.Add(body.Joints[JointType.Head].Position);
                        }
                    }
                    dataAvailable = true;

                    if (dataAvailable) dataAvailableEvent.Set();
                }
        }

        // This function also multiply the coords with 100, so we measure the things centimeters
        // and not in meters (as the kinect SDK does)
        private Vector3 ConvertCameraSpacePointToVector3D(CameraSpacePoint cameraSpacePoint)
        {
            return new Vector3(cameraSpacePoint.X * 100.0f, cameraSpacePoint.Y * 100.0f, cameraSpacePoint.Z * 100.0f);
        }

        private Rectangle GetViewerFaceRect(Vector3 viewerPosition)
        {
            var centerx = 960;
            var centery = 540;

            var sub = viewerPosition.z - 120;
            int width = 300;
            int height = 300;

            int xOffset = 100;
            int yOffset = 100;
            double xScale = 7;
            double yScale = 7;
            if (sub > 0)
            {
                width = (int)Math.Round(width - sub * 2);
                height = (int)Math.Round(height - sub * 2);
                xOffset = (int)Math.Round(xOffset - sub / 2);
                yOffset = (int)Math.Round(yOffset - sub / 2);
            }
            else
            {
                sub = Math.Abs(sub);
                width = (int)Math.Round(width + sub * 2);
                height = (int)Math.Round(height + sub * 2);
                xOffset = (int)Math.Round(xOffset + sub / 2);
                yOffset = (int)Math.Round(yOffset + sub / 2);
            }

            var xHead = centerx + (int)Math.Round(viewerPosition.x * xScale);
            var yHead = centery - (int)Math.Round(viewerPosition.z * yScale);

            int xRect = xHead - xOffset;
            int yRect = yHead - yOffset;
            xRect = Math.Max(Math.Min(xRect, 1920), 0);
            yRect = Math.Max(Math.Min(yRect, 1080), 0);

            if (xRect + width > 1920)
            {
                width = 1920 - xRect;
            }
            if (yRect + height > 1080)
            {
                height = 1080 - yRect;
            }

            if (width < 100 || height < 100)
            {
                return new Rectangle(xRect, yRect, width, height); // TODO: it was originally null, we don't know why
            }

            return new Rectangle(xRect, yRect, width, height);
        }
    }
}
