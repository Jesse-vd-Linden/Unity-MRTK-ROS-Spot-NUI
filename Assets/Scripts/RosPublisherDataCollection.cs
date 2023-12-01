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
    private string CompressedDataTopic = "compressed_data";
    private string GazeHitObjectTopic = "gaze_hit_object";
    public int updateFrequency = 20; // Interval in seconds at which the update function should run
    float updateInterval;
    private float nextUpdateTime = 0f;  // Keeps track of when the next update should occur
    private Camera cam;
    private string currentDateTime;
    [SerializeField]
    private ExtendedEyeGazeDataProvider extendedEyeGazeDataProvider;
    private ExtendedEyeGazeDataProvider.GazeReading gazeReading;

    private DateTime timestamp;

    // Start is called before the first frame update
    void Start()
    {
        updateInterval = (1 / updateFrequency); // Interval in seconds at which the update function should run
        cam = Camera.main;
        currentDateTime = DateTime.Now.ToString("yyyy_MM_dd_HHmm");

        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Float32MultiArrayMsg>(DataCollectionPublisherTopic);
        ros.RegisterPublisher<Float32MultiArrayMsg>(CompressedDataTopic);
        ros.RegisterPublisher<StringMsg>(GazeHitObjectTopic);
    }

    // Update is called once per frame
    void Update()
    {
        // camera position
        Vector3 CameraPosition = Camera.main.transform.position;
        Quaternion CameraOrientation = Camera.main.transform.rotation;

        // head gaze
        //Vector3 HeadLocation = CoreServices.InputSystem.GazeProvider.GazeOrigin;
        //Vector3 HeadDirection = CoreServices.InputSystem.GazeProvider.GazeDirection;
        Vector3 HeadMovementDirection = CoreServices.InputSystem.GazeProvider.HeadMovementDirection;
        Vector3 HeadVelocity = CoreServices.InputSystem.GazeProvider.HeadVelocity;
        string HeadHitObject = CoreServices.InputSystem.GazeProvider.GazeTarget.name;

        // eye gaze
        Vector3 EyePosition = CoreServices.InputSystem.EyeGazeProvider.GazeOrigin;
        Vector3 GazeDirection = CoreServices.InputSystem.EyeGazeProvider.GazeDirection;
        Vector3 ScreenGazePos = cam.WorldToScreenPoint(GazeDirection);
        string GazeHitObject = CoreServices.InputSystem.EyeGazeProvider.GazeTarget.name;

        // single eye gaze
        timestamp = DateTime.Now;
        // TODO: change it to camera space
#if UNITY_EDITOR
        Vector3 leftEyePosition = new Vector3(0f,0f,0f);
        Vector3 leftEyeDirection = new Vector3(0f,0f,0f);
        Vector3 rightEyePosition = new Vector3(0f,0f,0f);
        Vector3 rightEyeDirection = new Vector3(0f,0f,0f);
        Vector3 combinedEyePosition = new Vector3(0f,0f,0f);
        Vector3 combinedEyeDirection = new Vector3(0f,0f,0f);
#endif

#if ENABLE_WINMD_SUPPORT
        gazeReading = extendedEyeGazeDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Left, timestamp);
        Vector3 leftEyePosition = gazeReading.EyePosition;
        Vector3 leftEyeDirection = gazeReading.GazeDirection;
        gazeReading = extendedEyeGazeDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Right, timestamp);
        Vector3 rightEyePosition = gazeReading.EyePosition;
        Vector3 rightEyeDirection = gazeReading.GazeDirection;
        gazeReading = extendedEyeGazeDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Combined, timestamp);
        Vector3 combinedEyePosition = gazeReading.EyePosition;
        Vector3 combinedEyeDirection = gazeReading.GazeDirection;
#endif
        if (Time.time >= nextUpdateTime)
        {
            float[] points = new float[16];
            points[0] = EyePosition.z;
            points[1] = -1 * EyePosition.x;
            points[2] = EyePosition.y;
            points[3] = GazeDirection.z;
            points[4] = -1 * GazeDirection.x;
            points[5] = GazeDirection.y;
            points[6] = ScreenGazePos.z;
            points[7] = ScreenGazePos.x;
            points[8] = ScreenGazePos.y;
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

        // 12 Vector 3 + 1 Quaternion
        float[] compressedData = new float[12*3+4];
        Vector3[] vectorArray = new Vector3[12];
        // camera pos
        vectorArray[0] = CameraPosition;
        // head
        //vectorArray[1] = HeadLocation;
        //vectorArray[2] = HeadDirection;
        vectorArray[1] = HeadMovementDirection;
        vectorArray[2] = HeadVelocity;
        // gaze
        vectorArray[3] = EyePosition;
        vectorArray[4] = GazeDirection;
        vectorArray[5] = ScreenGazePos;
        // extended gaze
        vectorArray[6] = leftEyePosition;
        vectorArray[7] = leftEyeDirection;
        vectorArray[8] = rightEyePosition;
        vectorArray[9] = rightEyeDirection;
        vectorArray[10] = combinedEyePosition;
        vectorArray[11] = combinedEyeDirection;
        for (int i = 0; i < vectorArray.Length; i++)
        {
            compressedData[i * 3] = vectorArray[i].x;
            compressedData[i * 3 + 1] = vectorArray[i].y;
            // TODO: left to right hand coordinate
            compressedData[i * 3 + 2] = vectorArray[i].z;
        }
        // camera ori
        compressedData[36] = CameraOrientation.x;
        compressedData[37] = CameraOrientation.y;
        compressedData[38] = CameraOrientation.z;
        compressedData[39] = CameraOrientation.w;

        Float32MultiArrayMsg CompressedMessage = new Float32MultiArrayMsg()
        {
            data = compressedData
        };

        StringMsg HitObjectMessage = new StringMsg()
        {
            data = HeadHitObject + "," + GazeHitObject
        };
        ros.Publish(CompressedDataTopic, CompressedMessage);
        ros.Publish(GazeHitObjectTopic, HitObjectMessage);
    }

    private void WriteJointsToFile(float[] compressedData)
    {
        string filename = currentDateTime + ".txt";
        string path = Path.Combine(Application.persistentDataPath, filename);
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            string jointData = string.Join(",", compressedData);
            writer.WriteLine(jointData);
        }
        Debug.Log($"Hand joints saved to: {path}");
    }
}