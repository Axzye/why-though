using UnityEngine;

public class UINumber : MonoBehaviour
{
    public SpriteRenderer sr;
    public Sprite[] sprites;
    protected float time;
    protected Vector3 vel;
    protected float scale;

    private void OnEnable()
    {
        vel = Vector3.up * 4f;
        vel.x += Random.Range(-2f, 2f);
    }

    public virtual void Set(int sprite)
    {
        time = 1f;
        scale = 1.25f;
        sr.sprite = sprites[sprite];
    }

    public virtual void Set(int sprite, int amt) { }

    public virtual void Set(int sprite, DamageInfo inf) { }

    private void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0f) Destroy(gameObject);
        transform.position += vel * Time.deltaTime;
        transform.localScale = Vector3.one * scale;
    }

    protected virtual void FixedUpdate()
    {
        vel *= 0.9f;
        scale = time < 0.25f ? scale * 0.85f : (scale - 1f) * 0.85f + 1f;
    }
}
