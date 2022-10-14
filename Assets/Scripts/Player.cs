using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UltEvents;
using DG.Tweening;


[RequireComponent(typeof(MovementController), typeof(PlayerInput), typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    // ==============================================================================
    public UltEvent OnMoveForward = new UltEvent();
    public UltEvent OnDamageTaken = new UltEvent();
    public UltEvent OnFall = new UltEvent();
    public UltEvent OnKilled = new UltEvent();

    [Space]
    public float PressThreshold = 20f;
    public float FallOffset = 3.95f;
    public float MoveDistance = 1f;
    public float DeathAnimTime = 2.75f;
    public float DeathAnimAmplitude = 5f;
    public Ease DeathEase = Ease.InOutElastic;
    public float FallForce = 50f;

    [Space]
    public AudioSource JumpSFX;
    public AudioSource DeathSFX;
    public AudioSource FallSFX;

    [Space]
    public MovementController CachedMovementController;
    public PlayerInput CachedPlayerInput;
    public Rigidbody CachedRigidbody;

    public bool IsDead { get; private set; }
    public Vector3 TargetMovePosition { get; private set; }
    
    private Vector2 _pressStartPosition;
    private Tween _deathTween;


    // ==============================================================================
    public void TakeDamage()
    {
        if (IsDead)
            return;

        Kill();

        _deathTween = transform.DOScale(Vector3.zero, DeathAnimTime).SetEase(DeathEase, DeathAnimAmplitude, 1f);
        DeathSFX.Play();

        OnDamageTaken.Invoke();
    }

    protected void Reset()
    {
        CachedMovementController = GetComponent<MovementController>();
        CachedPlayerInput = GetComponent<PlayerInput>();
        CachedRigidbody = GetComponent<Rigidbody>();
    }

    protected void Awake()
    {
        if (!CachedMovementController)
            CachedMovementController = GetComponent<MovementController>();

        if (!CachedPlayerInput)
            CachedPlayerInput = GetComponent<PlayerInput>();

        if (!CachedRigidbody)
            CachedRigidbody = GetComponent<Rigidbody>();

        CachedMovementController.OnStartMoving += JumpSFX.Play;
        
        TargetMovePosition = transform.position;
    }

    protected void Update()
    {
        if (IsDead)
            return;

        if (Mathf.Abs(transform.position.x) > FallOffset)
            Fall();
    }

    protected void OnDestroy()
    {
        if (_deathTween.IsActive())
            _deathTween.Kill();
    }

    private void Fall()
    {
        if (IsDead)
            return;

        Kill();

        CachedRigidbody.isKinematic = false;
        StartCoroutine(FallDelayed());
        FallSFX.Play();

        OnFall.Invoke();
    }

    private void Kill()
    {
        if (IsDead)
            return;

        IsDead = true;
        CachedPlayerInput.DeactivateInput();

        OnKilled.Invoke();
    }

    private IEnumerator FallDelayed()
    {
        yield return new WaitForFixedUpdate();

        CachedRigidbody.AddForce(Vector3.down * FallForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(3f);

        CachedRigidbody.isKinematic = true;
    }

    private void MoveRight()
    {
        TargetMovePosition += new Vector3(MoveDistance, 0f, 0f);
        CachedMovementController.Move(TargetMovePosition);
    }

    private void MoveLeft()
    {
        TargetMovePosition += new Vector3(-MoveDistance, 0f, 0f);
        CachedMovementController.Move(TargetMovePosition);
    }

    private void MoveForward()
    {
        TargetMovePosition += new Vector3(0f, MoveDistance, MoveDistance);
        CachedMovementController.Move(TargetMovePosition);

        OnMoveForward.Invoke();
    }

    private void OnTap(InputValue input)
    {
        if (input.isPressed)
        {
            _pressStartPosition = Pointer.current.position.ReadValue();
        }
        else
        {
            var pointerPos = Pointer.current.position.ReadValue();
            var dX = pointerPos.x - _pressStartPosition.x;
            if (dX > PressThreshold)
                MoveRight();
            else if (dX < -PressThreshold)
                MoveLeft();
            else
                MoveForward();
        }
    }
}