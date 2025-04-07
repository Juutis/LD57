using System.Runtime.CompilerServices;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    private float minDistance = 0.75f;
    private float distanceCheckInterval = 0.1f;
    private float distanceCheckTimer = 0.1f;

    private bool isActive = true;


    void Update()
    {
        if (!isActive) {
            return;
        }

        distanceCheckTimer += Time.deltaTime;

        if (distanceCheckTimer > distanceCheckInterval)
        {
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
            SoundManager.main.PlaySound(GameSoundType.KeyPickup);
        }
        LoreMessage loreMessage = GetComponent<LoreMessage>();
        if (loreMessage != null)
        {
            Debug.Log("Showing lore..");
            string msg = MapGenerator.main.GetLoreMessage();
            SoundManager.main.PlaySound(GameSoundType.LorePickup);
            MapGenerator.main.Player.Stats.LoreFound += 1;
            if (msg != "") {
                UIManager.main.ShowMessage(msg);
            }
        }

        if (TryGetComponent(out Ammo gunAmmo))
        {
            FpsManager.Main.AddAmmo(gunAmmo.GunIndex, gunAmmo.AmmoAmount);
            SoundManager.main.PlaySound(GameSoundType.AmmoPickup);
        }

        if (TryGetComponent(out MoneyPickup moneyPickup))
        {
            UIManager.main.AddScore(moneyPickup.Value);
            MapGenerator.main.Player.Stats.ScoreGained += moneyPickup.Value;
            SoundManager.main.PlaySound(GameSoundType.MoneyPickup);
        }

        if (TryGetComponent(out HPPickup hpPickup))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerTest>().Heal(hpPickup.Value);
        }

        if (TryGetComponent(out Gun gun))
        {
            FpsManager.Main.EnableGun(gun.GunIndex, gun.Sprite);
            SoundManager.main.PlaySound(GameSoundType.GunPickup);
        }
    }
}
