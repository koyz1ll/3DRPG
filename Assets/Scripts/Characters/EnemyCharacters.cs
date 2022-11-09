using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Utils;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCharacters : MonoBehaviour
{
    private NavMeshAgent agent;
    private EnemyStatus enemyStatus;
    private Animator anim;

    [Header("Basic Settings")] 
    public float sightRadius;
    private GameObject attackTarget;
    public bool isGuard;
    private float speed;
    private float lookAtTime;
    private float remainLookAtTime;

    private bool isWalk;
    private bool isChase;
    private bool isFollow;

    [Header("Patrol State")] 
    public float patrolRange;
    private Vector3 wayPoint;
    private Vector3 originPoint;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        speed = agent.speed;
        originPoint = transform.position;
        lookAtTime = Random.Range(2, 4);
        remainLookAtTime = lookAtTime;
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
    }

    private void Update()
    {
        SwitchStatus();
        SwitchAnimations();
    }

    void SwitchAnimations()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
    }

    private void SwitchStatus()
    {
        if (FindPlayer())
        {
            enemyStatus = EnemyStatus.CHASE;
            Debug.Log("找到player了");
        }

        switch (enemyStatus)
        {
            case EnemyStatus.GUARD:
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
                //TODO:追player
                //TODO:拉脱回到上一个状态
                //TODO:在攻击范围内则攻击
                //TODO:配合动画
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
                break;
            case EnemyStatus.DEAD:
                break;
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
}


public enum EnemyStatus
{
    GUARD,//守卫
    PATROL,//巡逻
    CHASE,//追击
    DEAD//死亡
}