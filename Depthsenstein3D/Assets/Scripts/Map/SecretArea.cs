using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SecretArea : MonoBehaviour
{
    int secretId = 0;

    private bool isActive = true;

    private float minDistance = 0.25f;
    private float distanceCheckInterval = 0.1f;
    private float distanceCheckTimer = 0.1f;


    public void Initialize(int mapId)
    {
        secretId = mapId;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isActive)
        {
            return;
        }

        distanceCheckTimer += Time.deltaTime;

        if (distanceCheckTimer > distanceCheckInterval)
        {
            distanceCheckTimer = 0f;

            if (MapGenerator.main.Player == null)
            {
                return;
            }

            if (Vector3.Distance(MapGenerator.main.Player.transform.position, transform.position) <= minDistance)
            {
                SoundManager.main.PlaySound(GameSoundType.SecretArea);
                List<int> secrets = MapGenerator.main.Player.Stats.SecretsFound;
                if (!secrets.Contains(secretId))
                {
                    secrets.Add(secretId);
                }
                Debug.Log("Secret area!");
                Kill();
            }
        }
    }

    public void Kill() {
        MapGenerator.main.ClearSecretAreas(secretId);
    }
}
