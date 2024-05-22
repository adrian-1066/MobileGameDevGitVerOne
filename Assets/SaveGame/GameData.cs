using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public int HighestLevel;
    public List<int> LevelHighScores;
    public GameData()
    {
        LevelHighScores = new List<int>();
        HighestLevel = 0;
    }
}
