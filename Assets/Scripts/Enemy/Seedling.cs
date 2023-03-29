using UnityEngine;

public class Seedling : EnemyGround
{
    public bool canFly;
    private float flyRcTime;
    private bool flying;
    private bool attackJump;
    protected static readonly int
        Attack = Animator.StringToHash("Attack");

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void UpdateTimers()
    {
        base.UpdateTimers();
        if (canFly)
        {
            flying = !Utils.TimeUp(ref flyRcTime, 3f);
        }

        if (playerDistance.sqrMagnitude < 4f && onGround)
        {
            if (timeSinceLastJump > 0.5f)
                GJump(true);
            attackJump = true;
        }
    }

    protected override int GetAnimationState()
    {
        if (velTargX != 0f) return Walk;
        return flying ? Idle : Idle;
    }

    protected override void UpdateVisual()
    {
        animator.SetFloat(MoveS, inLos ? 1f : 0.5f);
        base.UpdateVisual();
    }

    public override bool Damage(DamageInfo inf)
    {
        flyRcTime = 0f;
        return base.Damage(inf);
    }
}
