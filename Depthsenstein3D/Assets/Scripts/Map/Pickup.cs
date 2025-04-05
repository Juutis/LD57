using UnityEngine;

public class Pickup : MonoBehaviour
{

    private float minDistance = 0.5f;
    private float distanceCheckInterval = 0.1f;
    private float distanceCheckTimer = 0.1f;


    void Update()
    {
        distanceCheckTimer += Time.deltaTime;
        if (distanceCheckTimer > distanceCheckInterval) {
            distanceCheckTimer = 0f;
            if (MapGenerator.main.Player == null) {
                return;
            }
            if (Vector3.Distance(MapGenerator.main.Player.transform.position, transform.position) <= minDistance) {
                Debug.Log($"hi! You found {gameObject.name}");
                Destroy(gameObject);
            }
        }
    }
}
