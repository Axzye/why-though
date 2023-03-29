using UnityEngine;

public class DespawnOffScreen : MonoBehaviour
{
    public Behaviour disable;
    public Rigidbody2D sleep;
    public Transform myTrans;
    private Transform plrTrans;
    private bool active;

    private void Start()
    {
        plrTrans = Player.Inst.transform;
        if (!myTrans)
            myTrans = transform;
    }

    private void FixedUpdate()
    {
        bool inRange = (Mathf.Abs(plrTrans.position.x - myTrans.position.x) < 12.5f);
        if (active != inRange)
        {
            disable.enabled = inRange;

            if (sleep)
                sleep.isKinematic = !inRange;

            active = inRange;
        }
    }
}
