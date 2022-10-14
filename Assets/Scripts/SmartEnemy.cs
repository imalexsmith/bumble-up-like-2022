using UnityEngine;
using DG.Tweening;


public class SmartEnemy : Enemy
{
    // ==============================================================================
    public float RotationStrength = 10f;
    public Ease RotationEase = Ease.InOutElastic;
    
    protected override Vector3 NextMovePosition
    {
        get
        {
            var pos = base.NextMovePosition;
            var x = Mathf.MoveTowards(pos.x, _player.transform.position.x, 1f);
            return new Vector3(x, pos.y, pos.z);
        }
    }

    private Player _player;
    private Tween _rotateTween;


    // ==============================================================================
    protected override void Start()
    {
        base.Start();

        _player = FindObjectOfType<Player>();
    }

    protected void OnDestroy()
    {
        if (_rotateTween.IsActive())
            _rotateTween.Kill();
    }

    protected override void NextMove()
    {
        base.NextMove();

        if (_rotateTween.IsActive())
            _rotateTween.Complete();

        var sign = Random.value < 0.5f ? -1f : 1f;
        _rotateTween = transform.DOPunchRotation(sign * RotationStrength * Vector3.up, NextMoveDelay).SetEase(RotationEase);
    }
}