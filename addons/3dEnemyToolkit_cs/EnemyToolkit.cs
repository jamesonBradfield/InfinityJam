using Godot;

[Tool]
public partial class EnemyToolkit : EditorPlugin
{
    public override void _EnterTree()
    {
        // Load our C# scripts
        var simpleVisionScript = GD.Load<Script>("res://addons/3dEnemyToolkit_cs/SimpleVision3D.cs");
        var randomTargetScript = GD.Load<Script>("res://addons/3dEnemyToolkit_cs/RandomTarget3D.cs");
        var followTargetScript = GD.Load<Script>("res://addons/3dEnemyToolkit_cs/FollowTarget3D.cs");

        // Load our icons
        var followTargetIcon = GD.Load<Texture2D>("res://addons/3dEnemyToolkit_cs/icons/FollowTarget3D.svg");
        var randomTargetIcon = GD.Load<Texture2D>("res://addons/3dEnemyToolkit_cs/icons/RandomMovement3D.svg");
        var visionIcon = GD.Load<Texture2D>("res://addons/3dEnemyToolkit_cs/icons/Vision3D.svg");

        // Register custom types
        AddCustomType("SimpleVision3D", "Node3D", simpleVisionScript, visionIcon);
        AddCustomType("RandomTarget3D", "Node3D", randomTargetScript, randomTargetIcon);
        AddCustomType("FollowTarget3D", "NavigationAgent3D", followTargetScript, followTargetIcon);
    }

    public override void _ExitTree()
    {
        // Clean up by removing our custom types
        RemoveCustomType("SimpleVision3D");
        RemoveCustomType("RandomTarget3D");
        RemoveCustomType("FollowTarget3D");
    }
}
