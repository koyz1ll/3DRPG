using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour,IEndGameObserver
{
   private NavMeshAgent agent;
   private Animator anim;
   private GameObject attackTarget;
   private CharacterStats characterStats;
   private float lastAttackTime;
   private bool isDead;
   public float stopDistance;

   private void Awake()
   {
      agent = GetComponent<NavMeshAgent>();
      anim = GetComponent<Animator>();
      characterStats = GetComponent<CharacterStats>();
      stopDistance = agent.stoppingDistance;
      lastAttackTime = 0;
   }

   private void OnEnable()
   {
      MouseManager.Instance.OnMouseClicked += MoveToTarget;
      MouseManager.Instance.OnEnemyClicked += EventAttack;

   }

   private void Start()
   {
      GameManager.Instance.RegisterPlayer(characterStats);
   }

   private void OnDisable()
   {
      MouseManager.Instance.OnMouseClicked -= MoveToTarget;
      MouseManager.Instance.OnEnemyClicked -= EventAttack;
   }

   private void Update()
   {
      isDead = characterStats.CurrentHealth == 0;
      if (isDead)
      {
         EventCenter.Instance.NotifyNormalEvent(EventType.PLAYER_DEAD);
      }
      SwitchAnimation();
      lastAttackTime -= Time.deltaTime;
   }

   private void SwitchAnimation()
   {
      anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
      anim.SetBool("Death", isDead);
   }

   void MoveToTarget(Vector3 target)
   {
      StopAllCoroutines();
      if (isDead) return;
      agent.stoppingDistance = stopDistance;
      agent.isStopped = false;
      agent.destination = target;
   }

   void EventAttack(GameObject target)
   {
      if (isDead) return;
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
      agent.stoppingDistance = characterStats.attackData.attackRange;
      
      
      transform.LookAt(attackTarget.transform);
      Debug.Log("distance:" + Vector3.Distance(transform.position, attackTarget.transform.position));
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
      if (attackTarget.layer.Equals(LayerUtils.Attackable))
      {
         if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)
         {
            attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemey;
            attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
            attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
         }
      }
      else
      {
         var targetStats = attackTarget.GetComponent<CharacterStats>();
         targetStats.TakeDamage(characterStats, targetStats);   
      }
   }

   public void EndNotify(string eventId, object arg)
   {
      
   }
}
