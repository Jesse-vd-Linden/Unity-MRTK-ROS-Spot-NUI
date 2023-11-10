using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

public class RosPublisherDataCollection : MonoBehaviour
{

    ROSConnection ros;

    [SerializeField]
    private string DataCollectionPublisherTopic = "data_collection";

    public float updateInterval = 0.05f; // Interval in seconds at which the update function should run
    private float nextUpdateTime = 0f;  // Keeps track of when the next update should occur


    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Float32MultiArrayMsg>(DataCollectionPublisherTopic);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextUpdateTime)
        {
            Debug.Log("User gaze is currently over game object: " + CoreServices.InputSystem.GazeProvider.GazeTarget);
                
            Vector3 PersonLocation = CoreServices.InputSystem.GazeProvider.GazeOrigin;
            Vector3 GazeDirectionPerson = CoreServices.InputSystem.GazeProvider.GazeDirection;

            float[] points = new float[6];

            points[0] = PersonLocation.z;
            points[1] = -1* PersonLocation.x;
            points[2] = PersonLocation.y;
            points[3] = GazeDirectionPerson.z;
            points[4] = -1 * GazeDirectionPerson.x;
            points[5] = GazeDirectionPerson.y;

            Float32MultiArrayMsg message = new Float32MultiArrayMsg()
            {
                data = points
            };

            ros.Publish(DataCollectionPublisherTopic, message);

            // Set the time for the next update
            nextUpdateTime = Time.time + updateInterval;
        }
    }
}
