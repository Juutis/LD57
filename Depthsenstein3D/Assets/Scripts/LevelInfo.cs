using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    [SerializeField]
    private Vector2 spawnPos;
    [SerializeField]
    private Vector2 elevatorPos;
    [SerializeField]
    private float levelYRotation;
    [SerializeField]
    private int depth;

    public int Depth { get { return depth; } }
    public Vector2 SpawnPos { get { return spawnPos; } }
    public Vector2 ElevatorPos { get { return elevatorPos; } }
    public float LevelYRotation { get { return levelYRotation; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
