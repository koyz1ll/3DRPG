

using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStates
    {
        HitPlayer,
        HitEnemey,
        HitNothing
    }
    private Rigidbody rb;

    public RockStates rockStates;

    
    [Header("Basic Settings")] 
    public float force;

    public int damage;
    [HideInInspector]
    public GameObject target;

    [HideInInspector] public GameObject attacker;

    private Vector3 direction;
    public GameObject breakEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        
        rockStates = RockStates.HitPlayer;
    }

    private void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude < 1f)
        {
            rockStates = RockStates.HitNothing;
        }
    }

    public void FlyToTarget()
    {
        if (target == null)
        {
            target = FindObjectOfType<PlayerController>().gameObject;
        }
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
        
    }

    private void DestroySelf(float f, object o)
    {
        GameObject.Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                if (other.gameObject.layer.Equals(LayerUtils.Player))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;
                    
                    other.gameObject.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, other.gameObject.GetComponent<CharacterStats>());
                    rockStates = RockStates.HitNothing;
                }
                break;
            case  RockStates.HitEnemey:
                if (other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
        }
    }
}
