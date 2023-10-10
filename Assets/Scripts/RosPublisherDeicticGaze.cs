using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using UnityEngine.InputSystem;

public class RosPublisherDeicticGaze : MonoBehaviour
{
    ROSConnection ros1;
    ROSConnection ros2;
    ROSConnection ros3;
    Vector3 hitPoint;

    public string topicName1 = "deictic_pick_up";
    public string topicName2 = "deictic_drop_off";
    public string topicName3 = "deictic_walk";

    public float fixedDistance = 2.0f; // Define the distance you consider as "where the user is looking at"

    public float LengthPerson = 1.85f; // Length of person to determine the point of contact to the ground

    public GameObject gameObjectToMove;

    public TextMeshProUGUI CountDownTimerCalibration;

    Vector3 CalibrationOrigin = new Vector3(0,0,0);
    Vector3 CalibrationDirection = new Vector3(0, 0, 0);

    void Start()
    {
        // start the ROS connection
        ros1 = ROSConnection.GetOrCreateInstance();
        ros2 = ROSConnection.GetOrCreateInstance();
        ros3 = ROSConnection.GetOrCreateInstance();

        ros1.RegisterPublisher<Float32MultiArrayMsg>(topicName1);
        ros2.RegisterPublisher<Float32MultiArrayMsg>(topicName2);
        ros3.RegisterPublisher<Float32MultiArrayMsg>(topicName3);

        Vector3 PersonLocation = CoreServices.InputSystem.GazeProvider.GazeOrigin;

        Debug.Log("Initial position HoloLens:" + PersonLocation.ToString());
    }

    // Update is called once per frame
    public void Update()
    {
        Vector3 gazePoint = CalculatePlanarGazeLocation();
        gameObjectToMove.transform.position = gazePoint;
    }

    // Update is called once per frame
    public void CalibratePositionAndDirection()
    {
        // timer and looking at target instruction
        for (int i = 5; i > 0; i--)
        {
            Thread.Sleep(1000);
            string timeCountDown = string.Format("00:{0:F2}", i);
            CountDownTimerCalibration.text = timeCountDown;
        }
        CountDownTimerCalibration.text = new string("");

        CalibrationOrigin = CoreServices.InputSystem.GazeProvider.GazeOrigin;

        Vector3 CurrentGazeDirection = CoreServices.InputSystem.GazeProvider.GazeDirection;
        float xDirection = CurrentGazeDirection[0];
        float yDirection = CurrentGazeDirection[1];
        float zDirection = -1 * (1 - CurrentGazeDirection[2]);
        CalibrationDirection = new Vector3(xDirection, yDirection, zDirection);


    }

    Vector3 CalculatePlanarGazeLocation()
    {
        Vector3 PersonLocation = CoreServices.InputSystem.GazeProvider.GazeOrigin - CalibrationOrigin;
        PersonLocation[1] = PersonLocation[1] + LengthPerson - 0.05f;
        Vector3 GazeDirection = CoreServices.InputSystem.GazeProvider.GazeDirection - CalibrationDirection;

        float VectorLengthToGround = -1 * (PersonLocation[1] / GazeDirection[1]);
        double subAnswer = Math.Pow(GazeDirection[0], 2.0f) + Math.Pow(GazeDirection[2], 2.0f);
        float UnitMaxMultiplier = (float)(5.0f / Math.Pow(subAnswer, 0.5f));
        float x;
        float z;

        if (VectorLengthToGround > UnitMaxMultiplier)
        {
            x = PersonLocation[0] + UnitMaxMultiplier * GazeDirection[0];
            z = PersonLocation[2] + UnitMaxMultiplier * GazeDirection[2];
        }
        else if (VectorLengthToGround < 0)
        {
            x = PersonLocation[0] + UnitMaxMultiplier * GazeDirection[0];
            z = PersonLocation[2] + UnitMaxMultiplier * GazeDirection[2];
        } 
        else
        {
            x = PersonLocation[0] + VectorLengthToGround * GazeDirection[0];
            z = PersonLocation[2] + VectorLengthToGround * GazeDirection[2];
        }

        Vector3 gazePoint = new Vector3(x, -LengthPerson, z);

        return gazePoint;

    }

    // Update is called once per frame
    public void SendPickUp()
    {
        Vector3 PersonLocation = CoreServices.InputSystem.GazeProvider.GazeOrigin;
        Vector3 GazeLocation = CoreServices.InputSystem.GazeProvider.GazeDirection;

        float[] message = new float[6];
        message[0] = PersonLocation[2];
        message[1] = -1 * PersonLocation[0];
        message[2] = PersonLocation[1];

        message[3] = GazeLocation[2];
        message[4] = -1 * GazeLocation[0];
        message[5] = GazeLocation[1];

        Float32MultiArrayMsg Points = new Float32MultiArrayMsg()
        {
            data = message
        };

        ros1.Publish(topicName1, Points);
        Debug.Log("Deictic Pick Up");
    }

    public void SendDropOff()
    {
        Vector3 PersonLocation = CoreServices.InputSystem.GazeProvider.GazeOrigin;
        Vector3 GazeLocation = CoreServices.InputSystem.GazeProvider.GazeDirection;

        float[] message = new float[6];
        message[0] = PersonLocation[2];
        message[1] = -1 * PersonLocation[0];
        message[2] = PersonLocation[1];

        message[3] = GazeLocation[2];
        message[4] = -1 * GazeLocation[0];
        message[5] = GazeLocation[1];

        Float32MultiArrayMsg Points = new Float32MultiArrayMsg()
        {
            data = message
        };

        ros2.Publish(topicName2, Points);
        Debug.Log("Deictic Drop Off");
    }

    public void SendWalk()
    {
        Vector3 PersonLocation = CoreServices.InputSystem.GazeProvider.GazeOrigin;
        Vector3 GazeDirection = CoreServices.InputSystem.GazeProvider.GazeDirection;

        // Raycasting to find out what the user is looking at
        RaycastHit hitInfo;
        if (Physics.Raycast(PersonLocation, GazeDirection, out hitInfo))
        {
            Vector3 hitPoint = hitInfo.point;
            // Now hitPoint contains the 3D location where the user is looking
        }

        Vector3 gazePoint = PersonLocation + (GazeDirection * fixedDistance);


        float[] message = new float[6];
        message[0] = PersonLocation[2];
        message[1] = -1 * PersonLocation[0];
        message[2] = PersonLocation[1];

        message[3] = gazePoint[2];
        message[4] = -1 * gazePoint[0];
        message[5] = gazePoint[1];

        Float32MultiArrayMsg Points = new Float32MultiArrayMsg()
        {
            data = message
        };

        ros3.Publish(topicName3, Points);
        Debug.Log("Deictic Walk");
    }
}
