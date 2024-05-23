using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private string filePath;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "gamedata.json");
    }
    

    public void SaveGame(GameData data)
    {
        //data.dateTimeAtLogOffData = System.DateTime.Now.ToString("o");
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);
    }

    public GameData LoadGame()
    {
        if(File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        return NewFileData();
    }

    GameData NewFileData()
    {
        GameData newData;
        newData = new GameData 
        {
            NumOfAbilityOne = 5, NumOfAbilityTwo = 5, FreeMoney = 1000, NumOfLifes = 5, PreMoney = 100, MultiLevel = 10
        
        };

        return newData;
    }
}
