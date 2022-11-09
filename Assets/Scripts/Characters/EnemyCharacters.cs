using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCharacters : MonoBehaviour
{
    private NavMeshAgent agent;
    private EnemyStatus enemyStatus;
    private Animator anim;

    [Header("Basic Settings")] 
    public float sightRadius;
    public GameObject attackTarget;
    public bool isGuard;
    private float speed;

    private bool isWalk;
    private bool isChase;
    private bool isFollow;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        speed = agent.speed;
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
                    //TODO:拉脱回到上一个状态
                    isFollow = false;
                    agent.destination = transform.position;
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
}


public enum EnemyStatus
{
    GUARD,//守卫
    PATROL,//巡逻
    CHASE,//追击
    DEAD//死亡
}