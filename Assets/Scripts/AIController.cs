using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [SerializeField] float chaseRange = 10f;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float attackDamage = 30f;
    NavMeshAgent agent;
    GameObject player;
    Animator animator;
    Health health;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
    }

    void Update()
    {
        if (health.IsDead())
        {
            return;
        }

        if (PlayerInRange(attackRange))
        {
            AttackBehavior();
        }
        else if (PlayerInRange(chaseRange))
        {
            ChaseBehavior();
        }
        else
        {
            agent.isStopped = true;
        }

        UpdateBlendTree();
    }

    void ChaseBehavior()
    {
        agent.isStopped = false;
        agent.SetDestination(player.transform.position);
        animator.ResetTrigger("attack");
    }

    void AttackBehavior()
    {
        agent.isStopped = true;
        animator.SetTrigger("attack");
        LookAtPlayer();
    }

    bool PlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    void LookAtPlayer()
    {
        Vector3 lookDirection = player.transform.position - transform.position;
        lookDirection.y = 0f;

        if (lookDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void UpdateBlendTree()
    {
        float velocity = transform.InverseTransformDirection(agent.velocity).magnitude;
        animator.SetFloat("movementSpeed", velocity, 0.1f, Time.deltaTime);
    }

    // Called in Unity Events
    void Hit()
    {
        if (PlayerInRange(attackRange))
        {
            player.GetComponent<Health>().TakeDamage(attackDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
