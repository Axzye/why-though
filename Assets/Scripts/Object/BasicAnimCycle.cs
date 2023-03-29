using UnityEngine;

public class BasicAnimCycle : MonoBehaviour
{
    public Sprite[] sprites;
    public float fps;
    private float time;
    private SpriteRenderer sr;

    private void Start() => sr = GetComponent<SpriteRenderer>();

    private void Update()
    {
        time += Time.deltaTime;
        if (Mathf.FloorToInt(time * fps) >= sprites.Length)
        {
            Destroy(gameObject);
            return;
        }
        sr.sprite = sprites[Mathf.FloorToInt(time * fps)];
    }
}
