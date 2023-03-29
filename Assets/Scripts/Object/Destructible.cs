using UnityEngine;

public class Destructible : MonoBehaviour, ITarget
{
    public Stat hp;
    public AudioA hitClip;
    public AudioA killClip;
    public bool immune;
    public GameObject breakFx;

    public virtual void Start() => ReSet();

    public bool Damage(DamageInfo _)
    {
        if (immune) return false;

        hp.Add(-1);
        if (hp <= 0)
            Kill();
        else
        {
            if (hitClip.clip)
            {
                 AudioManager.Play(hitClip);
            }
        }
        return true;
    }

    void ReSet()
    {
        hp.Add(99);
    }

    public virtual void Kill()
    {
        if (killClip.clip)
        {
            AudioManager.Play(killClip);
        }

        if (breakFx) GameManager.Spawn(breakFx, transform.position);

        Destroy(gameObject);
    }
}