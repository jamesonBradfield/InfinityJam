using Godot;
using GodotTools;

[GlobalClass]
public partial class MissileState : MyState
{
    [Export] Enemy enemy;
    [Export] PackedScene missileScene; // Drag your missile scene here in editor
    [Export] public float missileSpeed = 15f;
    [Export] public float missileArcHeight = 8f;
    [Export] public int missilesPerVolley = 1; // How many missiles to fire
    [Export] public float volleyDelay = 0.3f; // Delay between missiles in volley
    [Export] public Node3D missileSpawnPoint; // Optional spawn point, or uses enemy position

    private int missilesFired = 0;
    private float nextMissileTime = 0f;
    private bool volleyComplete = false;

    public override void Enter()
    {
        base.Enter();
        // Stop movement
        enemy.followTarget3D.isEnabled = false;

        // Hide health bar during attack
        enemy.health.healthBar.Hide();

        // Play missile launch animation if it exists
        if (enemy.animationPlayer.HasAnimation("LaunchMissile"))
        {
            enemy.animationPlayer.AnimationFinished += OnAnimationFinished;
            enemy.animationPlayer.Play("LaunchMissile");
        }
        else if (enemy.animationPlayer.HasAnimation("Shoot"))
        {
            // Fallback to shoot animation
            enemy.animationPlayer.AnimationFinished += OnAnimationFinished;
            enemy.animationPlayer.Play("Shoot");
        }
        else
        {
            // No animation, fire immediately
            StartMissileVolley();
        }

        GodotLogger.Info($"Enemy entering missile state - targeting {enemy.Target?.Name ?? "no target"}");
    }

    public override void Update(float delta)
    {
        base.Update(delta);

        // Handle volley firing
        if (!volleyComplete && Time.GetTicksMsec() / 1000.0f >= nextMissileTime)
        {
            FireMissile();
        }
    }

    private void StartMissileVolley()
    {
        missilesFired = 0;
        volleyComplete = false;
        nextMissileTime = Time.GetTicksMsec() / 1000.0f; // Fire first missile immediately
        GodotLogger.Info($"Starting missile volley: {missilesPerVolley} missiles");
    }

    private void FireMissile()
    {
        if (enemy.Target == null)
        {
            GodotLogger.Info("No target for missile - aborting volley");
            volleyComplete = true;
            TransitionAfterVolley();
            return;
        }

        if (missileScene == null)
        {
            GodotLogger.Error("No missile scene assigned to MissileState!");
            volleyComplete = true;
            TransitionAfterVolley();
            return;
        }

        // Create missile
        Missile missile = missileScene.Instantiate() as Missile;
        if (missile == null)
        {
            GodotLogger.Error("Failed to instantiate missile!");
            return;
        }

        // Add to scene
        enemy.GetTree().CurrentScene.AddChild(missile);

        // Position missile at spawn point or enemy position
        Vector3 spawnPosition = missileSpawnPoint?.GlobalPosition ?? enemy.GlobalPosition;
        spawnPosition.Y += 1.5f; // Slight height offset to avoid ground collision
        missile.GlobalPosition = spawnPosition;

        // Configure missile
        missile.speed = missileSpeed;
        missile.arcHeight = missileArcHeight;
        missile.debugTrajectory = true; // Enable for testing, disable in production

        // Calculate target position (add some prediction if target is moving)
        Vector3 targetPosition = PredictTargetPosition();

        // Launch missile
        missile.LaunchToward(targetPosition);

        GodotLogger.Info($"Missile {missilesFired + 1}/{missilesPerVolley} fired at {targetPosition}");

        // Update volley tracking
        missilesFired++;

        if (missilesFired >= missilesPerVolley)
        {
            volleyComplete = true;
            TransitionAfterVolley();
        }
        else
        {
            // Schedule next missile
            nextMissileTime = Time.GetTicksMsec() / 1000.0f + volleyDelay;
        }
    }

    private Vector3 PredictTargetPosition()
    {
        if (enemy.Target == null) return Vector3.Zero;

        Vector3 targetPos = enemy.Target.GlobalPosition;

        // Optional: Add target movement prediction
        if (enemy.Target is CharacterBody3D characterBody)
        {
            // Predict where target will be based on current velocity
            float timeToTarget = targetPos.DistanceTo(enemy.GlobalPosition) / missileSpeed;
            Vector3 predictedOffset = characterBody.Velocity * timeToTarget * 0.5f; // 50% prediction
            targetPos += predictedOffset;

            if (enemy.debugTrajectory)
            {
                GodotLogger.Info($"Target prediction: {predictedOffset} -> {targetPos}");
            }
        }

        return targetPos;
    }

    private void TransitionAfterVolley()
    {
        // Decide next state based on distance to target and target status
        if (enemy.Target == null)
        {
            fsm.TransitionTo("RandomPatrol");
        }
        else
        {
            float distanceToTarget = enemy.GlobalPosition.DistanceTo(enemy.Target.GlobalPosition);

            // If target is close, try melee; otherwise pursue
            if (distanceToTarget <= 3f) // Adjust melee range as needed
            {
                fsm.TransitionTo("Melee");
            }
            else
            {
                fsm.TransitionTo("Pursue");
            }
        }
    }

    public void OnAnimationFinished(StringName animName)
    {
        if (animName == "LaunchMissile" || animName == "Shoot")
        {
            enemy.animationPlayer.AnimationFinished -= OnAnimationFinished;
            StartMissileVolley();
        }
    }

    public override void Exit()
    {
        base.Exit();

        // Clean up animation connection
        if (enemy.animationPlayer.IsConnected("animation_finished", new Callable(this, nameof(OnAnimationFinished))))
        {
            enemy.animationPlayer.AnimationFinished -= OnAnimationFinished;
        }

        // Reset state
        volleyComplete = true;
        missilesFired = 0;

        GodotLogger.Info("Exiting missile state");
    }
}
