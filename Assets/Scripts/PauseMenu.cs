using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private static AudioA clip, unClip;
    private Animator animate;
    private readonly int
        True = Animator.StringToHash("True"),
        False = Animator.StringToHash("False");
    private InputMaster input;
    private bool inPause;
    private void OnEnable() => input.Player.Enable();
    private void OnDisable() => input.Player.Disable();

    private static bool loaded;
    private void Awake()
    {
        if (!loaded)
        {
            clip = new("pause", 0.4f);
            unClip = new("unpause", 0.4f);
            loaded = true;
        }
        input = new();
    }

    private void Start() => animate = GetComponent<Animator>();

    private void Update() 
    { 
        if (input.Player.Pause.triggered)
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        if (GameManager.paused)
        {
            AudioManager.Play(unClip);
            animate.CrossFade(False, 0f);

            GameManager.paused = false;
        }
        else
        {
            AudioManager.Play(clip);
            animate.CrossFade(True, 0f);

            GameManager.paused = true;
        }
    }
}
