using Godot;
[GlobalClass]
public partial class WeaponResource : Resource
{
    [Export] public float FireRate = 0.5f;
    [Export] public float ReloadTime = 1.5f;
    [Export] public int BulletsPerShot = 1;
    [Export] public int MaxAmmo = 30;
    [Export] public float DamagePerBullet = 10f;
    [Export] public Mesh WeaponMesh;
    [Export] public Vector3 weaponMeshPosition = Vector3.Zero;
    [Export] public Vector3 gunRaycastPosition = Vector3.Zero;
    [Export] public AudioStream shootAudio;
    [Export] public AudioStream reloadAudio;
}
