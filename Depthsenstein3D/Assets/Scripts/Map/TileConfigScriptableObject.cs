using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileConfig", menuName = "TileConfig")]
public class TileConfigScriptableObject : ScriptableObject
{
    [SerializeField]
    private MapPrefab defaultTexturedCube;

    public MapPrefab DefaultTexturedCube { get { return defaultTexturedCube; } }
    
    [SerializeField]
    private MapPrefab defaultBillboard;

    public MapPrefab DefaultBillboard { get { return defaultBillboard; } }


    [SerializeField]
    private List<CustomTile> customTiles = new List<CustomTile>();

    public CustomTile GetTile(Sprite sprite)
    {
        return customTiles.Find(x => x.Sprite.name == sprite.name);
    }
}


[System.Serializable]
public class CustomTile
{
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private MapPrefab prefab;
    public Sprite Sprite { get { return sprite; } }
    public MapPrefab Prefab { get { return prefab; } }
}