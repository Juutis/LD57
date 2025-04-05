using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    float rotateSpeed = 60f;
    float moveSpeed = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + h * rotateSpeed * Time.deltaTime, 0);
        transform.position = transform.position + transform.forward * v * moveSpeed * Time.deltaTime;



        if (Input.GetKeyDown(KeyCode.Space))
        {
            Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, 1f, ~0);

            if (hitInfo.collider != null)
            {
                GameObject target = hitInfo.collider.gameObject;

                if (target.TryGetComponent(out Door door))
                {
                    Destroy(door.gameObject);
                }
                else if (target.TryGetComponent(out ElevatorSwitch elevatorSwitch))
                {
                    Debug.Log("Load level");
                }
            }
        }
    }
}
