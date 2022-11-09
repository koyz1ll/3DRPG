using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
   private NavMeshAgent agent;
   private Animator anim;
   private GameObject attackTarget;
   private float lastAttackTime;

   private void Awake()
   {
      agent = GetComponent<NavMeshAgent>();
      anim = GetComponent<Animator>();
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
         StartCoroutine(MoveToAttackTarget());
      }
   }

   IEnumerator MoveToAttackTarget()
   {
      agent.isStopped = false;
      transform.LookAt(attackTarget.transform);
      while (Vector3.Distance(transform.position, attackTarget.transform.position) > 1)
      {
         agent.destination = attackTarget.transform.position;
         yield return null;
      }
      //Attack;
      agent.isStopped = true;
      if (lastAttackTime < 0)
      {
         anim.SetTrigger("Attack");
         lastAttackTime = 1;
      }
   }
}
