using Godot;
using GodotTools;

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
    [Export] public Health health;
    [Export] public ExplosionBig explosion;

    // Missile attack settings
    [Export] public float missileRange = 15f; // Min distance to use missiles
    [Export] public float meleeRange = 3f; // Max distance for melee
    [Export] public float missileCooldown = 5f; // Seconds between missile attacks
    [Export] public bool debugTrajectory = false;

    private float lastMissileTime = 0f;

    public override void _Ready()
    {
        if (fsm == null)
        {
            fsm = GetNode<StateMachine>("StateMachine");
            GodotLogger.Info($"FSM Found: {fsm != null}");
        }
        if (followTarget3D == null)
        {
            followTarget3D = GetNode<FollowTarget3D>("FollowTarget3D");
            GodotLogger.Info($"followTarget3D Found: {followTarget3D != null}");
        }

        if (randomTarget3D == null)
        {
            randomTarget3D = GetNode<RandomTarget3D>("RandomTarget3D");
            GodotLogger.Info($"randomTarget3D Found: {randomTarget3D != null}");
        }

        if (simpleVision3D == null)
        {
            simpleVision3D = GetNode<SimpleVision3D>("SimpleVision3D");
            GodotLogger.Info($"simpleVision3D Found: {simpleVision3D != null}");
        }

        if (health == null)
        {
            health = GetNode<Health>("Health");
            GodotLogger.Info($"Health found: {health != null}");
        }

        if (health != null)
        {
            health.DamageSignal += OnDamage;
            health.DeathSignal += () => { fsm.TransitionTo("Death"); };
            GodotLogger.Info("Damage signal connected");
        }

        followTarget3D.NavigationFinished += OnFollowTarget3DNavigationFinished;
        followTarget3D.ReachedTarget += OnReachedTarget;
        simpleVision3D.LostSight += OnSimpleVision3DLostSight;
        simpleVision3D.GetSight += OnSimpleVision3DGetSight;
    }

    private void OnDamage(float value)
    {
        GodotLogger.Info("OnDamage called");
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

    private void OnReachedTarget(Node3D target)
    {
        if (target.GetGroups().Contains("PlayerCharacter"))
        {
            // Since we're now at the target, check for melee attack
            // (Missile decisions are handled in PursueState)
            float distanceToTarget = GlobalPosition.DistanceTo(Target.GlobalPosition);

            if (distanceToTarget <= meleeRange)
            {
                fsm.TransitionTo("Melee");
            }
            else
            {
                // If we somehow reached target but not in melee range, keep pursuing
                fsm.TransitionTo("Pursue");
            }
        }
    }

    // Public methods for state access
    public float GetLastMissileTime()
    {
        return lastMissileTime;
    }

    public void SetLastMissileTime(float time)
    {
        lastMissileTime = time;
    }

    // Check if we have clear line of sight to target (now public for states to use)
    public bool HasLineOfSightToTarget()
    {
        if (Target == null) return false;

        // Simple raycast check
        PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(
            GlobalPosition + Vector3.Up * 1.5f, // Start slightly above ground
            Target.GlobalPosition + Vector3.Up * 1.5f // Target slightly above ground
        );

        var result = spaceState.IntersectRay(query);

        // If no collision or collision is with target, we have line of sight
        return result.Count == 0 || result["collider"].AsGodotObject() == Target;
    }

    // Public method to force missile attack (useful for testing or scripted events)
    public void ForceMissileAttack()
    {
        if (Target != null)
        {
            lastMissileTime = Time.GetTicksMsec() / 1000.0f;
            fsm.TransitionTo("Missile");
        }
    }
}
