using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class LevelStatistics : MonoBehaviour
{

    private List<int> secretsFound = new();
    public List<int> SecretsFound {get {return secretsFound;} }
    private int enemiesKilled = 0;
    public int EnemiesKilled {get {return enemiesKilled; } set { enemiesKilled = value;} }
    private int scoreGained = 0;
    public int ScoreGained {get {return scoreGained;} set {scoreGained = value;} }

    private int loreFound = 0;
    public int LoreFound { get { return loreFound; } set { loreFound = value; } }


    private List<LevelStats> allStats = new();
    public List<LevelStats> AllStats { get {return allStats;} }
    public LevelStats CalculateCurrentLevelStats() {
        var maxSecrets = MapGenerator.main.GetMaxSecrets();
        var maxEnemies = MapGenerator.main.GetMaxEnemies();
        var maxScore = MapGenerator.main.GetMaxScore();
        var maxLore = MapGenerator.main.GetMaxLore();
        var maxHealth = 100;
        var singleStats = new List<SingleLevelStat> {
            new SingleLevelStat("Secrets", maxSecrets, secretsFound.Count),
            new SingleLevelStat("Enemies", maxEnemies, enemiesKilled),
            new SingleLevelStat("Score", maxScore, scoreGained),
            new SingleLevelStat("Health", maxHealth, MapGenerator.main.Player.Health),
            new SingleLevelStat("Lore", maxLore, loreFound)
        };
        LevelStats levelStats  = new LevelStats(singleStats);
        allStats.Add(levelStats);
        ResetCurrentStats();
        return levelStats;
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