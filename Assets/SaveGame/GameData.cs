using System;
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
    public int NumOfLifes;
    //public DateTime dateTimeAtLogOff;
    public string dateTimeAtLogOffData;
    public string TimeSinceLastAd;
    public List<int> LevelHighScores;
    public GameData()
    {
        LevelHighScores = new List<int>();
        dateTimeAtLogOffData = System.DateTime.Now.ToString("o");
        TimeSinceLastAd = System.DateTime.Now.ToString("o");
       
        
    }
}
