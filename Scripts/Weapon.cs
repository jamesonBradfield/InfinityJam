using Godot;
using System.Collections.Generic;
using GodotTools;
public partial class Weapon : Node3D
{
    [Export] int currentWeaponResourceIndex;
    [Export] WeaponResource[] weaponResources;
    [Export] Camera3D camera;
    [Export] Node3D cameraRecoil;
    Node3D gunRecoil;
    [Export] CollisionObject3D player;
    [Export] AudioStream hitMarkerSound;
    AnimatedSprite3D muzzleAnimation;
    OmniLight3D muzzleLight;
    SubViewport subViewport;

    AudioStream shootAudio, reloadAudio;
    GodotObject Audio;
    Node3D WeaponInScene;
    Vector3 aimPoint;
    public int BulletsPerShot, MaxAmmo, CurrentAmmo;
    public float DamagePerBullet, spread, timeBetweenLastShot, timeBetweenReload, FireRate, ReloadTime;
    bool shouldStartShooting = false;
    bool shouldFire = false;
    List<CollisionObject3D> objectsHit = new();
    [Export] private bool debug;
    private const int RayLength = 1000;

    public override void _Ready()
    {
        gunRecoil = GetNode<Node3D>("ProceduralRecoil");
        InitializeWeapon();
        Audio = GetNode("/root/Audio");
        GD.Randomize();
        subViewport = GetNode<SubViewport>("/root/TemplateMap/SubViewportContainer/SubViewport");
    }


    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
        Vector2 mousePos = subViewport.GetMousePosition();

        Vector3 origin = camera.ProjectRayOrigin(mousePos);
        Vector3 direction = camera.ProjectRayNormal(mousePos);
        Vector3 end = origin + direction * RayLength;
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(origin, end);
        query.CollideWithAreas = true;
        var result = spaceState.IntersectRay(query);

