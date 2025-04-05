using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private List<MapPrefab> mapObjects = new();
    private List<MapPrefab> mapWalls = new();
    public void Initialize()
    {
        mapObjects = new();
        mapWalls = new();
    }

    public void AddObject(MapPrefab mapPrefab) {
        mapObjects.Add(mapPrefab);
    }

    public void AddWall(MapPrefab mapPrefab)
    {
        mapWalls.Add(mapPrefab);
    }

    public void TriggerSecret(int secretId) {
        foreach (var mapObject in mapObjects.FindAll(obj => obj.MapId == secretId)) {
            SecretTarget target = mapObject.GetComponent<SecretTarget>();
            if (target != null) {
                target.Trigger();
            }
        }
    }

    public void ClearWall(Vector2Int position) {
        MapPrefab wallPrefab = mapWalls.FirstOrDefault(wall => wall.Position == position);
        if (wallPrefab != null) {
            mapWalls.Remove(wallPrefab);
            Destroy(wallPrefab.gameObject);
        }
    }

    public void GetTriggerTarger(int triggerId) {

    }
}
