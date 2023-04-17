using UnityEngine;

public class EnemyGround : Enemy
{
    public float speed;
    public float jumpForce = 7f;
    public float groundSmooth = 0.08f;
    public float airSmooth = 0.2f;

    public Rect gCheck;
    public Vector2 jCheck;

    protected bool onGround;

    private static AudioA
        Jump;
    protected static readonly int
        Walk = Animator.StringToHash("Walk"),
        MoveS = Animator.StringToHash("MoveS");

    protected float timeSinceLastJump;
    private float wanderRandom = 1f;
    private float wanderTime;
    protected float velTargX;
    private float velCurX;

    private static bool loaded;
    protected override void Awake()
    {
        base.Awake();

        if (TryGetComponent(out BoxCollider2D coll))
        {
            coll.size -= Vector2.one * 0.05f;
        }

        if (!loaded)
        {
            Jump = new("jump", 0.3f);
            loaded = true;
        }
    }

    protected override void Start()
    {
        base.Start();
        wanderTime = Random.value;
    }

    protected override void UpdateTimers()
    {
        timeSinceLastJump += Time.deltaTime;
        base.UpdateTimers();
    }

    protected override void Move()
    {
        if (onGround)
        {
            if (Physics2D.OverlapCircle(transform.position
                    + new Vector3(jCheck.x * (facingRight ? 1f : -1f), jCheck.y),
                    0.1f, collLayers))
            {
                if (timeSinceLastJump > 0.5f)
                    GJump(false);
            }
        }

        if (onGround)
            rb.velocity = new(Mathf.SmoothDamp(rb.velocity.x, velTargX, ref velCurX, groundSmooth,
                Mathf.Infinity, Time.deltaTime), rb.velocity.y);
        else if (velTargX != 0f)
            rb.velocity = new(Mathf.SmoothDamp(rb.velocity.x, velTargX, ref velCurX, airSmooth,
                Mathf.Infinity, Time.deltaTime), rb.velocity.y);
    }

    protected override int GetAnimationState()
    {
        if (velTargX != 0f) return Walk;
        return Idle;
    }

    protected override void UpdateAI()
    {
        onGround = false;
        if (CollCheck()) onGround = true;

        if (Random.value < 0.05f)
            wanderRandom = Random.value;

        if (inLos)
            velTargX = playerDistance.x < 0f ? speed : -speed;
        else
            velTargX = speed * CalcWander();

        if (velTargX != 0f) facingRight = velTargX > 0f;

        base.UpdateAI();
    }

    protected override void UpdateLate()
    {
        velTargX = 0f;
        base.UpdateLate();
    }

    protected override bool CollCheck() => Physics2D.OverlapBox((Vector2)transform.position + gCheck.position, gCheck.size, 0f, collLayers);

    protected virtual void GJump(bool attack)
    {
        timeSinceLastJump = 0f;
        rb.velocity = new Vector2(rb.velocity.x * 1f, jumpForce);
        if (attack) AudioManager.Play(Jump);
    }

    protected virtual float CalcWander()
    {
        wanderTime += Time.deltaTime * wanderRandom;
        return Mathf.Round(Mathf.Sin(wanderTime * 1.5f)) * 0.5f;
    }

    public override bool Damage(DamageInfo inf)
    {
        wanderTime = 0f;
        return base.Damage(inf);
    }
}
