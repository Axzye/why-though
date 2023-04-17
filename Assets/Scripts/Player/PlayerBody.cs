using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    public Vector2 scale;    
    public bool snapToFacing;
    private Vector2 target;
    private Vector2 current;
    private Player player;
    private Weapon weapon;

    private void Start()
    {
        scale.x = -1f;
        scale.y = 1f;
        target.y = 1f;
        current = default;
        player = Player.Inst;
        weapon = player.weapon;
    }

    private void FixedUpdate()
    {
        if (snapToFacing)
            scale.x = target.x = player.facingRight ? -1f : 1f;
        else if (!weapon.Thrown) // ??
            target.x = weapon.Flipped ? 1f : -1f;

        scale = Vector2.SmoothDamp(scale, target, ref current, 0.1f, Mathf.Infinity);

        if (scale.y >= 1f)
        {
            transform.localPosition = Vector3.zero;
            float inverseY = 2f - scale.y;
            transform.localScale = new(inverseY * scale.x, 1f / inverseY, 1f);
        }
        else
        {
            transform.localPosition = Vector3.up * (scale.y - 1f) * 0.5f;
            transform.localScale = new((1f / scale.y) * scale.x, scale.y, 1f);
        }
    }
}
