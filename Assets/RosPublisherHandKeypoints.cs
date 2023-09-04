using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;
using Microsoft.MixedReality.Toolkit;

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

public class RosPublisherHandKeypoints : MonoBehaviour
{
    private IMixedRealityHand handRight;

    private MixedRealityPose Wrist;

    private MixedRealityPose ThumbMetacarpalJoint;
    private MixedRealityPose ThumbProximalJoint;
    private MixedRealityPose ThumbDistalJoint;
    private MixedRealityPose ThumbTip;

    //private MixedRealityPose IndexMetacarpal; // Not avaible in mediapipe
    private MixedRealityPose IndexKnuckle;
    private MixedRealityPose IndexMiddleJoint;
    private MixedRealityPose IndexDistalJoint;
    private MixedRealityPose IndexTip;

    //private MixedRealityPose MiddleMetacarpal; // Not avaible in mediapipe
    private MixedRealityPose MiddleKnuckle;
    private MixedRealityPose MiddleMiddleJoint;
    private MixedRealityPose MiddleDistalJoint;
    private MixedRealityPose MiddleTip;

    //private MixedRealityPose RingMetacarpal; // Not avaible in mediapipe
    private MixedRealityPose RingKnuckle;
    private MixedRealityPose RingMiddleJoint;
    private MixedRealityPose RingDistalJoint;
    private MixedRealityPose RingTip;

    //private MixedRealityPose PinkyMetacarpal; // Not avaible in mediapipe
    private MixedRealityPose PinkyKnuckle;
    private MixedRealityPose PinkyMiddleJoint;
    private MixedRealityPose PinkyDistalJoint;
    private MixedRealityPose PinkyTip;

    ROSConnection ros;

    [SerializeField]
    private string HandKeypointsPublisherTopic = "hand_keypoints";

    // Start is called before the first frame update
    void Start()
    {
        // Ros for hand keypoints
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Float32MultiArrayMsg>(HandKeypointsPublisherTopic);
    }

