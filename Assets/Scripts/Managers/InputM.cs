/*
using UnityEngine;

public class InputM : MonoSingleton<InputM>
{
    public static float move;
    public static float jump;
    public static bool skillA, skillAHold;
    public static int switchTo = -1;
    public static bool[] skill = new bool[3];
    public static bool crouch;
    public static bool fire, reload;
    public static bool use;
    public static Vector2 mousePos;
    private Transform mouseReference;
    private Camera main;

    protected override void Awake()
    {
        base.Awake();
        main = Camera.main;
    }

    private void Start()
    {
        mouseReference = Player.Inst.weapon.transform;
    }

    // Update is called once per frame
    void Update()
    {
        move = Input.GetAxisRaw("Horizontal");
        crouch = Input.GetButton("Crouch");

        if (Input.GetButtonDown("Jump"))
            jump = 0.08f;

        // bruh
        mousePos = Input.mousePosition - main.WorldToScreenPoint(mouseReference.position);

        fire = Input.GetButton("Fire1");

        if (Input.GetButtonDown("Use"))
            use = true;
        if (Input.GetButtonDown("Reload"))
            reload = true;

        if (Input.GetButtonDown("SkillA"))
            skillA = true;
        skillAHold = Input.GetButton("SkillA");
        for (int i = 0; i < 3; i++)
        {
            if (Input.GetButtonDown("Skill" + i))
                skill[i] = true;
        }

        for (int i = 0; i < 3; i++)
        {
            if (Input.GetButtonDown("Switch" + i))
                switchTo = i;
        }
    }
}
*/