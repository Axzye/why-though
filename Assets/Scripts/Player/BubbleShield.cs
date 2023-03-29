using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleShield : MonoBehaviour
{
    private SpriteRenderer sr;
    private Player player;
    private float scale;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        player = Player.Inst;
    }

    private void FixedUpdate()
    { 
        if (player.shieldTime == 0f) gameObject.SetActive(false);

        sr.color = new(1f, 1f, 1f, 
            ((player.shieldTime < 1f && player.shieldTime % 0.1f > 0.05f)?
            0.3f : 0.4f
            + player.shieldTime * 0.04f)
            + Utils.Pow(player.iFrames, 3) * 4f);
    }
}
