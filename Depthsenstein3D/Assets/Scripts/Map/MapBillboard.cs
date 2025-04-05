using UnityEngine;

public class MapBillboard : MonoBehaviour
{
    private Transform mainCameraTransform;
    public Vector3 upDirection = Vector3.up; // Default: world up

    void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(mainCameraTransform);
        transform.forward = -transform.forward;
    }
}

