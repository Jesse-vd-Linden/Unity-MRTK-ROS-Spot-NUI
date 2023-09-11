using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Windows.WebCam;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class RosPublisherImage : MonoBehaviour
{
    WebCamTexture webcam;

    ROSConnection ros;
    public string topicName = "image_hololens";

    void Start()
    {
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Float32MultiArrayMsg>(topicName);

        webcam = new WebCamTexture();
        webcam.Play();
        Debug.LogFormat("webcam {0} {1} x {2}", webcam.deviceName, webcam.width, webcam.height);


    }

    void NoUpdate()
    {
        Texture2D webcamImage = new Texture2D(webcam.width, webcam.height);
        webcamImage.SetPixels(webcam.GetPixels());
        webcamImage.Apply();

        /*float[] imageVar = new float[] { webcamImage. };

        Float32MultiArrayMsg Points = new Float32MultiArrayMsg()
        {
            data = imageVar
        };

        ros.Publish(topicName, Points);*/
    }
    
    Texture2D TakePhoto()
    {
        Texture2D webcamImage = new Texture2D(webcam.width, webcam.height);
        webcamImage.SetPixels(webcam.GetPixels());
        webcamImage.Apply();

        return webcamImage;
    }

    public void TakePhotoPreview(Renderer preview)
    {
        Texture2D image = TakePhoto();
        preview.material.mainTexture = image;

        float aspectRatio = (float)image.width / (float)image.height;
        Vector3 scale = preview.transform.localScale;
        scale.x = scale.y * aspectRatio;
        preview.transform.localScale = scale;
    }



}