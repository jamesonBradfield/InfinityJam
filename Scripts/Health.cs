using Godot;
using GodotTools;
[GlobalClass]
public partial class Health : Node
{
    [Export] public Node3D healthBar;
    [Export] Control uiHealthBar;
    [Export] float MaxHealth;
    [Export] float CurrentHealth;
    [Export] float healthBarDisplayTime;
    float currentTime = 0f;
    bool timerOn;
    [Signal]
    public delegate void HealthChangedSignalEventHandler(float currentHealth, float maxHealth);
    [Signal]
    public delegate void MaxHealthChangedSignalEventHandler(float currentHealth, float maxHealth);
    [Signal]
    public delegate void DamageSignalEventHandler(float damage);
    [Signal]
    public delegate void HealSignalEventHandler(float healValue);
    [Signal]
    public delegate void DeathSignalEventHandler();

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
        if (healthBar != null)
        {
            healthBar.Set("max_value", MaxHealth);
            healthBar.Set("value", CurrentHealth);
            healthBar.VisibilityChanged += StartVisibilityTimer;
        }
        if (uiHealthBar != null)
        {
            uiHealthBar.Set("max_value", MaxHealth);
            uiHealthBar.Set("value", CurrentHealth);
        }
    }
    public override void _Process(double delta)
    {
        if (timerOn)
            currentTime += (float)delta;
        if (currentTime >= healthBarDisplayTime && healthBar != null)
        {
            healthBar.Hide();
            timerOn = false;
        }

    }
    public void TakeDamage(float damage)
    {
        GodotLogger.Info($"damage {damage}");
        if (healthBar != null)
            healthBar.Show();
        CurrentHealth -= damage;

        if (healthBar != null)
            healthBar.Set("value", CurrentHealth);

        if (uiHealthBar != null)
            uiHealthBar.Set("progress", CurrentHealth);
        EmitSignal(SignalName.HealthChangedSignal, CurrentHealth, MaxHealth);
        EmitSignal(SignalName.DamageSignal, damage);
        if (CurrentHealth <= 0)
        {
            EmitSignal(SignalName.DeathSignal);
        }
    }

    public void Heal(float healthToAdd)
    {
        if (healthBar != null)
            healthBar.Show();
        CurrentHealth += healthToAdd;

        if (healthBar != null)
            healthBar.Set("value", CurrentHealth);

        if (uiHealthBar != null)
            uiHealthBar.Set("value", CurrentHealth);
        EmitSignal(SignalName.HealSignal, healthToAdd);
        EmitSignal(SignalName.HealthChangedSignal, CurrentHealth, MaxHealth);
    }

    public void IncreaseMaxHealth(float value)
    {
        if (healthBar != null)
            healthBar.Show();
        MaxHealth += value;
        if (healthBar != null)
            healthBar.Set("max_value", MaxHealth);

        if (uiHealthBar != null)
            uiHealthBar.Set("max_value", MaxHealth);
        EmitSignal(SignalName.HealthChangedSignal, CurrentHealth, MaxHealth);
        EmitSignal(SignalName.MaxHealthChangedSignal, CurrentHealth, MaxHealth);
    }

    public void DecreaseMaxHealth(float value)
    {
        if (healthBar != null)
            healthBar.Show();
        MaxHealth -= value;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        if (healthBar != null)
        {
            healthBar.Set("max_value", MaxHealth);
            healthBar.Set("value", CurrentHealth);
        }

        if (uiHealthBar != null)
        {
            uiHealthBar.Set("max_value", MaxHealth);
            uiHealthBar.Set("value", CurrentHealth);
        }
        EmitSignal(SignalName.HealthChangedSignal, CurrentHealth, MaxHealth);
        EmitSignal(SignalName.MaxHealthChangedSignal, CurrentHealth, MaxHealth);
        if (CurrentHealth <= 0)
        {
            EmitSignal(SignalName.DeathSignal);
        }
    }
    private void StartVisibilityTimer()
    {
        if (healthBar.Visible)
        {
            timerOn = true;
            currentTime = 0f;
        }
    }
}
