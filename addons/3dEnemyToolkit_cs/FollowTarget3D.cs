using Godot;
[GlobalClass]
public partial class FollowTarget3D : NavigationAgent3D
{
    [Signal]
    public delegate void ReachedTargetEventHandler(Node3D target);

    [Export] public float Speed { get; set; } = 5.0f;
    [Export] public float TurnSpeed { get; set; } = 0.3f;
    [Export] public float ReachTargetMinDistance { get; set; } = 1.3f;
    [Export] public bool isEnabled { get; set; } = true;

    private Node3D target;
    private bool isTargetSet = false;
    private Vector3 targetPosition = Vector3.Zero;
    private Vector3 lastTargetPosition = Vector3.Zero;
    private bool fixedTarget = false;

    private CharacterBody3D parent;

    public override void _Ready()
    {
        VelocityComputed += OnVelocityComputed;
        parent = GetParent() as CharacterBody3D; // This was missing
    }

    public override void _Process(double delta)
    {
        if (isEnabled)
        {
            if (fixedTarget)
            {
                GoToLocation(targetPosition);
            }
            else if (target != null)
            {
                GoToLocation(target.GlobalPosition);
                if (target != null && parent.GlobalPosition.DistanceTo(target.GlobalPosition) <= ReachTargetMinDistance)
                {
                    EmitSignal(SignalName.ReachedTarget, target); // Use SignalName for consistency
                }
            }

            parent.MoveAndSlide();
        }
    }

    public void SetFixedTarget(Vector3 newTarget)
    {
        target = null;
        targetPosition = newTarget;
        fixedTarget = true;
        isTargetSet = true;
    }

    public void SetTarget(Node3D newTarget)
    {
        target = newTarget;
        targetPosition = Vector3.Zero;
        fixedTarget = false;
        isTargetSet = true;
    }

    public void ClearTarget()
    {
        target = null;
        targetPosition = Vector3.Zero;
        isTargetSet = false;
    }

    private void GoToLocation(Vector3 targetPos)
    {
        if (!isTargetSet || lastTargetPosition != targetPos)
        {
            SetTargetPosition(targetPos);
            lastTargetPosition = targetPos;
            isTargetSet = true;
        }

        float lookDir = Mathf.Atan2(-parent.Velocity.X, -parent.Velocity.Z);
        parent.Rotation = new Vector3(
            parent.Rotation.X,
            Mathf.LerpAngle(parent.Rotation.Y, lookDir, TurnSpeed),
            parent.Rotation.Z
        );

        if (IsNavigationFinished())
        {
            isTargetSet = false;
            return;
        }

        Vector3 nextPathPosition = GetNextPathPosition();
        Vector3 currentPosition = parent.GlobalPosition;
        Vector3 newVelocity = (nextPathPosition - currentPosition).Normalized() * Speed;

        if (AvoidanceEnabled)
        {
            SetVelocity(newVelocity.MoveToward(newVelocity, 0.25f));
        }
        else
        {
            parent.Velocity = newVelocity.MoveToward(newVelocity, 0.25f);
        }
    }

    private void OnVelocityComputed(Vector3 safeVelocity)
    {
        parent.Velocity = parent.Velocity.MoveToward(safeVelocity, 0.25f);
    }
}
