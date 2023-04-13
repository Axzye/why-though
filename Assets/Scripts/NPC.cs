using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class NPC : Interactable
{
    private DialogueTrigger dl;
    private SpriteRenderer indicator;
    private LayerMask playerLayer = 64;

    protected override void Awake()
    {
        base.Awake();
        dl = GetComponent<DialogueTrigger>();

        Sprite indS = Resources.Load<Sprite>("Sprites/UI/button_f");
        GameObject indNew = new();
        indNew.transform.parent = transform;
        indNew.transform.localPosition = Vector2.up;
        indicator = indNew.AddComponent<SpriteRenderer>();
        indicator.sprite = indS;
    }

    private void OnEnable() => input.Player.Enable();
    private void OnDisable() => input.Player.Disable();

    public void FixedUpdate()
    {
        indicator.enabled = false;
        if (Physics2D.OverlapCircle(transform.position, 0.75f, playerLayer))
        {
            indicator.enabled = true;
            indicator.transform.localPosition = (Mathf.Sin(Time.time * Mathf.PI * 0.5f) * (1f / 16f) + 1f)  * Vector2.up;
            if (input.Player.Use.triggered)
            {
                dl.Trigger();
            }
        }
    }
}