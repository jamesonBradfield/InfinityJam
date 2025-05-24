using Godot;
[GlobalClass]
public partial class ShootState : MyState
{
    [Export] Enemy enemy;
    public override void Enter()
    {
        base.Enter();
        enemy.health.healthBar.Hide();
        enemy.followTarget3D.isEnabled = false;
        enemy.animationPlayer.AnimationFinished += OnAnimationFinished;
        enemy.animationPlayer.Play("Shoot");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public void OnAnimationFinished(StringName animName)
    {
        if (animName == "Shoot")
        {
            enemy.animationPlayer.AnimationFinished -= OnAnimationFinished;
            if (enemy.Target != null)
            {
                fsm.TransitionTo("Pursue");
            }
            else
            {
                fsm.TransitionTo("RandomPatrol");
            }
        }
    }
}
