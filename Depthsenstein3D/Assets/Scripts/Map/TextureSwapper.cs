using UnityEngine;

public class TextureSwapper : MonoBehaviour
{
    [SerializeField]
    private Renderer rend;

    [SerializeField]
    private Sprite sprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (sprite != null) {
            Init(sprite);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(Sprite sprite) {
        if (this.sprite != null) {
            var texture = MapPrefabHelper.main.ExtractTexture(this.sprite);
            rend.material.mainTexture = texture;
        } else {
            var texture = MapPrefabHelper.main.ExtractTexture(sprite);
            rend.material.mainTexture = texture;
        }
    }
}
