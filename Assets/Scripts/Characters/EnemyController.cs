using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Utils;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{
    private NavMeshAgent agent;
    [HideInInspector]
    public EnemyStatus enemyStatus;
    private Animator anim;

    private CharacterStats characterStats;

    [Header("Basic Settings")] 
    public float sightRadius;
    private GameObject attackTarget;
    public bool isGuard;
    private float speed;
    private float lookAtTime;
    private float remainLookAtTime;
    private float lastAttackTime;

    private bool isWalk;
    private bool isChase;
    private bool isFollow;
    private bool isDead;

    private bool playerDead = false;

    [Header("Patrol State")] 
    public float patrolRange;
    private Vector3 wayPoint;
    private Vector3 originPoint;
    private Quaternion originRotation;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        speed = agent.speed;
        originPoint = transform.position;
        originRotation = transform.rotation;
        lookAtTime = Random.Range(2, 4);
        remainLookAtTime = lookAtTime;
        characterStats = GetComponent<CharacterStats>();
    }

    private void Start()
    {
        if (isGuard)
        {
            enemyStatus = EnemyStatus.GUARD;
        }
        else
        {
            enemyStatus = EnemyStatus.PATROL;
            GetNewWayPoint();
        }
        //TODO:
        EventCenter.Instance.SubscribeNormalEvent(EventType.PLAYER_DEAD, EndNotify);
    }

    protected virtual void OnDestroy()
    {
        if (EventCenter.Instance != null)
        {
            EventCenter.Instance.UnSubscribeNormalEvent(EventType.PLAYER_DEAD, EndNotify);    
        }
    }

    private void Update()
    {
        if (characterStats.CurrentHealth == 0)
        {
            isDead = true;
        }

        if (!playerDead)
        {
            SwitchStatus();
            SwitchAnimations();
            lastAttackTime -= Time.deltaTime;    
        }
    }

    void SwitchAnimations()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }

    private void SwitchStatus()
    {
        if (isDead)
        {
            enemyStatus = EnemyStatus.DEAD;
        }
        else if (FindPlayer())
        {
            enemyStatus = EnemyStatus.CHASE;
        }

        switch (enemyStatus)
        {
            case EnemyStatus.GUARD:
                isChase = false;
                if (transform.position != originPoint)
                {
                    isWalk = true;
                    agent.destination = originPoint;
                    if (Vector3.SqrMagnitude(originPoint - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, originRotation, 0.01f);
                    }
                }
                break;
            case EnemyStatus.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;
                //是否走到了
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                    {
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        GetNewWayPoint();    
                    }
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                break;
            case EnemyStatus.CHASE:
                isWalk = false;
                isChase = true;
                agent.speed = speed;
                if (!FindPlayer())
                {
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        enemyStatus = isGuard ? EnemyStatus.GUARD : EnemyStatus.PATROL;
                    }
                }
                else
                {
                    isFollow = true;
                    agent.destination = attackTarget.transform.position;
                }
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.destination = transform.position;
                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.attackData.cooldown;
                        //暴击判断
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                        //执行攻击
                        Attack();
                    }
                }
                break;
            case EnemyStatus.DEAD:
                agent.enabled = false;
                Destroy(gameObject, 2f);
                break;
        }
    }
    
    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
           //近身攻击动画   
           anim.SetTrigger("Attack");
        }

        if (TargetInSkillRange())
        {
            anim.SetTrigger("Skill");
        }
    }

    bool FindPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {
            if (target.gameObject.layer.Equals(LayerUtils.Player))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position,
                transform.position) <= characterStats.attackData.attackRange;
        }
        else
        {
            return false;
        }
    }
    
    bool TargetInSkillRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position,
                transform.position) <= characterStats.attackData.skillRange;
        }
        else
        {
            return false;
        }
    }

    void GetNewWayPoint()
    {
        remainLookAtTime = Random.Range(2,4);
        var randomX = Random.Range(-patrolRange, patrolRange);
        var randomZ = Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(originPoint.x + randomX, transform.position.y, originPoint.z + randomZ);
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        GizmosUtils.DrawGizmosCircle(originPoint, Vector3.up, patrolRange, 100);
        Gizmos.DrawSphere(wayPoint, 0.1f);
        Gizmos.color = Color.green;
        GizmosUtils.DrawGizmosCircle(transform.position, Vector3.up, sightRadius, 100);
        Gizmos.color = Color.yellow;
        if (agent && agent.path.corners.Length > 1)
        {
            for (var i = 0; i < agent.path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(agent.path.corners[i], agent.path.corners[i+1]);
            }
        }
    }
    
    void Hit()
    {
        if (attackTarget != null)
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);    
        }
    }

    public void EndNotify(string arg1, object arg2)
    {
        playerDead = true;
        isChase = false;
        isWalk = false;
        attackTarget = null;
        anim.SetBool("Win", true);
        //获胜了
        //停止所有移动
        //停止agent
    }
}


public enum EnemyStatus
{
    GUARD,//守卫
    PATROL,//巡逻
    CHASE,//追击
    DEAD//死亡
}