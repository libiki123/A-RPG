using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public float patrolTime = 10;       // time in seconds to wait before seeking a new patrol destination
    public float aggroRange = 10;       // distance in scene units below which the NPC will increase speed and seek the player
    public Transform[] waypoints;       // collection of waypoints which define a patrol area
    public AttackDefinition attack;
    public AudioClip spellClip;
    public MobType mobType;

    public Transform SpellHotSpot;      // fire point
    public Events.EventMobDeath OnMobDeath;

    int index;                  // the current waypoint index in the waypoints array
    float speed, agentSpeed;    // current agent speed and NavMeshAgent component speed
    Transform player;           // reference to the player object transform

    Animator animator;
    NavMeshAgent agent;

    private float timeOfLastAttack;

    private bool playerIsAlive;         // check if player alive to handle null reference in update()

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agentSpeed = agent.speed;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        index = Random.Range(0, waypoints.Length);

        MobManager mobManager = FindObjectOfType<MobManager>();
        if (mobManager != null)
            OnMobDeath.AddListener(mobManager.OnMobDeath);

        InvokeRepeating("Tick", 0, 0.5f);       // Call Tick() repeatedly at 0.5f

        if (waypoints.Length > 0)       // If npc not at waypoint
        {
            InvokeRepeating("Patrol", Random.Range(0, patrolTime), patrolTime);     // Get the the waypoint
        }

        timeOfLastAttack = float.MinValue;      // keep it empty till npc start to attack
        playerIsAlive = true;

        player.gameObject.GetComponent<DestructedEvent>().IDied += PlayerDied;      // listen to the IDied event
    }

    private void PlayerDied()
    {
        playerIsAlive = false;
    }

    void Update()
    {
        speed = Mathf.Lerp(speed, agent.velocity.magnitude, Time.deltaTime * 10);
        animator.SetFloat("Speed", speed);       // Change to walk/run animation at speed

        float timeSinceLastAttack = Time.time - timeOfLastAttack;       // current time - timeOfLastAttack
        bool attackOnCooldown = timeSinceLastAttack < attack.Cooldown;      // check if attack still on cooldown

        agent.isStopped = attackOnCooldown;     // stop moving when attacking

        if (playerIsAlive)      // check if player still alive
        {
            float distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
            bool attackInRange = distanceFromPlayer < attack.Range;

            if (!attackOnCooldown && attackInRange)     // attack not on cooldown and player within range
            {
                transform.LookAt(player.transform);     // face toward the player
                animator.SetTrigger("Attack");          // trigger the attack animation
                timeOfLastAttack = Time.time;       // reset the timer after finish the attack
            }
        }
    }

    public void Hit()       // Needed b/c of attack animation event
    {
        if (!playerIsAlive)
            return;

        if(attack is Weapon)        // if the attack is from a weapon
        {
            ((Weapon)attack).ExecuteAttack(gameObject, player.gameObject);
        }
        else if(attack is Spell)        // if attack is from a spell
        {
            ((Spell)attack).Cast(gameObject, SpellHotSpot.position, player.transform.position, LayerMask.NameToLayer("EnemySpells"));
            GetComponent<AudioSource>().PlayOneShot(spellClip);
        }
    }

    void Patrol()
    {
        index = index == waypoints.Length - 1 ? 0 : index + 1;      // If reached the waypoint find the next waypoint
    }

    void Tick()
    {
        agent.destination = waypoints[index].position;      // Set the destination base on the waypoint
        agent.speed = agentSpeed / 2;                       // Set speed to walk (agentSpeed / 2)

        if (player != null && Vector3.Distance(transform.position, player.transform.position) < aggroRange)     // Check if Player is in aggroRange
        {
            agent.speed = agentSpeed;               // Set the destination to the Player
            agent.destination = player.position;    // Set speed to run 
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }

}

[System.Serializable]
public enum MobType
{
    ClawGoblin,
    SpellCaster
}