using UnityEngine;

public class MapEnemy : MonoBehaviour
{
    [SerializeField]
    private RangedEnemy enemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize()
    {
        enemy.Initialize();
    }
}
