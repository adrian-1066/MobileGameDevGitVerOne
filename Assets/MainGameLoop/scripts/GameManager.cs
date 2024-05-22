using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public InnitBoardSetUp GridSet;
    public int avgFrameRate;
    public List<int> highScores = new List<int>();
    public int highestLevelReached;
    SaveLoadManager saveLoadManager;
    GameData dataToSave;


    private void Awake()
    {
        saveLoadManager = GetComponent<SaveLoadManager>();
    }

    private void OnApplicationQuit()
    {
        Debug.Log("gaming quitting");
        SaveTheGame();
    }

    private void Start()
    {
        /*dataToSave = new GameData
        {
            HighestLevel = 0,
            LevelHighScores = highScores
        };

        saveLoadManager.SaveGame(dataToSave);
        */
        GameData loadedData = saveLoadManager.LoadGame();
        highScores.AddRange(loadedData.LevelHighScores);
        foreach(float value in loadedData.LevelHighScores)
        {
            Debug.Log(value);
        }
        Debug.Log(loadedData.HighestLevel);

    }
    public void SetSeedForGrid(int seed)
    {
        GridSet.Seed = seed;
    }

    private void SaveTheGame()
    {
        dataToSave = new GameData
        {
            HighestLevel = highestLevelReached,
            LevelHighScores = highScores
        };

        saveLoadManager.SaveGame(dataToSave);
    }

   

    private void Update()
    {
        float current = 0;
        current = Time.frameCount / Time.time;
        avgFrameRate = (int)current;
        //Debug.Log("the avg framerate is: " +  avgFrameRate);
    }
}
