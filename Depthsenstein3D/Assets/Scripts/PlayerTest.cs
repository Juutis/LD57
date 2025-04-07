using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTest : MonoBehaviour
{
    float rotateSpeed = 120f;
    float mouseSensitivity = 1f;
    float moveSpeed = 3f;
    private Rigidbody rb;
    private Vector2 moveInput;
    public bool UseMouse = true;

    public int Health = 100;
    public int MaxHealth = 100;
    public bool Dead = false;

    private bool canAct = true;

    private float elevatorRotateDuration = 0.5f;
    private float elevatorRotateTimer = 0f;
    private bool isElevatorRotating = false;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    private UnityAction rotationCallback;

    public LevelStatistics Stats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        Health = MaxHealth;
        UIManager.main.SetHealth(Health);
    }

    public void FreezeControls() {
        canAct = false;
    }

    public void RestoreControls() {
        canAct = true;
    }

    public void ElevatorRotate(Vector3 target, UnityAction rotationCallback) {
        this.rotationCallback = rotationCallback;
        isElevatorRotating = true;
        elevatorRotateTimer = 0f;
        startRotation = transform.rotation;
        Vector3 direction = target - transform.position;
        targetRotation = Quaternion.LookRotation(direction);
    }

    // Update is called once per frame
    void Update()
    {
        if (isElevatorRotating) {
            elevatorRotateTimer += Time.deltaTime;
            float t = elevatorRotateTimer / elevatorRotateDuration;
            // 3. Use Quaternion.Lerp
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            if (t >= 1.0f)
            {
                isElevatorRotating = false;
                rotationCallback.Invoke();
            }
        }
        if (!canAct) {
            rb.linearVelocity = Vector3.zero;
            return;
        }
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

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, 1f, ~0);

            foreach (RaycastHit hitInfo in hits)
            {
                bool hasActivatedSomething = true;
                Debug.Log($"{(hitInfo.collider != null ? "hit" : "nop")} | ${(hitInfo.collider != null ? hitInfo.collider.gameObject.name : "null")}");

                if (hitInfo.collider != null)
                {
                    GameObject target = hitInfo.collider.gameObject;

                    if (target.TryGetComponent(out NormalDoor door))
                    {
                        Destroy(door.gameObject);
                        SoundManager.main.PlaySound(GameSoundType.OpenDoor);
                    }
                    else if (target.TryGetComponent(out SecretTrigger secretTrigger))
                    {
                        secretTrigger.Trigger();
                    }
                    else if (target.TryGetComponent(out LockedDoor lockedDoor))
                    {
                        if (lockedDoor.TryToOpen())
                        {
                            SoundManager.main.PlaySound(GameSoundType.OpenDoor);
                        }
                        else
                        {
                            SoundManager.main.PlaySound(GameSoundType.Prod);
                        }
                    }
                    else if (target.TryGetComponent(out ElevatorDoors elevatorDoors))
                    {
                        Debug.Log("Elevator");
                        SoundManager.main.PlaySound(GameSoundType.ElevatorOpen);
                        elevatorDoors.OpenDoorsFromPlayerAction(delegate
                        {
                            SoundManager.main.PlaySound(GameSoundType.ElevatorDing);
                            Debug.Log("Doors opened");
                        });
                    }
                    else if (target.TryGetComponent(out ElevatorSwitch elevatorSwitch))
                    {
                        elevatorSwitch.Use();
                    }
                    else
                    {
                        SoundManager.main.PlaySound(GameSoundType.Prod);
                        hasActivatedSomething = false;
                    }

                    if (hasActivatedSomething)
                    {
                        break;
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            UIManager.main.FadeOut();
            Invoke("Restart", 0.5f);
        }
    }

    private void Restart()
    {
        LevelManager.main.RestartLevel();
    }

    void FixedUpdate()
    {
        if (!canAct) {
            rb.linearVelocity = Vector3.zero;
            return;
        }
        rb.linearVelocity = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized * moveSpeed;
    }

    public void Heal(int points)
    {
        Health += points;
        if (Health > 100)
        {
            Health = 100;
        }
        UIManager.main.SetHealth(Health);
    }

    public void Hurt(int damage)
    {
        Health -= damage;
        UIManager.main.SetHealth(Health);
        SoundManager.main.PlaySound(GameSoundType.PlayerIsHitMelee);
        if (Health <= 0)
        {
            Die();
        }
        ScreenShake.Instance.Shake();
    }

    public void Die()
    {
        if (Dead) return;
        Dead = true;
        Debug.Log("YOU DIED");
        Time.timeScale = 0;
        UIManager.main.ShowDeath(delegate {
            LevelManager.main.RestartLevel();
            Time.timeScale = 1;
        });
    }

    public void ResetPlayer(Vector3 spawnPos, Quaternion spawnRot)
    {
        Health = MaxHealth;
        Dead = false;
        // rb.linearVelocity = Vector3.zero;
        transform.position = spawnPos;
        transform.rotation = spawnRot;
        rb.position = spawnPos;
        UIManager.main.SetHealth(Health);
    }
}
