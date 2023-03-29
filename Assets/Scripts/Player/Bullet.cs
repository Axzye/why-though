using UnityEngine;

public class Bullet : Projectile
{
    public Transform visual;
    public AudioA impactClip;
    public GameObject impact;

    public bool special;
    public Mb shotBy;

    private static bool loaded;
    private void Awake()
    {
        if (!loaded)
        {
            loaded = true;
        }
    }

    private void Start()
    {
        TrailRenderer trail = visual.GetComponent<TrailRenderer>();
        transform.localScale = Vector3.one;

        trail.time = 4f / 60f;
        speed = 45f;
        switch (shotBy)
        {
            case Mb.Vi:
                maxTime = 8f / 60f;
                break;
            case Mb.Kabbu:
                maxTime = 4f / 60f;
                break;
        }
    }

    private void FixedUpdate()
    {
        transform.position += speed * Time.deltaTime * transform.right;

        if (!Utils.TimeUp(ref time, maxTime))
            Die();

        RaycastHit2D check = Physics2D.Linecast(
            transform.position + transform.right * -1f,
            transform.position,
            129);
        if (check)
            OnHit(check);
    }

    protected virtual void OnHit(RaycastHit2D check)
    {
        if (check.collider.TryGetComponent<ITarget>(out var hit))
        {
            inf.addForce = transform.right * inf.knockback;
            hit.Damage(inf);
        }
        else
            AudioManager.PlayAdv(impactClip, check.point);

        Die(check.point);
    }

    protected override void Die(Vector2? pos = null)
    {
        if (!pos.HasValue) pos = transform.position;

        visual.parent = null;
        visual.position = pos.Value;
        GameManager.Spawn(impact, pos.Value, transform.rotation);

        Destroy(gameObject);
    }
}