using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField]
    private GameObject closedDoor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Fix closed door for elevator");
        closedDoor?.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseDoor()
    {
        Debug.Log("Fix closed door for elevator");
        closedDoor?.SetActive(true);
        closedDoor?.transform.SetParent(LevelManager.main.CurrentLevel.transform);
        transform.SetParent(null);
    }
}
