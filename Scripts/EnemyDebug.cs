using Godot;
using System;

public partial class EnemyDebug : PanelContainer
{
    [Export] StateMachine fsm;
    [Export] Button patrol;
    [Export] Button pursue;
    [Export] Button hurt;
    [Export] Button death;
    [Export] Label currentState;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        patrol.ButtonDown += () => { fsm.TransitionTo("RandomPatrol"); GD.Print($"fsm transitioned to RandomPatrol"); };
        pursue.ButtonDown += () => { fsm.TransitionTo("Pursue"); GD.Print($"fsm transitioned to Pursue"); };
        hurt.ButtonDown += () => { fsm.TransitionTo("Hurt"); GD.Print($"fsm transitioned to Hurt"); };
        fsm.changedState += () => { currentState.Text = "CurrentState: " + fsm.CurrentState; };
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
