using Godot;
using GodotTools;

public partial class LaunchedState : MyState
{
    [Export] Missile missile;


    public override void Enter()
    {
        missile.CreateTrajectoryPath();
        GodotLogger.Info($"Missile launched toward {missile.TargetPosition}");
    }
    public override void PhysicsUpdate(float delta)
    {
        if (missile.pathFollower == null) return;
        if (missile.pathLength > 0)
        {
            missile.distanceTraveled += missile.speed * (float)delta;
            missile.pathFollower.Progress = missile.distanceTraveled;

            // Check if we've reached the end of the path
            if (missile.distanceTraveled >= missile.pathLength)
            {
                if (missile.debugTrajectory)
                {
                    float flightTime = (Time.GetTicksMsec() / 1000.0f) - missile.launchTime;
                    Vector3 finalError = missile.pathFollower.GlobalTransform.Origin - missile.TargetPosition;
                    GD.Print($"Path complete - Time: {flightTime:F2}s, Error: {finalError.Length():F3}");
                }

                fsm?.TransitionTo("Exploded");
            }
        }
    }

    public override void Exit()
    {
        GodotLogger.Info("Launch state exited");
    }
}
