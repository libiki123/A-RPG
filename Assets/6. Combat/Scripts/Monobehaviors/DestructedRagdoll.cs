using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructedRagdoll : MonoBehaviour, IDestructible
{
    public Ragdoll RagdollObject;
    public float Force;
    public float Lift;

    public void OnDestruction(GameObject destroyer)
    {
        var ragdoll = Instantiate(RagdollObject, transform.position, transform.rotation);       // create a ragdoll

        var vectorFromDestroyer = transform.position - destroyer.transform.position;        // distance from destroyer/attacker
        vectorFromDestroyer.Normalize();
        vectorFromDestroyer.y += Lift;              // add some upward force

        ragdoll.ApplyForce(vectorFromDestroyer * Force);
    }
}
