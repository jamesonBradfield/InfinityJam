using Godot;
public partial class ExplosionBig : Node3D
{
    private GpuParticles3D _fireparticles;
    private GpuParticles3D _smokeparticles;
    private GpuParticles3D _sparkparticles;
    private GpuParticles3D _debrisparticles;
    private GpuParticles3D _debrissmokeparticles;
    public override void _Ready()
    {
        _fireparticles = GetNode<GpuParticles3D>("Fire");
        _smokeparticles = GetNode<GpuParticles3D>("Smoke");
        _sparkparticles = GetNode<GpuParticles3D>("Sparks");
        _debrisparticles = GetNode<GpuParticles3D>("Debri");
        _debrissmokeparticles = GetNode<GpuParticles3D>("DebriSmoke");
    }

    public void Play()
    {
        _fireparticles.Emitting = true;
        _smokeparticles.Emitting = true;
        _sparkparticles.Emitting = true;
        _debrisparticles.Emitting = true;
        _debrissmokeparticles.Emitting = true;
    }
}
