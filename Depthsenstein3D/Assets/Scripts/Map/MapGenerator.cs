using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;
using System;

public class MapGenerator : MonoBehaviour
{

    public static MapGenerator main;

    void Awake()
    {
        main = this;
    }

    [SerializeField]
    private Tilemap floorMap;
    [SerializeField]
    private Tilemap ceilingMap;
    [SerializeField]
    private Tilemap wallMap;
    [SerializeField]
    private List<Tilemap> objectMaps;
    [SerializeField]
    private TileConfigScriptableObject tileConfig;
    [SerializeField]
    private Transform mapContainer;

    public Transform Container { get { return mapContainer; } }

    [SerializeField]

    private Transform containerPrefab;

    [SerializeField]
    private PlayerTest playerCharacterPrefab;
    private PlayerTest player;

    public PlayerTest Player { get { return player; } }

    Transform floorContainer;
    Transform ceilingContainer;
    Transform wallContainer;
    Transform objectContainer;

    private List<LevelLoreMessage> loreMessages;

    [SerializeField]
    private LoreMessageConfigScriptableObject loreMessageConfig;

    [SerializeField]
    private MapManager mapManager;

    private int ceilingLayer;

    void Start()
    {
        ceilingLayer = LayerMask.NameToLayer("Ceiling");
        Generate();
    }

    public int GetMaxSecrets() {
        return mapManager.GetMaxSecrets();
    }
    public int GetMaxEnemies() {
        return mapManager.GetMaxEnemies();
    }
    public int GetMaxScore() {
        return mapManager.GetMaxScore();
    }
    public int GetMaxLore() {
        return mapManager.GetMaxLore();
    }
    private void SetupContainers()
    {
        floorContainer = Instantiate(containerPrefab, mapContainer);
        floorContainer.transform.localPosition = new Vector3(0, -1, 0f);
        floorContainer.name = "FLOOR";
        ceilingContainer = Instantiate(containerPrefab, mapContainer);
        ceilingContainer.transform.localPosition = new Vector3(0, 1, 0f);
        ceilingContainer.name = "CEILING";
        wallContainer = Instantiate(containerPrefab, mapContainer);
        wallContainer.name = "WALL";
        objectContainer = Instantiate(containerPrefab, mapContainer);
        objectContainer.name = "OBJECTS";
    }

    public void Generate()
    {
        loreMessages = loreMessageConfig.LoreMessages;
        mapManager.Initialize();
        SetupContainers();
        int mapId = 0;
        LoopTiles(floorMap, mapId++, SpawnFloorTile);
        LoopTiles(ceilingMap, mapId++, SpawnCeilingTile);
        LoopTiles(wallMap, mapId++, SpawnWallTile);

        foreach (var objectMap in objectMaps)
        {
            LoopTiles(objectMap, mapId++, SpawnObject);
        }

        SpawnPlayer();
        mapManager.CalculateSecrets();
        player.Stats.ResetCurrentStats();
    }

    public void SpawnPlayer()
    {
        if (player == null)
        {
            MapPrefab spawn = mapManager.GetSpawnPoint();

            player = FindFirstObjectByType<PlayerTest>();
            if (player != null) {
                return;
            }
            if (spawn == null)
            {
                if (player == null)
                {
                    Debug.LogError("No spawn and player was not found!");
#if UNITY_EDITOR
                    player = Instantiate(playerCharacterPrefab, null);

                    if (spawn != null)
                    {
                        player.transform.localPosition = new Vector3(spawn.Position.x, 0, spawn.Position.y);
                        player.transform.rotation = mapManager.SpawnRotation();
                    }
#endif
                }

                return;
            }

            player = Instantiate(playerCharacterPrefab, null);

            if (spawn != null)
            {
                player.transform.localPosition = new Vector3(spawn.Position.x, 0, spawn.Position.y);
                player.transform.rotation = mapManager.SpawnRotation();
            }
        }
    }

    public void StartEnemies()
    {
        mapManager.StartEnemies();
    }

    public void ClearWall(Vector2Int position)
    {
        mapManager.ClearWall(position);
    }

    public bool TryToOpenLockedDoor(int mapId)
    {
        return mapManager.TryToOpenLockedDoor(mapId);
    }

