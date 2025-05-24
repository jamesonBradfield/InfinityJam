using Godot;
using GodotTools;

[GlobalClass]
public partial class Missile : Node3D
{
    [Export] public float speed = 10f;
    [Export] public float arcHeight = 5.0f;
    [Export] public Vector3 TargetPosition = Vector3.Zero;
    [Export] private StateMachine fsm;
    [Export] public bool debugTrajectory = false;
    [Export] public Path3D trajectoryPath;
    [Export] public PathFollow3D pathFollower;
    [Export] public MeshInstance3D meshInstance3D;
    public Vector3 startPosition = Vector3.Zero;
    public float pathLength;
    public float distanceTraveled;
    public float launchTime = 0f;
    public override void _Ready()
    {
        base._Ready();

        startPosition = GlobalTransform.Origin;

        startPosition = GlobalTransform.Origin;

        // Set up the scene tree structure
        if (debugTrajectory)
        {
            GD.Print($"Missile initialized at {startPosition}");
        }
    }

    public void CreateTrajectoryPath()
    {
        if (trajectoryPath == null)
        {
            GD.PrintErr("TrajectoryPath is null! Make sure SetupPathComponents was called.");
            return;
        }

        // Create smooth arc curve
        Curve3D curve = new Curve3D();
        Vector3 start = startPosition;
        Vector3 end = TargetPosition;
        Vector3 midpoint = (start + end) / 2f;
        midpoint.Y += arcHeight;

        // Calculate control vectors for vertical launch then target approach
        float distance = start.DistanceTo(end);
        float controlStrength = distance * 0.4f;

        // Start point: controlled vertical takeoff (don't overshoot arc height)
        Vector3 startOut = Vector3.Up * (controlStrength * 0.5f);

        // Midpoint: incoming from vertical, outgoing toward target
        Vector3 toTarget = (end - midpoint).Normalized();
        Vector3 midIn = Vector3.Up * controlStrength; // Coming from vertical path
        Vector3 midOut = toTarget * controlStrength; // Turn toward target

        // End point: approaching from midpoint direction
        Vector3 endIn = (midpoint - end).Normalized() * controlStrength;

        // Add curve points with improved control vectors
        curve.AddPoint(start, Vector3.Zero, startOut);
        curve.AddPoint(midpoint, midIn, midOut);
        curve.AddPoint(end, endIn, Vector3.Zero);

        trajectoryPath.Curve = curve;
        pathLength = curve.GetBakedLength();

        // Initialize path following
        pathFollower.Progress = 0f;
        distanceTraveled = 0f;
        launchTime = Time.GetTicksMsec() / 1000.0f;

        if (debugTrajectory)
        {
            float estimatedTime = pathLength / speed;
            GD.Print($"Path created: {pathLength:F2} units, {estimatedTime:F2}s estimated");
            GD.Print($"Start: {start}, End: {end}, Midpoint: {midpoint}");
        }
    }
    public void LaunchToward(Vector3 target, float customArcHeight = -1f)
    {
        TargetPosition = target;

        if (customArcHeight >= 0)
            arcHeight = customArcHeight;

        fsm?.TransitionTo("Launched");
    }
}
