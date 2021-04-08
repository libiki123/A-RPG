using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    private const float DOT_THRESHOLD = 0.5f;

    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();

        float dot = Vector3.Dot(transform.forward, vectorToTarget);      // Dot product (use to check if 2 object is facing each other - return 1 -> -1)

        return dot >= DOT_THRESHOLD;
    }
}
