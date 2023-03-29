using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public float lifetime = 1f;
    public float startAlpha = 1f;
    private SpriteRenderer sr;
    private float time;

    void Start() => sr = GetComponent<SpriteRenderer>();

    // Update is called once per frame
    void FixedUpdate()
    {
        time += Time.deltaTime;
        if (time > lifetime)
            Destroy(gameObject);
        sr.color = new(1f, 1f, 1f, (1f - time / lifetime) * startAlpha);
    }
}
