using UnityEngine;

public class MapEnemy : MonoBehaviour
{
    [SerializeField]
    private RangedEnemy enemy;
    [SerializeField]
    private Boss boss;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetPos()
    {
        if (enemy != null) enemy.ResetPos();
        if (boss != null) boss.ResetPos();
    }

    public void Initialize()
    {
        if (enemy != null) enemy.Initialize();
        if (boss != null) boss.Initialize();
    }
}
