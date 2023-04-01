using UnityEngine;

public class NPC : DialogueTrigger
{
    private SpriteRenderer indicator;
    private InputMaster input;
    private LayerMask playerLayer = 64;

    private void Awake()
    {
        input = new();
        Sprite indS = Resources.Load<Sprite>("Sprites/UI/button_f");
        GameObject indNew = new();
        indNew.transform.parent = transform;
        indNew.transform.localPosition = Vector2.up;
        indicator = indNew.AddComponent<SpriteRenderer>();
        indicator.sprite = indS;
    }

    private void OnEnable() => input.Player.Enable();
    private void OnDisable() => input.Player.Disable();

    public void Update()
    {
        indicator.enabled = false;
        if (Physics2D.OverlapCircle(transform.position, 0.75f, playerLayer))
        {
            indicator.enabled = true;
            if (input.Player.Use.triggered)
            {
                Trigger();
            }
        }
    }
}