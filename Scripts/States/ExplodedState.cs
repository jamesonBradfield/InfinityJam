using Godot;
using GodotTools;

public partial class ExplodedState : MyState
{
    [Export] Missile missile;
    [Export] public float explosionDuration = 0.5f; // Time before missile disappears

    private float explosionTimer = 0f;

    public override void Enter()
    {
        if (missile != null)
        {
            // Add explosion effects here:
            CreateExplosionEffects();

            GodotLogger.Info("Missile exploded!");
            explosionTimer = 0f;
        }
    }

    public override void Update(float delta)
    {
        explosionTimer += delta;

        // Remove missile after explosion duration
        if (explosionTimer >= explosionDuration)
        {
            if (missile != null)
            {
                missile.QueueFree();
            }
        }
    }

    private void CreateExplosionEffects()
    {
        // TODO: Add your explosion effects here:
        // - Particle systems
        // - Sound effects  
        // - Camera shake
        // - Damage to nearby objects
        // - Light flash

        if (missile.debugTrajectory)
        {
            Vector3 explosionPos = missile.Transform.Origin;
            GD.Print($"💥 Explosion at {explosionPos}!");
        }
    }

    public override void Exit()
    {
        // Final cleanup if needed
    }
}
