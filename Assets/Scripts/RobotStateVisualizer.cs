
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using System.Collections;

public class RobotStateVisualizer : MonoBehaviour
{
    ROSConnection ros;
    public AudioSource audioSource;

    void Start()
    {
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<StringMsg>("/robot_status", DisplayRobotState);
    }

    void Update()
    {

    }

    // this is the callback function
    void DisplayRobotState(StringMsg robotState)
    {
        StartCoroutine(WaitAndPlaySound());
        Debug.Log("message received");
    }

    // Coroutine for waiting and then playing the sound
    IEnumerator WaitAndPlaySound()
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        // Play the sound
        audioSource.Play();
    }
}
