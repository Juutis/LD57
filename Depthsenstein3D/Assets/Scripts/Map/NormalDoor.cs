using UnityEngine;

public class NormalDoor : MonoBehaviour
{
    public void Initialize()
    {
        foreach (Transform child in transform)
        {
            BoxCollider boxCollider = child.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.enabled = false;
            }
        }
    }
}
