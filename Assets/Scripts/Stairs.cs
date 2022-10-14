using UnityEngine;


public class Stairs : MonoBehaviour
{
    // ==============================================================================
    public Vector3 Size = Vector3.one;


    // ==============================================================================
    protected void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, Size);
    }
}
