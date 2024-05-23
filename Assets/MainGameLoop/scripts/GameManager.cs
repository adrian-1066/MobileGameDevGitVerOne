using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;

public class GameManager : MonoBehaviour
{
    public InnitBoardSetUp GridSet;
    public int avgFrameRate;
    public List<int> highScores = new List<int>();
    public int highestLevelReached;

    public int curFreeMoney;
    public int curPreMoney;
    public int levelMulti;
    SaveLoadManager saveLoadManager;
    GameData dataToSave;
    public WheelSpinAndSetUp wheelSetUp;


    public TMP_Text upgradeCostText;
    public TMP_Text currentLevel;
    private int upgradeCost;

    public int NumOfAbilityOne;
    public int NumOfAbilityTwo;

    public TMP_Text LifesText;
    public TMP_Text FreeMunText;
    public TMP_Text PreMunText;

    public int Ability1FreeCost;
    public int Ability2FreeCost;

    public int Ability1PreCost;
    public int Ability2PreCost;

    public int AmountOfAbility1;
    public int AmountOfAbility2;
    private void Awake()
    {
        saveLoadManager = GetComponent<SaveLoadManager>();
    }

    private void OnEnable()
    {
        
    }


    private void Start()
    {


        GameData loadedData = saveLoadManager.LoadGame();
        highScores.AddRange(loadedData.LevelHighScores);
        highestLevelReached = loadedData.HighestLevel;
        curFreeMoney = loadedData.FreeMoney;
        curPreMoney = loadedData.PreMoney;
        levelMulti = loadedData.MultiLevel;
        AmountOfAbility1 = loadedData.NumOfAbilityOne;
        AmountOfAbility2 = loadedData.NumOfAbilityTwo;
        //highestLevelReached = highScores.Count;
        foreach(float value in loadedData.LevelHighScores)
        {
            Debug.Log(value);
        }
        Debug.Log(loadedData.HighestLevel);

        UpdateNums();

    }

    public void UpdateNums()
    {
        FreeMunText.text = curFreeMoney.ToString();
        PreMunText.text = curPreMoney.ToString();
    }

    public void SetUpUpgradeShop()
    {
        upgradeCost = levelMulti * 100;
        upgradeCostText.text = upgradeCost.ToString();
        currentLevel.text = levelMulti.ToString();
    }


    public void BuyAbility1(int choice)
    {
        //1 is free 2 is pre
        if (choice == 1)
        {
            if(curFreeMoney >= Ability1FreeCost)
            {
                AmountOfAbility1 += 1;
                curFreeMoney -= Ability1FreeCost;
                UpdateNums();
            }

        }
        else if (choice == 2)
        {
            if(curPreMoney >= Ability1PreCost)
            {
                AmountOfAbility1 += 1;
                curPreMoney -= Ability1PreCost;
                UpdateNums();
            }
        }
    }

    public void BuyAbility2(int choice)
    {
        //1 is free 2 is pre
        if (choice == 1)
        {
            if (curFreeMoney >= Ability2FreeCost)
            {
                AmountOfAbility2 += 1;
                curFreeMoney -= Ability2FreeCost;
                UpdateNums();
            }

        }
        else if (choice == 2)
        {
            if (curPreMoney >= Ability2PreCost)
            {
                AmountOfAbility2 += 1;
                curPreMoney -= Ability2PreCost;
                UpdateNums();
            }
        }
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
            LevelHighScores = highScores,
            FreeMoney = curFreeMoney,
            PreMoney = curPreMoney,
            MultiLevel = levelMulti,
            NumOfAbilityOne = AmountOfAbility1,
            NumOfAbilityTwo = AmountOfAbility2
           
        };

        saveLoadManager.SaveGame(dataToSave);
    }

   
    public void SetWheelToCurrentLevel(int level)
    {
        Debug.Log("setting wheel from game manager");
        wheelSetUp.SetWheelToLevel(level);
    }

    public void IncreaseMultiplier()
    {
        if (curFreeMoney >= upgradeCost)
        {
            levelMulti++;
            upgradeCostText.text = (levelMulti * 100).ToString();
            curFreeMoney -= upgradeCost;
            currentLevel.text = levelMulti.ToString();
            UpdateNums();
        }
        else
        {
            //indicate no
        }
    }
    private void Update()
    {
        float current = 0;
        current = Time.frameCount / Time.time;
        avgFrameRate = (int)current;
        //Debug.Log("the avg framerate is: " +  avgFrameRate);
    }

    private void OnApplicationQuit()
    {
        Debug.Log("gaming quitting");
        SaveTheGame();
    }
}
