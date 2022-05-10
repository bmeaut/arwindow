using ARWindow.PlayerManagement;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Windows.Kinect;
using System.Threading;
using System;
using Microsoft.Kinect.Face;

public class KinectFaceDetection : IFaceDataProvider
{
    private KinectSensor KinectSensor { get; set; }
    private MultiSourceFrameReader MultiSourceFrameReader { get; set; }
    private int bodyCount;
    private Body[] bodies = null;

    private AutoResetEvent dataAvailableEvent = new AutoResetEvent(false);
    private volatile bool processingData = false;

    private List<CameraSpacePoint> headPositionsJoint = new List<CameraSpacePoint>();

    private Vector3 HeadPosition;

    FaceFrameSource _faceSource;

    public override Vector3 GetFacePosition() => HeadPosition;
    public override Rectangle GetFaceRect() => GetDriverFaceRect(HeadPosition);

    // Start is called before the first frame update
    void Start()
    {
        InitKinect();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void InitKinect()
    {
        KinectSensor = KinectSensor.GetDefault();

        FrameSourceTypes frameSourceTypes = FrameSourceTypes.Body;
        MultiSourceFrameReader = KinectSensor.OpenMultiSourceFrameReader(frameSourceTypes);
        MultiSourceFrameReader.MultiSourceFrameArrived += OnMultiFrameArrived;

        // elcrashel az editor, nem igy kellene hasznalni
        _faceSource = FaceFrameSource.Create(KinectSensor, 0, FaceFrameFeatures.BoundingBoxInColorSpace |
                                              FaceFrameFeatures.FaceEngagement |
                                              FaceFrameFeatures.Glasses |
                                              FaceFrameFeatures.Happy |
                                              FaceFrameFeatures.LeftEyeClosed |
                                              FaceFrameFeatures.MouthOpen |
                                              FaceFrameFeatures.PointsInColorSpace |
                                              FaceFrameFeatures.RightEyeClosed);
        _faceSource.OpenReader();

        bodyCount = KinectSensor.BodyFrameSource.BodyCount;
        bodies = new Body[bodyCount];
        KinectSensor.Open();
    }

    private Vector3? GetBestDriverHeadPositionInWindshieldCentered(List<CameraSpacePoint> points)
    {
        double maximalHeight = double.MinValue;
        Vector3? closestHeadPosition = null;

        foreach (var p in points)
        {
            Vector3 headPositionInKinectCoordinates = ConvertCameraSpacePointToVector3D(p);

            // TODO: egyelore ay elsot valasztjuk ki
            closestHeadPosition = headPositionInKinectCoordinates;
            break;

            //Vector3 headPosition = environment.TransformKinectCoordinatesToWindshieldCenteredCoordinates(headPositionInKinectCoordinates);

            //var height = environment.GetDriverPosHeightMapValue(headPosition.X, headPosition.Y, headPosition.Z);
            //if (height > maximalHeight)
            //{
            //    maximalHeight = height;
            //    closestHeadPosition = headPosition;
            //    driverPosition = headPositionInKinectCoordinates;
            //}
        }
        return closestHeadPosition;
    }

    private void OnMultiFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
    {
        bool dataAvailable = false;
        MultiSourceFrame msf = e.FrameReference.AcquireFrame();
        if (msf != null) using (BodyFrame bodyFrame = msf.BodyFrameReference.AcquireFrame())
        {
            if (bodyFrame != null)
            {
                headPositionsJoint.Clear();
                bodyFrame.GetAndRefreshBodyData(bodies);
                foreach (var body in bodies)
                {
                    if (body.IsTracked)
                    {
                        headPositionsJoint.Add(body.Joints[JointType.Head].Position);
                    }
                }
                dataAvailable = true;
            }

            if (dataAvailable)
            {
                Vector3? headPosition = GetBestDriverHeadPositionInWindshieldCentered(headPositionsJoint);

                if (headPosition.HasValue)
                {
                    HeadPosition = headPosition.Value;
                }
            }
        }
    }

    private Vector3 ConvertCameraSpacePointToVector3D(CameraSpacePoint cameraSpacePoint)
    {
        return new Vector3(cameraSpacePoint.X * 100.0f, cameraSpacePoint.Y * 100.0f, cameraSpacePoint.Z * 100.0f);
    }

    private Rectangle GetDriverFaceRect(Vector3 driverPosition)
    {
        var centerx = 960;
        var centery = 540;

        var sub = driverPosition.z - 120;
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

        var xHead = centerx + (int)Math.Round(driverPosition.x * xScale);
        var yHead = centery - (int)Math.Round(driverPosition.z * yScale);
        
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
            return new Rectangle(xRect, yRect, width, height); // TODO: ey eredetileg null volt, nem tom miert
        }

        return new Rectangle(xRect, yRect, width, height);
    }
}
