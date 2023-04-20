using System.Collections;
using UnityEngine;

public class MainManager : MonoSingleton<MainManager>
{
    public GameObject[] levels;
    public GameObject game;
    public GameObject levelObj, gameObj;

    public static int currentIndex = -1;
    public static Level current;

    public bool transitioning;

    public delegate void LevelHandler(Level l, int i);
    public static event LevelHandler OnLoad;

    protected override void Awake()
    {
        base.Awake();

        Load(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            StartCoroutine(LoadLevel(1));
        if (Input.GetKeyDown(KeyCode.L))
            StartCoroutine(LoadLevel(2));
    }

    public IEnumerator LoadLevel(int index)
    {
        transitioning = true;
        TransitionManager.Inst.In(0);
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(0.5f);

        Load(index);

        transitioning = false;
        TransitionManager.Inst.Out();
    }

    private void Load(int index)
    {
        Destroy(levelObj);
        // TODO: ughhhhhh
        if (index == 0)
        {
            if (gameObj)
                Destroy(gameObj);
        }
        else
        {
            if (!gameObj)
                gameObj = Instantiate(game);
        }

        // Load new scene
        levelObj = Instantiate(levels[index]);
        current = levelObj.GetComponent<Level>();
        currentIndex = index;

        OnLoad?.Invoke(current, currentIndex);
    }
}
