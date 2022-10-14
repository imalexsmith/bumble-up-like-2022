using UnityEngine;
using DG.Tweening;
using UltEvents;


public class MovementController : MonoBehaviour
{
    // ==============================================================================
    public UltEvent OnStartMoving = new UltEvent();
    public UltEvent OnCompleteMoving = new UltEvent();

    [Space]
    public float MoveSpeed = 2.5f;
    public float JumpHeight = 1f;
    public Ease JumpEase = Ease.OutQuad;

    private Tween _moveTween;


    // ==============================================================================
    public void Move(Vector3 endPos)
    {
        if (_moveTween.IsActive())
            _moveTween.Complete(true);

        var t = (endPos - transform.position).magnitude / MoveSpeed;
        _moveTween = transform.DOJump(endPos, JumpHeight, 1, t).SetEase(JumpEase);
        _moveTween.onComplete += OnCompleteMoving.Invoke;

        OnStartMoving.Invoke();
    }

    public void Stop()
    {
        if (_moveTween.IsActive())
            _moveTween.Kill();
    }

    protected void OnDestroy()
    {
        Stop();
    }
}