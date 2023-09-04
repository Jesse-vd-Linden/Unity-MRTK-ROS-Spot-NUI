using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;

public class RosPublisherHandPose : MonoBehaviour
{
    private IMixedRealityHand handRight;
    private MixedRealityPose Palmpose;
    private MixedRealityPose IndexTipPose;
    private MixedRealityPose ThumbTipPose;
    private bool gripper_state_changed;
    private string gripper_state;
    private string prev_gripper_state;
    private bool direct_control_active;


    private float angle;
    ROSConnection ros_action;
    ROSConnection ros_palm;
    ROSConnection ros_gripper;
    public string ActionTopic = "chatter";
    public string gripperOpenOrCloseTopic = "gripper";

    [SerializeField]
    private string HandPosePublisherTopic = "hand_pose";

    /*[SerializeField]
    private GameObject HandObject;

    [SerializeField]
    private GameObject ServiceObject;

    [SerializeField]
    private GameObject DirectControlEnvObject;

    [SerializeField]
    private GameObject RobotHandle;*/

    // Start is called before the first frame update
    void Start()
    {
        //mrirac_trajectory_planner_Fr3/unity_hand_pose
        // Ros for action
        ros_action = ROSConnection.GetOrCreateInstance();
        ros_action.RegisterPublisher<StringMsg>(ActionTopic);

        // Ros for hand palm pose
        ros_palm = ROSConnection.GetOrCreateInstance();
        ros_palm.RegisterPublisher<PoseMsg>(HandPosePublisherTopic);

        // Ros for gripper
        ros_gripper = ROSConnection.GetOrCreateInstance();
        ros_gripper.RegisterPublisher<StringMsg>(gripperOpenOrCloseTopic);

        gripper_state_changed = false;
        gripper_state = null;
        prev_gripper_state = null;
        direct_control_active = false;

        //HandObject.transform.SetParent(DirectControlEnvObject.transform);

    }

    public bool check_gripper_change(string gripper_state, string prev_gripper_state)
    {
        if (gripper_state != prev_gripper_state) {
            return true;
        } else {
            return false;
        }
    }

    public void toggle_direct_control()
    {
        Debug.Log("Toggling direct control");

        if (direct_control_active) {
            StringMsg action = new StringMsg("stop_action");
            ros_action.Publish(ActionTopic, action);
            direct_control_active = false;
        } else {
            StringMsg action = new StringMsg("start_direct_arm_control");
            ros_action.Publish(ActionTopic, action);

            Vector3 gaze_origin = CoreServices.InputSystem.GazeProvider.GazeOrigin;
            Vector3 gaze_direction = CoreServices.InputSystem.GazeProvider.GazeDirection;
            float x_dif = gaze_direction[0] - gaze_origin[0];
            float z_dif = gaze_direction[2] - gaze_origin[2];
            angle = Mathf.Atan2(z_dif, x_dif) - (Mathf.PI / 2);

            /*if (x_dif < 0)
            {
                angle = angle + Mathf.PI;
            }*/

            System.Threading.Thread.Sleep(2000);
            direct_control_active = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        handRight = HandJointUtils.FindHand(Handedness.Right); 
        if (handRight == null) {

        } else {
            handRight.TryGetJoint(TrackedHandJoint.Palm, out MixedRealityPose PalmPose);
            Vector3 T_Hand_DirectEnv = PalmPose.Position;
            Quaternion R_Hand_DirectEnv_lh = PalmPose.Rotation; 
            Quaternion R_EndEffector = new Quaternion(-R_Hand_DirectEnv_lh.y, R_Hand_DirectEnv_lh.z, R_Hand_DirectEnv_lh.x, R_Hand_DirectEnv_lh.w);

            float z_rot = T_Hand_DirectEnv[2]; // * Mathf.Cos(angle) - T_Hand_DirectEnv[0] * Mathf.Sin(angle);
            float x_rot = T_Hand_DirectEnv[0]; // * Mathf.Sin(angle) + T_Hand_DirectEnv[0] * Mathf.Cos(angle);

            PoseMsg handPalmPose = new PoseMsg() {
                position = new PointMsg((z_rot), -(x_rot), (T_Hand_DirectEnv[1])), 
                orientation = new QuaternionMsg(R_EndEffector[0], R_EndEffector[1], R_EndEffector[2], R_EndEffector[3])
            };

            HeaderMsg handPalmHeader = new HeaderMsg() {
                frame_id = "fr3_link0"
            }; 

            PoseStampedMsg handPalmMessage = new PoseStampedMsg() {
                header = handPalmHeader,
                pose = handPalmPose
            };


            if (direct_control_active)
            {
                handRight.TryGetJoint(TrackedHandJoint.IndexTip, out MixedRealityPose IndexTipPose);
                handRight.TryGetJoint(TrackedHandJoint.ThumbTip, out MixedRealityPose ThumbTipPose);

                // Calculate euclidean distance between index tip and thumb tip
                float dist = Vector3.Distance(IndexTipPose.Position, ThumbTipPose.Position);

                if (dist < 0.03f) {
                    gripper_state = "closed";
                    gripper_state_changed = check_gripper_change(gripper_state, prev_gripper_state);

                    if (gripper_state_changed) {
                        StringMsg gripperStatus = new StringMsg("close");
                        ros_gripper.Publish(gripperOpenOrCloseTopic, gripperStatus);
                    }

                } else {
                    gripper_state = "open";
                    gripper_state_changed = check_gripper_change(gripper_state, prev_gripper_state);

                    if (gripper_state_changed) {
                        StringMsg gripperStatus = new StringMsg("open");
                        ros_gripper.Publish(gripperOpenOrCloseTopic, gripperStatus);
                    }
                }

                prev_gripper_state = gripper_state;

                ros_palm.Publish(HandPosePublisherTopic, handPalmPose);
            } else {            }

        }
    }
}
