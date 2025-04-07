using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private List<MapPrefab> mapObjects = new();
    private List<MapPrefab> mapWalls = new();
    private List<MapPrefab> mapCeilings = new();
    private List<MapPrefab> mapFloors = new();
    private List<MapPrefab> mapEnemies = new();
    private GameObject elevatorParent;

    private MapPrefab spawn = null;
    private List<LockedDoorKey> pickedUpKeys = new();

    [SerializeField]
    private MapPrefab elevatorDoorsPrefab;

    private int secretsCreated = 0;
    private int enemiesCreated = 0;
    private int scoreAvailable = 0;
    private int loreCreated = 0;

    public void Initialize()
    {
        mapObjects = new();
        mapWalls = new();
        pickedUpKeys = new();
        spawn = null;
        secretsCreated = 0;
        enemiesCreated = 0;
        scoreAvailable = 0;
        loreCreated = 0;
    }


    public int GetMaxSecrets()
    {
        return secretsCreated;
    }
    public int GetMaxEnemies()
    {
        return enemiesCreated;
    }
    public int GetMaxScore()
    {
        return scoreAvailable;
    }
    public int GetMaxLore() {
        return loreCreated;
    }

    public bool TryToOpenLockedDoor(int mapId)
    {
        LockedDoorKey foundKey = pickedUpKeys.FirstOrDefault(pKey => pKey.MapId == mapId);
        foreach (LockedDoorKey key2 in pickedUpKeys)
        {
        }
        if (foundKey != null)
        {
            pickedUpKeys.Remove(foundKey);
            UIManager.main.RemoveKeyFromInventory(foundKey);
            Destroy(foundKey.gameObject);
            return true;
        }
        return false;
    }

    public void PickupKey(LockedDoorKey pickupKey)
    {
        pickedUpKeys.Add(pickupKey);
        mapObjects.Remove(pickupKey.GetComponent<MapPrefab>());
    }

    public void ResetKeys()
    {
        pickedUpKeys.Clear();
    }

    public void CalculateSecrets() {
        int secrets = 0;
        foreach (var layer in mapObjects.GroupBy(m => m.MapId))
        {
            foreach(var mapObject in layer) {
                SecretTrigger trigger = mapObject.GetComponent<SecretTrigger>();
                if (trigger != null)
                {
                    secrets += 1;
                    break;
                }
            }
        }
        secretsCreated = secrets;
    }

    public void AddObject(MapPrefab mapPrefab)
    {
        if (mapPrefab.Type == MapPrefabType.Spawn)
        {
            Debug.Log("We've set up spawn");
            spawn = mapPrefab;
        }
        else if (mapPrefab.Type == MapPrefabType.BasicMeleeMob || mapPrefab.Type == MapPrefabType.BasicRangedMob)
        {
            Debug.Log("Add enemy");
            mapEnemies.Add(mapPrefab);
            enemiesCreated++;
        }
        else if (mapPrefab.Type == MapPrefabType.Money)
        {
            Debug.Log("Add money");
            scoreAvailable += mapPrefab.GetComponent<MoneyPickup>().Value;
        }
        else if (mapPrefab.Type == MapPrefabType.LoreMessage)
        {
            loreCreated += 1;
        }

        mapObjects.Add(mapPrefab);
    }

    public void AddWall(MapPrefab mapPrefab)
    {
        mapWalls.Add(mapPrefab);
    }

    public void AddCeiling(MapPrefab mapPrefab)
    {
        mapCeilings.Add(mapPrefab);
    }

    public void AddFloor(MapPrefab mapPrefab)
    {
        mapFloors.Add(mapPrefab);
    }

    public void StartEnemies()
    {
        mapEnemies.ForEach(x => x.GetComponent<MapEnemy>()?.Initialize());
    }

    public void SetupElevator(MapPrefab elevator, Transform parent)
    {
        Debug.Log("Setting up elevator");
        elevatorParent = new GameObject("elevatorParent");
        elevatorParent.transform.parent = transform;
        elevatorParent.tag = "Finish";

        Vector2Int emptyNeighbor = FirstEmptyNeighbor(elevator);
        Vector2Int direction = elevator.Position - emptyNeighbor;
        Vector2Int doorPosition = emptyNeighbor - direction;
        MapPrefab elevatorDoors = Instantiate(elevatorDoorsPrefab, parent);
        elevatorDoors.transform.localPosition = new Vector3(doorPosition.x, 0, doorPosition.y);
        elevatorDoors.transform.parent = elevatorParent.transform;

        ChangeNeighborParents(emptyNeighbor, mapCeilings, elevatorParent.transform);
        ChangeNeighborParents(emptyNeighbor, mapWalls, elevatorParent.transform);
        ChangeNeighborParents(emptyNeighbor, mapFloors, elevatorParent.transform);

        elevator.transform.parent = elevatorParent.transform;
    }

    private void ChangeNeighborParents(Vector2Int origin, List<MapPrefab> tiles, Transform newParent) {
        for (int xPos = -1; xPos <= 1; xPos += 1)
        {
            for (int yPos = -1; yPos <= 1; yPos += 1)
            {
                var pos = origin + new Vector2Int(xPos, yPos);
                var tile = tiles.Find(foundTile => foundTile.Position == pos);
                if (tile != null) {
                    tile.transform.parent = newParent;
                }
            }
        }
    }

    public MapPrefab GetSpawnPoint()
    {
        return spawn;
    }

    public void TriggerSecret(SecretTrigger secretTrigger)
    {
        bool targetsWereFound = false;
        foreach (var mapObject in mapObjects.FindAll(obj => obj.MapId == secretTrigger.SecretId))
        {
            SecretTarget target = mapObject.GetComponent<SecretTarget>();
            if (target != null)
            {
                targetsWereFound = true;
                target.Trigger();
            }
        }
        if (!targetsWereFound) {
            foreach (var mapObject in mapObjects.FindAll(obj => obj.MapId == secretTrigger.SecretId))
            {
                SecretTrigger trigger = mapObject.GetComponent<SecretTrigger>();
                if (trigger != null)
                {
                    trigger.TriggerSelf();
                }
            }
        }
    }

    private Vector2Int FirstEmptyNeighbor(MapPrefab origin, int distance = 1) {
        Vector2Int emptyPos = Vector2Int.up;
        Vector2Int pos = origin.Position;
        for (int xPos = -distance; xPos <= distance; xPos += 1)
        {
            for (int yPos = -distance; yPos <= distance; yPos += 1)
            {
                if (xPos == 0 && yPos == 0) {
                    continue;
                }
                Vector2Int neighborPos = new Vector2Int(xPos + pos.x, yPos + pos.y);
                MapPrefab wall = mapWalls.Find(mapWall => mapWall.Position == neighborPos);
                if (wall == null)
                {
                    emptyPos = neighborPos;
                    break;
                }
            }
        }
        return emptyPos;
    }

    public Quaternion SpawnRotation()
    {
        Vector2Int pos = spawn.Position;
        Vector2Int emptyPos = Vector2Int.up;
        for (int xPos = -1; xPos <= 1; xPos += 1)
        {
            for (int yPos = -1; yPos <= 1; yPos += 1)
            {
                Vector2Int neighborPos = new Vector2Int(xPos + pos.x, yPos + pos.y);
                MapPrefab wall = mapWalls.Find(mapWall => mapWall.Position == neighborPos);
                if (wall == null)
                {
                    emptyPos = neighborPos;
                    break;
                }
            }
        }

        // Convert Vector2Int to Vector3 (assuming Y is 0)
        Vector3 positionA = new Vector3(pos.x, 0, pos.y);
        Vector3 positionB = new Vector3(emptyPos.x, 0, emptyPos.y);

        // Calculate the direction vector from A to B
        Vector3 direction = positionB - positionA;

        // Check if the direction is zero
        if (direction == Vector3.zero)
        {
            // If the direction is zero, return the identity rotation
            return Quaternion.identity;
        }

        // Calculate the rotation using LookRotation
        Quaternion rotation = Quaternion.LookRotation(direction);
        return rotation;
    }

    public void ClearWall(Vector2Int position)
    {
        MapPrefab wallPrefab = mapWalls.FirstOrDefault(wall => wall.Position == position);
        if (wallPrefab != null)
        {
            mapWalls.Remove(wallPrefab);
            Destroy(wallPrefab.gameObject);
        }
    }

}
