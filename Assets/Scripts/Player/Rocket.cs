#if false
using UnityEngine;

public class Rocket : Bullet
{
    public ParticleSystem trail;
    private static GameObject expFx;
    private static AudioClip boomS;

    private static bool loaded;
    private void Awake()
    {
        if (!loaded)
        {
            expFx = Resources.Load<GameObject>("Object/Explosion");
            boomS = Resources.Load<AudioClip>("Sounds/boom_special");
            loaded = true;
        }
    }

    private void OnEnable()
    {
        lifetime = 1f;
        speed = 10f;
        trail.transform.parent = transform;
        trail.transform.localPosition = Vector3.zero;
        trail.Play();
    }

    protected override void Die() => OnHit(null);

    protected override void OnHit(Collider2D coll)
    {
        BTarget hitDir = null;
        if (coll)
            hitDir = coll.GetComponent<BTarget>();

        Collider2D[] exColls = Physics2D.OverlapCircleAll(transform.position, 1.5f, 192);
        int count = 0;
        foreach (Collider2D exColl in exColls)
        {
            inf = new() { damage = special ? 3 : 2, addForce = Ext.ExplosionForce(transform.position, exColl.transform.position) };

            Player player = exColl.GetComponent<Player>();
            if (player)
            {
                inf.damage = 1;
                player.canEndJump = false;
                player.Damage(inf);
                continue;
            }
            else
            {
                BTarget hit = exColl.GetComponent<BTarget>();
                if (!hit) continue;

                if (hit == hitDir)
                {
                    inf.damage += 2;
                    inf.crit = true;
                }

                count++;
                if (count > 4) break;

                hit.Damage(inf);

                BaseEnemy hitEn = exColl.GetComponent<BaseEnemy>();
                if (!hitEn) continue;

                hitEn.AddStatusEffect(Status.Frozen, 4f);
            }
        }

        FollowCamera.Shake(0.8f);
        GameObject temp = MainManager.Spawn(expFx, transform.position);
        if (special)
        {
            temp.GetComponent<AudioSource>().clip = boomS;
        }
        temp.SetActive(true);

        trail.transform.parent = null;
        trail.Stop();
        special = false;

        base.Die();
    }
}
#endif