    public void SendHandPose()
    {
        handRight = HandJointUtils.FindHand(Handedness.Right);
        if (handRight == null)
        {
            handRight = HandJointUtils.FindHand(Handedness.Right);
        }
        else
        {
            handRight = HandJointUtils.FindHand(Handedness.Right);

            float[] MainList = new float[63];
            //int i = 0;
            /*foreach (MixedRealityPose keypoint in System.Enum.GetValues(typeof(TrackedHandJoint)))
            {
                float x_pos = keypoint.Position.x;
                float y_pos = keypoint.Position.y;
                float z_pos = keypoint.Position.z;
                //float[] list = new float[3] { x_pos, y_pos, z_pos };

                MainList[i] = x_pos;
                MainList[(i+1)] = y_pos;
                MainList[(i+2)] = z_pos;
                i = i + 3;
            }
            //Debug.Log(MainList);*/

            // Get all joints 21 that are the same to mediapipe's keypoints
            handRight.TryGetJoint(TrackedHandJoint.Palm, out MixedRealityPose Wrist);

            handRight.TryGetJoint(TrackedHandJoint.ThumbMetacarpalJoint, out MixedRealityPose ThumbMetacarpalJoint);
            handRight.TryGetJoint(TrackedHandJoint.ThumbProximalJoint, out MixedRealityPose ThumbProximalJoint);
            handRight.TryGetJoint(TrackedHandJoint.ThumbDistalJoint, out MixedRealityPose ThumbDistalJoint);
            handRight.TryGetJoint(TrackedHandJoint.ThumbTip, out MixedRealityPose ThumbTip);

            handRight.TryGetJoint(TrackedHandJoint.IndexKnuckle, out MixedRealityPose IndexKnuckle);
            handRight.TryGetJoint(TrackedHandJoint.IndexMiddleJoint, out MixedRealityPose IndexMiddleJoint);
            handRight.TryGetJoint(TrackedHandJoint.IndexDistalJoint, out MixedRealityPose IndexDistalJoint);
            handRight.TryGetJoint(TrackedHandJoint.IndexTip, out MixedRealityPose IndexTip);

            handRight.TryGetJoint(TrackedHandJoint.MiddleKnuckle, out MixedRealityPose MiddleKnuckle);
            handRight.TryGetJoint(TrackedHandJoint.MiddleMiddleJoint, out MixedRealityPose MiddleMiddleJoint);
            handRight.TryGetJoint(TrackedHandJoint.MiddleDistalJoint, out MixedRealityPose MiddleDistalJoint);
            handRight.TryGetJoint(TrackedHandJoint.MiddleTip, out MixedRealityPose MiddleTip);

            handRight.TryGetJoint(TrackedHandJoint.RingKnuckle, out MixedRealityPose RingKnuckle);
            handRight.TryGetJoint(TrackedHandJoint.RingMiddleJoint, out MixedRealityPose RingMiddleJoint);
            handRight.TryGetJoint(TrackedHandJoint.RingDistalJoint, out MixedRealityPose RingDistalJoint);
            handRight.TryGetJoint(TrackedHandJoint.RingTip, out MixedRealityPose RingTip);

            handRight.TryGetJoint(TrackedHandJoint.PinkyKnuckle, out MixedRealityPose PinkyKnuckle);
            handRight.TryGetJoint(TrackedHandJoint.PinkyMiddleJoint, out MixedRealityPose PinkyMiddleJoint);
            handRight.TryGetJoint(TrackedHandJoint.PinkyDistalJoint, out MixedRealityPose PinkyDistalJoint);
            handRight.TryGetJoint(TrackedHandJoint.PinkyTip, out MixedRealityPose PinkyTip);

            int i = 0;

            Vector3 JointPos = Wrist.Position;
            MainList[i] = -1 * JointPos.y;
            MainList[(i + 1)] = -1 * JointPos.x;
            MainList[(i + 2)] = JointPos.z;
            i = i + 3;

            Vector3 ThumbMetacarpalJointPos = ThumbMetacarpalJoint.Position;
            MainList[i] = -1 * ThumbMetacarpalJointPos.y;
            MainList[(i + 1)] = -1 * ThumbMetacarpalJointPos.x;
            MainList[(i + 2)] = ThumbMetacarpalJointPos.z;
            i = i + 3;
            Vector3 ThumbProximalJointPos = ThumbProximalJoint.Position;
            MainList[i] = -1 * ThumbProximalJointPos.y;
            MainList[(i + 1)] = -1 * ThumbProximalJointPos.x;
            MainList[(i + 2)] = ThumbProximalJointPos.z;
            i = i + 3;
            Vector3 ThumbDistalJointPos = ThumbDistalJoint.Position;
            MainList[i] = -1 * ThumbDistalJointPos.y;
            MainList[(i + 1)] = -1 * ThumbDistalJointPos.x;
            MainList[(i + 2)] = ThumbDistalJointPos.z;
            i = i + 3;
            Vector3 ThumbTipPos = ThumbTip.Position;
            MainList[i] = -1 * ThumbTipPos.y;
            MainList[(i + 1)] = -1 * ThumbTipPos.x;
            MainList[(i + 2)] = ThumbTipPos.z;
            i = i + 3;

            Vector3 IndexKnucklePos = IndexKnuckle.Position;
            MainList[i] = -1 * IndexKnucklePos.y;
            MainList[(i + 1)] = -1 * IndexKnucklePos.x;
            MainList[(i + 2)] = IndexKnucklePos.z;
            i = i + 3;
            Vector3 IndexMiddleJointPos = IndexMiddleJoint.Position;
            MainList[i] = -1 * IndexMiddleJointPos.y;
            MainList[(i + 1)] = -1 * IndexMiddleJointPos.x;
            MainList[(i + 2)] = IndexMiddleJointPos.z;
            i = i + 3;
            Vector3 IndexDistalJointPos = IndexDistalJoint.Position;
            MainList[i] = -1 * IndexDistalJointPos.y;
            MainList[(i + 1)] = -1 * IndexDistalJointPos.x;
            MainList[(i + 2)] = IndexDistalJointPos.z;
            i = i + 3;
            Vector3 IndexTipPos = IndexTip.Position;
            MainList[i] = -1 * IndexTipPos.y;
            MainList[(i + 1)] = -1 * IndexTipPos.x;
            MainList[(i + 2)] = IndexTipPos.z;
            i = i + 3;

            Vector3 MiddleKnucklePos = MiddleKnuckle.Position;
            MainList[i] = -1 * MiddleKnucklePos.y;
            MainList[(i + 1)] = -1 * MiddleKnucklePos.x;
            MainList[(i + 2)] = MiddleKnucklePos.z;
            i = i + 3;
            Vector3 MiddleMiddleJointPos = MiddleMiddleJoint.Position;
            MainList[i] = -1 * MiddleMiddleJointPos.y;
            MainList[(i + 1)] = -1 * MiddleMiddleJointPos.x;
            MainList[(i + 2)] = MiddleMiddleJointPos.z;
            i = i + 3;
            Vector3 MiddleDistalJointPos = MiddleDistalJoint.Position;
            MainList[i] = -1 * MiddleDistalJointPos.y;
            MainList[(i + 1)] = -1 * MiddleDistalJointPos.x;
            MainList[(i + 2)] = MiddleDistalJointPos.z;
            i = i + 3;
            Vector3 MiddleTipPos = MiddleTip.Position;
            MainList[i] = -1 * MiddleTipPos.y;
            MainList[(i + 1)] = -1 * MiddleTipPos.x;
            MainList[(i + 2)] = MiddleTipPos.z;
            i = i + 3;

            Vector3 RingKnucklePos = RingKnuckle.Position;
            MainList[i] = -1 * RingKnucklePos.y;
            MainList[(i + 1)] = -1 * RingKnucklePos.x;
            MainList[(i + 2)] = RingKnucklePos.z;
            i = i + 3;
            Vector3 RingMiddleJointPos = RingMiddleJoint.Position;
            MainList[i] = -1 * RingMiddleJointPos.y;
            MainList[(i + 1)] = -1 * RingMiddleJointPos.x;
            MainList[(i + 2)] = RingMiddleJointPos.z;
            i = i + 3;
            Vector3 RingDistalJointPos = RingDistalJoint.Position;
            MainList[i] = -1 * RingDistalJointPos.y;
            MainList[(i + 1)] = -1 * RingDistalJointPos.x;
            MainList[(i + 2)] = RingDistalJointPos.z;
            i = i + 3;
            Vector3 RingTipPos = RingTip.Position;
            MainList[i] = -1 * RingTipPos.y;
            MainList[(i + 1)] = -1 * RingTipPos.x;
            MainList[(i + 2)] = RingTipPos.z;
            i = i + 3;

            Vector3 PinkyKnucklePos = PinkyKnuckle.Position;
            MainList[i] = -1 * PinkyKnucklePos.y;
            MainList[(i + 1)] = -1 * PinkyKnucklePos.x;
            MainList[(i + 2)] = PinkyKnucklePos.z;
            i = i + 3;
            Vector3 PinkyMiddleJointPos = PinkyMiddleJoint.Position;
            MainList[i] = -1 * PinkyMiddleJointPos.y;
            MainList[(i + 1)] = -1 * PinkyMiddleJointPos.x;
            MainList[(i + 2)] = PinkyMiddleJointPos.z;
            i = i + 3;
            Vector3 PinkyDistalJointPos = PinkyDistalJoint.Position;
            MainList[i] = -1 * PinkyDistalJointPos.y;
            MainList[(i + 1)] = -1 * PinkyDistalJointPos.x;
            MainList[(i + 2)] = PinkyDistalJointPos.z;
            i = i + 3;
            Vector3 PinkyTipPos = PinkyTip.Position;
            MainList[i] = -1 * PinkyTipPos.y;
            MainList[(i + 1)] = -1 * PinkyTipPos.x;
            MainList[(i + 2)] = PinkyTipPos.z;
            i = i + 3;

            /*PoseMsg WristPose = new PoseMsg()
            {
                position = new PointMsg(WristPos[2], WristPos[0], WristPos[1]),
            };

            HeaderMsg handPalmHeader = new HeaderMsg()
            {
                frame_id = "fr3_link0"
            };

            PoseStampedMsg handPalmMessage = new PoseStampedMsg()
            {
                header = handPalmHeader,
                pose = WristPose
            };*/

            Float32MultiArrayMsg Points = new Float32MultiArrayMsg()
            {
                data = MainList
            };

            ros.Publish(HandKeypointsPublisherTopic, Points);

        }
    }

