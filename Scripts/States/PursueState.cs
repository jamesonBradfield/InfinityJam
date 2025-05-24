using Godot;
using GodotTools;

[GlobalClass]
public partial class PursueState : MyState
{
    [Export] Enemy enemy;
    [Export] public float attackDecisionInterval = 0.5f; // How often to check for attack opportunities

    private float lastAttackCheck = 0f;

    public override void Enter()
    {
        base.Enter();
        enemy.followTarget3D.isEnabled = true;
        enemy.followTarget3D.Speed = enemy.runSpeed;
        enemy.followTarget3D.SetTarget(enemy.Target);
        enemy.animationPlayer.Play("Run");

        // Reset attack decision timer
        lastAttackCheck = Time.GetTicksMsec() / 1000.0f;

        GodotLogger.Info("Pursuing target");
    }

    public override void Update(float delta)
    {
        base.Update(delta);

        // Check if we should attack instead of continuing to pursue
        float currentTime = Time.GetTicksMsec() / 1000.0f;
        if (currentTime - lastAttackCheck >= attackDecisionInterval)
        {
            lastAttackCheck = currentTime;
            CheckForAttackOpportunity();
        }
    }

    private void CheckForAttackOpportunity()
    {
        if (enemy.Target == null)
        {
            // Lost target, go back to patrol
            fsm.TransitionTo("RandomPatrol");
            return;
        }

        float distanceToTarget = enemy.GlobalPosition.DistanceTo(enemy.Target.GlobalPosition);
        float currentTime = Time.GetTicksMsec() / 1000.0f;
        bool missileReady = (currentTime - enemy.GetLastMissileTime()) >= enemy.missileCooldown;

        if (enemy.debugTrajectory)
        {
            GD.Print($"Pursue check: Distance={distanceToTarget:F1}, MissileReady={missileReady}, MissileRange={enemy.missileRange}");
        }

        // Priority 1: Use missiles if in good range and ready
        if (distanceToTarget >= enemy.missileRange && missileReady && enemy.HasLineOfSightToTarget())
        {
            GodotLogger.Info($"Switching to missile attack from distance {distanceToTarget:F1}");
            enemy.SetLastMissileTime(currentTime);
            fsm.TransitionTo("Missile");
            return;
        }

        // Priority 2: Switch to melee if very close
        if (distanceToTarget <= enemy.meleeRange)
        {
            GodotLogger.Info($"Switching to melee attack from distance {distanceToTarget:F1}");
            fsm.TransitionTo("Melee");
            return;
        }

        // Priority 3: Keep pursuing if in the "dead zone" between missile and melee range
        // or if missiles aren't ready yet
        if (distanceToTarget < enemy.missileRange && distanceToTarget > enemy.meleeRange)
        {
            if (enemy.debugTrajectory)
            {
                GD.Print($"In dead zone - continuing pursuit (missile range: {enemy.missileRange}, melee range: {enemy.meleeRange})");
            }
        }

        // Continue pursuing (no state change needed)
    }

    public override void Exit()
    {
        base.Exit();
        enemy.animationPlayer.Stop();
        GodotLogger.Info("Exiting pursue state");
    }
}
