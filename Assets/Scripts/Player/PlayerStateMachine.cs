using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    protected State
        idle,
        walk,
        jump,
        fall, 
        crouch,
        slide, 
        wallslide, 
        hurt,
        fly, 
        dash;

    private void Awake()
    {
        idle = new Idle(this);
        walk = new Walk(this);
        jump = new Jump(this);
        fall = new Fall(this);
        crouch = new Crouch(this);
        slide = new Slide(this);
        wallslide = new Wallslide(this);
        hurt = new Hurt(this);
        fly = new Fly(this);
        dash = new Dash(this);
    }

    protected override State GetInitialState()
    {
        if (idle == null) Debug.LogError("something has gone very wrong");
        return idle;
    }


    public class Idle : State
    {
        private PlayerStateMachine ownSM;

        public Idle(PlayerStateMachine stateMachine) : base("Idle", stateMachine) { ownSM = stateMachine; }
    }

    public class Walk : State
    {
        public Walk(StateMachine stateMachine)
           : base("Walk", stateMachine) { }
    }

    public class Jump : State
    {
        public Jump(StateMachine stateMachine)
           : base("Jump", stateMachine) { }

    }

    public class Fall : State
    {
        public Fall(StateMachine stateMachine)
           : base("Fall", stateMachine) { }

    }

    public class Crouch : State
    {
        public Crouch(StateMachine stateMachine)
           : base("Crouch", stateMachine) { }

    }

    public class Slide : State
    {
        public Slide(StateMachine stateMachine)
           : base("Slide", stateMachine) { }

    }

    public class Wallslide : State
    {
        public Wallslide(StateMachine stateMachine)
           : base("Wallslide", stateMachine) { }

    }

    public class Hurt : State
    {
        public Hurt(StateMachine stateMachine)
           : base("Hurt", stateMachine) { }

    }

    public class Fly : State
    {
        public Fly(StateMachine stateMachine)
           : base("Fly", stateMachine) { }

    }

    public class Dash : State
    {
        public Dash(StateMachine stateMachine)
           : base("Dash", stateMachine) { }

    }
}