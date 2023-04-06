using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public static Player Inst { get; private set; }

    [Space]
    [Header("Stats")]
    public float groundSmooth = 0.08f;
    public float speed = 4f;
    public float airSmooth = 0.2f;
    public float jumpForce = 10f;
    public float maxGrav = 10f;
    public float maxFlyTime = 1f;
    public float setCoyoteTime = 0.08f;
    public float slopeCorrect = 0.5f;
    public float slideMult = 1.5f, slideSpeedLoss = 0.2f;
    public Rect gCheck = new(new(0f, -0.5f), new(0.42f, 0.1f));

    [Space]
    [Header("References")]
    public List<AudioA> clips;
    public AudioA footstepClips;
    public GameObject
        damageNumber,
        healNumber,
        doubleJumpFx,
        dashTrailFx,
        swingFx,
        actionText;
    public PlayerBody playerBody;
    public Weapon weapon;
    // TODO: Implement proper looping audio system
    public AudioSource audioEx, audioSldEx;
    public ParticleSystem fsFx;
    public BubbleShield shield;
    private Party party;
    private SpriteRenderer sr;
    private Animator animator;
    private Rigidbody2D rb;

    // Loaded resources
    private static readonly int
        Idle = Animator.StringToHash("Idle"),
        Walk = Animator.StringToHash("Walk"),
        Jump = Animator.StringToHash("Jump"),
        Fall = Animator.StringToHash("Fall"),
        Crouch = Animator.StringToHash("Crouch"),
        Slide = Animator.StringToHash("Slide"),
        Wallslide = Animator.StringToHash("Wallslide"),
        Hurt = Animator.StringToHash("Hurt"),
        SkillA = Animator.StringToHash("SkillA"),
        Skill0 = Animator.StringToHash("Skill0"),
        Skill1 = Animator.StringToHash("Skill1"),
        Skill2 = Animator.StringToHash("Skill2"),
        MoveSpeed = Animator.StringToHash("MoveSpeed");

    private Vector2 lastTouchedGround;
    private float currentVel;

    private bool onGround = true;
    private bool wasOnGround;

    private bool sliding;
    private float slideStoredVel;
    private bool wallSliding;
    private int wallSlideDir;

    private float targetX;
    public bool crouching;

    private bool endedJump;
    public bool canEndJump;
    private bool doubleJump;

    private bool canRecoil = true;

    // literally just timers
    private float coyoteTime;
    private float notOnGroundTime;
    private float noControlTime;
    private float flyTime = 1f;
    private bool flying;
    private float dashCooldown, dashTime;
    private bool dashDir;
    private bool dashFx;

    // spaghetti code!
    public float damageTime;
    public float iFrames;
    public float shieldTime;
    private float healWait;
    private int currentState;
    private float lockedUntil;

    private float gravityCap = -10f;

    // Visual
    private float fsCycle;
    private Color hurtColor = new(1f, 1f, 1f, 0.25f);

    private static bool loaded;
    protected override void Awake()
    {
        base.Awake();

        Inst = this;
        print("Player loaded in");

        rb = GetComponent<Rigidbody2D>();
        sr = playerBody.GetComponent<SpriteRenderer>();
        animator = playerBody.GetComponent<Animator>();
        input = new();
    }

    private void Start()
    {
        ResetPos();
        party = Party.Inst;
        transform.position = startPos;
    }

    // Input
    private InputMaster input;
    private float inMove, inJumpBuffer;
    private bool inJump, inCrouch;
    private int inSwitch;
    private bool[] inSkill = new bool[3];
    private bool inSkillA, inSkillAHeld;
    private void OnEnable() => input.Player.Enable();
    private void OnDisable() => input.Player.Disable();

    private void Update()
    {
        if (input.Player.Jump.triggered) inJumpBuffer = 0.08f;
        inJump = input.Player.Jump.IsPressed();
        inMove = input.Player.Horizontal.ReadValue<float>();
        inCrouch = input.Player.Crouch.IsPressed();

        // oh the misery
        if (input.Player.Switch0.triggered) inSwitch = 0;
        else if (input.Player.Switch1.triggered) inSwitch = 1;
        else if (input.Player.Switch2.triggered) inSwitch = 2;

        if (input.Player.Skill0.triggered) inSkill[0] = true;
        else if (input.Player.Skill1.triggered) inSkill[1] = true;
        else if (input.Player.Skill2.triggered) inSkill[2] = true;

        if (input.Player.SkillA.triggered) inSkillA = true;
        inSkillAHeld = input.Player.SkillA.IsPressed();
    }

    // whew
    private void FixedUpdate()
    {
        // just for convenience
        Vector2 vel = rb.velocity;

        // check movement input and update facing direction
        if (inMove > 0f && !facing) facing = true;
        else if (inMove < 0f && facing) facing = false;

        if (inSwitch > -1)
        {
            Switch(inSwitch);
        }

        // update timers
        Utils.TimeDown(ref coyoteTime);
        Utils.TimeDown(ref noControlTime);
        Utils.TimeDown(ref shieldTime);
        Utils.TimeDown(ref damageTime);
        Utils.TimeDown(ref iFrames);
        Utils.TimeDown(ref notOnGroundTime);
        Utils.TimeDown(ref dashCooldown);
        if (Utils.TimeDownTick(ref healWait))
            Heal(5, false);

        if (Utils.TimeDown(ref dashTime))
            UpdateDash();

        // check for ground collision
        wasOnGround = onGround;
        onGround = false;

        if (notOnGroundTime == 0f
            && Physics2D.OverlapBox(
                (Vector2)transform.position + gCheck.position,
                gCheck.size, 0f, 9))
        {
            onGround = true;
            if (!wasOnGround)
            {
                if (vel.y < 1f)
                {
                    playerBody.scale.y = 0.7f;
                    fsCycle = 1f;
                }
            }
        }

        if (Utils.TimeDown(ref inJumpBuffer))
        {
            GJump();
        }

        // activate skills
        // TODO: fix this mess
        switch (party.current)
        {
            case Mb.Vi:
                if (Skill(0, 0))
                {
                    StartAnim(Skill0, 6f / 12f);
                    weapon.Throw();
                }
                if (Skill(0, 1))
                {
                    StartAnim(Skill1, 2f / 12f);
                    weapon.FlipCoin();
                }
                if (Skill(0, 2))
                {
                    healWait = 6f / 12f;
                    AudioManager.Play(clips[16]);
                    StartAnim(Skill2, 6f / 12f);
                }
                break;
            case Mb.Kabbu:
                if (Skill(1, 0))
                {
                    Cut();
                }
                if (inSkillA)
                {
                    if (dashCooldown == 0f)
                    {
                        AudioManager.Play(clips[4]);
                        dashTime = 0.15f;
                        dashCooldown = 0.4f;
                        noControlTime = 0.15f;
                        dashDir = facing;
                    }
                }
                break;
            case Mb.Leif:
                if (Skill(2, 1))
                {
                    AudioManager.Play(clips[6]);
                    shieldTime = 10f;
                    shield.gameObject.SetActive(true);
                }
                break;
        }

        // check for wall collision
        if (Physics2D.OverlapBox((Vector2)transform.position
            + new Vector2(-0.24f, -0.35f),
            new(0.1f, 0.2f), 0f, 1))
            wallSlideDir = -1;
        else if (Physics2D.OverlapBox((Vector2)transform.position
            + new Vector2(0.24f, -0.35f),
            new(0.1f, 0.2f), 0f, 1))
            wallSlideDir = 1;
        else
            wallSlideDir = 0;

        wallSliding = wallSlideDir != 0 && inMove != 0f && vel.y <= 0f && !onGround;

        // find target velocity
        targetX = inMove * speed;

        if (onGround)
        {
            if (crouching) targetX *= 0.5f;

            // attempt vertical normalisation
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down * 0.5f, 1f, 9);
            if (hit.collider)
            {
                lastTouchedGround = transform.position;
                /*if (Mathf.Abs(hit.normal.x) > 0.1f)
                {
                    // Apply the opposite force against the slope force 
                    vel.x -= hit.normal.x * slopeCorrect;

                    //Move Player up or down to compensate for the slope below them
                    Vector3 pos = transform.position;
                    pos.y += -hit.normal.x * Mathf.Abs(vel.x) * Time.deltaTime * (vel.x - hit.normal.x > 0 ? 1 : -1);
                    transform.position = pos;
                }*/
            }
            
            // Sliding & crouching
            if (noControlTime > 0f)
            {
                // Cancel crouch
                crouching = false;
            }
            else
            {
                if (inCrouch)
                {
                    // Start crouch
                    if (!crouching)
                    {
                        playerBody.scale.y = 0.8f;
                        crouching = true;
                        if (inMove != 0f)
                        {
                            sliding = true;
                            slideStoredVel = speed * slideMult * Mathf.Round(inMove); 
                            //Mathf.Max(speed , Mathf.Abs(vel.x)) * slideMult * Mathf.Round(inMove);
                            audioSldEx.Play();
                        }
                    }
                }
                else if (crouching)
                {
                    crouching = false;
                    sliding = false;
                }
            }

            if (!sliding)
            {
                if (noControlTime == 0f)
                {
                    vel.x = Mathf.SmoothDamp(vel.x, targetX, ref currentVel,
                        Mathf.Abs(targetX) > 0f ? groundSmooth : groundSmooth * 0.5f,
                        Mathf.Infinity, Time.deltaTime);
                }

                if (targetX != 0f)
                {
                    fsCycle += Time.deltaTime * 3f;
                    if (fsCycle > 1f)
                    {
                        fsCycle = 0f;
                        AudioManager.Play(footstepClips);
                        fsFx.Emit(1);
                    }
                }
            }
            // Reset variables on ground
            coyoteTime = setCoyoteTime;
            canEndJump = true;
            doubleJump = true;
            canRecoil = true;
            wallSlideDir = 0;
            flyTime = maxFlyTime;
        }
        else
        {
            sliding = false;
            if (noControlTime == 0f)
            {
                vel.x = Mathf.SmoothDamp(vel.x, targetX, ref currentVel,
                    airSmooth, Mathf.Infinity, Time.deltaTime);
            }
        }

        // end jump early if jump is released
        if ((!endedJump && !inJump))
        {
            endedJump = true;
            noControlTime = 0f;
        }

        flying = false;
        if (party.current == Mb.Vi
            && inSkillAHeld
            && flyTime > 0f
            && noControlTime == 0f)
        {
            flying = true;
            if (onGround)
            {
                vel.y = 6f;
                notOnGroundTime = 0.1f;
            }
            else
            {
                vel.y *= 0.94f;
                if (vel.y < 0f)
                {
                    vel.y += 15f * Time.deltaTime;
                    if (vel.y > 0f) vel.y = 0f;
                }
            }

            if (!audioEx.isPlaying) audioEx.Play();

            Utils.TimeDown(ref flyTime);
        }

        rb.gravityScale = 1f;
        gravityCap = maxGrav;
        if (inCrouch)
        {
            gravityCap += 5f;
        }

        if (flying || dashTime > 0.05f)
            rb.gravityScale = 0f;
        else
        {
            if (vel.y > 0f)
            {
                if (canEndJump && endedJump)
                {
                    rb.gravityScale = 5f;
                }
            }
            else
            {
                if (wallSliding)
                {
                    rb.gravityScale = 0.1f;
                    gravityCap = 3f;
                }
            }
        }

        // TEMP
        if (sliding) vel.x = slideStoredVel;
        slideStoredVel += slideSpeedLoss * (slideStoredVel > 0f ? -1f : 1f);
        if (Mathf.Abs(slideStoredVel) <= 2f)
        {
            sliding = false;
        }

        if (vel.y < -gravityCap)
            vel.y = -gravityCap;

        if (transform.position.y < -6.5f)
        {
            AudioManager.Play(clips[11]);
            ResetPos();
        }
        /*else if (transform.position.y > 10.5f)
        {
            transform.position = new(transform.position.x, 10.5f);
            vel.y = 0f;
        }*/

        // finally, update with new velocity
        rb.velocity = vel;

        // update visuals
        animator.SetFloat(MoveSpeed, Mathf.Abs(inMove));

        /* DEBUG
        sr.color = Color.white;
        if (damageTime > 0f) sr.color = Color.red;
        if (notOnGroundTime > 0f) sr.color = Color.green;
        if (noControlTime > 0f) sr.color = Color.yellow;
        if (dashTime > 0f) sr.color = Color.blue;
        */
        
        if (iFrames > 0f && dashTime == 0f)
            sr.color = Time.time % 0.1f >= 0.05f ? Color.white : hurtColor;
        else
            sr.color = Color.white;

        playerBody.snapToFacing = wallSliding || dashTime > 0f || sliding;

        int state = GetAnimationState();
        if (Time.time > lockedUntil && currentState != state)
        {
            animator.CrossFade(state, 0f);
            currentState = state;
        }

        // stop looping
        if (!flying)
        {
            if (audioEx.isPlaying)
                audioEx.Stop();
        }

        if (!sliding)
        {
            if (audioSldEx.isPlaying)
                audioSldEx.Stop();
        }

        // reset input
        // (god this is such an awkward way to do this)
        inSwitch = -1;
        for (int i = 0; i < inSkill.Length; i++) inSkill[i] = false;
        inSkillA = false;

        int GetAnimationState()
        {
            if (damageTime > 0f) return Hurt;

            if (flying || dashTime > 0f) return SkillA;

            if (onGround)
            {
                if (sliding) return Slide;
                if (crouching) return Crouch;
                if (Mathf.Abs(targetX) > 0f) return Walk;
            }
            else
            {
                if (rb.velocity.y > 0f) return Jump;
                else return wallSliding ? Wallslide : Fall;
            }
            return Idle;
        }

        void GJump()
        {
            int type;
            if (onGround || coyoteTime > 0f)
                type = 0;
            else if (wallSlideDir != 0)
                type = 1;
            else if (doubleJump)
                type = 2;
            else return;

            notOnGroundTime = 0.1f;
            coyoteTime = 0f;
            inJumpBuffer = 0f;
            vel.y = jumpForce;

            switch (type)
            {
                case 0 when dashTime > 0f || sliding:
                    AudioManager.Play(clips[17]);
                    vel.x = (facing ? 1f : -1f) * 10f;
                    vel.y *= 0.7f;
                    noControlTime = 0.15f;
                    canEndJump = false;
                    break;
                case 0:
                    AudioManager.Play(clips[0]);
                    break;
                case 1:
                    AudioManager.Play(clips[1]);
                    vel.x = 6f * -wallSlideDir;
                    vel.y -= 1f;
                    // neutral jump (celeste reference ?!)
                    noControlTime = inMove == 0f ? 0.05f : 0.15f;
                    break;
                case 2:
                    AudioManager.Play(clips[1]);
                    vel.x = inMove * speed;
                    doubleJump = false;
                    GameManager.Spawn(doubleJumpFx, transform.position + Vector3.down * 0.5f);
                    break;
            }

            dashTime = 0f;
            endedJump = false;
            onGround = false;
            playerBody.scale.y = 1.35f;
        }

        void UpdateDash()
        {
            if (dashTime > 0.05f)
            {
                vel.x = dashDir ? 15f : -15f;
                vel.y = 0f;
            }

            if (dashFx)
            {
                GameObject dt = GameManager.Spawn(dashTrailFx, transform.position);
                dt.transform.localScale = new(facing ? -1f : 1f, 1f);
                dashFx = false;
            }
            else dashFx = true;

            Collider2D check = Physics2D.OverlapBox(transform.position,
                new(0.6f, 0.85f), 0f, 1);
            if (check)
            {
                vel = new(facing ? -4f : 4f, 6f);
                dashTime = 0f;
                noControlTime = 0.1f;
                notOnGroundTime = 0.1f;
                canEndJump = false;
                FollowCamera.Shake(1f);
                damageTime = 0.25f;
                AudioManager.Play(clips[5]);
            }
        }

        void Cut()
        {
            StartAnim(Skill0, 5f / 12f);
            AudioManager.Play(clips[9]);
            // TODO: make this not reliant on a visual variable
            bool dir = dashTime > 0f ? facing : !weapon.flipped;
            GameManager.Spawn(swingFx, transform.position)
                .GetComponent<SpriteRenderer>().flipX = dir;
            Collider2D[] colls = Physics2D.OverlapBoxAll(
                (Vector2)transform.position
                + Vector2.up * 0.375f
                + Vector2.right * (dir ? 0.75f : -0.75f),
                new(1.5f, 1.75f), 0f);
            bool first = true;
            foreach (Collider2D coll in colls)
            {
                if (coll.TryGetComponent(out Enemy enemy))
                {
                    DamageInfo inf = new()
                    {
                        damage = 2,
                        addForce = new(dir ? 5f : -5f, 7f),
                        iFrames = 0.25f
                    };

                    if (dashTime > 0f)
                    {
                        inf.damage *= 2;
                        inf.addForce.x = dir ? 8f : -8f;
                        inf.addForce.y = 3f;
                    }
                    enemy.Damage(inf);
                    GameManager.Freeze(1f / 60f);
                }

                if (coll.TryGetComponent(out Projectile proj))
                {
                    if (!proj.enemy) return;
                    if (first)
                    {
                        AudioManager.Play(clips[14]);
                        GameManager.Freeze(12f / 60f);
                        SpawnActionText(6, 0.5f);
                        first = false;
                    }
                    proj.time = 0f;
                    proj.inf.damage++;
                    proj.inf.crit = true;
                }
            }
        }
    }

    private void StartAnim(int state, float t)
    {
        animator.CrossFade(state, 0f);
        currentState = state;
        lockedUntil = Time.time + t;
    }

    public override bool Damage(DamageInfo inf)
    {
        if (iFrames > 0f && !inf.ignoreIFrames) return false;
        if (sliding) return false;

        // this is quite generous
        iFrames = 1f;

        if (shieldTime > 0f)
        {
            inf.damage = 0;
            AudioManager.Play(clips[10]);
        }
        else if (dashTime > 0f)
        {
            iFrames *= 0.5f;
            SpawnActionText(4, 0.5f);
            return false;
        }

        if (inf.damage > 0)
        {
            party.hp.Add(-inf.damage);

            damageTime = 0.5f;
            noControlTime = 0.25f;
            notOnGroundTime = 0.1f;

            FollowCamera.Shake(0.6f + 0.2f * Mathf.Min(inf.damage, 10));

            AudioManager.Play(clips[7]);

            Vector2 newVel = rb.velocity;
            newVel *= 0.5f;
            newVel.y = 4f;
            newVel += inf.addForce;
            rb.velocity = newVel;
            canEndJump = false;
        }
        else
        {
            damageTime = 0.25f;
            AudioManager.Play(clips[8]);
        }

        sliding = false;

        if (party.hp <= 0)
        {
            if (inf.noKill)
            {
                party.hp.Set(1);
            }
            // TEMP
            party.hp.Add(99);
            party.tp.Add(99);
            //ResetPos();
        }

        GameManager.Spawn(damageNumber, transform.position + Vector3.up * 0.75f)
            .GetComponent<UINumberDamage>().Set(1, inf);

        return true;
    }

    public void Heal(int amount, bool healTp)
    {
        if (healTp)
            party.tp.Add(amount);
        else
        {
            party.hp.Add(amount);
            AudioManager.Play(clips[13]);
        }

        GameManager.Spawn(healNumber, transform.position + Vector3.up * 0.75f)
            .GetComponent<UINumberHeal>().Set(healTp ? 1 : 0, amount);
    }

    public bool Skill(int user, int stat)
    {
        Skill skill = party.allies[user].skills[stat];

        if (inSkill[stat])
        {
            if (skill.time == 0f
                && party.tp >= skill.cost)
            {
                skill.time = skill.cooldown;
                //if (skill.cost > 0)
                //    party.tp.Add(-skill.cost);
                return true;
            }
            else
            {
                AudioManager.Play(clips[3]);
                return false;
            }
        }
        return false;
    }

    public override void AddStatusEffect(Status status, float time)
    {
        if (time != -1f)
        {
            if (time > party.Cur.se[(int)status])
                party.Cur.se[(int)status] = time;
        }
        else
            party.Cur.se[(int)status] = 0f;
    }

    private void Switch(int to)
    {
        if (to == (int)party.current) return;
        if (to >= party.allies.Count) return;

        if (CanSwitch())
        {
            party.current = (Mb)to;

            AudioManager.Play(clips[2]);
            AudioManager.Play(party.Cur.switchToClip);

            weapon.SwitchAlly();

            currentState = Idle;
            animator.runtimeAnimatorController = party.Cur.animation;
            iFrames = 0.1f; // sure hope this won't cause any problems
            playerBody.scale.x = 0f;
            dashTime = 0f;
        }
        else
        {
            AudioManager.Play(clips[3]);
        }

        bool CanSwitch()
        {
            if (weapon.thrown) return false;
            if (healWait != 0f) return false;
            return true;
        }
    }

    private void ResetPos()
    {
        transform.position = lastTouchedGround;
        rb.velocity = default;
        currentVel = 0f;
        coyoteTime = 0f;
        onGround = false;
    }

    public void ApplyRecoil(Vector2 push)
    {
        if (!canRecoil) return;
        canRecoil = false;
        canEndJump = false;
        Vector2 newVel = rb.velocity;
        newVel.y = 1f;
        newVel += push.normalized * -4f;
        dashTime = 0f;
        rb.velocity = newVel;
        noControlTime = 0.1f;
    }

    public void Spring()
    {
        flyTime = maxFlyTime;

        noControlTime = 0.1f;

        canRecoil = true;
        doubleJump = true;
        onGround = false;
    }

    public void RefreshCoin()
    {
        party.allies[0].skills[0].time = 0f;
        party.tp.Add(party.allies[0].skills[0].cost);
        AudioManager.Play(clips[12]);
    }

    public void SpawnActionText(int type, float yOffset)
    {
        UINumber uiat
            = GameManager.Spawn(actionText, transform.position + Vector3.up * yOffset)
            .GetComponent<UINumber>();
        uiat.Set(type);
    }
}
