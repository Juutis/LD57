using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private List<MapPrefab> mapObjects = new();
    private List<MapPrefab> mapWalls = new();
    private List<MapPrefab> mapCeilings = new();
    private List<MapPrefab> mapFloors = new();
    private GameObject elevatorParent;

    private MapPrefab spawn = null;
    private List<LockedDoorKey> pickedUpKeys = new();

    public void Initialize()
    {
        mapObjects = new();
        mapWalls = new();
        pickedUpKeys = new();
        spawn = null;
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

    public void AddObject(MapPrefab mapPrefab)
    {
        if (mapPrefab.Type == MapPrefabType.Spawn)
        {
            spawn = mapPrefab;
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

    public void SetupElevator(MapPrefab elevator)
    {
        elevatorParent = new GameObject("elevatorParent");
        elevatorParent.AddComponent<Elevator>();
        elevatorParent.transform.parent = transform;

        int elevatorX = elevator.TileMapTileData.Position.x;
        int elevatorY = elevator.TileMapTileData.Position.y;

        foreach (MapPrefab ceiling in mapCeilings)
        {
            MoveToElevatorParentIfNeeded(elevatorX, elevatorY, ceiling);
        }

        foreach (MapPrefab floor in mapFloors)
        {
            MoveToElevatorParentIfNeeded(elevatorX, elevatorY, floor);
        }

        foreach (MapPrefab wall in mapWalls)
        {
            MoveToElevatorParentIfNeeded(elevatorX, elevatorY, wall);
        }

        elevator.transform.parent = elevatorParent.transform;
    }

    private void MoveToElevatorParentIfNeeded(int elevatorX, int elevatorY, MapPrefab ceiling)
    {
        int objX = ceiling.TileMapTileData.Position.x;
        int objY = ceiling.TileMapTileData.Position.y;

        if (objX > (elevatorX - 2) && objX < (elevatorX + 2) && objY > (elevatorY - 3) && objY < (elevatorY + 1))
        {
            ceiling.transform.parent = elevatorParent.transform;
        }
    }

    public MapPrefab GetSpawnPoint()
    {
        return spawn;
    }

    public void TriggerSecret(int secretId)
    {
        foreach (var mapObject in mapObjects.FindAll(obj => obj.MapId == secretId))
        {
            SecretTarget target = mapObject.GetComponent<SecretTarget>();
            if (target != null)
            {
                target.Trigger();
            }
        }
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
