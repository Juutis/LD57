using UnityEngine;

public class SecretTarget : MonoBehaviour
{

    private int secretId;

    private bool hasTriggered = false;

    private Vector2Int tilePosition;
    public void Initialize(TileMapTileData mapTileData)
    {
        tilePosition = mapTileData.Position;
        secretId = mapTileData.MapId;
    }

    public void Trigger()
    {
        if (hasTriggered)
        {
            return;
        }

        MapGenerator.main.ClearWall(tilePosition);

        hasTriggered = true;
    }

}
