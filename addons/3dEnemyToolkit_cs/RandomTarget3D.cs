using Godot;

[GlobalClass]
public partial class RandomTarget3D : Node3D
{
    [Export] public float MinRadius { get; set; } = 1.0f;
    [Export] public float MaxRadius { get; set; } = 10.0f;
    [Export] public int MaxAngleRange { get; set; } = 120;
    [Export] public int MinAngleRange { get; set; } = -120;
    [Export] public bool isEnabled;
    private SpringArm3D targetArm;
    private Marker3D target;

    public override void _Ready()
    {
        targetArm = new();
        target = new();
        AddChild(targetArm);
        targetArm.AddChild(target);
    }

    public Vector3 GetNextPoint()
    {
        if (isEnabled)
        {
            GD.Randomize();
            float angle = Mathf.DegToRad(GD.RandRange(MinAngleRange, MaxAngleRange));
            float distance = (float)GD.RandRange(MinRadius, MaxRadius);

            Vector3 tempRotation = Rotation;
            tempRotation.Y = angle;
            Rotation = tempRotation;

            targetArm.SpringLength = distance;
        }

        return target.GlobalPosition;
    }
}
