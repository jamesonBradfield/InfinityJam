using Godot;
using GodotTools;

public partial class MeleeState : MyState
{
    [Export] Enemy enemy;
    [Export] float meleeDamage;
    Node3D player;
    public override void Enter()
    {
        base.Enter();
        player = (Node3D)GetTree().GetFirstNodeInGroup("PlayerCharacter");
        enemy.health.healthBar.Hide();
        enemy.followTarget3D.isEnabled = false;
        enemy.animationPlayer.AnimationFinished += OnAnimationFinished;
        enemy.animationPlayer.Play("Attack");
        //NOTE: need to add enemy damaging logic here
        Health health = (Health)player.Get("health");
        GodotLogger.Info($"health {health}");
        health.Call("TakeDamage", meleeDamage);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public void OnAnimationFinished(StringName animName)
    {
        if (animName == "Attack")
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
            // NOTE : need to add logic for transitioning into Melee again. (maybe)
        }
    }
}
