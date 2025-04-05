using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    private PlayerTest player;
    private Animator anim;
    private Rigidbody rb;
    private float moveSpeed = 1.0f;
    private float attackDistance = 2.0f;

    public GameObject DieEffect;
    public Transform BulletOrigin;
    public float accuracyDegrees = 20.0f;
    public float damage = 5.0f;
    private int rayCastLayers;

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
}
