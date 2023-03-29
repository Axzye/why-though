using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Camera cam;
    private static float shakeTime;
    private Player player;
    private Rigidbody2D playerRb;
    private static Transform focus;
    private static float focusTime;

    public float moveST = 0.4f, scaleST = 0.3f;
    public Vector2 lookAheadAmt;

    private float scale = 6f;
    private Vector3 moveCur;
    private float scaleCur;

    private void Start()
    {
        player = Player.Inst;
        playerRb = player.GetComponent<Rigidbody2D>();
        ResetPos();
    }

    public static void Shake(float intensity)
    {
        if (intensity < shakeTime) return;
        shakeTime = intensity;
    }

    public static void Focus(Transform on, float time = 2f)
    {
        focus = on;
        focusTime = time;
    }

    private void ResetPos()
    {
        Vector2 pos = player.transform.position;
        transform.position = new(pos.x, Mathf.Max(pos.y, -2f), -10f);
    }

    private void FixedUpdate()
    {
        if (shakeTime > 0f)
        {
            shakeTime *= 0.9f;
            if (shakeTime < 0.2f) shakeTime = 0f;
            cam.transform.localPosition = Random.insideUnitCircle * shakeTime * 0.1f;
        }
        else
        {
            shakeTime = 0f;
            cam.transform.localPosition = Vector3.zero;
        }

        if (Utils.TimeDownTick(ref focusTime))
            focus = null;

    }

    private void Update()
    {
        Vector3 playerPos = playerRb.position + playerRb.velocity * lookAheadAmt;
        Vector3 focusPos = focus ? focus.position : playerPos;
        float dist = 4f + (playerPos - focusPos).magnitude * 0.25f;
        if (dist > 6f) focus = null;

        Vector3 moveTarg = (playerPos + focusPos) * 0.5f;
        moveTarg.z = -8f;
        moveTarg.y = Mathf.Clamp(moveTarg.y, -2f, 5f);

        transform.position = Vector3.SmoothDamp(transform.position, moveTarg, ref moveCur, moveST);

        float scaleTarg = focus ? Mathf.Clamp(dist, 4f, 6f) : 6f;

        scale = Mathf.SmoothDamp(scale, scaleTarg, ref scaleCur, scaleST);
        cam.orthographicSize = scale;
    }
}
