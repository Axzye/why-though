using TMPro;
using UnityEngine;

public class UINumberHeal : UINumber
{
    public TMP_Text text;

    // Update is called once per frame
    public override void Set(int sprite, int amt)
    {
        base.Set(sprite);

        text.text = amt.ToString("");
        if (sprite == 1)
            text.color = Color.yellow;
    }
}
