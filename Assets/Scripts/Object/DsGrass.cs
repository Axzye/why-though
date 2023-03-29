using UnityEngine;

public class DsGrass : MonoBehaviour
{
    public int grassType;
    public GameObject phys;
    public Sprite cutSprite;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private bool cut;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = phys.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!cut)
        {
            Collider2D check = Physics2D.OverlapBox(transform.position + Vector3.down * 0.25f, Vector2.one, 0f, 512);
            if (check)
                Detach();
        }
    }

    private void Detach()
    {
        sr.sprite = cutSprite;
        phys.SetActive(true);
        rb.velocity = (Random.insideUnitCircle + Vector2.up) * 3f;
        if (Random.value < 0.01f)
            rb.velocity *= 4f; // funni

        rb.angularVelocity = Random.Range(-1f, 1f) * 360f;
        cut = true;
    }
}
