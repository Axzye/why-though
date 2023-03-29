using UnityEngine;

public class Inichas : EnemyGround
{
    private float timeToCharge;

    protected override void UpdateAI()
    {
        if (inLos)
        {
            if (!Utils.TimeUp(ref timeToCharge, 3f))
            {
                timeToCharge = 0f;
            }
        }

        base.UpdateAI();
    }
}
