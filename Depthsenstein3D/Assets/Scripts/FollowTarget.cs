using System.Diagnostics;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField]
    private bool followX;
    [SerializeField]
    private bool followY;
    [SerializeField]
    private bool followZ;

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private Transform target;

    public void SetTarget(Transform target) {
        this.target = target;
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;

        if (followX) {
            pos.x = target.position.x + offset.x;
        }
        if (followY) {
            pos.y = target.position.y + offset.y;
        }
        if (followZ) {
            pos.z = target.position.z + offset.z;
        }

        transform.position = pos;
    }
}
