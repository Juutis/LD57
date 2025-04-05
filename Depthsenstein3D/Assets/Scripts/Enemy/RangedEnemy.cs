using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    private GameObject player;
    private Animator anim;
    private Rigidbody rb;
    private float moveSpeed = 1.0f;
    private float attackDistance = 2.0f;

    public GameObject DieEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < attackDistance) {
            AnimateAttack();
        } else if (rb.linearVelocity.magnitude > 0.01f) {
            AnimateRun();
        } else {
            AnimateIdle();
        }
    }

    void FixedUpdate()
    {
        var dir = player.transform.position - transform.position;
        if (dir.magnitude > attackDistance) {
            dir = dir.normalized;
            rb.linearVelocity = dir * moveSpeed;
        } else {
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void AnimateRun() {
        anim.Play("Run");
    }

    private void AnimateIdle() {
        anim.Play("Idle");
    }

    private void AnimateAttack() {
        anim.Play("Attack");
    }

    public void Die() {
        var fx = Instantiate(DieEffect);
        fx.transform.position = transform.position;
        Destroy(gameObject);
    }
}
