using UnityEngine;

public class MapBillboard : MonoBehaviour
{
    private Transform mainCameraTransform;
    public Vector3 upDirection = Vector3.up; // Default: world up

    private void GetCamera() {
        if (mainCameraTransform == null) {
            mainCameraTransform = Camera.main?.transform;
        }
    }

    void LateUpdate()
    {
        if (mainCameraTransform == null) {
            GetCamera();
            return;
        }
        transform.LookAt(mainCameraTransform);
        transform.forward = -transform.forward;
    }
}

