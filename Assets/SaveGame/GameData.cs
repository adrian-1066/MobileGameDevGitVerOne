using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public int HighestLevel;
    public int MultiLevel;
    public int FreeMoney;
    public int PreMoney;
    public int NumOfAbilityOne;
    public int NumOfAbilityTwo;
    public List<int> LevelHighScores;
    public GameData()
    {
        LevelHighScores = new List<int>();
        
    }
}
