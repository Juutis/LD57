using UnityEngine;

public class SecretTarget : MonoBehaviour
{

    private int secretId;

    private bool hasTriggered = false;

    private Vector2Int tilePosition;
    public void Initialize(int secretId, Vector2Int tilePosition)
    {
        this.tilePosition = tilePosition;
        this.secretId = secretId;
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
