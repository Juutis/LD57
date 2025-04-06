using System;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    float rotateSpeed = 120f;
    float mouseSensitivity = 1f;
    float moveSpeed = 3f;
    private Rigidbody rb;
    private Vector2 moveInput;
    public bool UseMouse = true;

    public float Health = 100;
    public float MaxHealth = 100;
    public bool Dead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        Health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (UseMouse)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            moveInput = new Vector2(x, y);

            float rot = Input.GetAxisRaw("Mouse X");
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + rot * mouseSensitivity, 0);
        }
        else
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            moveInput = new Vector2(0.0f, v);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + h * rotateSpeed * Time.deltaTime, 0);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, 1f, ~0);
            Debug.DrawLine(transform.position, transform.position + transform.forward, Color.red, 5f);

            Debug.Log($"{(hitInfo.collider != null ? "hit" : "nop")} | ${(hitInfo.collider != null ? hitInfo.collider.gameObject.name : "null")}");

            if (hitInfo.collider != null)
            {
                GameObject target = hitInfo.collider.gameObject;

                if (target.TryGetComponent(out Door door))
                {
                    Destroy(door.gameObject);
                }
                else if (target.TryGetComponent(out SecretTrigger secretTrigger))
                {
                    secretTrigger.Trigger();
                }
                else if (target.TryGetComponent(out LockedDoor lockedDoor))
                {
                    lockedDoor.TryToOpen();
                }
                else if (target.TryGetComponent(out ElevatorSwitch elevatorSwitch))
                {
                    Debug.Log("Load level");
                    LevelManager.main.LoadNextLevel();
                }
            }
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized * moveSpeed;
    }

    public void Hurt(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (Dead) return;
        Dead = true;
        Debug.Log("YOU DIED");
    }
}
