using UnityEngine;

public class TransitionManager : MonoSingleton<TransitionManager>
{
    private Transition[] transitions;
    private int current;

    protected override void Awake()
    {
        base.Awake();

        transitions = GetComponentsInChildren<Transition>();
    }

    public void In(int which)
    {
        transitions[which].Set(true);
        current = which;
    }

    public void Out()
    {
        transitions[current].Set(false);
    }
}
