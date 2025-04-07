
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    private PlayerTest player;
    private Animator anim;
    private Rigidbody rb;
    public float moveSpeed = 1.0f;
    public float attackDistance = 2.0f;
    public float aggroDistance = 5.0f;

    public GameObject DieEffect;
    public List<Transform> BulletOrigins;
    public float accuracyDegrees = 20.0f;
    public float damage = 5.0f;
    public float minDelayBetweenAttacks = 1.0f;
    public float maxDelayBetweenAttacks = 2.0f;
    private float nextAttack;
    private int rayCastLayers;
    private int playerLosLayers;
    private Vector3 lastDir;

    public Projectile projectile;

    private Vector3 navigationTarget;
    private NavMeshPath path;
    private int waypointIndex = 0;
    private bool navigationActive = true;
    private float navigationUpdateInterval = 0.2f;
    private float waypointDistanceCheckEpsilon = 0.1f;
    private float targetRange = 0.0f;
    private State state = State.PATROL;
    private float navMeshY = -0.5f;

    public bool isActive = false;
    private bool dead = false;
    private bool attacking = false;

    public Damageable Body;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerTest>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.detectCollisions = false;
    }

    public void ResetPos()
    {
        transform.localPosition = Vector3.zero;

        if (rb != null && transform != null)
        {
            rb.position = transform.position;
        }
    }

    int timesDied = 0;

    public void BodyKilled() {
        if (stunned) return;
        stunned = true;
        Invoke("ResetBody", 5.0f + timesDied * 2.5f);
        timesDied++;
    }

    public void ResetBody() {
        stunned = false;
        Body.Reset();
    }

    public void HeartKilled() {
        Die();
        Debug.Log("YOU WIN");
    }

    public void Initialize()
    {
        rayCastLayers = LayerMask.GetMask("Default", "Player", "Ceiling");
        playerLosLayers = LayerMask.GetMask("Default", "Player");
        isActive = true;
        RandomizeNavigationTarget();
        EnableNavigation();

        transform.localPosition = Vector3.zero;
        rb.position = transform.position;
        rb.isKinematic = false;
        rb.detectCollisions = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive || dead)
        {
            return;
        }
        
        switch (state)
        {
            case State.PATROL:
                handlePatrol();
                break;
            case State.ATTACK:
                handleAttack();
                break;
        }

    }

    private void handlePatrol()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < aggroDistance)
        {
            if (canSeePlayer())
            {
                state = State.ATTACK;
                SoundManager.main.PlaySound(GameSoundType.EnemySeesPlayer);
                AttackModeNavigation();
            }
        }
        AnimateIdle();
    }

    private bool playerInAttackRange()
    {
        return Vector3.Distance(transform.position, player.transform.position) < attackDistance;
    }

    private bool canSeePlayer()
    {
        var dir = player.transform.position - transform.position;
        if (Physics.Raycast(BulletOrigins[1].position, dir, out RaycastHit hitInfo, 1000f, playerLosLayers))
        {
            return hitInfo.collider.gameObject == player.gameObject;
        }
        return false;
    }

    public void RandomizeNavigationTarget()
    {
        if (dead) return;
        if (!isActive)
        {
            Invoke("RandomizeNavigationTarget", Random.Range(2.0f, 5.0f));
        }
        if (state != State.PATROL) return;
        var maxDist = 2.0f;
        var targetPos = transform.position + new Vector3(Random.Range(-maxDist, maxDist), 0, Random.Range(-maxDist, maxDist));
        targetPos.y = navMeshY;
        navigationTarget = targetPos;
        Invoke("RandomizeNavigationTarget", Random.Range(2.0f, 5.0f));
    }

    public void AttackModeNavigation()
    {
        if (dead) return;
        if (!isActive) return;
        if (state != State.ATTACK) return;
        recalculateAttackNavigation();
        Invoke("AttackModeNavigation", Random.Range(1.0f, 2.0f));
    }

    private void recalculateAttackNavigation()
    {
        if (dead) return;
        if (!isActive) return;
        if (state != State.ATTACK) return;
        if (!playerInAttackRange() || !canSeePlayer())
        {
            navigationTarget = player.transform.position;
        }
        else
        {
            var maxDist = 1.0f;
            var targetPos = transform.position + new Vector3(Random.Range(-maxDist, maxDist), 0, Random.Range(-maxDist, maxDist));
            targetPos.y = navMeshY;
            navigationTarget = targetPos;
        }
    }

    private bool stunned = false;

    private void handleAttack()
    {
        if (!stunned) {
            var dirToPlayer = player.transform.position - transform.position;
            dirToPlayer.y = 0;
            transform.forward = dirToPlayer;
            anim.Play("Idle");
        } else {
            anim.Play("Stunned");
            return;
        }

        var readyToAttack = nextAttack < Time.time;

        //targetRange = attackDistance;
        if (readyToAttack && playerInAttackRange() && canSeePlayer())
        {
            Shoot();
            nextAttack = Time.time + Random.Range(minDelayBetweenAttacks, maxDelayBetweenAttacks);
            attacking = true;
        }
        if (!attacking)
        {
            if (rb.linearVelocity.magnitude > 0.01f)
            {
                AnimateRun();
                lastDir = rb.linearVelocity;
            }
            else
            {
                AnimateIdle();
            }
        }
    }

    void FixedUpdate()
    {
        if (!isActive || dead || stunned)
        {
            return;
        }

        if (attacking)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        runTowardsWaypoint();

        if (isFinalWaypoint() && GetDistanceToTarget() < targetRange)
        {
            rb.linearVelocity = Vector3.zero;
        }

        if (isFinalWaypoint() && GetDistanceToTarget() < 0.1f)
        {
            rb.linearVelocity = Vector3.zero;
        }

    }

    private void AnimateRun()
    {
        var dirToPlayer = player.transform.position - transform.position;
        var angle = Vector3.SignedAngle(dirToPlayer, lastDir, Vector3.up);
        if (angle > 45 && angle < 135)
        {
            if (hasAnimState("Run_right"))
            {
                anim.Play("Run_right");
            }
            else
            {
                anim.Play("Run");
            }
        }
        else if (angle > -135 && angle < -45)
        {
            if (hasAnimState("Run_left"))
            {
                anim.Play("Run_left");
            }
            else
            {
                anim.Play("Run");
            }
        }
        else
        {
            anim.Play("Run");
        }
    }

    private void AnimateIdle()
    {
        var dirToPlayer = player.transform.position - transform.position;
        var angle = Vector3.SignedAngle(dirToPlayer, lastDir, Vector3.up);
        if (angle > 45 && angle < 135)
        {
            if (hasAnimState("Idle_right"))
            {
                anim.Play("Idle_right");
            }
            else
            {
                anim.Play("Idle");
            }
        }
        else if (angle > -135 && angle < -45)
        {
            if (hasAnimState("Idle_left"))
            {
                anim.Play("Idle_left");
            }
            else
            {
                anim.Play("Idle");
            }
        }
        else
        {
            anim.Play("Idle");
        }
    }

    private void AnimateAttack()
    {
        anim.Play("Attack");
    }

    private bool hasAnimState(string state)
    {
        var stateId = Animator.StringToHash(state);
        return anim.HasState(0, stateId);
    }

    public void Die()
    {
        if (dead) return;
        //var fx = Instantiate(DieEffect);
        //fx.transform.position = transform.position;
        MapGenerator.main.Player.Stats.EnemiesKilled += 1;
        anim.Play("Die");
        dead = true;
        DisableNavigation();
        SoundManager.main.PlaySound(GameSoundType.EnemyDie);
        foreach(var coll in GetComponentsInChildren<Collider>()) {
            coll.enabled = false;
        }
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    int nextHead = 0;

    public void Shoot()
    {
        if (dead) return;

        if (projectile == null)
        {
            var dir = player.transform.position - BulletOrigins[nextHead].position;
            dir.y = 0;
            var inAccuracy = Random.Range(0.0f, 1.0f) * accuracyDegrees;
            var randomRoll = Random.Range(0.0f, 360.0f);
            var yaw = Quaternion.AngleAxis(inAccuracy, Vector3.up);
            var roll = Quaternion.AngleAxis(randomRoll, dir);
            dir = yaw * dir;
            dir = roll * dir;
            if (Physics.Raycast(BulletOrigins[nextHead].position, dir, out RaycastHit hitInfo, 1000f, rayCastLayers))
            {
                var other = hitInfo.collider;
                if (other.gameObject == player.gameObject)
                {
                    var effect = Instantiate(FpsManager.Main.BloodEffect);
                    effect.transform.position = hitInfo.point;
                    player.Hurt((int)damage);
                }
                else
                {
                    var effect = Instantiate(FpsManager.Main.HitEffect);
                    effect.transform.position = hitInfo.point;
                }
            }
        }
        else
        {
            var proj = Instantiate(projectile);
            proj.transform.position = BulletOrigins[nextHead].position;
            proj.Target = player.transform;
            SoundManager.main.PlaySound(GameSoundType.Laser);
        }
        nextHead++;
        if(nextHead > 2) nextHead = 0;
    }

    public void AttackDone()
    {
        attacking = false;
        recalculateAttackNavigation();
    }


    public void WasHurt()
    {
        Debug.Log("hurt!");
        if (state == State.PATROL)
        {
            state = State.ATTACK;
            AttackModeNavigation();
        }
    }


    public float GetDistanceToTarget()
    {
        if (path == null || path.corners == null || path.corners.Length == 0)
        {
            return float.MaxValue;
        }
        var distanceSum = 0.0f;
        var nextWaypoint = waypointIndex;
        var myPos = transform.position;
        myPos.y = navMeshY;
        distanceSum += Vector3.Distance(myPos, path.corners[nextWaypoint]);
        nextWaypoint++;
        while (nextWaypoint < path.corners.Length)
        {
            distanceSum += Vector3.Distance(path.corners[nextWaypoint - 1], path.corners[nextWaypoint]);
            nextWaypoint++;
        }
        return distanceSum;
    }

    private void updatePathing()
    {
        NavMeshPath newPath = new NavMeshPath();
        var sourcePos = transform.position;
        sourcePos.y = navMeshY;
        var targetPos = navigationTarget;
        targetPos.y = navMeshY;
        var success = NavMesh.CalculatePath(sourcePos, targetPos, NavMesh.AllAreas, newPath);
        if (success)
        {
            path = newPath;
            waypointIndex = 0;
        }

        /*
        Debug.DrawLine(sourcePos, targetPos, Color.green, 5.0f);
        var i = 0;
        foreach (var asd in path.corners) {
            var prev = i == 0 ? transform.position : path.corners[i-1];
            Debug.DrawLine(prev, asd, Color.red, 5.0f);
        }
        */

        if (navigationActive)
        {
            Invoke("updatePathing", navigationUpdateInterval);
        }
    }

    public void EnableNavigation()
    {
        navigationActive = true;
        updatePathing();
    }

    public void DisableNavigation()
    {
        navigationActive = false;
    }

    private void runTowardsWaypoint()
    {
        if (path == null || path.corners == null || path.corners.Length == 0)
        {
            return;
        }
        updateWayPoint();

        var targetPosition = path.corners[waypointIndex];
        targetPosition.y = transform.position.y;
        if (targetPosition != null)
        {
            rb.linearVelocity = (targetPosition - transform.position).normalized * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void updateWayPoint()
    {
        var targetPosition = path.corners[waypointIndex];
        if (targetPosition != null)
        {
            if (waypointReached() && !isFinalWaypoint())
            {
                waypointIndex++;
            }
        }
    }

    private bool waypointReached()
    {
        var targetPosition = path.corners[waypointIndex];
        targetPosition.y = transform.position.y;
        return Vector3.Distance(transform.position, targetPosition) < waypointDistanceCheckEpsilon;
    }

    private bool isFinalWaypoint()
    {
        if (path == null) return true;
        return waypointIndex == path.corners.Length - 1;
    }

    public bool HasLOSToTarget()
    {
        if (path == null || path.corners == null || path.corners.Length == 0)
        {
            return false;
        }
        return isFinalWaypoint();
    }

    public enum State
    {
        PATROL,
        ATTACK
    }
}
