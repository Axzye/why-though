using UnityEngine;

public class SkillsMenu : MonoBehaviour
{
    public SkillHUD[] images;
    public RectTransform rt;
    private Ally current;
    private float flipTime;
    private Party party;

    private void Start()
    {
        party = Party.Inst;
        current = party.CurrentAlly;
        UpdateIcons();
    }

    private void FixedUpdate()
    {
        if (party.CurrentAlly != current)
        {
            current = party.CurrentAlly;
            flipTime = 0.5f;
            UpdateIcons();
        }

        if (Utils.TimeDown(ref flipTime))
            rt.localScale = new(1f, 1f - Utils.Pow(2f * flipTime, 3), 1f);

        for (int i = 0; i < images.Length; i++)
        {
            float fill;
            if (party.tp < party.CurrentAlly.skills[i].cost)
                fill = 1f;
            else
                fill = party.CurrentAlly.skills[i].time / party.CurrentAlly.skills[i].cooldown;

            images[i].fill.fillAmount = fill;
        }
    }

    private void UpdateIcons()
    {
        for (int i = 0; i < 3; i++)
        {
            images[i].icon.sprite = current.skills[i].icon;
            int val = current.skills[i].cost;
            images[i].text.text = val == 0? string.Empty : val.ToString();
        }
    }
}
