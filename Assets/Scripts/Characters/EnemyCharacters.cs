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

    [Header("Basic Settings")] 
    public float sightRadius;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        SwtichStatus();
    }

    private void SwtichStatus()
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
                return true;
            }
        }
        return false;
    }
}


public enum EnemyStatus
{
    GUARD,
    PATROL,
    CHASE,
    DEAD
}