    public string GetLoreMessage()
    {

        int level = 0;
        if (LevelManager.main != null)
        {
            level = LevelManager.main.CurrentLevelNum;
        }
        LevelLoreMessage message = loreMessages.FirstOrDefault(
            msg => msg.Level == level
        );
        if (message == null)
        {
            return "";
        }
        loreMessages.Remove(message);
        return message.Message;
    }

    public void PickupKey(LockedDoorKey pickupKey)
    {
        mapManager.PickupKey(pickupKey);
        UIManager.main.AddKeyToInventory(pickupKey);
    }

    public void TriggerSecret(SecretTrigger secretTrigger)
    {
        mapManager.TriggerSecret(secretTrigger);
    }

    public void SetupElevator(MapPrefab elevator)
    {
        mapManager.SetupElevator(elevator, objectContainer);
    }

    private void SpawnFloorTile(TileMapTileData mapTileData)
    {
        var prefab = tileConfig.DefaultTexturedCube;
        if (mapTileData.Tile != null && mapTileData.Tile.Prefab != null)
        {
            prefab = mapTileData.Tile.Prefab;
        }
        MapPrefab spawnedTile = Instantiate(prefab, floorContainer);
        spawnedTile.Spawn(mapTileData);
        mapManager.AddFloor(spawnedTile);
    }


    private void SpawnCeilingTile(TileMapTileData mapTileData)
    {
        var prefab = tileConfig.DefaultTexturedCube;
        if (mapTileData.Tile != null && mapTileData.Tile.Prefab != null)
        {
            prefab = mapTileData.Tile.Prefab;
        }
        MapPrefab spawnedTile = Instantiate(prefab, ceilingContainer);
        spawnedTile.Spawn(mapTileData);
        setLayers(spawnedTile.gameObject, ceilingLayer);
        mapManager.AddCeiling(spawnedTile);
    }

    private void SpawnWallTile(TileMapTileData mapTileData)
    {
        var prefab = mapTileData.Tile?.Prefab ?? tileConfig.DefaultTexturedCube;

        MapPrefab spawnedTile = Instantiate(prefab, wallContainer);
        spawnedTile.Spawn(mapTileData);
        mapManager.AddWall(spawnedTile);
    }

    private void SpawnObject(TileMapTileData mapTileData)
    {
        var prefab = mapTileData.Tile?.Prefab ?? tileConfig.DefaultBillboard;

        MapPrefab spawnedTile = Instantiate(prefab, objectContainer);
        spawnedTile.Spawn(mapTileData);
        mapManager.AddObject(spawnedTile);
    }

    private void LoopTiles(Tilemap tilemap, int mapId, ProcessSingleTile processTileCallback)
    {
        // loop through tilemap nodes
        if (tilemap == null)
        {
            Debug.LogError("Tilemap not assigned!");
            return;
        }

        BoundsInt bounds = tilemap.cellBounds;

        for (int xPos = bounds.min.x; xPos < bounds.max.x; xPos += 1)
        {
            for (int yPos = bounds.min.y; yPos < bounds.max.y; yPos += 1)
            {
                Vector3Int cellPosition = new Vector3Int(xPos, yPos, 0);
                Tile tile = tilemap.GetTile(cellPosition) as Tile;

                if (tile == null) { continue; }
                if (tile.sprite == null) { continue; }

                CustomTile customTile = tileConfig.GetTile(tile.sprite);

                processTileCallback(new TileMapTileData
                {
                    Tile = customTile,
                    Sprite = tile.sprite,
                    Position = (Vector2Int)cellPosition,
                    MapId = mapId
                }
                );
            }
        }
    }

    public void InitAINavigation() {
        NavMeshManager.Main.BuildNavMesh();
    }

    private void setLayers(GameObject gameObject, int layer) {
        gameObject.layer = layer;
        foreach(Transform t in gameObject.transform) {
            setLayers(t.gameObject, layer);
        }
    }

}

public struct TileMapTileData
{
    public CustomTile Tile;
    public Sprite Sprite;
    public Vector2Int Position;
    public int MapId; // used with secret triggers
}

public delegate void ProcessSingleTile(TileMapTileData mapData);