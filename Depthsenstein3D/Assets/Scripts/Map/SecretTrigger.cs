using UnityEngine;

public class SecretTrigger : MonoBehaviour
{

    private int secretId;

    private bool hasTriggered = false;

    private Vector2Int tilePosition;
    public int SecretId {get {return secretId;}}
    public void Initialize(TileMapTileData mapTileData)
    {
        tilePosition = mapTileData.Position;
        secretId = mapTileData.MapId;
    }

    public void Trigger() {
        if (hasTriggered) {
            return;
        }

        MapGenerator.main.TriggerSecret(this);

        hasTriggered = true;
    }

    public void TriggerSelf() {
        MapGenerator.main.ClearWall(tilePosition);
    }
    

    void Update()
    {
        // uncomment to test
        /*if (Input.GetKeyDown(KeyCode.Space)) {
            Trigger();
        }*/
    }

}
