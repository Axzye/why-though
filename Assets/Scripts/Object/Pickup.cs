using UnityEngine;

public class Pickup : MonoBehaviour
{
    public bool isTp;
    public AudioA pickupClip;
    public TrailRenderer trail;
    private Player player;
    private Vector3 vel;
    private float time;

    void Start()
    {
        player = Player.Inst;
        vel = Random.insideUnitCircle * 5f;
    }

    void FixedUpdate()
    {
        time += Time.deltaTime;

        if (isTp ? !Party.Inst.tp.Full : !Party.Inst.hp.Full)
        {
            float sqrDistToPlr = (transform.position - player.transform.position).sqrMagnitude;

            if (sqrDistToPlr < 4f && time > 0.5f)
            {
                vel += (player.transform.position - transform.position).normalized;
            }

            Collider2D check = Physics2D.OverlapCircle(transform.position, 0.1f, 64);
            if (check)
            {
                player.Heal(1, isTp);
                AudioManager.Play(pickupClip);
                Die();
            }
        }

        transform.position += vel * Time.deltaTime;
        vel *= 0.92f;

        if (transform.position.y < -6f)
            transform.position = new(transform.position.x, -6f);
        else if (transform.position.y > 10f)
            transform.position = new(transform.position.x, 10f);
    }

    void Die()
    {
        if (trail)
        {
            trail.transform.parent = null;
            trail.autodestruct = true;
        }
        Destroy(gameObject);
    }
}
