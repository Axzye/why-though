using UnityEngine;

public class Spring : MonoBehaviour
{
    public AudioA springClip;
    private LayerMask layers = 192;
    private float cooldown;
    private Animator animator;
    private Player player;

    private readonly int Trigger = Animator.StringToHash("Spring");

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = Player.Inst;
    }

    private void FixedUpdate()
    {
        if (cooldown > 0f)
            cooldown -= Time.deltaTime;
        else
        {
            Collider2D collider = Physics2D.OverlapBox(transform.position + Vector3.up * 0.125f, new(1f, 0.25f), 0f, layers);
            if (!collider) return;

            Rigidbody2D rba = collider.GetComponent<Rigidbody2D>();
            if (!rba) return;
            
            Vector2 storedDir = rba.velocity * transform.right * 0.5f;
            storedDir.y = 5f;
            rba.velocity = storedDir + (Vector2)transform.up * 9f;

            cooldown = 0.25f;
            animator.CrossFade(Trigger, 0f);

            if (collider.gameObject == player.gameObject)
            {
                AudioManager.Play(springClip);
                player.Spring();
                player.canEndJump = false;
            }
        }
    }
}
