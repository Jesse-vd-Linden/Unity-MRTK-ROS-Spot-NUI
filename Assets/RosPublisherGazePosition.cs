using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using Microsoft.MixedReality.Toolkit;

public class RosPublisherGazePosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Gaze 6D position available");
    }

    // Update is called once per frame
    public void Gaze3DLocation()
    {
        Debug.Log("Gaze is looking in direction: "
        + CoreServices.InputSystem.GazeProvider.GazeDirection);

        Debug.Log("Gaze origin is: "
            + CoreServices.InputSystem.GazeProvider.GazeOrigin);

        if (CoreServices.InputSystem.GazeProvider.GazeTarget)
        {
            Debug.Log("User gaze is currently over game object: "
                + CoreServices.InputSystem.GazeProvider.GazeTarget);
        }
    }
}
