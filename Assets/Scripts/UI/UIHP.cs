using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHP : MonoBehaviour
{
    // boiler plate code!
    public TMP_Text textHp, textTp;
    public TickCounter hpCounter, tpCounter;
    public Image rainbow;
    private Party party;

    private void Start()
    {
        party = Party.Inst;

        
    }

    private void Update()
    {
        hpCounter.Set(party.hp);
        tpCounter.Set(party.tp);
        textHp.text = party.hp.ToString("00") + '/' + party.hp.GetMax.ToString("00");
        textTp.text = party.tp.ToString("00") + '/' + party.tp.GetMax.ToString("00");

        //rainbow.color = Color.HSVToRGB(Time.time * 0.25f % 1f, 0.6f, 1f);
    }
}
