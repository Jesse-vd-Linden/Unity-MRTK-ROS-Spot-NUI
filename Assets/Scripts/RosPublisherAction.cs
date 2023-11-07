using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

/// <summary>
///
/// </summary>
public class RosPublisherAction : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "chatter";

    void Start()
    {
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<StringMsg>(topicName);
        Debug.Log("RosNode registered");
    }

    public void SendMessageAction(string actionString)
    {
        StringMsg action = new StringMsg(actionString);
        ros.Publish(topicName, action);
    }
}                                       