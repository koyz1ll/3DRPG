using UnityEngine;

public static class ExtensionMethod
{
     private const float DOT_THRESHOLD = 0.5f;
     public static bool IsFacingTarget(this Transform transform, Transform target)
     {
          return (Vector3.Dot(transform.forward, target.position - transform.position) >= DOT_THRESHOLD);
     }
}