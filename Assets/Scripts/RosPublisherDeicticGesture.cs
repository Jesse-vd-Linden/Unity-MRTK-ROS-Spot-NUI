using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using UnityEngine.InputSystem;

public class RosPublisherDeicticGesture: MonoBehaviour
{
    ROSConnection ros;
    Vector3 hitPoint;
    public string topicName = "deictic_walk";

    public float fixedDistance = 2.0f; // Define the distance you consider as "where the user is looking at"

    public float LengthPerson = 1.85f; // Length of person to determine the point of contact to the ground

    public float ScaleFactor = 1.2f;

    Vector3 GazePoint;
    public GameObject gameObjectToMove;

    Vector3 CalibrationOrigin = new Vector3(0,0,0);
    float CorrectionAngleForPosition = 0.0f; // in radians please
    double Pi = 3.1415926535d;
    float[,] IdentityMatrix3D = { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
    float[,] RotationMatrixGazeDirection = { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };

    public TextMeshPro PositionLabel;
    public TextMeshPro PositionLabel2;
    public TextMeshPro CountDownTimerCalibration;


    public Handedness selectedHand = Handedness.Right;
    Vector3 endPoint;

    void Start()
    {
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Float32MultiArrayMsg>(topicName);

        CoreServices.DiagnosticsSystem.ShowProfiler = false;
    }

    float[,] MultiplyMatrix(float[,] A, float[,] B)
    {
        int rA = A.GetLength(0);
        int cA = A.GetLength(1);
        int rB = B.GetLength(0);
        int cB = B.GetLength(1);

        if (cA != rB)
        {
            Debug.Log("Matrixes can't be multiplied!!");
            Debug.Log($"{rA}, {cA}, {rB}, {cB}");
            return IdentityMatrix3D;
        }
        else
        {
            float temp = 0;
            float[,] kHasil = new float[rA, cB];

            for (int i = 0; i < rA; i++)
            {
                for (int j = 0; j < cB; j++)
                {
                    temp = 0;
                    for (int k = 0; k < cA; k++)
                    {
                        temp += A[i, k] * B[k, j];
                    }
                    kHasil[i, j] = temp;
                }
            }

            return kHasil;
        }
    }

    float[,] AddMatrix(float[,] A, float[,] B)
    {
        int rA = A.GetLength(0);
        int cA = A.GetLength(1);
        int rB = B.GetLength(0);
        int cB = B.GetLength(1);

        if (rA != rB || cA != cB)
        {
            Debug.Log("Matrixes can't be Added!!");
            return IdentityMatrix3D;
        } else
        {
            float[,] ReturnMatrix = new float[rA, cA];
            for (int i = 0; i < rA; i++)
            {
                for (int j = 0; j < cA; j++)
                {
                    ReturnMatrix[i, j] = A[i, j] +  B[i, j];
                }
            }

            return ReturnMatrix;
        }
    }

    // Update is called once per frame
    public void Update()
    {
        bool isPointerFound = PointerUtils.TryGetHandRayEndPoint(selectedHand, out endPoint);

        if (isPointerFound)
        {
            // Do something with the endpoint, like displaying or logging it
            Debug.Log("Hand Ray Endpoint: " + endPoint.ToString());
        }

        Vector3 PersonLocation = CoreServices.InputSystem.GazeProvider.GazeOrigin;
        Vector3 GazeDirectionPerson = CoreServices.InputSystem.GazeProvider.GazeDirection;
        Vector3 GazePointOriginal = CalculatePlanarGazeLocation(PersonLocation, GazeDirectionPerson);

        Vector3 PersonLocationCalibrated = CorrectPersonPositionFromCalibration();
        float[,] GazeDirectionForCalibration = { { GazeDirectionPerson.x }, { GazeDirectionPerson.y}, { GazeDirectionPerson.z} };
        float[,] result = MultiplyMatrix(RotationMatrixGazeDirection, GazeDirectionForCalibration);
        Vector3 GazeDirectionVectorCalibrated = new Vector3(result[0, 0], result[1, 0], result[2, 0]);

        GazePoint = CalculatePlanarGazeLocation(PersonLocationCalibrated, GazeDirectionVectorCalibrated);

        //PositionLabel.text = string.Format("Position: ({0:F2}, {1:F2}, {2:F2})", GazePoint.x, GazePoint.y, GazePoint.z);
        //PositionLabel2.text = string.Format("Position: ({0:F2}, {1:F2}, {2:F2})", GazePoint.x, GazePoint.y, GazePoint.z);

        gameObjectToMove.transform.position = ScaleVector3(GazePointOriginal, ScaleFactor);
    }

    Vector3 ScaleVector3(Vector3 vector, float scale)
    {
        float x = vector.x * scale;
        float y = vector.y;
        float z = vector.z * scale;

        Vector3 NewVector = new Vector3(x, y, z);

        return NewVector;
    }

    Vector3 CorrectPersonPositionFromCalibration()
    {
        Vector3 OffsetCorrectedPosition = CoreServices.InputSystem.GazeProvider.GazeOrigin - CalibrationOrigin;

        float z = (float) (Math.Cos(CorrectionAngleForPosition) * OffsetCorrectedPosition.z - Math.Sin(CorrectionAngleForPosition) * (-1 * OffsetCorrectedPosition.x));
        float x = (float) (Math.Sin(CorrectionAngleForPosition) * OffsetCorrectedPosition.z + Math.Cos(CorrectionAngleForPosition) * (-1 * OffsetCorrectedPosition.x));
        float y = OffsetCorrectedPosition.y;

        Vector3 PersonLocationCalibrated = new Vector3((-1 * x), y, z);

        return OffsetCorrectedPosition;

    }

    // Update is called once per frame
    public void CalibratePositionAndDirection()
    {
        /*// timer and looking at target instruction
        for (int i = 0; i >= 0; i--)
        {
            Thread.Sleep(1000);
            string timeCountDown = string.Format("00:{0:F2}", i); 
            CountDownTimerCalibration.text = timeCountDown;
        }
        CountDownTimerCalibration.text = new string("");*/

        CalibrationOrigin = CoreServices.InputSystem.GazeProvider.GazeOrigin;

        Vector3 GazeDirection = CoreServices.InputSystem.GazeProvider.GazeDirection;
        Vector3 GoalDirection = new Vector3(0,0,1);
        RodriguezVectorCalibration(GazeDirection, GoalDirection);

        double DivideSidesForAngle = GazeDirection.z / (-1 * GazeDirection.x);
        if (GazeDirection.z > 0.0f)
        {
            CorrectionAngleForPosition = (float)Math.Atan(DivideSidesForAngle);
        }
        else if (GazeDirection.z < 0.0f && GazeDirection.x < 0.0f)
        {
            CorrectionAngleForPosition = (float)(Math.Atan(DivideSidesForAngle) + Pi);

        }
        else if (GazeDirection.z < 0.0f && GazeDirection.x > 0.0f)
        {
            CorrectionAngleForPosition = (float)(Math.Atan(DivideSidesForAngle) - Pi);

        }

        CorrectionAngleForPosition = CorrectionAngleForPosition * -1;
    }

    void RodriguezVectorCalibration(Vector3 GazeDirection, Vector3 GoalDirection)
    {
        float i = (GazeDirection.y * GoalDirection.z) - (GazeDirection.z * GoalDirection.y);
        float j = (GazeDirection.z * GoalDirection.x) - (GazeDirection.x * GoalDirection.z);
        float k = (GazeDirection.x * GoalDirection.y) - (GazeDirection.y * GoalDirection.x);

        double AbsoluteLengthSub = Math.Pow(i, 2) + Math.Pow(j, 2) + Math.Pow(k, 2);
        float AbsoluteLength = (float)Math.Pow(AbsoluteLengthSub, 0.5f);

        float[] CrossProductNormalized = { i / AbsoluteLength , j / AbsoluteLength, k / AbsoluteLength };

        float dot_x = GazeDirection.x * GoalDirection.x;
        float dot_y = GazeDirection.y * GoalDirection.y;
        float dot_z = GazeDirection.z * GoalDirection.z;

        float DotProductAngle = (float) Math.Acos((dot_x + dot_y + dot_z));

        float a_x = CrossProductNormalized[0];
        float a_y = CrossProductNormalized[1];
        float a_z = CrossProductNormalized[2];

        float[,] SkewSymmetricMatrix = { {0, -1*a_z, a_y }, { a_z, 0, -1*a_x }, { -1*a_y, a_x, 0} };

        float[,] SkewSymmetricMatrixSquared = MultiplyMatrix(SkewSymmetricMatrix, SkewSymmetricMatrix);


        float SineFactor = (float)Math.Sin(DotProductAngle);
        float CosineFactor = (float)(1 - Math.Cos(DotProductAngle));

        float[,] SkewSymmetricMatrixSin = ApplyScalarMultiplicationOnArray(SkewSymmetricMatrix, SineFactor);
        float[,] SkewSymmetricMatrixSquaredCos = ApplyScalarMultiplicationOnArray(SkewSymmetricMatrixSquared, CosineFactor);

        float[,] SubResult = AddMatrix(SkewSymmetricMatrixSin, SkewSymmetricMatrixSquaredCos);
        RotationMatrixGazeDirection = AddMatrix(IdentityMatrix3D, SubResult);
        Debug.Log(RotationMatrixGazeDirection);
    }

    float[,] ApplyScalarMultiplicationOnArray(float[,] array, float factor)
    {
        int rows = array.GetLength(0);
        int columns = array.GetLength(1);

        float[,] ReturnMatrix = new float[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                ReturnMatrix[i, j] = array[i,j] * factor;
            }
        }

        return ReturnMatrix;
    }

    Vector3 CalculatePlanarGazeLocation(Vector3 PersonLocation, Vector3 GazeDirection)
    {
        PersonLocation[1] = PersonLocation[1] + LengthPerson - 0.05f;

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

        Vector3 point = new Vector3(x, -LengthPerson, z);

        return point;
    }

    public void SendWalk()
    {
        Vector3 PersonLocation = CorrectPersonPositionFromCalibration();

        float[] message = new float[6];
        message[0] = PersonLocation[2];
        message[1] = -1 * PersonLocation[0];
        message[2] = PersonLocation[1];

        message[3] = GazePoint[2];
        message[4] = -1 * GazePoint[0];
        message[5] = GazePoint[1];

        Float32MultiArrayMsg Points = new Float32MultiArrayMsg()
        {
            data = message
        };

        ros.Publish(topicName, Points);
        Debug.Log("Deictic Walk");
    }
}
