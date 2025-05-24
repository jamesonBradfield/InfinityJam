using Godot;
using System.Collections.Generic;

public partial class PowerupManager : Node
{
    // References to systems that can be modified
    [Export] public Weapon PlayerWeapon;
    [Export] public CharacterBody3D PlayerController;
    [Export] public Health PlayerHealth;
    // Add other systems as needed
    // Available powerups for the current wave
    [Export] public PowerupResource[] AvailablePowerups;

    // Track applied powerups
    private List<PowerupResource> _appliedPowerups = new List<PowerupResource>();

    [Signal] public delegate void PowerupAppliedEventHandler(PowerupResource powerup);

    // Call this when a powerup is selected from UI
    public void ApplyPowerup(PowerupResource powerup)
    {
        Node targetNode = null;

        // Determine which system to modify
        switch (powerup.TargetSystem)
        {
            case "Weapon":
                targetNode = PlayerWeapon;
                break;
            case "Player":
                targetNode = PlayerController;
                break;
            case "Health":
                targetNode = PlayerHealth;
                break;
                // Add other systems as needed
        }
        if (targetNode == null)
        {
            GD.PrintErr($"Target system {powerup.TargetSystem} not found");
            return;
        }

        // Apply the modification
        float currentValue = (float)targetNode.Get(powerup.TargetProperty);
        float newValue = currentValue;

        switch (powerup.ModificationType)
        {
            case "Add":
                newValue = currentValue + powerup.ModificationValue;
                break;
            case "Multiply":
                newValue = currentValue * powerup.ModificationValue;
                break;
            case "Set":
                newValue = powerup.ModificationValue;
                break;
        }

        // Apply the new value
        targetNode.Set(powerup.TargetProperty, newValue);

        GD.Print($"Applied powerup {powerup.Name}: {powerup.TargetProperty} is now {newValue}");

        // Track applied powerup
        _appliedPowerups.Add(powerup);

        // Emit signal
        EmitSignal(SignalName.PowerupApplied, powerup);
    }

    // Generate random powerups for the upgrade screen
    public PowerupResource[] GetRandomPowerups(int count)
    {
        if (AvailablePowerups == null || AvailablePowerups.Length == 0)
            return new PowerupResource[0];

        List<PowerupResource> result = new List<PowerupResource>();

        // Simple random selection without duplicates
        var availableList = new List<PowerupResource>(AvailablePowerups);
        int toSelect = Mathf.Min(count, availableList.Count);

        for (int i = 0; i < toSelect; i++)
        {
            int index = GD.RandRange(0, availableList.Count - 1);
            result.Add(availableList[index]);
            availableList.RemoveAt(index);
        }

        return result.ToArray();
    }
}
