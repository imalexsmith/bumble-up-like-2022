using UnityEngine;


[RequireComponent(typeof(Collider))]
public class DamageTrigger : MonoBehaviour
{
    // ==============================================================================
    public Collider CachedCollider;


    // ==============================================================================
    protected void Reset()
    {
        CachedCollider = GetComponent<Collider>();
        CachedCollider.isTrigger = true;
    }

    protected void Awake()
    {
        if (!CachedCollider)
            CachedCollider = GetComponent<Collider>();
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
            player.TakeDamage();
    }
}