using UnityEngine;
using UnityEngine.Events;

public class ElevatorDoors : MonoBehaviour
{

    [SerializeField]
    private Animator animator;

    private UnityAction closeCallback;
    private UnityAction openCallback;

    [SerializeField]
    private MeshRenderer leftDoor;
    [SerializeField]
    private MeshRenderer rightDoor;
    [SerializeField]
    private BoxCollider elevatorCollider;

    private bool doorsOpened = false;
    public void Initialize(Texture2D doorTexture)
    {
        leftDoor.material.mainTexture = doorTexture;
        rightDoor.material.mainTexture = doorTexture;
    }

    public void OpenDoors(UnityAction openCallback) {
        animator.Play("elevatorDoorsOpen");
        this.openCallback = openCallback;
    }

    public void OpenDoorsFromPlayerAction(UnityAction openCallback) {
        if (doorsOpened) {return;}
        doorsOpened = true;
        OpenDoors(openCallback);
    }

    public void CloseDoors(UnityAction closeCallback)
    {
        this.closeCallback = closeCallback;
        animator.Play("elevatorDoorsClose");
    }

    public void OpenDoorsFinished() {
        elevatorCollider.enabled = false;
        if (openCallback != null) {
            openCallback.Invoke();
            openCallback = null;
        }
    }

    public void CloseDoorsFinished() {
        elevatorCollider.enabled = true;
        if (closeCallback != null)
        {
            closeCallback.Invoke();
            closeCallback = null;
        }
    }

}
