using UnityEngine;
using UnityEngine.UI;

public class UIAlly : MonoBehaviour
{
    public Mb which;
    public Sprite hurt;
    public TickCounter bulletCounter;
    public Image icon;
    public Image iconFlash;
    public Slider slider;

    private Player player;
    private Weapon weapon;
    private Ally ally;

    private RectTransform rt;
    private Image image;
    private Sprite defIcon;
    private Color defCol;
    private Vector2 startPos;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        player = Player.Inst;
        weapon = player.weapon;
        ally = Party.Inst.allies[(int)which];

        startPos = rt.anchoredPosition;
        slider.maxValue = ally.relCooldown;
        defIcon = icon.sprite;
        defCol = image.color;
        bulletCounter.Set(ally.clip.GetMax);
    }

    private void FixedUpdate()
    {
        bulletCounter.Set(ally.clip);

        iconFlash.color = Utils.clearW;
        rt.anchoredPosition = startPos;
        icon.sprite = defIcon;
        image.color = defCol;
        icon.color = Color.white;
        if (Party.Inst.CurrentID == which)
        {
            rt.anchoredPosition += Vector2.down * Mathf.Abs(Mathf.Sin(Time.time * Mathf.PI)) * 2f;

            iconFlash.color = Color.Lerp(Utils.clearW, Color.white, weapon.Flash * 6f);

            if (player.DamageTime > 0.25f)
            {
                rt.anchoredPosition += Random.insideUnitCircle * player.DamageTime * 4f;
                image.color = Color.Lerp(defCol, Color.red, player.DamageTime);
                icon.sprite = hurt;
            }
        }

        slider.gameObject.SetActive(ally.reloadTime > 0f);
        slider.value = ally.reloadTime;
    }
}
