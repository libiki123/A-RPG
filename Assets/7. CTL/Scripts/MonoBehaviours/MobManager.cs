using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using System.Collections.Generic;

public class MobManager : MonoBehaviour 
{
    public GameObject[] Mobs;
    public MobWave[] Waves;
    public List<DropTable> dropTables;

    public Events.EventIntegerEvent OnMobKilled;                // These events are listened by the Hero Controller
    public Events.EventIntegerEvent OnWaveCompleted;            // we can add the listener in the Hero Controller
    public UnityEvent OnOutOfWaves;                              // or use the inspector 
    public UnityEvent OnWaveSpawned;

    private int currentWaveIndex = 0;
    private int activeMobs;

    private Spawnpoint[] spawnpoints;

	// Use this for initialization
	void Start () 
    {
        spawnpoints = FindObjectsOfType<Spawnpoint>();
        SpawnWave();
	}
	
    public void SpawnWave()
    {
        if(Waves.Length -1 < currentWaveIndex)
        {
            StartCoroutine(GameManager.Instance.EndGame());
            OnOutOfWaves.Invoke();          // check for listeners
            return;
        }

        if (currentWaveIndex > 0)       // do sth only after 1st wave
        {
            SoundManager.Instance.PlaySoundEffect(SoundEffect.NextWave);
            OnWaveSpawned.Invoke();
        }
        activeMobs = Waves[currentWaveIndex].NumberOfMobs;

        for (int i = 0; i <= Waves[currentWaveIndex].NumberOfMobs - 1; i++)
        {
            Spawnpoint spawnpoint = selectRandomSpawnpoint();
            GameObject mob = Instantiate(selectRandomMob(), 
                                         spawnpoint.transform.position, Quaternion.identity);
            mob.GetComponent<NPCController>().waypoints = findClosestWayPoints(mob.transform);

            CharacterStats stats = mob.GetComponent<CharacterStats>();
            MobWave currentWave = Waves[currentWaveIndex];

            stats.SetInitialHealth(currentWave.MobHealth);
            stats.SetInitialResistance(currentWave.MobResistance);
            stats.SetInitialDamage(currentWave.MobDamage);
        }
    }

    public void OnMobDeath(MobType mobType, Vector3 mobPosition)
    {
        SoundManager.Instance.PlaySoundEffect(SoundEffect.MobDeath);
        spawnDrop(mobType, mobPosition);

        MobWave currentWave = Waves[currentWaveIndex];

        activeMobs -= 1;
        OnMobKilled.Invoke(currentWave.PointsPerKill);          // check for listeners
        Debug.LogWarningFormat("{0} killed at {1}", mobType, mobPosition);

        if(activeMobs == 0)
        {
            OnWaveCompleted.Invoke(currentWave.WaveValue);      // check for listeners
            currentWaveIndex += 1;
            SpawnWave();
        }
    }
	
    private GameObject selectRandomMob()
    {
        int mobIndex = Random.Range(0, Mobs.Length);
        return Mobs[mobIndex];
    }

    private Spawnpoint selectRandomSpawnpoint()
    {
        int pointIndex = Random.Range(0, spawnpoints.Length);
        return spawnpoints[pointIndex];
    }

    private Transform[] findClosestWayPoints(Transform mobTranform)
    {
        Vector3 mobPosition = mobTranform.position;

        Waypoint closetPoint = FindObjectsOfType<Waypoint>().OrderBy(
            w => (w.transform.position - mobPosition).sqrMagnitude).First();    // Order the array by the distance from mob pos and select the first point in the sorted array

        Transform parent = closetPoint.transform.parent;        // get the parent of the closetPoint

        Transform[] allTransforms = parent.GetComponentsInChildren<Transform>();        // Get all the points from the parent (include the parent)

        var transforms =                    // remove the parent tranform from allTranforms
            from t in allTransforms
            where t != parent
            select t;

        return transforms.ToArray();
   }

    private void spawnDrop(MobType mobType, Vector3 position)
    {
        ItemPickUps_SO item = getDrop(mobType);

        if (item != null)
            Instantiate(item.itemSpawnObject, position, Quaternion.identity);
    }


    private ItemPickUps_SO getDrop(MobType mobType)
    {
        DropTable mobDrops = dropTables.Find(mt => mt.mobType == mobType);

        if (mobDrops == null)
            return null;

        mobDrops.drops.OrderBy(d => d.DropChance);

        foreach(DropDefinition dropDef in mobDrops.drops)
        {
            bool shouldDrop = Random.value < dropDef.DropChance;    // check the drop chance

            if (shouldDrop)
                return dropDef.Drop;
        }

        return null;
    }
}
