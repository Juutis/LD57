using UnityEngine;

public class LockedDoor : MonoBehaviour
{

    private int mapId;

    public void Initialize(int mapId) {
        this.mapId = mapId;
        foreach(Transform child in transform) {
            BoxCollider boxCollider = child.GetComponent<BoxCollider>();
            if (boxCollider != null) {
                boxCollider.enabled = false;
            }
        }
    }

    public bool TryToOpen() {
        if (MapGenerator.main.TryToOpenLockedDoor(mapId)) {
            Destroy(gameObject);
            return true;
        }
        return false;
    }
}
