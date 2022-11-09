using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCharacters : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField]
    private EnemyStatus enemyStatus;

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
}


public enum EnemyStatus
{
    GUARD,
    PATROL,
    CHASE,
    DEAD
}