using Godot;
using System;
using GodotTools;

public partial class EnemyDebug : PanelContainer
{
    [Export] StateMachine fsm;
    [Export] Button patrol;
    [Export] Button pursue;
    [Export] Button hurt;
    [Export] Button shoot;
    [Export] Button melee;
    [Export] Button death;
    [Export] Label currentState;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        patrol.ButtonDown += () => { fsm.TransitionTo("RandomPatrol"); GodotLogger.Info($"fsm transitioned to RandomPatrol"); };
        pursue.ButtonDown += () => { fsm.TransitionTo("Pursue"); GodotLogger.Info($"fsm transitioned to Pursue"); };
        hurt.ButtonDown += () => { fsm.TransitionTo("Hurt"); GodotLogger.Info($"fsm transitioned to Hurt"); };
        shoot.ButtonDown += () => { fsm.TransitionTo("Shoot"); GodotLogger.Info($"fsm transitioned to Shoot"); };
        melee.ButtonDown += () => { fsm.TransitionTo("Melee"); GodotLogger.Info($"fsm transitioned to Melee"); };
        death.ButtonDown += () => { fsm.TransitionTo("Death"); GodotLogger.Info($"fsm transitioned to Death"); };
        fsm.changedState += () => { currentState.Text = "CurrentState: " + fsm.CurrentState; };
    }
}
