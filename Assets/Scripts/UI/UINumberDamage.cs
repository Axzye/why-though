using TMPro;
using UnityEngine;

public class UINumberDamage : UINumber
{
    public TMP_Text text;
    public Transform back;
    private int value;
    private float rot;

    private void OnEnable()
    {
        rot = Random.Range(-10f, 10f);
    }

    public override void Set(int sprite, DamageInfo inf)
    {
        base.Set(sprite);

        DamageInfo damageInfo = inf;

        value += damageInfo.damage;
        if (Mathf.Abs(rot) < 2f) rot = rot > 0f ? 2f : -2f;

        if (value == -1)
            text.text = "?";
        else
        {
            text.text = value.ToString("");
            if (damageInfo.crit) text.text += '!';
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        rot *= 0.9f;
        back.Rotate(new(0f, 0f, rot), Space.Self);
    }
}
