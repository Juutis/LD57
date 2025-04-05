using UnityEngine;

public class LockedDoorKey : MonoBehaviour
{
    private int mapId;
    public int MapId { get { return mapId; } }
    public void Initialize(int mapId)
    {
        this.mapId = mapId;
    }

}
