using Godot;
using GodotTools;

public partial class CollisionState : MyState
{
    [Export] Missile missile;
    [Export] public float impactDelay = 0.1f; // Brief delay before explosion

    private float impactTimer = 0f;

    public override void Enter()
    {
        if (missile != null)
        {
            // Handle impact effects
            CreateImpactEffects();

            GodotLogger.Info("Missile impact!");
            impactTimer = 0f;
        }
    }

    public override void Update(float delta)
    {
        impactTimer += delta;

        // Transition to explosion after brief impact delay
        if (impactTimer >= impactDelay)
        {
            if (fsm != null)
            {
                fsm.TransitionTo("Exploded");
            }
        }
    }

    private void CreateImpactEffects()
    {
        // TODO: Add your impact effects here:
        // - Impact particles (sparks, debris)
        // - Impact sound
        // - Apply damage to hit object
        // - Screen shake
        // - Impact decals

        if (missile.debugTrajectory)
        {
            Vector3 impactPos = missile.Transform.Origin;
            GD.Print($"🎯 Impact at {impactPos}!");
        }
    }

    public override void Exit()
    {
        GodotLogger.Info("Impact handled, proceeding to explosion");
    }
}
