using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MapPrefab : MonoBehaviour
{
    [SerializeField]
    private MapPrefabType mapPrefabType;

    public MapPrefabType Type { get { return mapPrefabType;}}
    [SerializeField]
    private VisualizationType visualizationType;

    private TileMapTileData tileMapTileData;

    public TileMapTileData TileMapTileData { get { return tileMapTileData; } }


    public Vector2Int Position {get {return tileMapTileData.Position; }}
    public int MapId { get {return tileMapTileData.MapId; }}

    public void Spawn(TileMapTileData mapTileData)
    {
        tileMapTileData = mapTileData;

        Sprite sprite = tileMapTileData.Tile?.Sprite ?? tileMapTileData.Sprite;
        // visualization
        if (visualizationType == VisualizationType.TexturedCube)
        {
            GameObject cube = MapPrefabHelper.main.CreateTexturedCube(sprite, transform);
        }
        else if (visualizationType == VisualizationType.Billboard)
        {
            GameObject billboard = MapPrefabHelper.main.CreateBillboardSprite(sprite, transform);
        }

        // custom
        if (mapPrefabType == MapPrefabType.SecretTrigger) {
            SecretTrigger secretTrigger = GetComponent<SecretTrigger>();
            secretTrigger.Initialize(tileMapTileData);
        }
        else if (mapPrefabType == MapPrefabType.SecretTarget) {
            SecretTarget secretTarget = GetComponent<SecretTarget>();
            secretTarget.Initialize(tileMapTileData);
        }
        else if (mapPrefabType == MapPrefabType.Door)
        {
            transform.GetChild(0).AddComponent<Door>();
        }
        else if (mapPrefabType == MapPrefabType.LockedDoor)
        {
            LockedDoor lockedDoor = GetComponent<LockedDoor>();
            lockedDoor.Initialize(tileMapTileData.MapId);
        }
        else if (mapPrefabType == MapPrefabType.Key)
        {
            LockedDoorKey key = GetComponent<LockedDoorKey>();
            key.Initialize(tileMapTileData.MapId, sprite);
        }
        else if (mapPrefabType == MapPrefabType.Elevator)
        {
            MapGenerator.main.SetupElevator(this);
            transform.GetChild(0).AddComponent<ElevatorSwitch>();
        }
        else if (mapPrefabType == MapPrefabType.SpecialWall)
        {
            TextureSwapper swapper = GetComponent<TextureSwapper>();
            swapper.Init(tileMapTileData.Sprite);
        } else if (mapPrefabType == MapPrefabType.Gun) {
            GetComponent<Gun>().Initialize(sprite);
        }

        // positioning & naming
        SpawnPrefab(new Vector3(tileMapTileData.Position.x, 0, tileMapTileData.Position.y));
    }

    private void SpawnPrefab(Vector3 position)
    {
        transform.localPosition = position;
        string typeName = mapPrefabType != MapPrefabType.None ? $"{mapPrefabType}" : "-";
        name = $"[X{position.x} Y{position.z}] {typeName}";
    }
}


public enum VisualizationType
{
    TexturedCube,
    Billboard,
    Custom
}


public enum MapPrefabType
{
    None,
    Floor,
    Wall,
    Door,
    Spawn,
    Elevator,
    SecretTrigger,
    SecretTarget,
    Ammo,
    Gun,
    HP,
    Money,
    Key,
    LockedDoor,
    BasicMeleeMob,
    BasicRangedMob,
    Clutter,
    LoreMessage,
    SpecialWall,
    ElevatorDoors

}