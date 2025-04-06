using UnityEngine;

public class ElevatorSwitch : MonoBehaviour
{
    private bool hasBeenUsed = false;
    public void Use() {
        if (hasBeenUsed) {return;}
        hasBeenUsed = true;
        LevelManager.main.LoadNextLevel(transform.parent.GetComponent<MapPrefab>().Position);
    }
}
