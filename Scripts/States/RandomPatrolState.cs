using Godot;
[GlobalClass]
public partial class RandomPatrolState : MyState
{

    [Export] Enemy enemy;
    public override void Enter()
    {
        base.Enter();
        enemy.followTarget3D.isEnabled = true;
        enemy.randomTarget3D.isEnabled = true;
        enemy.followTarget3D.ClearTarget();
        enemy.followTarget3D.Speed = enemy.walkSpeed;
        enemy.followTarget3D.SetFixedTarget(enemy.randomTarget3D.GetNextPoint());
        enemy.Target = null;
        enemy.animationPlayer.Play("Walk");
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.animationPlayer.Stop();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override void HandleInput(InputEvent @event)
    {
        base.HandleInput(@event);
    }

    public override void PhysicsUpdate(float delta)
    {
        base.PhysicsUpdate(delta);
    }

    public override void Ready()
    {
        base.Ready();
    }

    public override string ToString()
    {
        return base.ToString();
    }

    public override void Update(float delta)
    {
        base.Update(delta);
    }

    public override void _EnterTree()
    {
        base._EnterTree();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
    }
}
