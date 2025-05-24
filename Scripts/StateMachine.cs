using Godot;
using System.Collections.Generic;
using GodotTools;

[GlobalClass]
public partial class StateMachine : Node
{
    [Export] public NodePath initialState;
    private Dictionary<string, MyState> _states;
    private MyState _currentState;

    [Signal]
    public delegate void changedStateEventHandler();

    public MyState CurrentState { get => _currentState; set => _currentState = value; }

    public override void _Ready()
    {
        _states = new Dictionary<string, MyState>();

        foreach (Node node in GetChildren())
        {
            if (node is MyState s)
            {
                _states[node.Name] = s;
                GodotLogger.Info("Registered state: " + node.Name);
                s.fsm = this;
                s.Ready();
                s.Exit();
            }
        }

        if (initialState != null)
        {
            _currentState = GetNode<MyState>(initialState);
            _currentState.Enter();
            GodotLogger.Info("Started with initial state: " + _currentState.Name);
        }
    }

    public override void _Process(double delta)
    {
        if (_currentState != null)
        {
            _currentState.Update((float)delta);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_currentState != null)
        {
            _currentState.PhysicsUpdate((float)delta);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_currentState != null)
        {
            _currentState.HandleInput(@event);
        }
    }

    public void TransitionTo(string key)
    {
        if (!_states.ContainsKey(key))
        {
            GodotLogger.Error("State not found: " + key);
            return;
        }

        if (_currentState == _states[key])
        {
            GodotLogger.Info("Already in state: " + key);
            return;
        }

        MyState oldState = _currentState;
        MyState newState = _states[key];

        GodotLogger.Info("Transitioning from " + oldState.Name + " to " + newState.Name);

        _currentState.Exit();
        _currentState = newState;
        _currentState.Enter();

        EmitSignal(SignalName.changedState);
    }
}
