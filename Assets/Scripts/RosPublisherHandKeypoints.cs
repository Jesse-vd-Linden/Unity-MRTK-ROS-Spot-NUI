using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;
using Microsoft.MixedReality.Toolkit;

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using TMPro;


public class RosPublisherHandKeypoints : MonoBehaviour
{
    public Camera main_camera;
    // private Vector3[] handJoints = new Vector3[26]; 
    private float[] flattenedhandJoints = new float[78];
    private Quaternion handRotation; 
    public TMP_Text LoggingPanel;
    public TMP_Text ResPanel;
    string Logging = "";
    private float collectionCountdown;
    private float timeBeforeCollect = 8;
    public float collectionDuration = 15;
    private bool isCollectionActive = true; 

    ROSConnection ros;

    [SerializeField]
    private string HandKeypointsPublisherTopic = "hand_keypoints";

    private IMixedRealityHand currentTrackedHand = null;
    // Stores currently attached hand if valid (only possible values Left, Right, or None)
    protected Handedness currentTrackedHandedness = Handedness.None;
    [SerializeField]
    [Tooltip("If tracking hands or motion controllers, determines which hand(s) are valid attachments")]
    [FormerlySerializedAs("trackedHandness")]
    private Handedness trackedHandedness = Handedness.Right;
    private MixedRealityPose currentJointPose;

    // Start is called before the first frame update
    void Start()
    {
        // Ros for hand keypoints
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Float32MultiArrayMsg>(HandKeypointsPublisherTopic);
        ros.Subscribe<StringMsg>("/chatter", DisplayRecognizedGestures);
        collectionCountdown = 0;
    }

    void DisplayRecognizedGestures(StringMsg gestureMsg)
    {
        ResPanel.text = "Gesture: " + gestureMsg.ToString();
    }

    // Update is called once per frame, if the function name is: Update()
    void Update()
    {
        Logging = "";
        Debug.Log($"timeBeforeCollect: {timeBeforeCollect}");
        if (collectionCountdown < timeBeforeCollect)
        {
            collectionCountdown += Time.deltaTime;
            LoggingPanel.text = "Will collect data in " + Math.Ceiling(timeBeforeCollect - collectionCountdown).ToString() + " seconds";
        }
        else
        {
            if (collectionCountdown > timeBeforeCollect + collectionDuration)
            {
                isCollectionActive = false;
                LoggingPanel.text = "Data collection finished!";
            }
            else
            {
                collectionCountdown += Time.deltaTime;
                Logging += "Collect data in " + Math.Ceiling(timeBeforeCollect + collectionDuration - collectionCountdown).ToString() + " seconds\n";
            }
            if (IsValidHandFound())
            {
                UpdatePoseInformation();
            }
            else
            {
                LoggingPanel.text = "No valid hand found. Check the choosen handedness. ";
            }
        }
    }
    private bool IsValidHandFound()
    {
        if (HandJointUtils.FindHand(Handedness.Right) != null)
        {   
            Logging += "found right hand";
            // LoggingPanel.text = "found right hand";
            if (trackedHandedness == Handedness.Right)
            {
                currentTrackedHand = HandJointUtils.FindHand(Handedness.Right);
                return true;
            }
            
        }
        if (HandJointUtils.FindHand(Handedness.Left) != null)
        {   
            Logging += "found left hand";
            // LoggingPanel.text = "found left hand";
            if (trackedHandedness == Handedness.Left)
            {
                currentTrackedHand = HandJointUtils.FindHand(Handedness.Left);
                return true;
            }
            
        }
        return false;
    }
    public void UpdatePoseInformation()
    {   
        // TODO: add visual bouding box 
        // bool out_of_box = false;
        bool allJointsTracked = true;
        if (currentTrackedHand != null)
        {
            int i = 0;
            foreach (TrackedHandJoint trackedHandJoint in Enum.GetValues(typeof(TrackedHandJoint)))
            {
                if (trackedHandJoint != TrackedHandJoint.None)
                {
                    bool isJointTracked = currentTrackedHand.TryGetJoint(trackedHandJoint, out currentJointPose);
                    if (!isJointTracked)
                    {
                        allJointsTracked = false; // At least one joint is not valid
                        Logging += "break because" + trackedHandJoint.ToString() + "  joint is missing";
                        // LoggingPanel.text = "break because" + trackedHandJoint.ToString() + "  joint is missing";
                        break; // Exit the loop early
                    } 
                    Vector3 relativePosition = main_camera.transform.InverseTransformPoint(currentJointPose.Position);
                    // Quaternion relativeRotation = currentJointPose.Rotation * Quaternion.Inverse(main_camera.transform.rotation);
                    flattenedhandJoints[i * 3] = relativePosition.x;
                    flattenedhandJoints[i * 3 + 1] = relativePosition.y;
                    flattenedhandJoints[i * 3 + 2] = relativePosition.z;
                    i += 1;
                }
            }
        }
        if (allJointsTracked)
        {
            Float32MultiArrayMsg Points = new Float32MultiArrayMsg()
            {
                data = flattenedhandJoints
            };
            ros.Publish(HandKeypointsPublisherTopic, Points);
            if (isCollectionActive)
            {
                WriteJointsToFile();
            }
            Logging += " ROS message published \n";
            LoggingPanel.text = Logging;
        }

    }
    private void WriteJointsToFile()
    {
        string currentDateTime = DateTime.Now.ToString("yyyy_MM_dd_HHmm");
        string filename = currentDateTime + ".txt";
        string path = Path.Combine(Application.persistentDataPath, filename);
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            string jointData = string.Join(",", flattenedhandJoints);
            writer.WriteLine(jointData);
        }
        Debug.Log($"Hand joints saved to: {path}");
    }
}