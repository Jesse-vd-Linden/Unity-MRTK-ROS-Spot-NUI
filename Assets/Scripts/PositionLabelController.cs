using UnityEngine;
using TMPro;

public class PositionLabelController : MonoBehaviour
{
    public GameObject targetObject;  // Drag the object you want to track in the inspector
    public TextMeshPro positionLabel;  // Drag the TextMeshPro object you created earlier in the inspector

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (targetObject != null && positionLabel != null)
        {
            // Update the text to show the target object's position
            Vector3 targetPosition = targetObject.transform.position;

            float x = targetPosition.z;
            float y = -1 * targetPosition.x;
            float z = targetPosition.y;

            positionLabel.text = string.Format("Position: ({0:F2}, {1:F2}, {2:F2})",
                                                x, y, z);
        }
    }
}