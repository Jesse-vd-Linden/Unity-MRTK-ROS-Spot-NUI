using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using Microsoft.MixedReality.Toolkit;

using System.IO;
using System;

public class RosPublisherDataCollection : MonoBehaviour
{

    ROSConnection ros;

    [SerializeField]
    private string DataCollectionPublisherTopic = "data_collection";
    private string GazePositionTopic = "eye_gaze_in_pixel";
    private string GazeHitObjectTopic = "gaze_hit_object";
    public int updateFrequency = 20; // Interval in seconds at which the update function should run
    float updateInterval;
    private float nextUpdateTime = 0f;  // Keeps track of when the next update should occur
    private GameObject gazeIndicator;
    private Camera cam;
    private string currentDateTime;

    // Start is called before the first frame update
    void Start()
    {
        updateInterval = (1 / updateFrequency); // Interval in seconds at which the update function should run
        cam = Camera.main;
        // gazeIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // gazeIndicator.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        currentDateTime = DateTime.Now.ToString("yyyy_MM_dd_HHmm");

        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Float32MultiArrayMsg>(DataCollectionPublisherTopic);
        ros.RegisterPublisher<Float32MultiArrayMsg>(GazePositionTopic);
        ros.RegisterPublisher<StringMsg>(GazeHitObjectTopic);

    }

    // Update is called once per frame
    void Update()
    {
        
        String HitObject = CoreServices.InputSystem.EyeGazeProvider.GazeTarget.ToString();
        Vector3 PersonLocation = CoreServices.InputSystem.EyeGazeProvider.GazeOrigin;
        Vector3 GazeDirectionPerson = CoreServices.InputSystem.EyeGazeProvider.GazeDirection;
        Vector3 ScreenPos = cam.WorldToScreenPoint(GazeDirectionPerson);
        //Debug.Log("User gaze is currently over game object: " + CoreServices.InputSystem.EyeGazeProvider.GazeTarget);
        //Debug.Log($"ScreenPos: {ScreenPos}");
        //Debug.Log("Pixel width :" + cam.pixelWidth + " Pixel height : " + cam.pixelHeight);
        // gazeIndicator.transform.position = GazeDirectionPerson;

        

        if (Time.time >= nextUpdateTime)
        {
            Vector3 CameraPosition = Camera.main.transform.position;
            Quaternion CameraOrientation = Camera.main.transform.rotation;

            float[] points = new float[16];

            points[0] = PersonLocation.z;
            points[1] = -1* PersonLocation.x;
            points[2] = PersonLocation.y;
            points[3] = GazeDirectionPerson.z;
            points[4] = -1 * GazeDirectionPerson.x;
            points[5] = GazeDirectionPerson.y;
            points[6] = ScreenPos.z;
            points[7] = -1 * ScreenPos.x;
            points[8] = ScreenPos.y;
            points[9] = CameraPosition.z;
            points[10] = -1 * CameraPosition.x;
            points[11] = CameraPosition.y;
            points[12] = CameraOrientation.w;
            points[13] = CameraOrientation.x;
            points[14] = CameraOrientation.y;
            points[15] = CameraOrientation.z;

            Float32MultiArrayMsg message = new Float32MultiArrayMsg()
            {
                data = points
            };

            ros.Publish(DataCollectionPublisherTopic, message);

            // Set the time for the next update
            nextUpdateTime = Time.time + updateInterval;
        }

        float[] pixelPoints = new float[3];
        pixelPoints[0] = ScreenPos.x;
        pixelPoints[1] = ScreenPos.y;
        pixelPoints[2] = ScreenPos.z;

        Float32MultiArrayMsg ScreenPosMessage = new Float32MultiArrayMsg()
        {
            data = pixelPoints
        };

        StringMsg HitObjectMessage = new StringMsg()
        {
            data = HitObject
        };

        ros.Publish(GazePositionTopic, ScreenPosMessage);
        ros.Publish(GazeHitObjectTopic, HitObjectMessage);
    }

    private void WriteJointsToFile(Vector3 ScreenPos)
    {
        string filename = currentDateTime + ".txt";
        string path = Path.Combine(Application.persistentDataPath, filename);
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            string jointData = string.Join(",", ScreenPos);
            writer.WriteLine(jointData);
        }
        Debug.Log($"Hand joints saved to: {path}");
    }
}
