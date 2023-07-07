using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : Entity
{
    #region Stats
    [Header("Stats")]
    public Stat hp;
    public int def;
    public DamageInfo contactHit;
    public float kbMult = 1f;
    public float losRange = 4f;
    public bool facePlayer = true;
    public bool canDieToEnv = true;
    public bool stompable = false;
    protected const int collLayers = 9;
    [System.NonSerialized]
    public float[] statusEffects;
    #endregion
    #region References
    [Header("References")]
    public GameObject body;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected EnemyHPBar hpBar;
    protected Animator animator;
    private SpriteRenderer emoticon;
    protected Player player;
    #endregion
    #region Cached
    private float upperBound;
    private Rect hCheck;
    private UINumberDamage storedDN;

    private float iFrames;
    private int dmgCombo;
    private float damageTime;
    private float hitstunTime;
    #endregion
    #region Properties
    protected Vector2 playerDistance;
    protected bool inLos;
    #endregion
    #region Visual
    private float alpha;
    private float bodyScaleX = 1f;
    private float bodyTargX = 1f;
    private float bodyCurX;
    private int currentState;
    private float lockedUntil;
    #endregion
    // kill me
    #region Resources
    private static Sprite foundS, lostS;
    private static PhysicsMaterial2D material;
    private static AudioA hit, hitno, found, lost, splash;
    private static GameObject rHpBar, deathFx, tpPickup, damageNumber;
    protected static AudioClip[] statusClips;
    protected static readonly int
    Idle = Animator.StringToHash("Idle"),
    Hurt = Animator.StringToHash("Hurt");
    #endregion

    #region Initialization
    private static bool loaded;
    protected override void Awake()
    {
        startPos = transform.position;

        if (!loaded)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/UI/emoticons");
            foundS = sprites[0];
            lostS = sprites[1];
            material = Resources.Load<PhysicsMaterial2D>("NoFrict");
            hit = new("damage_enemy", 0.6f);
            hitno = new("damage_none", 0.6f);
            found = new("found", 0.4f);
            lost = new("lost", 0.4f);
            splash = new("splash");
            damageNumber = Resources.Load<GameObject>("Particle/UIDamageNum");
            rHpBar = Resources.Load<GameObject>("Object/HPBar");
            tpPickup = Resources.Load<GameObject>("Object/TPPickup");
            deathFx = Resources.Load<GameObject>("Particle/Death");
            loaded = true;
        }
        var dos = GetComponent<DespawnOffScreen>();
        dos.disable = this;
        dos.sleep = rb;

        if (TryGetComponent(out rb))
        {
            rb.sharedMaterial = material;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        sr = body.GetComponent<SpriteRenderer>();
        animator = body.GetComponent<Animator>();
        statusEffects = new float[System.Enum.GetValues(typeof(Status)).Length];
    }

    protected virtual void Start()
    {
        player = Player.Inst;

        Respawn();

        // Instantiate stuff cause i'm too lazy to position them myself
        upperBound = sr.bounds.extents.y;
        losRange *= losRange;
        BoxCollider2D hitbox = GetComponent<BoxCollider2D>();
        hCheck.position = hitbox.offset;
        hCheck.size = hitbox.size;

        hpBar = GameManager.Spawn(rHpBar,
            transform,
            Vector3.down * (upperBound + 0.25f))
            .GetComponent<EnemyHPBar>();

        hpBar.Set(hp);
        hpBar.enabled = true;

        GameObject iconNew = new();
        iconNew.transform.parent = transform;
        iconNew.transform.localPosition = Vector3.up * upperBound + Vector3.right * 0.25f;
        emoticon = iconNew.AddComponent<SpriteRenderer>();
        emoticon.color = Utils.clearW;
    }
    #endregion

    private void FixedUpdate()
    {
        UpdateEarly();
        UpdateTimers();
        UpdateAI();
        Move();
        UpdateVisual();
        UpdateLate();
        UpdateStatuses();
    }

    #region Base update loop
    /// <summary>
    /// Override for logic that runs before everything else.
    /// </summary>
    protected virtual void UpdateEarly() { }

    /// <summary>
    /// Override for logic that updates timers.
    /// </summary>
    protected virtual void UpdateTimers()
    {
        Utils.TimeDown(ref damageTime);
        Utils.TimeDown(ref iFrames);
        alpha += Time.deltaTime;
    }

    /// <summary>
    /// Override for AI.
    /// </summary>
    protected virtual void UpdateAI()
    {
        playerDistance = transform.position - player.transform.position;
        bool wasInLos = inLos;
        if (losRange != -1f) inLos = playerDistance.sqrMagnitude < losRange;

        if (!wasInLos && inLos)
        {
            AudioManager.Play(found);
            emoticon.sprite = foundS;
            alpha = -0.2f;
        }
        else if (wasInLos && !inLos)
        {
            AudioManager.Play(lost);
            emoticon.sprite = lostS;
            alpha = -0.2f;
        }

        if (contactHit.damage == 0f) return;
        Collider2D check = Physics2D.OverlapBox((Vector2)transform.position + hCheck.position, hCheck.size, 0f, 64);
        if (check)
        {
            if (Player.Inst.DamageContact(transform.position, contactHit))
            {
                Damage(new() { damage = 0, iFrames = 0.5f });
            }
        }
    }

    /// <summary>
    /// Override for movement.
    /// </summary>
    protected virtual void Move() { }

    /// <summary>
    /// Override for visual updates after everything else.
    /// </summary>
    protected virtual void UpdateVisual()
    {
        bodyTargX = facingRight ? 1f : -1f;
        bodyScaleX = Mathf.SmoothDamp(bodyScaleX, bodyTargX, ref bodyCurX, 0.1f, Mathf.Infinity);
        body.transform.localScale = new(bodyScaleX, 1f, 1f);
        emoticon.color = Color.Lerp(Color.white, Utils.clearW, alpha * 3f);

        /*
        if (damageTime > 0f)
        {
            body.transform.localPosition = new((Random.value - 0.5f) * 0.1f + damageTime * 0.5f, 0f);
        }
        else
        {
            body.transform.localPosition = Vector2.zero;
        }
        */

        int state = GetAnimationState();
        if (Time.time > lockedUntil && currentState != state)
        {
            animator.CrossFade(state, 0f);
            currentState = state;
        }
    }

    protected virtual int GetAnimationState() => Idle;

    private void StartAnim(int state, float t)
    {
        animator.CrossFade(state, 0f);
        currentState = state;
        lockedUntil = Time.time + t;
    }

    /// <summary>
    /// Override for anything that has to run after everything else (except status effects)
    /// </summary>
    protected virtual void UpdateLate()
    {
        if (transform.position.y < -6.5f)
        {
            if (canDieToEnv)
            {
                AudioManager.PlayAdv(splash, transform.position);
                Die();
            }
            else
                transform.position = new(transform.position.x, -6.5f);
        }
        else if (transform.position.y > 10.5f)
            transform.position = new(transform.position.x, 10.5f);
    }

    /// <summary>
    /// Updates status effects (duh)
    /// </summary>
    private void UpdateStatuses()
    {
        for (int i = 0; i < statusEffects.Length; i++)
        {
            Utils.TimeDown(ref statusEffects[i]);
        }
    }
    #endregion

    #region Functions
    public override bool Damage(DamageInfo inf)
    {
        if (iFrames > 0f && !inf.ignoreIFrames) return false;

        FollowCamera.Focus(transform);

        iFrames = inf.iFrames;
        hitstunTime = inf.iFrames;
        inf.damage -= def;

        if (damageTime == 0f) dmgCombo = 0;

        if (inf.damage > 0)
        {
            hp.Add(-inf.damage);

            StartAnim(Hurt, 0.25f);

            if (damageTime < 0.15f)
            {
                AudioA newHit = hit;
                newHit.pitch = Utils.Pow(1.0594631f, dmgCombo);
                AudioManager.PlayAdv(newHit, transform.position);
            }
        }
        else
        {
            inf.damage = 0;
            AudioManager.PlayAdv(hitno, transform.position);
        }

        if (kbMult > 0f)
            rb.velocity = inf.addForce * kbMult;

        if (hp <= 0)
        {
            if (inf.noKill)
            {
                hp.Set(1);
                return true;
            }
            Die();
        }

        Vector3 pos = transform.position + Vector3.up * upperBound;

        if (!storedDN || damageTime == 0f || (storedDN.transform.position - pos).sqrMagnitude > 1f)
        {
            storedDN = GameManager.Spawn(damageNumber, pos).GetComponent<UINumberDamage>();
        }
        storedDN.Set(0, inf);

        dmgCombo++;
        if (dmgCombo > 12) dmgCombo = 12;
        damageTime = 0.25f;

        hpBar.Set(hp);
        return true;
    }

    public override void AddStatusEffect(Status status, float time)
    {
        if (time != -1f)
        {
            if (time > statusEffects[(int)status])
                statusEffects[(int)status] = time;
        }
        else
            statusEffects[(int)status] = 0f;
    }

    protected virtual void Respawn()
    {
        hp.Add(99);
        transform.position = startPos;
        gameObject.SetActive(true);
    }

    protected virtual void Die()
    {
        GameManager.Spawn(deathFx, transform.position);

        float target = hp.GetMax * 0.2f;
        int amount = (Party.Inst.tp <= 4 ? 2 : 1) + Random.Range(0, Mathf.FloorToInt(target));

        while (amount > 0)
        {
            GameManager.Spawn(tpPickup, transform.position);
            amount--;
        }
        Destroy(gameObject);
    }

    protected virtual bool CollCheck() => false;
    #endregion
}