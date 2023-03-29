using UnityEngine;

[CreateAssetMenu(menuName = "New Dialogue")]
public class DlPersonData : ScriptableObject
{
    public string _name;
    public Color color = Color.white;
    public AudioA sound = new((AudioClip)null, 0.5f, 1f, 0.05f);
    public Sprite[] spriteset;
    
    private void Awake()
    {
        
    }
}
