using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField]
    private GameObject closedDoor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closedDoor.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseDoor()
    {
        closedDoor.SetActive(true);
        closedDoor.transform.SetParent(LevelManager.main.CurrentLevel.transform);
        transform.SetParent(null);
    }
}
