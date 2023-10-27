using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;


public class RosPublisherPinchGesture : MonoBehaviour
{
    private IMixedRealityHand handRight;
    private IMixedRealityHand handLeft;

    private MixedRealityPose PinkTipPoseRight;
    private MixedRealityPose RingTipPoseRight;
    private MixedRealityPose MiddleTipPoseRight;
    private MixedRealityPose IndexTipPoseRight;
    private MixedRealityPose ThumbTipPoseRight;

    private MixedRealityPose PinkTipPoseLeft;
    private MixedRealityPose RingTipPoseLeft;
    private MixedRealityPose MiddleTipPoseLeft;
    private MixedRealityPose IndexTipPoseLeft;
    private MixedRealityPose ThumbTipPoseLeft;

    ROSConnection ros;
    private string GesturePublisherTopic = "gesture_recognition";

    private float DistanceThreshold = 0.02f;

    // Start is called before the first frame update
    void Start()
    {
        //mrirac_trajectory_planner_Fr3/unity_hand_pose
        // Ros for action
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<StringMsg>(GesturePublisherTopic);

    }

    // Update is called once per frame
    void Update()
    {
        handRight = HandJointUtils.FindHand(Handedness.Right);
        handLeft = HandJointUtils.FindHand(Handedness.Left);
        if ((handRight == null) && (handLeft == null))
        {
            return;
        }
        else if (handLeft == null)
        {
            handRight.TryGetJoint(TrackedHandJoint.PinkyTip, out MixedRealityPose PinkTipPoseRight);
            handRight.TryGetJoint(TrackedHandJoint.RingTip, out MixedRealityPose RingTipPoseRight);
            handRight.TryGetJoint(TrackedHandJoint.MiddleTip, out MixedRealityPose MiddleTipPoseRight);
            handRight.TryGetJoint(TrackedHandJoint.IndexTip, out MixedRealityPose IndexTipPoseRight);
            handRight.TryGetJoint(TrackedHandJoint.ThumbTip, out MixedRealityPose ThumbTipPoseRight);

            // perform action based on distance between finger tips
            if (Vector3.Distance(IndexTipPoseRight.Position, ThumbTipPoseRight.Position) < DistanceThreshold)
            {
                StringMsg action = new StringMsg("walk_to_forward");
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
            else if (Vector3.Distance(MiddleTipPoseRight.Position, ThumbTipPoseRight.Position) < DistanceThreshold)
            {
                StringMsg action = new StringMsg("walk_to_right");
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
            else if (Vector3.Distance(RingTipPoseRight.Position, ThumbTipPoseRight.Position) < DistanceThreshold)
            {
                StringMsg action = new StringMsg("turn_to_right");
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
            else if (Vector3.Distance(PinkTipPoseRight.Position, ThumbTipPoseRight.Position) < DistanceThreshold)
            {
                StringMsg action = new StringMsg("sit_down");
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
            else
            {
                StringMsg action = new StringMsg("no_gesture");
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
        }
        else if (handRight == null)
        {

            handLeft.TryGetJoint(TrackedHandJoint.PinkyTip, out MixedRealityPose PinkTipPoseLeft);
            handLeft.TryGetJoint(TrackedHandJoint.RingTip, out MixedRealityPose RingTipPoseLeft);
            handLeft.TryGetJoint(TrackedHandJoint.MiddleTip, out MixedRealityPose MiddleTipPoseLeft);
            handLeft.TryGetJoint(TrackedHandJoint.IndexTip, out MixedRealityPose IndexTipPoseLeft);
            handLeft.TryGetJoint(TrackedHandJoint.ThumbTip, out MixedRealityPose ThumbTipPoseLeft);

            // perform action based on distance between finger tips
            if (Vector3.Distance(IndexTipPoseLeft.Position, ThumbTipPoseLeft.Position) < DistanceThreshold)
            {
                StringMsg action = new StringMsg("walk_to_backward");
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
            else if (Vector3.Distance(MiddleTipPoseLeft.Position, ThumbTipPoseLeft.Position) < DistanceThreshold)
            {
                StringMsg action = new StringMsg("walk_to_left");
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
            else if (Vector3.Distance(RingTipPoseLeft.Position, ThumbTipPoseLeft.Position) < DistanceThreshold)
            {
                StringMsg action = new StringMsg("turn_to_left");
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
            else if (Vector3.Distance(PinkTipPoseLeft.Position, ThumbTipPoseLeft.Position) < DistanceThreshold)
            {
                StringMsg action = new StringMsg("stand_up");
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
            else
            {
                StringMsg action = new StringMsg("no_gesture");
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
        }
        else
        {

            handRight.TryGetJoint(TrackedHandJoint.PinkyTip, out MixedRealityPose PinkTipPoseRight);
            handRight.TryGetJoint(TrackedHandJoint.RingTip, out MixedRealityPose RingTipPoseRight);
            handRight.TryGetJoint(TrackedHandJoint.MiddleTip, out MixedRealityPose MiddleTipPoseRight);
            handRight.TryGetJoint(TrackedHandJoint.IndexTip, out MixedRealityPose IndexTipPoseRight);
            handRight.TryGetJoint(TrackedHandJoint.ThumbTip, out MixedRealityPose ThumbTipPoseRight);

            handLeft.TryGetJoint(TrackedHandJoint.PinkyTip, out MixedRealityPose PinkTipPoseLeft);
            handLeft.TryGetJoint(TrackedHandJoint.RingTip, out MixedRealityPose RingTipPoseLeft);
            handLeft.TryGetJoint(TrackedHandJoint.MiddleTip, out MixedRealityPose MiddleTipPoseLeft);
            handLeft.TryGetJoint(TrackedHandJoint.IndexTip, out MixedRealityPose IndexTipPoseLeft);
            handLeft.TryGetJoint(TrackedHandJoint.ThumbTip, out MixedRealityPose ThumbTipPoseLeft);

            // perform action based on distance between finger tips
            if (Vector3.Distance(IndexTipPoseRight.Position, ThumbTipPoseRight.Position) < DistanceThreshold)
            {
                StringMsg action = new StringMsg("walk_to_forward");
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
            else if (Vector3.Distance(MiddleTipPoseRight.Position, ThumbTipPoseRight.Position) < DistanceThreshold)
            {
                StringMsg action = new StringMsg("walk_to_right");
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
            else if (Vector3.Distance(RingTipPoseRight.Position, ThumbTipPoseRight.Position) < DistanceThreshold)
            {
                Debug.Log("rotate_left");
                StringMsg action = new StringMsg("turn_to_right");
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
            else if (Vector3.Distance(PinkTipPoseRight.Position, ThumbTipPoseRight.Position) < DistanceThreshold)
            {
                StringMsg action = new StringMsg("sit_down");
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
            else if (Vector3.Distance(IndexTipPoseLeft.Position, ThumbTipPoseLeft.Position) < DistanceThreshold)
            {
                StringMsg action = new StringMsg("walk_to_backwards");
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
            else if (Vector3.Distance(MiddleTipPoseLeft.Position, ThumbTipPoseLeft.Position) < DistanceThreshold)
            {
                StringMsg action = new StringMsg("walk_to_left");
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
            else if (Vector3.Distance(RingTipPoseLeft.Position, ThumbTipPoseLeft.Position) < DistanceThreshold)
            {
                StringMsg action = new StringMsg("turn_to_left");
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
            else if (Vector3.Distance(PinkTipPoseLeft.Position, ThumbTipPoseLeft.Position) < DistanceThreshold)
            {
                StringMsg action = new StringMsg("stand_up");
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
            else if (Vector3.Distance(IndexTipPoseLeft.Position, IndexTipPoseRight.Position) < DistanceThreshold)
            {
                //StringMsg action = new StringMsg("pickup_object");
                StringMsg action = new StringMsg("stand_up"); //Temporary until pickup object is fixed
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
            else if (Vector3.Distance(MiddleTipPoseLeft.Position, MiddleTipPoseRight.Position) < DistanceThreshold)
            {
                StringMsg action = new StringMsg("start_direct_arm_control");
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
            else if (Vector3.Distance(RingTipPoseLeft.Position, RingTipPoseRight.Position) < DistanceThreshold)
            {
                StringMsg action = new StringMsg("start_gaze"); // Reserved for walk to object ?
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
            else if (Vector3.Distance(PinkTipPoseLeft.Position, PinkTipPoseRight.Position) < DistanceThreshold)
            {
                StringMsg action = new StringMsg("start_trajectory");
                ros.Publish(GesturePublisherTopic, action);
                return;
            } 
            else
            {
                StringMsg action = new StringMsg("no_gesture");
                ros.Publish(GesturePublisherTopic, action);
                return;
            }
        }
    }
}
