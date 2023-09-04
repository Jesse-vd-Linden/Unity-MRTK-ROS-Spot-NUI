using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;
using Microsoft.MixedReality.Toolkit;

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

public class RosPublisherImage : MonoBehaviour
{
    ROSConnection ros;
    [SerializeField]
    private string ÌmagePublisherTopic = "image_hololens";

/*    UnityEngine.Windows.WebCam.PhotoCapture photoCaptureObject = null;
    Texture2D targetTexture = null;*/

    // Start is called before the first frame update
    void Start()
    {
        // Ros for hand keypoints
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Float32MultiArrayMsg>(ÌmagePublisherTopic);
/*
        Resolution cameraResolution = UnityEngine.Windows.WebCam.PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        // Create a PhotoCapture object
        UnityEngine.Windows.WebCam.PhotoCapture.CreateAsync(false, delegate (UnityEngine.Windows.WebCam.PhotoCapture captureObject) {
            photoCaptureObject = captureObject;
            UnityEngine.Windows.WebCam.CameraParameters cameraParameters = new UnityEngine.Windows.WebCam.CameraParameters();
            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.cameraResolutionWidth = cameraResolution.width;
            cameraParameters.cameraResolutionHeight = cameraResolution.height;
            cameraParameters.pixelFormat = UnityEngine.Windows.WebCam.CapturePixelFormat.BGRA32;

            // Activate the camera
            photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (UnityEngine.Windows.WebCam.PhotoCapture.PhotoCaptureResult result) {
                // Take a picture
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            });
        });*/
    }

    // Update is called once per frame, if the function name is: Update()
    void Update()
    {

        /*Float32MultiArrayMsg Points = new Float32MultiArrayMsg()
        {
            data = imageArray
        };
        ros.Publish(ÌmagePublisherTopic, Points);*/
    }
}