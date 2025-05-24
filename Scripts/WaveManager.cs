using Godot;
using GodotTools;
public partial class WaveManager : Node
{
    [Export] public Curve pointsPerRound;
    [Export] public int round;
    [Export] Label waveCounter;
    public float CurrentBuyAmount;
    [Signal]
    public delegate void StartEventHandler();
    [Signal]
    public delegate void EndEventHandler();
    [Signal]
    public delegate void InitializeEventHandler(float budget);
    public override void _Ready()
    {
        GodotLogger.Info("Ready called");
        InitializeRound();
        StartRound();
    }
    public void InitializeRound()
    {
        round++;
        waveCounter.Text = "Wave : " + round;
        CurrentBuyAmount = (int)pointsPerRound.Sample(round);
        GodotLogger.Info($"CurrentBuyAmount {CurrentBuyAmount}");
        EmitSignal(SignalName.Initialize, CurrentBuyAmount);
    }
    public void StartRound()
    {
        EmitSignal(SignalName.Start);
    }
    // our enemySpawner is handling calling our signal
    // public void EndRound()
    // {
    //     EmitSignal(SignalName.End);
    // }
}
