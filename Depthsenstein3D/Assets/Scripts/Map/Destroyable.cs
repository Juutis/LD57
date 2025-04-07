using UnityEngine;

public class Destroyable : MonoBehaviour
{
    [SerializeField]
    private bool isEnabled;

    private string spriteName;
    private int HP = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Init(string name)
    {
        spriteName = name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hit()
    {
        if (!isEnabled)
        {
            return;
        }


        HP--;

        if (HP <= 0 )
        {
            gameObject.GetComponent<Renderer>().material.mainTexture = MapPrefabHelper.main.ExtractTexture(LevelManager.main.GetDestroyedSprite(spriteName));
        }
    }
}
