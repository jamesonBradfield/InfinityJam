using Godot;
[GlobalClass]
public partial class PowerupResource : Resource
{
    [Export] public string Name { get; set; } = "Powerup";
    [Export] public string Description { get; set; } = "Description";
    [Export(PropertyHint.Enum, "Bigger,Faster,More")]
    public string Category { get; set; } = "Bigger";
    [Export] public Texture2D Icon { get; set; }

    [Export(PropertyHint.Enum, "Weapon,Player,Health")]
    public string TargetSystem { get; set; } = "Weapon";
    [Export] public string TargetProperty { get; set; } = "DamagePerBullet";
    [Export(PropertyHint.Enum, "Add,Multiply,Set")]
    public string ModificationType { get; set; } = "Multiply";
    [Export] public float ModificationValue { get; set; } = 1.25f;
}
