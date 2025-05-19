using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class HurtState : MyState
{
    [Export] Enemy enemy;
    [Export] float hurtDuration = .125f;
    private bool transitioningOut = false;

    public override void Enter()
    {
        base.Enter();
        GD.Print("Entered hurt state");

        if (enemy == null)
        {
            enemy = GetParent().GetParent() as Enemy;
            if (enemy == null)
            {
                GD.PrintErr("Failed to get enemy reference in HurtState");
                return;
            }
        }

        // Stop movement
        if (enemy.followTarget3D != null)
        {
            enemy.followTarget3D.isEnabled = false;
        }

        // Play animation
        if (enemy.animationPlayer != null)
        {
            if (enemy.animationPlayer.HasAnimation("Hit"))
            {
                enemy.animationPlayer.Play("Hit");

                // Register for animation finished - might be useful
                enemy.animationPlayer.AnimationFinished += OnAnimationFinished;
            }
            else
            {
                GD.PrintErr("No 'Hit' animation found!");
            }
        }
        else
        {
            GD.PrintErr("No animation player found!");
        }

        // Schedule return to previous state
        transitioningOut = false;
        _ = ScheduleStateExit();
    }

    private async Task ScheduleStateExit()
    {
        await Task.Delay((int)(hurtDuration * 1000));

        if (!transitioningOut && IsInstanceValid(this))
        {
            GD.Print("Hurt state duration complete, transitioning out");

            // Transition back to patrol or pursue based on if we have a target
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

    private void OnAnimationFinished(StringName animName)
    {
        if (animName == "Hit")
        {
            enemy.animationPlayer.AnimationFinished -= OnAnimationFinished;

            // If animation is shorter than our hurt duration,
            // you could decide to end hurt state here instead
        }
    }

    public override void Exit()
    {
        GD.Print("Exiting hurt state");
        transitioningOut = true;

        if (enemy != null && enemy.animationPlayer != null)
        {
            enemy.animationPlayer.Stop();
            enemy.animationPlayer.AnimationFinished -= OnAnimationFinished;
        }

        base.Exit();
    }

    public override void Ready()
    {
        base.Ready();

        if (enemy == null)
        {
            enemy = GetParent().GetParent() as Enemy;
        }
    }
}
