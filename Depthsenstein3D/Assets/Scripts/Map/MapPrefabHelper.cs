using System.Collections.Generic;
using UnityEngine;

public class MapPrefabHelper : MonoBehaviour
{
    public static MapPrefabHelper main;



    [SerializeField]
    private GameObject cubePrefab;
    [SerializeField]
    private GameObject billboardPrefab;

    private Dictionary<Sprite, Texture2D> textureCache = new Dictionary<Sprite, Texture2D>();

    public GameObject CreateBillboardSprite(Sprite sprite, Transform parent)
    {
        GameObject billboard = Instantiate(billboardPrefab, parent);
        MeshRenderer billboardMesh = billboard.GetComponent<MeshRenderer>();
        billboardMesh.material.mainTexture = ExtractTexture(sprite);
        return billboard;
    }


    public GameObject CreateTexturedCube(Sprite sprite, Transform parent)
    {
        GameObject cube = Instantiate(cubePrefab, parent);
        MeshRenderer cubeMesh = cube.GetComponent<MeshRenderer>();
        cubeMesh.material.mainTexture = ExtractTexture(sprite);
        return cube;
    }

    public Texture2D ExtractTexture(Sprite sprite)
    {
        if (textureCache.ContainsKey(sprite))
        {
            return textureCache[sprite];
        }

        Texture2D originalTexture = sprite.texture;
        Rect spriteRect = sprite.rect;

        // Create a new texture with the size of the sprite
        Texture2D subTexture = new Texture2D((int)spriteRect.width, (int)spriteRect.height);
        subTexture.filterMode = FilterMode.Point;

        // Copy the pixels from the original texture to the new texture
        Color[] pixels = originalTexture.GetPixels((int)spriteRect.x, (int)spriteRect.y, (int)spriteRect.width, (int)spriteRect.height);
        subTexture.SetPixels(pixels);
        subTexture.Apply();
        textureCache[sprite] = subTexture;
        return subTexture;
    }

    void Awake()
    {
        main = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