        if (result.ContainsKey("position"))
        {
            aimPoint = (Vector3)result["position"];
            if (shouldFire)
            {
                objectsHit.Clear();
                Vector3 gunOrigin = muzzleAnimation.GlobalPosition;
                Vector3 baseDirection = (aimPoint - gunOrigin).Normalized();
                for (int i = 0; i < BulletsPerShot; i++)
                {
                    Vector3 spreadDirection = ApplySpread(baseDirection);
                    Vector3 spreadEnd = gunOrigin + spreadDirection * RayLength;

                    var gunQuery = PhysicsRayQueryParameters3D.Create(gunOrigin, spreadEnd);
                    gunQuery.Exclude = [player.GetRid()];
                    var bulletResult = spaceState.IntersectRay(gunQuery);
                    if (bulletResult.ContainsKey("position") && bulletResult.ContainsKey("collider"))
                    {
                        Vector3 hitPosition = (Vector3)bulletResult["position"];
                        CollisionObject3D hitObject = (CollisionObject3D)bulletResult["collider"];
                        if (!objectsHit.Contains(hitObject))
                        {
                            objectsHit.Add(hitObject);
                        }
                        if (debug)
                        {
                            DebugDraw3D.DrawLine(gunOrigin, hitPosition, new Color(1, 0, 0, 0.5f));
                            DebugDraw3D.DrawSphere(hitPosition, 0.1f, Colors.Red);
                        }
                    }
                    else if (debug)
                        DebugDraw3D.DrawLine(gunOrigin, spreadEnd, new Color(1, 1, 0, 0.3f));
                }
                shouldFire = false;
            }
            if (debug)
                DebugDraw3D.DrawSphere(aimPoint, 0.05f, Colors.Yellow);
        }
    }
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("switchNext"))
        {
            if (currentWeaponResourceIndex < weaponResources.Length - 1)
                currentWeaponResourceIndex++;
            else
                currentWeaponResourceIndex = 0;
            InitializeWeapon();
        }
        else if (@event.IsActionPressed("switchPrev"))
        {
            if (currentWeaponResourceIndex > 0)
                currentWeaponResourceIndex--;
            else
                currentWeaponResourceIndex = weaponResources.Length - 1;
            InitializeWeapon();
        }

        if (@event.IsActionPressed("shoot"))
        {
            shouldStartShooting = true;
        }
        else if (@event.IsActionReleased("shoot"))
        {
            shouldStartShooting = false;
        }
    }

    public override void _Process(double delta)
    {
        timeBetweenReload += (float)delta;
        timeBetweenLastShot += (float)delta;
        if (shouldStartShooting)
        {
            if (timeBetweenLastShot >= FireRate && CurrentAmmo > 0)
            {
                if (timeBetweenReload >= ReloadTime)
                {
                    timeBetweenLastShot = 0f;
                    CurrentAmmo--;
                    shouldFire = true;
                    foreach (CollisionObject3D hitObject in objectsHit)
                    {
                        if (hitObject.HasNode("Health"))
                        {
                            var health = hitObject.GetNode<Health>("Health");
                            health.TakeDamage(DamagePerBullet);
                            Audio.Call("play_sound", hitMarkerSound);
                        }
                    }
                    cameraRecoil.Call("recoilFire");
                    gunRecoil.Call("recoilFire");
                    Audio.Call("play_sound", shootAudio);
                    muzzleAnimation.Show();
                    muzzleLight.Show();
                    muzzleAnimation.Play();
                }
                else
                {
                    timeBetweenLastShot = 0f;
                }
            }

            if (CurrentAmmo <= 0)
            {
                timeBetweenReload = 0f;
                Audio.Call("play_sound", reloadAudio);
                CurrentAmmo = MaxAmmo;
            }
        }
    }
    private Vector3 ApplySpread(Vector3 baseDirection)
    {
        if (spread <= 0)
            return baseDirection;
        float spreadRadians = Mathf.DegToRad(spread);
        Vector3 perpendicularToUp;
        if (Mathf.Abs(baseDirection.Dot(Vector3.Up)) > 0.99f)
            perpendicularToUp = baseDirection.Cross(Vector3.Right).Normalized();
        else
            perpendicularToUp = baseDirection.Cross(Vector3.Up).Normalized();

        Vector3 perpendicularToRight = baseDirection.Cross(perpendicularToUp).Normalized();
        float randomAngle = (float)GD.RandRange(0, Mathf.Pi * 2);
        float randomRadius = (float)GD.RandRange(0, spreadRadians);
        float xSpread = Mathf.Cos(randomAngle) * randomRadius;
        float ySpread = Mathf.Sin(randomAngle) * randomRadius;

        Vector3 result = baseDirection;
        result += perpendicularToRight * xSpread;
        result += perpendicularToUp * ySpread;
        return result.Normalized();
    }

    private void InitializeWeapon()
    {
        if (WeaponInScene == null)
        {
            WeaponInScene = (Node3D)weaponResources[currentWeaponResourceIndex].WeaponScene.Instantiate();
            gunRecoil.AddChild(WeaponInScene);
            WeaponInScene.Position = weaponResources[currentWeaponResourceIndex].WeaponMeshPosition;
            muzzleAnimation = WeaponInScene.GetNode<AnimatedSprite3D>("Gun/AnimatedSprite3D");
            muzzleLight = WeaponInScene.GetNode<OmniLight3D>("Gun/OmniLight3D");
            muzzleAnimation.AnimationLooped += muzzleAnimation.Hide;
            muzzleAnimation.AnimationLooped += muzzleAnimation.Stop;
            muzzleAnimation.AnimationLooped += muzzleLight.Hide;
        }
        else
        {
            WeaponInScene.QueueFree();
            WeaponInScene = (Node3D)weaponResources[currentWeaponResourceIndex].WeaponScene.Instantiate();
            gunRecoil.AddChild(WeaponInScene);
        }
        WeaponInScene.Position = weaponResources[currentWeaponResourceIndex].WeaponMeshPosition;
        this.FireRate = weaponResources[currentWeaponResourceIndex].FireRate;
        this.spread = weaponResources[currentWeaponResourceIndex].Spread;
        this.BulletsPerShot = weaponResources[currentWeaponResourceIndex].BulletsPerShot;
        this.DamagePerBullet = weaponResources[currentWeaponResourceIndex].DamagePerBullet;
        this.ReloadTime = weaponResources[currentWeaponResourceIndex].ReloadTime;
        this.MaxAmmo = weaponResources[currentWeaponResourceIndex].MaxAmmo;
        this.CurrentAmmo = MaxAmmo;
        this.shootAudio = weaponResources[currentWeaponResourceIndex].shootAudio;
        this.reloadAudio = weaponResources[currentWeaponResourceIndex].reloadAudio;
    }
}
