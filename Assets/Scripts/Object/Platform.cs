using UnityEngine;

public class Platform : MonoBehaviour
{
    public Sprite overS;
    private SpriteRenderer sr;
    private void Start()
    {
        // auto scale
        sr = GetComponent<SpriteRenderer>();
        Vector2 scale = transform.localScale;
        scale.y = 1f;
        sr.size = scale;
        EdgeCollider2D coll = GetComponent<EdgeCollider2D>();
        Vector2[] points = coll.points;
        points[0].x = scale.x * -0.5f;
        points[1].x = scale.x * 0.5f;
        coll.points = points;
        transform.localScale = Vector3.one;
        if (overS) sr.sprite = overS;
    }
}
