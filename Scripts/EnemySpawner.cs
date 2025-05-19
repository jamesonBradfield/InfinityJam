using Godot;
using System.Collections.Generic;
using GodotTools;
public partial class EnemySpawner : Node
{
    // want to convert this to a resource where we have access to all relevant data.
    [Export] public PackedScene[] enemies;
    public List<PackedScene> enemiesToSpawnOnRoundStart = new();
    [Export] public int[] points;
    [Export] public Node3D[] spawnPoints;
    [Export] public WaveManager waveManager;
    public override void _Ready()
    {
        waveManager.Initialize += BuyForRound;
        waveManager.Start += SpawnEnemies;
    }

    private void BuyForRound(float budget)
    {
        GodotLogger.Info($"budget {budget}");
        enemiesToSpawnOnRoundStart.Clear();
        while (budget > 0)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (budget - points[i] >= 0)
                {
                    enemiesToSpawnOnRoundStart.Add(enemies[i]);
                    GodotLogger.Info($"bought {enemies[i]}");
                    budget -= points[i];
                }
            }
        }
    }

    private void SpawnEnemies()
    {
        float padding = 2.0f;
        int MaxXCount = 4;
        int MaxZCount = 4;

        int totalSpawnPoints = spawnPoints.Length;
        int totalEnemies = enemiesToSpawnOnRoundStart.Count;
        for (int spawnIndex = 0; spawnIndex < totalSpawnPoints; spawnIndex++)
        {
            int startEnemyIndex = (totalEnemies * spawnIndex) / totalSpawnPoints;
            int endEnemyIndex = (totalEnemies * (spawnIndex + 1)) / totalSpawnPoints;
            int enemiesForThisSpawn = endEnemyIndex - startEnemyIndex;

            if (enemiesForThisSpawn <= 0)
                continue;

            Node3D spawnPoint = spawnPoints[spawnIndex];
            for (int i = 0; i < enemiesForThisSpawn; i++)
            {
                int localIndex = i; // Index relative to this spawn point
                int xIndex = localIndex % MaxXCount;
                int zIndex = localIndex / MaxXCount;

                // Skip if we exceed the grid capacity for this spawn point
                if (zIndex >= MaxZCount)
                    break;

                // Get the enemy from the overall list
                int globalEnemyIndex = startEnemyIndex + i;
                if (globalEnemyIndex >= totalEnemies)
                    break;

                CharacterBody3D enemyInScene = enemiesToSpawnOnRoundStart[globalEnemyIndex].Instantiate() as CharacterBody3D;
                enemyInScene.Position = new Vector3(
                    spawnPoint.Position.X + (xIndex * padding),
                    spawnPoint.Position.Y,
                    spawnPoint.Position.Z + (zIndex * padding)
                );
                AddChild(enemyInScene);
                GodotLogger.Info($"Spawning enemy at position {enemyInScene.Position} (spawn point {spawnIndex})");
            }
        }
    }
}
