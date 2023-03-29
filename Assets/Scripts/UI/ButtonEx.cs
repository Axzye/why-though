using UnityEngine;
using UnityEngine.UI;

public class ButtonEx : MonoBehaviour
{
    public int toLoad;

    private void Start() => GetComponent<Button>().onClick.AddListener(Load);

    private void Load()
    {
        if (!MainManager.Inst.transitioning)
            StartCoroutine(MainManager.Inst.LoadLevel(toLoad));
    }
}
