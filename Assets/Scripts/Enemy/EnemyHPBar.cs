using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    public Transform bounds, slow, background;
    public SpriteRenderer fill;
    private Stat stored;
    private float slowValue;
    private float hitTime;

    private void Start()
    {
    }

    public void Set(Stat value)
    {
        hitTime = 0.5f;

        stored = value;
        if (stored > slowValue)
            slowValue = stored;

        float scale = 0.0625f * value.GetMax;
        bounds.localScale = new(scale, 0.125f);
        background.localScale = new(scale + 0.125f, 0.25f);
    }

    private void FixedUpdate()
    {
        if (!Utils.TimeDown(ref hitTime))
        {
            slowValue -= Time.deltaTime * 15f;
            if (slowValue < stored)
                slowValue = stored;
        }

        fill.transform.localScale = new(stored.Percent, 1f, 1f);
        fill.transform.localPosition = new(stored.Percent * 0.5f - 0.5f, 0f, 0f);
        // genius
        fill.color = Color.Lerp(Color.white, Utils.clearW, (hitTime * 2f) - 0.5f);

        slow.localScale = new(slowValue / stored.GetMax, 1f, 1f);
        slow.localPosition = new(slowValue / stored.GetMax * 0.5f - 0.5f, 0f, 0f);
    }
}
