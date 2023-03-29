using UnityEngine;
using UnityEngine.UI;

public class TickCounter : MonoBehaviour
{
    public Sprite full, empty;
    private Image[] ticks;
    private int current = -1, max;

    // Start is called before the first frame update
    void Start()
    {
        ticks = transform.GetComponentsInChildren<Image>();
        if (ticks.Length == 0)
        {
            Debug.LogError("Could not find ticks");
            //ticks = new Image[genAmount];
            //for (int i = 0; i < genAmount; i++)
            //{
            //    GameObject obj = new("Tick" + i);        
            //    obj.transform.parent = transform;
            //    obj.transform.localPosition = offset * i;
            //    Image im = obj.AddComponent<Image>();
            //    im.sprite = full;
            //    ticks[i] = im;
            //}
        }

        max = ticks.Length;
        if (!full) full = ticks[0].sprite;
        if (!empty) empty = ticks[1].sprite;
    }

    // Update is called once per frame
    public void Set(int set)
    {
        print("attempted to set  " + set);
        if (current != set)
        {
            print("set to " + set);
            for (int i = 0; i < max; i++)
                ticks[i].sprite = set > i ? full : empty;
            current = set;
        }
    }
}
