using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    private PlayerTest player;
    private Animator anim;
    private Rigidbody rb;
    public float moveSpeed = 1.0f;
    public float attackDistance = 2.0f;

    public GameObject DieEffect;
    public Transform BulletOrigin;
    public float accuracyDegrees = 20.0f;
    public float damage = 5.0f;
    private int rayCastLayers;
    private Vector3 lastDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerTest>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rayCastLayers = LayerMask.GetMask("Default", "Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < attackDistance) {
            AnimateAttack();
        } else if (rb.linearVelocity.magnitude > 0.01f) {
            AnimateRun();
            lastDir = rb.linearVelocity;
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
        var dirToPlayer = player.transform.position - transform.position;
        var angle = Vector3.SignedAngle(dirToPlayer, lastDir, Vector3.up);
        if (angle > 45 && angle < 135) {
            if (hasAnimState("Run_right")) {
                anim.Play("Run_right");
            } else {
                anim.Play("Run");
            }
        } else if (angle > -135 && angle < -45) {
            if (hasAnimState("Run_left")) {
                anim.Play("Run_left");
            } else {
                anim.Play("Run");
            }
        } else {
            anim.Play("Run");
        }
    }

    private void AnimateIdle() {
        var dirToPlayer = player.transform.position - transform.position;
        var angle = Vector3.SignedAngle(dirToPlayer, lastDir, Vector3.up);
        if (angle > 45 && angle < 135) {
            if (hasAnimState("Idle_right")) {
                anim.Play("Idle_right");
            } else {
                anim.Play("Idle");
            }
        } else if (angle > -135 && angle < -45) {
            if (hasAnimState("Idle_left")) {
                anim.Play("Idle_left");
            } else {
                anim.Play("Idle");
            }
        } else {
            anim.Play("Idle");
        }
    }

    private void AnimateAttack() {
        anim.Play("Attack");
    }

    private bool hasAnimState(string state) {
        var stateId = Animator.StringToHash(state);
        return anim.HasState(0, stateId);
    }

    public void Die() {
        var fx = Instantiate(DieEffect);
        fx.transform.position = transform.position;
        Destroy(gameObject);
    }

    public void Shoot() {
        var dir = player.transform.position - BulletOrigin.position;
        var inAccuracy = Random.Range(0.0f, 1.0f) * accuracyDegrees;
        var randomRoll = Random.Range(0.0f, 360.0f);
        dir = Quaternion.AngleAxis(inAccuracy, BulletOrigin.up) * dir;
        dir = Quaternion.AngleAxis(randomRoll, BulletOrigin.forward) * dir;
        if (Physics.Raycast(BulletOrigin.position, dir, out RaycastHit hitInfo, 1000f, rayCastLayers)) {
            var other = hitInfo.collider;
            if (other.gameObject == player.gameObject) {
                var effect = Instantiate(FpsManager.Main.BloodEffect);
                effect.transform.position = hitInfo.point;
                player.Hurt(damage);
            } else {
                var effect = Instantiate(FpsManager.Main.HitEffect);
                effect.transform.position = hitInfo.point;
            }
        }
    }

    public void Melee() {
        var dir = player.transform.position - BulletOrigin.position;
        if (Physics.Raycast(BulletOrigin.position, dir, out RaycastHit hitInfo, attackDistance, rayCastLayers)) {
            var other = hitInfo.collider;
            if (other.gameObject == player.gameObject) {
                var effect = Instantiate(FpsManager.Main.BloodEffect);
                effect.transform.position = hitInfo.point;
                player.Hurt(damage);
            }
        }
    }
}
