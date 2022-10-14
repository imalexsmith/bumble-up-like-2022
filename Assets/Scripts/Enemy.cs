using System.Collections;
using UnityEngine;


[RequireComponent(typeof(MovementController), typeof(DamageTrigger))]
public class Enemy : MonoBehaviour
{
    // ==============================================================================
    public Vector3 MoveStep = new Vector3(0f, -1f, -1f);
    public float NextMoveDelay = 0.3f;

    [Space]
    public MovementController CachedMovementController;

    protected virtual Vector3 NextMovePosition => transform.position + MoveStep;


    // ==============================================================================
    protected void Reset()
    {
        CachedMovementController = GetComponent<MovementController>();
    }

    protected void Awake()
    {
        if (!CachedMovementController)
            CachedMovementController = GetComponent<MovementController>();

        CachedMovementController.OnCompleteMoving += NextMove;
    }

    protected virtual void Start()
    {
        NextMove();
    }

    protected virtual void NextMove()
    {
        StartCoroutine(NextMoveDelayed(NextMoveDelay));
    }

    private IEnumerator NextMoveDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        CachedMovementController.Move(NextMovePosition);
    }
}