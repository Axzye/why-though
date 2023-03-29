using UnityEngine;

public class NPC : DialogueTrigger
{
    private InputMaster input;
    private LayerMask playerLayer = 64;

    private void Awake()
    {
        input = new();
    }

    private void OnEnable() => input.Player.Enable();
    private void OnDisable() => input.Player.Disable();

    public void Update()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.75f, playerLayer))
        {
            if (input.Player.Use.triggered)
            {
                Trigger();
            }
        }
    }
}