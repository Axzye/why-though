using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private LayerMask playerLayer = 64;

    protected InputMaster input;
    protected SpriteRenderer indicator;
    protected bool inUse;

    private void OnEnable() => input.Player.Enable();
    private void OnDisable() => input.Player.Disable();

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        input = new();
        Sprite indS = Resources.Load<Sprite>("Sprites/UI/button_f");
        GameObject indNew = new();
        indNew.transform.parent = transform;
        indNew.transform.localPosition = Vector2.up;
        indicator = indNew.AddComponent<SpriteRenderer>();
        indicator.sprite = indS;
    }

    protected virtual void Update()
    {
        if (input.Player.Use.triggered) inUse = true;
    }

    protected virtual void FixedUpdate()
    {
        indicator.enabled = false;
        if (Physics2D.OverlapCircle(transform.position, 0.75f, playerLayer))
        {
            indicator.enabled = true;
            indicator.transform.localPosition = (Mathf.Sin(Time.time * Mathf.PI * 0.5f) * (1f / 16f) + 1f) * Vector2.up;
            if (inUse)
            {
                DoAThing();
            }
        }
        inUse = false;
    }

    protected virtual void DoAThing() { }
}
