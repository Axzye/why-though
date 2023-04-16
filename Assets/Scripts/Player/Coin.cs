using System.Collections.Generic;
using UnityEngine;

public class Coin : Projectile
{
    public float check = 0.3f, checkSmall = 0.1f;
    public AudioA clip;
    public Transform visual;

    private List<Transform> hasHit = new();
    private Rigidbody2D rb;
    private int hitCounter;
    private float deadTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddTorque(-6f, ForceMode2D.Impulse);
        deadTime = 0.1f;
    }

    private void FixedUpdate()
    {
        time += Time.deltaTime;
        
        if (!Utils.TimeDown(ref deadTime))
        {
            Collider2D check = Physics2D.OverlapCircle(transform.position, this.check, 512);
            if (check)
            {
                if (!hasHit.Contains(check.transform))
                    Ricoshot(check.transform);
            }
        }

        Collider2D checkSmall = Physics2D.OverlapCircle(transform.position, this.checkSmall, 65);
        if (checkSmall)
        {
            if (checkSmall.gameObject.layer == 6)
            {
                if (time > 0.25f)
                {
                    Player.Inst.RefreshCoin();
                    Die();
                }
            }
            else
                Die();
        }

        if (Player.Inst.weapon.Thrown)
        {
            if ((transform.position - Player.Inst.weapon.transform.position).sqrMagnitude < 0.12f)
            {
                Player.Inst.RefreshCoin();
                Die();
            }
        }

        void Ricoshot(Transform hit)
        {
            if (!hit) return;

            Transform closest = null;
            float distToClosest = 90f;
            foreach (GameObject target in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (target == gameObject)
                    continue;

                float dist = (target.transform.position - hit.position).sqrMagnitude;
                if (dist >= distToClosest)
                    continue;

                if (Physics2D.Linecast(hit.position, target.transform.position, 1))
                    continue;

                closest = target.transform;
                distToClosest = dist;
            }

            Quaternion rotation;
            if (closest)
            {
                rotation = Quaternion.FromToRotation(Vector2.right, closest.position - hit.position);
            }
            else
            {
                rotation = Quaternion.FromToRotation(Vector2.right, Vector2.Reflect(hit.right, Vector2.down));
            }

            if (hit.TryGetComponent(out Bullet proj))
            {
                if (proj.shotBy == Mb.Vi)
                {
                    
                    proj.inf.damage += 3;
                    proj.inf.crit = true;
                    proj.time = 0f;
                }
                hit.rotation = rotation;
            }

            rb.velocity = rotation * Vector2.right * -4f + Vector3.up * 4f;
            rb.AddTorque(-3f, ForceMode2D.Impulse);

            hasHit.Add(hit);

            AudioA newClip = clip;
            newClip.pitch = Utils.Pow(1.0594631f, hitCounter);
            AudioManager.PlayAdv(newClip, transform.position);

            deadTime = 0.1f;
            hitCounter++;
            if (hitCounter >= 4)
                Die();
        }
    }

    protected override void Die(Vector2? pos = null)
    {
        visual.parent = null;
        Destroy(gameObject);
    }
}
