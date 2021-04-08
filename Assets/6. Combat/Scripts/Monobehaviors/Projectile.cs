using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour 
{
    private GameObject caster;          // reference to char that cast the spell
    private float speed;                // speed of the projectile
    private float range;                // range of the projectile
    private Vector3 travelDirection;    // direction toward target

    private float distanceTraveled;     // distance till destroyed

    public event Action<GameObject, GameObject> ProjectileCollided;     // event callback when a projectile collide with target/ miss

    public void Fire(GameObject Caster, Vector3 Target, float Speed, float Range)
    {
        caster = Caster;
        speed = Speed;
        range = Range;

        //calculate travel direction
        travelDirection = Target - transform.position;
        travelDirection.y = 0f;
        travelDirection.Normalize();

        // initialize distance traveled
        distanceTraveled = 0f;
    }

	void Update () 
    {
        // move this projectile through space
        float distanceToTravel = speed * Time.deltaTime;

        transform.Translate(travelDirection * distanceToTravel);

        // check to see if we traveled too far, if so destroy this projectile
        distanceTraveled += distanceToTravel;
        if(distanceTraveled > range)
        {
            Destroy(gameObject);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        // Raise an event

        if(ProjectileCollided != null)
        {
            ProjectileCollided(caster, other.gameObject);
        }

        //Destroy Object
        Destroy(gameObject);
    }
}
