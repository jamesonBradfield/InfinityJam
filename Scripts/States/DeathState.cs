using Godot;
[GlobalClass]
public partial class DeathState : MyState
{
    [Export] Enemy enemy;
    public override void Enter()
    {
        base.Enter();
        enemy.health.healthBar.Hide();
        enemy.followTarget3D.isEnabled = false;
        enemy.animationPlayer.AnimationFinished += OnAnimationFinished;
        enemy.animationPlayer.Play("TurnOff");
    }

    public override void Exit()
    {
        base.Exit();
    }
    public void OnAnimationFinished(StringName animName)
    {
        if (animName == "TurnOff")
        {
            enemy.animationPlayer.AnimationFinished -= OnAnimationFinished;
            enemy.explosion.Play();
            enemy.QueueFree();
        }
    }
}
