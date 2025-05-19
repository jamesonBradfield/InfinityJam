using Godot;
using System.Linq;

[GlobalClass]
public partial class SimpleVision3D : Node3D
{
    [Signal]
    public delegate void GetSightEventHandler(Node3D body);

    [Signal]
    public delegate void LostSightEventHandler();

    [Export]
    public bool Enabled { get; set; } = true;

    [Export]
    public string LookUpGroup { get; set; } = "player";

    [ExportCategory("Vision Area")]
    [Export]
    public float Distance { get; set; } = 50.0f;

    [Export]
    public float BaseWidth { get; set; } = 10.0f;

    [Export]
    public float EndWidth { get; set; } = 30.0f;

    [Export]
    public float BaseHeight { get; set; } = 5.0f;

    [Export]
    public float EndHeight { get; set; } = 5.0f;

    [Export]
    public float BaseConeSize { get; set; } = 1.0f;

    [Export]
    public uint Mask { get; set; }

    [Export]
    public CollisionShape3D VisionArea { get; set; }

    private Area3D vision;
    private Node3D target;

    public override void _Ready()
    {
        vision = new Area3D();
        if (VisionArea == null)
        {
            VisionArea = new CollisionShape3D();
            VisionArea.Shape = BuildVisionShape();
        }
        vision.AddChild(VisionArea);
        vision.CollisionMask = Mask;
        AddChild(vision);
    }

    public override void _Process(double delta)
    {
        if (!Enabled)
            return;

        if (target != null)
        {
            if (!CheckSight(target))
            {
                target = null;
                EmitSignal(SignalName.LostSight);
            }
        }
        else
        {
            CheckOverlapping();
        }
    }

    private bool CheckSight(Node3D sightTarget)
    {
        var space = GetWorld3D().DirectSpaceState;
        var query = PhysicsRayQueryParameters3D.Create(GlobalPosition, sightTarget.GlobalPosition);
        var collision = space.IntersectRay(query);

        if (collision.Count > 0)
        {
            var collider = collision["collider"].As<Node3D>();
            if (collider == sightTarget)
            {
                return true;
            }
        }
        return false;
    }

    private void CheckOverlapping()
    {
        var overlappingBodies = vision.GetOverlappingBodies();
        // Using LINQ Where instead of GDScript's filter function
        var targetOverlap = overlappingBodies.Where(item => item.IsInGroup(LookUpGroup)).ToArray();

        if (targetOverlap.Length > 0 && CheckSight(targetOverlap[0]))
        {
            target = targetOverlap[0];
            EmitSignal(SignalName.GetSight, target);
        }
    }

    private ConvexPolygonShape3D BuildVisionShape()
    {
        var result = new ConvexPolygonShape3D();
        var points = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(BaseHeight/2, 0, -BaseConeSize),
            new Vector3(EndWidth/2, 0, -Distance),
            new Vector3(-(BaseHeight/2), 0, -BaseConeSize),
            new Vector3(-(EndWidth/2), 0, -Distance),
            new Vector3(0, BaseHeight, 0),
            new Vector3(BaseHeight/2, BaseHeight, -BaseConeSize),
            new Vector3(EndWidth/2, BaseHeight, -Distance),
            new Vector3(-(BaseHeight/2), BaseHeight, -BaseConeSize),
            new Vector3(-(EndWidth/2), BaseHeight, -Distance)
        };

        result.Points = points;
        return result;
    }
}
