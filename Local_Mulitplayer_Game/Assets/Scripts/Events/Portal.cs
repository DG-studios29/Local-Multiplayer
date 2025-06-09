using UnityEngine;
using System.Collections.Generic;

public class Portal : MonoBehaviour
{
    public Transform destination;
    private static Dictionary<Transform, float> cooldownMap = new();
    public float portalCooldown = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (destination == null) return;
        if (!ArenaEventManager.Instance.IsActive("triggerPortalSpawn")) return;

        if (!other.CompareTag("Player") && !other.CompareTag("Army")) return;

        Transform target = other.transform;
        if (cooldownMap.TryGetValue(target, out float lastTime))
        {
            if (Time.time - lastTime < portalCooldown) return;
        }

        target.position = destination.position;
        cooldownMap[target] = Time.time;

        // Optional: trigger effect/sound
    }
}
