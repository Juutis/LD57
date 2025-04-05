using System.Runtime.CompilerServices;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    private float minDistance = 0.5f;
    private float distanceCheckInterval = 0.1f;
    private float distanceCheckTimer = 0.1f;

    private bool isActive = true;


    void Update()
    {
        if (!isActive) {
            return;
        }
        distanceCheckTimer += Time.deltaTime;
        if (distanceCheckTimer > distanceCheckInterval) {
            distanceCheckTimer = 0f;
            if (MapGenerator.main.Player == null) {
                return;
            }
            if (Vector3.Distance(MapGenerator.main.Player.transform.position, transform.position) <= minDistance) {
                HandlePickup();
                Kill();
            }
        }
    }

    private void Kill() {
        isActive = false;
        gameObject.SetActive(false);
    }

    void HandlePickup() {
        LockedDoorKey key = GetComponent<LockedDoorKey>();
        if (key != null) {
            MapGenerator.main.PickupKey(key);
        }
        LoreMessage loreMessage = GetComponent<LoreMessage>();
        Debug.Log("Showing lore..");
        if (loreMessage != null)
        {
            Debug.Log("Showing lore..");
            string msg = MapGenerator.main.GetLoreMessage();
            if (msg != "") {
                UIManager.main.ShowMessage(msg);
            }
        }
    }
}
