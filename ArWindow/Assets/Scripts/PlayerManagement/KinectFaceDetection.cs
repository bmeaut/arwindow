using ARWindow.PlayerManagement;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Windows.Kinect;
using System;
using Microsoft.Kinect.Face;
using System.Linq;

public class KinectFaceDetection : IFaceDataProvider
{
    private KinectSensor KinectSensor { get; set; }
    private BodyFrameSource _bodySource;
    private BodyFrameReader _bodyReader;
    private HighDefinitionFaceFrameSource _faceSource;
    private HighDefinitionFaceFrameReader _faceReader;
    private FaceAlignment _faceAlignment;
    private FaceModel _faceModel;

    private Vector3 HeadPosition;

    public GameObject _vertexPrefab;
    private List<GameObject> _facePoints = new List<GameObject>();

    public override Vector3 GetFacePosition() => HeadPosition;
    public override Rectangle GetFaceRect() => GetDriverFaceRect(HeadPosition);

    // Start is called before the first frame update
    void Start()
    {
        InitKinect();
    }

    private void InitKinect()
    {
        KinectSensor = KinectSensor.GetDefault();

        if (KinectSensor == null)
        {
            Debug.LogError("Kinect Sensor not found.");
            return;
        }

        _bodySource = KinectSensor.BodyFrameSource;
        _bodyReader = _bodySource.OpenReader();
        _bodyReader.FrameArrived += BodyReader_FrameArrived;
        _faceSource = HighDefinitionFaceFrameSource.Create(KinectSensor);
        _faceReader = _faceSource.OpenReader();
        _faceReader.FrameArrived += FaceReader_FrameArrived;
        _faceAlignment = FaceAlignment.Create();
        _faceModel = FaceModel.Create();

        KinectSensor.Open();
    }

    private void BodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
    {
        if (!_faceSource.IsTrackingIdValid)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    Body[] bodies = new Body[frame.BodyCount];
                    frame.GetAndRefreshBodyData(bodies);

                    Body body = bodies.Where(b => b.IsTracked).FirstOrDefault();

                    if (body != null)
                    {
                        _faceSource.TrackingId = body.TrackingId;
                    }
                }
            }
        }
    }

    private void FaceReader_FrameArrived(object sender, HighDefinitionFaceFrameArrivedEventArgs e)
    {
        using (var frame = e.FrameReference.AcquireFrame())
        {
            if (frame != null && frame.IsFaceTracked)
            {
                frame.GetAndRefreshFaceAlignmentResult(_faceAlignment);
                UpdateFacePoints();
            }
        }
    }

    private void UpdateFacePoints()
    {
        var vertices = _faceModel.CalculateVerticesForAlignment(_faceAlignment);
        if (vertices.Count == 0) return;

        //ideiglenes, csak tesztelésre
        //vertex pontok megjelenítése térben
        if (_facePoints.Count == 0)
        {
            var parent = new GameObject();
            for (int i = 0; i < vertices.Count; i++)
            {
                var v = GameObject.Instantiate(_vertexPrefab, parent.transform);
                _facePoints.Add(v);
            }
        }
        for (int i = 0; i < vertices.Count; i++)
        {
            CameraSpacePoint vertex = vertices[i];
            _facePoints[i].transform.position = new Vector3(vertex.X, vertex.Y, vertex.Z);
        }

        HeadPosition = ConvertCameraSpacePointToVector3D(vertices[(int)HighDetailFacePoints.NoseTop]);
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
            return new Rectangle(xRect, yRect, width, height); // TODO: ez eredetileg null volt, nem tom miert
        }

        return new Rectangle(xRect, yRect, width, height);
    }
}
