using UnityEngine;

public class EnableObjectOnDisable : MonoBehaviour
{
    public GameObject objectToEnable;
    private void OnDisable()
    {
        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);
        }
    }
}
