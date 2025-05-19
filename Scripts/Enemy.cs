using Godot;
[GlobalClass]
public partial class Enemy : CharacterBody3D
{
    StateMachine fsm;

    public Node3D Target;
    [Export] public AnimationPlayer animationPlayer;
    [Export] public FollowTarget3D followTarget3D;
    [Export] public RandomTarget3D randomTarget3D;
    [Export] public SimpleVision3D simpleVision3D;
    [Export] public float walkSpeed = 2.0f;
    [Export] public float runSpeed = 5.0f;
    public Health health;

    public override void _Ready()
    {
        if (fsm == null)
        {
            fsm = GetNode<StateMachine>("StateMachine");
            GD.Print($"FSM Found: {fsm != null}");
        }
        if (followTarget3D == null)
        {
            followTarget3D = GetNode<FollowTarget3D>("FollowTarget3D");
            GD.Print($"followTarget3D Found: {followTarget3D != null}");
        }

        if (randomTarget3D == null)
        {
            randomTarget3D = GetNode<RandomTarget3D>("RandomTarget3D");
            GD.Print($"randomTarget3D Found: {randomTarget3D != null}");
        }

        if (simpleVision3D == null)
        {
            simpleVision3D = GetNode<SimpleVision3D>("SimpleVision3D");
            GD.Print($"simpleVision3D Found: {simpleVision3D != null}");
        }

        if (health == null)
        {
            health = GetNode<Health>("Health");
            GD.Print($"Health found: {health != null}");
        }

        if (health != null)
        {
            health.DamageSignal += OnDamage;
            GD.Print("Damage signal connected");
        }
        followTarget3D.NavigationFinished += OnFollowTarget3DNavigationFinished;
        simpleVision3D.LostSight += OnSimpleVision3DLostSight;
        simpleVision3D.GetSight += OnSimpleVision3DGetSight;
    }

    private void OnDamage(float value)
    {
        GD.Print("OnDamage called");
        fsm.TransitionTo("Hurt");
    }

    public override void _Process(double delta)
    {
        if (!IsOnFloor())
            Velocity += GetGravity() * (float)delta;

    }

    public void OnFollowTarget3DNavigationFinished()
    {
        followTarget3D.SetFixedTarget(randomTarget3D.GetNextPoint());
    }

    private void OnSimpleVision3DGetSight(Node3D body)
    {
        Target = body;
        fsm.TransitionTo("Pursue");
    }

    private void OnSimpleVision3DLostSight()
    {
        fsm.TransitionTo("RandomPatrol");
    }
}
