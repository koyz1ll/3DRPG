
using System.Timers;
using UnityEngine;

public class Rock : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Basic Settings")] 
    public float force;

    [HideInInspector]
    public GameObject target;

    [HideInInspector] public GameObject attacker;

    private Vector3 direction;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void FlyToTarget()
    {
        if (target == null)
        {
            target = FindObjectOfType<PlayerController>().gameObject;
        }
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
        Timer.Register(3, DestroySelf);
    }

    private void DestroySelf(float f, object o)
    {
        GameObject.Destroy(gameObject);
    }
}
