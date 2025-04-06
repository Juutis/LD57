using UnityEngine;

public class SecretTrigger : MonoBehaviour
{

    private int secretId;

    private bool hasTriggered = false;
    private bool hasTriggeredSelf = false;

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
        if (hasTriggeredSelf)
        {
            return;
        }
        hasTriggeredSelf = true;
        MapGenerator.main.ClearWall(tilePosition);
        //Destroy(gameObject);
    }
    

    void Update()
    {
        // uncomment to test
        /*if (Input.GetKeyDown(KeyCode.Space)) {
            Trigger();
        }*/
    }

}
