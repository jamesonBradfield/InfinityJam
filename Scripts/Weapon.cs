using Godot;
public partial class Weapon : Node3D
{
    [Export] WeaponResource weaponResource;
    [Export] Camera3D camera;
    [Export] Node3D gunRaycastPositionNode;
    [Export] MeshInstance3D WeaponMeshInstance;
    [Export] Node3D cameraRecoil;
    [Export] Node3D gunRecoil;
    float FireRate;
    float ReloadTime;
    int BulletsPerShot;
    int MaxAmmo;
    int CurrentAmmo;
    float DamagePerBullet;
    Mesh WeaponMesh;
    float timeBetweenLastShot;
    float timeBetweenReload;
    AudioStream shootAudio;
    AudioStream reloadAudio;
    GodotObject Audio;
    [Export] private bool debug;
    private const int RayLength = 1000;
    public override void _Ready()
    {
        this.FireRate = weaponResource.FireRate;
        this.BulletsPerShot = weaponResource.BulletsPerShot;
        this.DamagePerBullet = weaponResource.DamagePerBullet;
        this.ReloadTime = weaponResource.ReloadTime;
        this.WeaponMesh = weaponResource.WeaponMesh;
        this.MaxAmmo = weaponResource.MaxAmmo;
        this.CurrentAmmo = MaxAmmo;
        this.WeaponMesh = weaponResource.WeaponMesh;
        this.WeaponMeshInstance.Mesh = this.WeaponMesh;
        this.WeaponMeshInstance.Position = weaponResource.weaponMeshPosition;
        this.gunRaycastPositionNode.Position = weaponResource.gunRaycastPosition;
        this.shootAudio = weaponResource.shootAudio;
        this.reloadAudio = weaponResource.reloadAudio;
        Audio = GetNode("/root/Audio");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        var spaceState = GetWorld3D().DirectSpaceState;
        var cam = camera;
        var mousePos = GetViewport().GetMousePosition();

        var origin = cam.ProjectRayOrigin(mousePos);
        var end = origin + cam.ProjectRayNormal(mousePos) * RayLength;
        var query = PhysicsRayQueryParameters3D.Create(origin, end);
        query.CollideWithAreas = true;

        var result = spaceState.IntersectRay(query);
        if (result.ContainsKey("position"))
        {

            WeaponMeshInstance.LookAt((Vector3)result["position"], Vector3.Up);
            if (timeBetweenLastShot == 0f + Mathf.Epsilon)
            {
                var gunOrigin = gunRaycastPositionNode.GlobalPosition;
                var gunEnd = ((Vector3)result["position"] - gunOrigin).Normalized();

                var gunQuery = PhysicsRayQueryParameters3D.Create(gunOrigin, gunEnd);
                gunQuery.CollideWithAreas = true;
                var gunResult = spaceState.IntersectRay(gunQuery);
                if (debug)
                {
                    DebugDraw3D.DrawSphere((Vector3)result["position"]);
                    DebugDraw3D.DrawRay(origin, end, RayLength);
                    DebugDraw3D.DrawRay(gunOrigin, gunEnd, RayLength);
                }
            }
        }
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("shoot"))
        {
            if (timeBetweenLastShot >= FireRate && CurrentAmmo > 0)
            {
                // reload timer check, we should put a misfire audio cue if you try to shoot while reloading
                if (timeBetweenReload >= ReloadTime)
                {
                    timeBetweenLastShot = 0f;
                    CurrentAmmo -= BulletsPerShot;
                    cameraRecoil.Call("recoilFire");
                    gunRecoil.Call("recoilFire");
                    Audio.Call("play_sound", shootAudio);
                    if (debug)
                        GD.Print($"Shot {BulletsPerShot} {CurrentAmmo} left");
                }
                else
                {
                    if (debug)
                        GD.Print("trying to shoot while reloading");
                    timeBetweenLastShot = 0f;
                }
            }
            // this should be available at any time (not constrained by the timer of our last bullet.
            if (CurrentAmmo <= 0)
            {
                //reload
                if (debug)
                    GD.Print("Reloading");
                timeBetweenReload = 0f;
                Audio.Call("play_sound", reloadAudio);
                // this should be reset at the end of our reload timer.
                CurrentAmmo = MaxAmmo;
            }
        }
        timeBetweenReload += (float)delta;
        timeBetweenLastShot += (float)delta;
    }
}
