using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class LevelStatistics : MonoBehaviour
{

    private List<int> secretsFound = new();
    public List<int> SecretsFound {get {return secretsFound;} }

    private int secretsFoundTotal = 0;
    private int secretsMaxTotal = 0;
    public int SecretsFoundTotal {get {return secretsFoundTotal;}}
    private int enemiesKilled = 0;
    public int EnemiesKilled {get {return enemiesKilled; } set { enemiesKilled = value;} }
    private int enemiesKilledTotal = 0;
    private int enemiesMaxTotal = 0;

    public int EnemiesKilledTotal { get { return enemiesKilledTotal; } }

    private int scoreGained = 0;
    public int ScoreGained {get {return scoreGained;} set {scoreGained = value;} }
    private int scoreGainedTotal = 0;
    private int scoreMaxTotal = 0;
    public int ScoreGainedTotal { get { return scoreGainedTotal; } }

    private int loreFound = 0;
    public int LoreFound { get { return loreFound; } set { loreFound = value; } }

    private int loreFoundTotal = 0;
    public int LoreFoundTotal {get {return loreFoundTotal;}}
    private int loreMaxtotal = 0;


    private List<LevelStats> allStats = new();
    public List<LevelStats> AllStats { get {return allStats;} }
    public LevelStats CalculateCurrentLevelStats() {
        var maxSecrets = MapGenerator.main.GetMaxSecrets();
        var maxEnemies = MapGenerator.main.GetMaxEnemies();
        var maxScore = MapGenerator.main.GetMaxScore();
        var maxLore = MapGenerator.main.GetMaxLore();
        var maxHealth = 100;
        int ts = (int)LevelManager.main.GetStopwatch().ElapsedMilliseconds;

        var singleStats = new List<SingleLevelStat> {
            new SingleLevelStat("Secrets", maxSecrets, secretsFound.Count),
            new SingleLevelStat("Enemies", maxEnemies, enemiesKilled),
            new SingleLevelStat("Score", maxScore, scoreGained),
            new SingleLevelStat("Health", maxHealth, MapGenerator.main.Player.Health),
            new SingleLevelStat("Lore", maxLore, loreFound),
            new SingleLevelStat("Total time", 999999, ts)
        };
        LevelStats levelStats  = new LevelStats(singleStats);
        allStats.Add(levelStats);

        secretsMaxTotal += maxSecrets;
        enemiesMaxTotal += maxEnemies;
        scoreMaxTotal += maxScore;
        loreMaxtotal += maxLore;

        secretsFoundTotal += secretsFound.Count;
        enemiesKilledTotal += enemiesKilled;
        scoreGainedTotal += scoreGained;
        loreFoundTotal += loreFound;
        ResetCurrentStats();
        return levelStats;
    }

    public List<SingleLevelStat> EndStats() {
        CalculateCurrentLevelStats();
        int ts = (int)LevelManager.main.GetStopwatch().ElapsedMilliseconds;
        var singleStats = new List<SingleLevelStat> {
            new SingleLevelStat("Secrets", secretsMaxTotal, secretsFoundTotal),
            new SingleLevelStat("Enemies", enemiesMaxTotal, enemiesKilledTotal),
            new SingleLevelStat("Score", scoreMaxTotal, scoreGainedTotal),
            new SingleLevelStat("Health", 100, MapGenerator.main.Player.Health),
            new SingleLevelStat("Lore", loreMaxtotal, loreFoundTotal),
            new SingleLevelStat("Total time", 999999, ts)
        };
        return singleStats;
    }

    public void ResetCurrentStats() {
        secretsFound = new();
        enemiesKilled = 0;
        scoreGained = 0;
        loreFound = 0;
    }
}

public class LevelStats {
    private List<SingleLevelStat> stats;
    public List<SingleLevelStat> Stats {get {return stats;}}
    public LevelStats(List<SingleLevelStat> stats) {
        this.stats = new(stats);
    }
}

public class SingleLevelStat {
    public string Name;
    public int Max;
    public int Value;
    public SingleLevelStat(string name, int max, int value) {
        this.Name = name;
        this.Max = max;
        this.Value = value;
    }
}