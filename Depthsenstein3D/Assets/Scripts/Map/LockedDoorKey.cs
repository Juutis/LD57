using UnityEngine;

public class LockedDoorKey : MonoBehaviour
{
    private int mapId;
    public int MapId { get { return mapId; } }
    private Sprite sprite;
    public void Initialize(int mapId, Sprite sprite)
    {
        this.mapId = mapId;
        this.sprite = sprite;
    }

    public Sprite GetSprite() {
        return sprite;
    }

}
