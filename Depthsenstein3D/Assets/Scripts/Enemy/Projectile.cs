using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Transform Target;
    public float TrackingStrength = 360.0f;
    public float Speed = 5.0f;
    public float Damage = 10.0f;
    private Vector3 direction;
    private int rayCastLayers;
    private PlayerTest player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        direction = Target.position - transform.position;
        direction.Normalize();
        rayCastLayers = LayerMask.GetMask("Default", "Player");
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerTest>();
        Invoke("Destroy", 10.0f);
    }

    // Update is called once per frame
    void Update()
    {
        var dirToTarget = Target.position - transform.position;
        dirToTarget.Normalize();
        var newDir = Vector3.RotateTowards(direction, dirToTarget, Mathf.Deg2Rad * TrackingStrength * Time.deltaTime, 0.0f);
        if (Physics.Raycast(transform.position, newDir, out RaycastHit hitInfo, Speed * Time.deltaTime, rayCastLayers)) {
            var other = hitInfo.collider;
            if (other.gameObject == player.gameObject) {
                var effect = Instantiate(FpsManager.Main.BloodEffect);
                effect.transform.position = hitInfo.point;
                player.Hurt(Damage);
            } else {
                var effect = Instantiate(FpsManager.Main.HitEffect);
                effect.transform.position = hitInfo.point;
            }
            Destroy();
        } else {
            transform.position = transform.position + newDir.normalized * Speed * Time.deltaTime;
            direction = newDir;
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
