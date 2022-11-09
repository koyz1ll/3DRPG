using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
   private NavMeshAgent agent;
   private Animator anim;
   private GameObject attackTarget;
   private CharacterStats characterStats;
   private float lastAttackTime;

   private void Awake()
   {
      agent = GetComponent<NavMeshAgent>();
      anim = GetComponent<Animator>();
      characterStats = GetComponent<CharacterStats>();
      lastAttackTime = 0;
   }

   private void Start()
   {
      MouseManager.Instance.OnMouseClicked += MoveToTarget;
      MouseManager.Instance.OnEnemyClicked += EventAttack;
   }

   private void Update()
   {
      SwitchAnimation();

      lastAttackTime -= Time.deltaTime;
   }

   private void SwitchAnimation()
   {
      anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
   }

   void MoveToTarget(Vector3 target)
   {
      StopAllCoroutines();
      agent.isStopped = false;
      agent.destination = target;
   }

   void EventAttack(GameObject target)
   {
      if (target != null)
      {
         attackTarget = target;
         characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
         StartCoroutine(MoveToAttackTarget());
      }
   }

   IEnumerator MoveToAttackTarget()
   {
      agent.isStopped = false;
      transform.LookAt(attackTarget.transform);
      while (Vector3.Distance(transform.position, attackTarget.transform.position) > characterStats.attackData.attackRange)
      {
         agent.destination = attackTarget.transform.position;
         yield return null;
      }
      //Attack;
      agent.isStopped = true;
      if (lastAttackTime < 0)
      {
         anim.SetBool("Critical", characterStats.isCritical);
         anim.SetTrigger("Attack");
         lastAttackTime = characterStats.attackData.cooldown;
      }
   }
   
   private void OnDrawGizmosSelected()
   {
      Gizmos.color = Color.yellow;
      if (agent && agent.path.corners.Length > 1)
      {
         for (var i = 0; i < agent.path.corners.Length - 1; i++)
         {
            Gizmos.DrawLine(agent.path.corners[i], agent.path.corners[i+1]);
         }
      }
   }
   
   //Animation Event
   void Hit()
   {
      var targetStats = attackTarget.GetComponent<CharacterStats>();
      targetStats.TakeDamage(characterStats, targetStats);
   }
}
