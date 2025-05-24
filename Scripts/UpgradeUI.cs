using Godot;
using System;

public partial class UpgradeUI : Control
{
    [Export] public PowerupManager PowerupManager;
    [Export] public WaveManager waveManager;
    [Export] public Button[] PowerupButtons;
    [Export] public Label[] PowerupLabels;
    [Export] public TextureRect[] PowerupIcons;

    private PowerupResource[] _currentPowerups;

    public override void _Ready(){
        waveManager.End += ShowUpgrades;
    }

    public void ShowUpgrades()
    {
        // Get three random powerups
        _currentPowerups = PowerupManager.GetRandomPowerups(3);

        // Update UI elements
        for (int i = 0; i < PowerupButtons.Length; i++)
        {
            if (i < _currentPowerups.Length)
            {
                var powerup = _currentPowerups[i];
                PowerupLabels[i].Text = $"{powerup.Name}\n{powerup.Description}";
                PowerupIcons[i].Texture = powerup.Icon;
                PowerupButtons[i].Visible = true;
            }
            else
            {
                PowerupButtons[i].Visible = false;
            }
        }

        Visible = true;
    }

    // Connect these to your button signals
    public void OnPowerupSelected(int index)
    {
        if (index >= 0 && index < _currentPowerups.Length)
        {
            PowerupManager.ApplyPowerup(_currentPowerups[index]);
            Visible = false;
            // Signal to wave manager to continue
            waveManager.InitializeRound();
        }
    }
}
