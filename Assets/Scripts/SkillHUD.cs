using UnityEngine;
using UnityEngine.UI;

public class SkillHUD : MonoBehaviour
{
    public Image fill;
    public TMPro.TMP_Text text;
    [System.NonSerialized]
    public Image icon;
    private void Awake() => icon = GetComponent<Image>();
}
