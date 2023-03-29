using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public static bool paused;
    public static bool CanMove => !paused && !DialogueManager.inDlMode;
    private static float freezeTime;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable() => MainManager.OnLoad += Load;
    private void OnDisable() => MainManager.OnLoad -= Load;

    private void Load(Level l, int i)
    {
        paused = false;
    }

    private void Update()
    {
        if (!CanMove) return;

        if (Utils.TimeDown(ref freezeTime, Time.unscaledDeltaTime))
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    // this is dumb bad code, never do this
    public static GameObject Spawn(GameObject obj, Vector2 pos, Quaternion rot = default) => Spawn(obj, pos, rot, null);
    public static GameObject Spawn(GameObject obj, Vector2 pos, float rot) => Spawn(obj, pos, Quaternion.AngleAxis(rot, Vector3.forward), null);
    public static GameObject Spawn(GameObject obj, Transform refT, Vector2 offset) => Spawn(obj, (Vector2)refT.position + offset, Quaternion.identity, refT);
    public static GameObject Spawn(GameObject obj, Vector2 pos, Quaternion rot, Transform parent)
    {
        GameObject spawnObj = Instantiate(
            obj, pos,
            rot,
            parent ? parent : MainManager.Inst.levelObj.transform);
        return spawnObj;
    }

    public static void Freeze(float time)
    {
        if (freezeTime < time) freezeTime = time;
    }
}
