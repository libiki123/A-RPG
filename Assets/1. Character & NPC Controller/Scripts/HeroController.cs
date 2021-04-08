using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;

public class HeroController : MonoBehaviour
{
    public AttackDefinition demoAttack;
    public Aoe aoeStompAttack;

    Animator animator; // reference to the animator component
    NavMeshAgent agent; // reference to the NavMeshAgent
    CharacterStats stats;   // reference to the CharacterStats

    private GameObject attackTarget;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        stats = GetComponent<CharacterStats>();
    }

    private void Start()
    {
        stats.characterDefinition.OnLevelUp.AddListener(GameManager.Instance.OnHeroLeveledUp);      // we want to garantee that char stats is fully initialize before adding the listeners
        stats.characterDefinition.OnHeroDamaged.AddListener(GameManager.Instance.OnHeroDamaged);
        stats.characterDefinition.OnHeroGainedHealth.AddListener(GameManager.Instance.OnHeroGainedHealth);
        stats.characterDefinition.OnHeroDeath.AddListener(GameManager.Instance.OnHeroDied);
        stats.characterDefinition.OnHeroInitialized.AddListener(GameManager.Instance.OnHeroInit);

        stats.characterDefinition.OnHeroInitialized.Invoke();
    }

    void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);       // Set animation base on player speed
    }

    public void SetDestination(Vector3 destination)     // Used in the Mouse Manager event (Inspector)
    {
        StopAllCoroutines();
        agent.isStopped = false;
        agent.destination = destination;
    }

    public void DoStomp(Vector3 destination)         // got called in the MouseManager click event in the Inspector
    {
        StopAllCoroutines();
        agent.isStopped = false;
        StartCoroutine(GoToTargetAndStomp(destination));
    }

    private IEnumerator GoToTargetAndStomp(Vector3 destination)     
    {
        while (Vector3.Distance(transform.position, destination) > aoeStompAttack.Range)
        {
            agent.destination = destination;
            yield return null;
        }
        agent.isStopped = true;
        animator.SetTrigger("Stomp");
    }

    public void AttackTarget(GameObject target)     // got called in the MouseManager click event in the Inspector
    {
        var weapon = stats.GetCurrentWeapon();

        if (weapon != null)
        {
            StopAllCoroutines();

            agent.isStopped = false;
            attackTarget = target;
            StartCoroutine(PursueAndAttackTarget());
        }
    }

    private IEnumerator PursueAndAttackTarget()         // Chase the target if not in range
    {
        agent.isStopped = false;             // start moving
        var weapon = stats.GetCurrentWeapon();

        while (Vector3.Distance(transform.position, attackTarget.transform.position) > weapon.Range)        // move until within weapon range
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }

        agent.isStopped = true;

        transform.LookAt(attackTarget.transform);       // face toward the target
        animator.SetTrigger("Attack");      // trigger attack animation
    }

    public void Hit()       // Needed b/c of attack animation event
    {
        // Have our weapon attack the attack target
        if (attackTarget != null)
            stats.GetCurrentWeapon().ExecuteAttack(gameObject, attackTarget);
    }

    public void Stomp()
    {
        aoeStompAttack.Fire(gameObject, gameObject.transform.position, LayerMask.NameToLayer("PlayerSpells"));
    }

    public int GetCurrentHealth()
    {
        return stats.characterDefinition.currentHealth;
    }

    public int GetMaxHealth()
    {
        return stats.characterDefinition.maxHealth;
    }

    public int GetCurrentLevel()
    {
        return stats.characterDefinition.charLevel;
    }

    public int GetCurrentXP()
    {
        return stats.characterDefinition.charExperience;
    }

    #region Callbacks

    public void OnMobDeath(int pointVal)
    {
        stats.IncreaseXP(pointVal);
    }

    public void OnWaveComplete(int pointVal)
    {
        stats.IncreaseXP(pointVal);
    }

    public void OnOutOfWaves()
    {
        Debug.LogWarning("No more waves. you Win!");
    }


    #endregion
}
