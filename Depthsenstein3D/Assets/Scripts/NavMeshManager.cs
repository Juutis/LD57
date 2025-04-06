using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshManager : MonoBehaviour
{
    public static NavMeshManager Main;
    
    private NavMeshSurface surface;

    void Awake()
    {
        Main = this;
        surface = GetComponent<NavMeshSurface>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildNavMesh() {
        surface.RemoveData();
        surface.BuildNavMesh();
    }
}
