using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class Weapon : MonoBehaviour
{
    [Header("Stats")]
    public Sprite bmSprite;
    public float throwSpeed, throwKnockback, throwMaxTime;

    public GameObject tfx;
    private Player player;
    private Party party;
    private Ally current;
    private Rigidbody2D rb2d;

    public GameObject image;
    public Light2D light2d;
    private SpriteRenderer sr;
    private ParticleSystem ptcSystem;
    private Camera main;

    [Header("References")]
    public AudioA coinFlip;
    public AudioA throwClip;
    public AudioA throwHitClip;
    public AudioA catchClip;
    public GameObject coin;

    private float fireTime;
    private float angle;
    private float actualAngle;
    private float currentAngle;
    private float offset;
    public bool flipped;
    public float flash;

    private InputMaster input;
    private Vector2 mousePos;
    private bool inFire;

    // ok
    private AudioSource throwAudios;
    private TrailRenderer throwTrail;
    public bool thrown;
    private float throwTime;
    private float throwFreeze;
    private Vector2 throwAngle;
    private Vector2 throwAddVel;
    private int throwAmountHit;
    private float throwCatchSpin;

    private void Awake()
    {
        throwAudios = tfx.GetComponent<AudioSource>();
        throwTrail = tfx.GetComponent<TrailRenderer>();
        sr = image.GetComponent<SpriteRenderer>();
        ptcSystem = image.GetComponent<ParticleSystem>();
        main = Camera.main;
        input = new();
    }

    private void OnEnable() => input.Player.Enable();
    private void OnDisable() => input.Player.Disable();

    private void Start()
    {
        player = Player.Inst;
        party = Party.Inst;
        current = party.Cur;
        rb2d = player.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

        inFire = input.Player.Fire.IsPressed();
        mousePos = Mouse.current.position.ReadValue() - (Vector2)main.WorldToScreenPoint(transform.position);
        angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        flipped = (Mathf.Abs(angle) > 90f);

        if (thrown)
        {
            actualAngle += Time.deltaTime * 360f * 3f;
            image.transform.localPosition = Vector3.zero;
            image.transform.rotation = Quaternion.AngleAxis(actualAngle, Vector3.forward);
        }
        else
        {
            float yOffset = 0f;
            if (current.id != Mb.Leif) yOffset = -0.0625f;
            transform.localPosition = Vector3.up * yOffset;

            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            image.transform.localPosition = 0.15f * offset * Vector2.left;

            sr.flipY = flipped;
            image.transform.rotation = Quaternion.AngleAxis(actualAngle + offset * (flipped ? -15f : 15f) + throwCatchSpin, Vector3.forward);

            actualAngle = Mathf.SmoothDampAngle(actualAngle, angle, ref currentAngle, 0.05f);
        }
    }

    private void FixedUpdate()
    {
        if (current != party.Cur)
        {
            current = party.Cur;
            SwitchAlly();
        }

        Utils.TimeDown(ref fireTime);

        if (!thrown)
            UpdateDef();
        else
            UpdateThrow();

        foreach (Ally set in party.allies)
        {
            if (!set.reloading && !(set == current && fireTime > 0f) && !set.clip.Full)
            {
                set.reloading = true;
            }

            if (set.reloading)
            {
                if (!Utils.TimeUp(ref set.reloadTime, set.relCooldown))
                    Reload(set);
            }
        }

        light2d.intensity = Mathf.Min(Utils.Pow(flash, 4) * 20000f, 6f);
        if (flash > 0f)
        {
            flash -= Time.deltaTime;
            if (flash < 0.05f) sr.sprite = current.gunA;
        }

        void UpdateDef()
        {
            throwTrail.emitting = false;

            if (inFire && CanFire)
            {
                Fire();
            }

            if (offset > 0.05f)
                offset *= (current.id == Mb.Leif) ? 0.9f : 0.8f;
            else
                offset = 0f;

            if (throwCatchSpin < -1f)
                throwCatchSpin *= 0.8f;
            else
                throwCatchSpin = 0f;
        }

        void UpdateThrow()
        {
            if (throwTime > 0.5f && (transform.position - player.transform.position).sqrMagnitude < 0.5f)
                Catch();

            Vector2 vel;
            if (throwTime > throwMaxTime)
            {
                vel = (player.transform.position - transform.position).normalized * throwSpeed;
                throwTrail.emitting = false;
            }
            else
            {
                vel = throwAngle * (1f - throwTime * 2f) * throwSpeed;
                vel += throwAddVel / (throwTime + 1f);
                throwTrail.emitting = true;
                Coll();
                void Coll()
                {
                    if (throwTime == 0f) return;

                    Collider2D checkG = Physics2D.OverlapCircle(transform.position, 0.25f, 1);
                    if (checkG)
                    {
                        FollowCamera.Shake(1f);
                        throwTime = throwMaxTime;
                        AudioManager.Play(throwHitClip);
                    }

                    Collider2D checkH = Physics2D.OverlapCircle(transform.position, 0.4f, 128);
                    if (checkH)
                    {
                        if (checkH.TryGetComponent<ITarget>(out var hit))
                        {
                            DamageInfo inf = new()
                            {
                                damage = throwAmountHit == 0 ? 2 : 1,
                                addForce = throwAngle * throwKnockback + Vector2.up,
                                iFrames = 0.12f
                            };

                            if (hit.Damage(inf))
                            {
                                throwFreeze = 3f / 60f;
                                throwAmountHit++;
                                if (throwAmountHit >= 4) throwTime = throwMaxTime;
                            }
                        }
                    }
                }
            }
            throwTime += Time.deltaTime;

            if (!Utils.TimeDown(ref throwFreeze))
                transform.position += (Vector3)(vel * Time.deltaTime);
        }

        void Reload(Ally set)
        {
            set.clip.Add(set.reloadAll ? 99 : 1);

            if (set.clip.Full)
                set.reloading = false;

            AudioManager.Play(set.relEndClip);
            set.reloadTime = 0f;
        }
    }

    private bool CanFire
    {
        get
        {
            if (fireTime > 0f)
                return false;
            if (current.clip == 0)
                return false;
            if (thrown)
                return false;
            return true;
        }
    }

    private void Fire()
    {
        current.reloading = false;
        current.reloadTime = 0f;

        fireTime = current.fireCooldown;

        AudioManager.Play(current.fireClip);

        switch (current.id)
        {
            case Mb.Vi:
                int damage = 1;

                CreateBullet(0f, damage);
                break;
            case Mb.Kabbu:
                // always hit enemies in close proximity
                Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position + transform.right * 0.6f, 7f / 16f, 128);

                if (colls.Length > 0)
                {
                    foreach (Collider2D coll in colls)
                    {
                        if (coll.TryGetComponent<ITarget>(out var hit))
                        {
                            DamageInfo inf = new() { damage = 4 };
                            hit.Damage(inf);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                        CreateBullet(-12f + i * 8f + Random.Range(-3f, 3f), 1);
                }

                if (!player.crouching) player.ApplyRecoil(mousePos);
                break;
            case Mb.Leif:
                break;
        }

        current.clip.Add(-1);

        offset = current.recAmt;
        flash = current.flashAmt;
        FollowCamera.Shake(current.recAmt * 0.4f);

        ptcSystem.Emit(1);
        sr.sprite = current.gunB;

        void CreateBullet(float addAngle, int damage)
        {
            GameObject proj = GameManager.Spawn(current.projectile, transform.position + transform.right * 0.5f);
            Bullet bullet = proj.GetComponent<Bullet>();
            bullet.shotBy = current.id;
            bullet.inf = current.infBase;
            bullet.inf.damage = damage;

            if (player.crouching)
                addAngle *= 0.5f;

            proj.transform.rotation = Quaternion.AngleAxis(angle + addAngle, Vector3.forward);
            proj.transform.position = transform.position + transform.right * 0.5f;

            proj.SetActive(true);
        }
    }

    public void SwitchAlly()
    {
        sr.sprite = party.Cur.gunA;
    }

    public void FlipCoin()
    {
        GameObject obj = GameManager.Spawn(coin, player.transform.position, angle);
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        obj.transform.rotation = Quaternion.AngleAxis(actualAngle, Vector3.forward);
        obj.transform.position += Vector3.up * 0.25f;
        rb.velocity = (Vector2)transform.right * 8f + Vector2.up * 6f + rb2d.velocity * 0.5f;

        AudioManager.Play(coinFlip);
    }

    public void Throw()
    {
        thrown = true;
        // ehh??
        transform.parent = MainManager.Inst.levelObj.transform;

        throwTime = 0f;
        throwAmountHit = 0;
        throwAngle = mousePos.normalized;
        throwAddVel = rb2d.velocity * 0.5f;

        sr.sprite = bmSprite;
        AudioManager.Play(throwClip);
        throwAudios.Play();
    }

    private void Catch()
    {
        thrown = false;
        transform.parent = player.transform;

        throwCatchSpin = -360f;
        actualAngle = angle;
        if (throwAmountHit > 0) player.SpawnActionText(Mathf.Max(throwAmountHit - 1, 0), 0.5f);

        sr.sprite = party.Cur.gunA;
        AudioManager.Play(catchClip);
        throwAudios.Stop();
    }
}