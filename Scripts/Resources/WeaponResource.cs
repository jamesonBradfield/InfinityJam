using Godot;
[GlobalClass]
public partial class WeaponResource : Resource
{
    [ExportGroup("Stats")]
    [Export] public float FireRate = 0.5f;
    [Export] public float ReloadTime = 1.5f;
    [Export] public int BulletsPerShot = 1;
    [Export] public int MaxAmmo = 30;
    [Export] public float DamagePerBullet = 10f;
    [Export] public float Spread = 0.0f;
    [ExportGroup("Visuals and Feedback")]
    [Export] public PackedScene WeaponScene;
    [Export] public Vector3 WeaponMeshPosition;
    [Export] public AudioStream shootAudio;
    [Export] public AudioStream reloadAudio;

}