        // Update is called once per frame, if the function name is: Update()
    void UpdateNOT()
    {
        handRight = HandJointUtils.FindHand(Handedness.Right);
        if (handRight == null) {
            handRight = HandJointUtils.FindHand(Handedness.Right);
        }
        else {
            handRight = HandJointUtils.FindHand(Handedness.Right);

            float[] MainList = new float[63];
            //int i = 0;
            /*foreach (MixedRealityPose keypoint in System.Enum.GetValues(typeof(TrackedHandJoint)))
            {
                float x_pos = keypoint.Position.x;
                float y_pos = keypoint.Position.y;
                float z_pos = keypoint.Position.z;
                //float[] list = new float[3] { x_pos, y_pos, z_pos };

                MainList[i] = x_pos;
                MainList[(i+1)] = y_pos;
                MainList[(i+2)] = z_pos;
                i = i + 3;
            }
            //Debug.Log(MainList);*/

            // Get all joints 21 that are the same to mediapipe's keypoints
            handRight.TryGetJoint(TrackedHandJoint.Palm, out MixedRealityPose Wrist);

            handRight.TryGetJoint(TrackedHandJoint.ThumbMetacarpalJoint, out MixedRealityPose ThumbMetacarpalJoint);
            handRight.TryGetJoint(TrackedHandJoint.ThumbProximalJoint, out MixedRealityPose ThumbProximalJoint);
            handRight.TryGetJoint(TrackedHandJoint.ThumbDistalJoint, out MixedRealityPose ThumbDistalJoint);
            handRight.TryGetJoint(TrackedHandJoint.ThumbTip, out MixedRealityPose ThumbTip);

            handRight.TryGetJoint(TrackedHandJoint.IndexKnuckle, out MixedRealityPose IndexKnuckle);
            handRight.TryGetJoint(TrackedHandJoint.IndexMiddleJoint, out MixedRealityPose IndexMiddleJoint);
            handRight.TryGetJoint(TrackedHandJoint.IndexDistalJoint, out MixedRealityPose IndexDistalJoint);
            handRight.TryGetJoint(TrackedHandJoint.IndexTip, out MixedRealityPose IndexTip);

            handRight.TryGetJoint(TrackedHandJoint.MiddleKnuckle, out MixedRealityPose MiddleKnuckle);
            handRight.TryGetJoint(TrackedHandJoint.MiddleMiddleJoint, out MixedRealityPose MiddleMiddleJoint);
            handRight.TryGetJoint(TrackedHandJoint.MiddleDistalJoint, out MixedRealityPose MiddleDistalJoint);
            handRight.TryGetJoint(TrackedHandJoint.MiddleTip, out MixedRealityPose MiddleTip);

            handRight.TryGetJoint(TrackedHandJoint.RingKnuckle, out MixedRealityPose RingKnuckle);
            handRight.TryGetJoint(TrackedHandJoint.RingMiddleJoint, out MixedRealityPose RingMiddleJoint);
            handRight.TryGetJoint(TrackedHandJoint.RingDistalJoint, out MixedRealityPose RingDistalJoint);
            handRight.TryGetJoint(TrackedHandJoint.RingTip, out MixedRealityPose RingTip);

            handRight.TryGetJoint(TrackedHandJoint.PinkyKnuckle, out MixedRealityPose PinkyKnuckle);
            handRight.TryGetJoint(TrackedHandJoint.PinkyMiddleJoint, out MixedRealityPose PinkyMiddleJoint);
            handRight.TryGetJoint(TrackedHandJoint.PinkyDistalJoint, out MixedRealityPose PinkyDistalJoint);
            handRight.TryGetJoint(TrackedHandJoint.PinkyTip, out MixedRealityPose PinkyTip);

            int i = 0;

            Vector3 JointPos = Wrist.Position;
            MainList[i] = -1 * JointPos.y;
            MainList[(i + 1)] = -1 * JointPos.x;
            MainList[(i + 2)] = JointPos.z;
            i = i + 3;

            Vector3 ThumbMetacarpalJointPos = ThumbMetacarpalJoint.Position;
            MainList[i] = -1 * ThumbMetacarpalJointPos.y;
            MainList[(i + 1)] = -1 * ThumbMetacarpalJointPos.x;
            MainList[(i + 2)] = ThumbMetacarpalJointPos.z;
            i = i + 3;
            Vector3 ThumbProximalJointPos = ThumbProximalJoint.Position;
            MainList[i] = -1 * ThumbProximalJointPos.y;
            MainList[(i + 1)] = -1 * ThumbProximalJointPos.x;
            MainList[(i + 2)] = ThumbProximalJointPos.z;
            i = i + 3;
            Vector3 ThumbDistalJointPos = ThumbDistalJoint.Position;
            MainList[i] = -1 * ThumbDistalJointPos.y;
            MainList[(i + 1)] = -1 * ThumbDistalJointPos.x;
            MainList[(i + 2)] = ThumbDistalJointPos.z;
            i = i + 3;
            Vector3 ThumbTipPos = ThumbTip.Position;
            MainList[i] = -1 * ThumbTipPos.y;
            MainList[(i + 1)] = -1 * ThumbTipPos.x;
            MainList[(i + 2)] = ThumbTipPos.z;
            i = i + 3;

            Vector3 IndexKnucklePos = IndexKnuckle.Position;
            MainList[i] = -1 * IndexKnucklePos.y;
            MainList[(i + 1)] = -1 * IndexKnucklePos.x;
            MainList[(i + 2)] = IndexKnucklePos.z;
            i = i + 3;
            Vector3 IndexMiddleJointPos = IndexMiddleJoint.Position;
            MainList[i] = -1 * IndexMiddleJointPos.y;
            MainList[(i + 1)] = -1 * IndexMiddleJointPos.x;
            MainList[(i + 2)] = IndexMiddleJointPos.z;
            i = i + 3;
            Vector3 IndexDistalJointPos = IndexDistalJoint.Position;
            MainList[i] = -1 * IndexDistalJointPos.y;
            MainList[(i + 1)] = -1 * IndexDistalJointPos.x;
            MainList[(i + 2)] = IndexDistalJointPos.z;
            i = i + 3;
            Vector3 IndexTipPos = IndexTip.Position;
            MainList[i] = -1 * IndexTipPos.y;
            MainList[(i + 1)] = -1 * IndexTipPos.x;
            MainList[(i + 2)] = IndexTipPos.z;
            i = i + 3;

            Vector3 MiddleKnucklePos = MiddleKnuckle.Position;
            MainList[i] = -1 * MiddleKnucklePos.y;
            MainList[(i + 1)] = -1 * MiddleKnucklePos.x;
            MainList[(i + 2)] = MiddleKnucklePos.z;
            i = i + 3;
            Vector3 MiddleMiddleJointPos = MiddleMiddleJoint.Position;
            MainList[i] = -1 * MiddleMiddleJointPos.y;
            MainList[(i + 1)] = -1 * MiddleMiddleJointPos.x;
            MainList[(i + 2)] = MiddleMiddleJointPos.z;
            i = i + 3;
            Vector3 MiddleDistalJointPos = MiddleDistalJoint.Position;
            MainList[i] = -1 * MiddleDistalJointPos.y;
            MainList[(i + 1)] = -1 * MiddleDistalJointPos.x;
            MainList[(i + 2)] = MiddleDistalJointPos.z;
            i = i + 3;
            Vector3 MiddleTipPos = MiddleTip.Position;
            MainList[i] = -1 * MiddleTipPos.y;
            MainList[(i + 1)] = -1 * MiddleTipPos.x;
            MainList[(i + 2)] = MiddleTipPos.z;
            i = i + 3;

            Vector3 RingKnucklePos = RingKnuckle.Position;
            MainList[i] = -1 * RingKnucklePos.y;
            MainList[(i + 1)] = -1 * RingKnucklePos.x;
            MainList[(i + 2)] = RingKnucklePos.z;
            i = i + 3;
            Vector3 RingMiddleJointPos = RingMiddleJoint.Position;
            MainList[i] = -1 * RingMiddleJointPos.y;
            MainList[(i + 1)] = -1 * RingMiddleJointPos.x;
            MainList[(i + 2)] = RingMiddleJointPos.z;
            i = i + 3;
            Vector3 RingDistalJointPos = RingDistalJoint.Position;
            MainList[i] = -1 * RingDistalJointPos.y;
            MainList[(i + 1)] = -1 * RingDistalJointPos.x;
            MainList[(i + 2)] = RingDistalJointPos.z;
            i = i + 3;
            Vector3 RingTipPos = RingTip.Position;
            MainList[i] = -1 * RingTipPos.y;
            MainList[(i + 1)] = -1 * RingTipPos.x;
            MainList[(i + 2)] = RingTipPos.z;
            i = i + 3;

            Vector3 PinkyKnucklePos = PinkyKnuckle.Position;
            MainList[i] = -1 * PinkyKnucklePos.y;
            MainList[(i + 1)] = -1 * PinkyKnucklePos.x;
            MainList[(i + 2)] = PinkyKnucklePos.z;
            i = i + 3;
            Vector3 PinkyMiddleJointPos = PinkyMiddleJoint.Position;
            MainList[i] = -1 * PinkyMiddleJointPos.y;
            MainList[(i + 1)] = -1 * PinkyMiddleJointPos.x;
            MainList[(i + 2)] = PinkyMiddleJointPos.z;
            i = i + 3;
            Vector3 PinkyDistalJointPos = PinkyDistalJoint.Position;
            MainList[i] = -1 * PinkyDistalJointPos.y;
            MainList[(i + 1)] = -1 * PinkyDistalJointPos.x;
            MainList[(i + 2)] = PinkyDistalJointPos.z;
            i = i + 3;
            Vector3 PinkyTipPos = PinkyTip.Position;
            MainList[i] = -1 * PinkyTipPos.y;
            MainList[(i + 1)] = -1 * PinkyTipPos.x;
            MainList[(i + 2)] = PinkyTipPos.z;
            i = i + 3;

            /*PoseMsg WristPose = new PoseMsg()
            {
                position = new PointMsg(WristPos[2], WristPos[0], WristPos[1]),
            };

            HeaderMsg handPalmHeader = new HeaderMsg()
            {
                frame_id = "fr3_link0"
            };

            PoseStampedMsg handPalmMessage = new PoseStampedMsg()
            {
                header = handPalmHeader,
                pose = WristPose
            };*/

            Float32MultiArrayMsg Points = new Float32MultiArrayMsg()
            {
                data = MainList
            };

            ros.Publish(HandKeypointsPublisherTopic, Points);

        }
    }
}