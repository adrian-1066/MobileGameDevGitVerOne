using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using GoogleMobileAds.Api;
using System;
using UnityEngine.Audio;
using UnityEngine.UI;

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

    public int AmountOfLife;
    public int MaxAmountOfLife;
    private DateTime dateTimeOnLogOn;
    private DateTime timeOfLastAd;

    public AudioManager audioManager;
    public AudioMixer audioMixer;
    
    private float Volume;
    private bool IsMuted;
    public Slider VolSlider;
    public Toggle muteToggle;

    public Slider InGameVolSlider;
    public Toggle InGameMuteToggle;

    //#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-3940256099942544/1033173712";
//#endif

    private InterstitialAd _interstitialAd;
    private void Awake()
    {
        saveLoadManager = GetComponent<SaveLoadManager>();
    }

    private void OnEnable()
    {
        
    }


    private void Start()
    {
        MobileAds.Initialize(initStatus => { });


        
        GameData loadedData = saveLoadManager.LoadGame();
        highScores.AddRange(loadedData.LevelHighScores);
        highestLevelReached = loadedData.HighestLevel;
        curFreeMoney = loadedData.FreeMoney;
        curPreMoney = loadedData.PreMoney;
        levelMulti = loadedData.MultiLevel;
        AmountOfAbility1 = loadedData.NumOfAbilityOne;
        AmountOfAbility2 = loadedData.NumOfAbilityTwo;
        AmountOfLife = loadedData.NumOfLifes;
        Volume = loadedData.Volume;
        IsMuted = loadedData.IsMuted;


        audioMixer.SetFloat("MasterVolume", Volume);
        if(IsMuted)
        {
            audioMixer.SetFloat("MasterVolume", -80.0f);
        }

        VolSlider.value = Volume;
        muteToggle.isOn = IsMuted;

        InGameVolSlider.value = Volume;
        InGameMuteToggle.isOn = IsMuted;
        //timeOfLastAd = loadedData.TimeSinceLastAd;
        //dateTimeOnLogOn = loadedData.dateTimeAtLogOff;

        if (DateTime.TryParse(loadedData.dateTimeAtLogOffData, out dateTimeOnLogOn))
        {
            Debug.Log("got the previous time, it is: " + dateTimeOnLogOn);
        }
        else
        {
            Debug.Log("failed to get date time");
        }

        if (DateTime.TryParse(loadedData.TimeSinceLastAd, out timeOfLastAd))
        {
            Debug.Log("got the previous ad time, it is: " + timeOfLastAd);
        }
        else
        {
            Debug.Log("failed to get ad date time");
        }

        if (AmountOfLife != MaxAmountOfLife)
        {
            CalculateTimeDiff();
        }
        
        //highestLevelReached = highScores.Count;
        foreach(float value in loadedData.LevelHighScores)
        {
            Debug.Log(value);
        }
        Debug.Log(loadedData.HighestLevel);

        UpdateNums();
        
        LoadInterstitialAd();
        ShowInterstitialAd();

        audioManager.Play("BgMusic");
        
    }

    public void SetVolumeMute(bool State)
    {
        IsMuted = State;

        if(IsMuted)
        {
            audioMixer.SetFloat("MasterVolume", -80.0f);
        }
        else
        {
            audioMixer.SetFloat("MasterVolume", Volume);
        }
        InGameMuteToggle.isOn = IsMuted;
        muteToggle.isOn = IsMuted;
    }

    public void LoseLife()
    {
        AmountOfLife--;
        UpdateNums();
    }

    public void SetVolume(float volume)
    {
        Volume = volume;
        audioMixer.SetFloat("MasterVolume", Volume);
        
        InGameVolSlider.value = Volume;
       VolSlider.value = Volume;    
    }
    
    public void LoadInterstitialAd()
    {
        if(_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad");

        var adRequest = new AdRequest();

        InterstitialAd.Load(_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if(error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                 "with error : " + error);
                    return;
                }
                Debug.Log("Interstitial ad loaded with response : "
                        + ad.GetResponseInfo());

                _interstitialAd = ad;
            }
            );
    }

    public void ShowInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
            timeOfLastAd = DateTime.Now;
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }
    

    public void CheckIfTimeToPlayAd()
    {
        float timeSinceLastAd = CalculateTimeDifferenceInSeconds(timeOfLastAd, DateTime.Now);
        if (timeSinceLastAd >= 6) 
        {
            LoadInterstitialAd();
            ShowInterstitialAd();
        }
    }

    private void CalculateTimeDiff()
    {
        float timeDiff = CalculateTimeDifferenceInSeconds(dateTimeOnLogOn, DateTime.Now);
        int lifeTillMax = MaxAmountOfLife - AmountOfLife;
        int timeTillHealed = lifeTillMax * 300;
        
        if(timeDiff >= timeTillHealed)
        {
            AmountOfLife = MaxAmountOfLife;
        }
        else
        {
            int healthGain = Mathf.FloorToInt(timeDiff /300.0f);
            AmountOfLife += healthGain;
            if(AmountOfLife >= MaxAmountOfLife)
            {
                AmountOfLife = MaxAmountOfLife;
            }
            else if(AmountOfLife < MaxAmountOfLife)
            {
                StartCoroutine(beginHealthRegen());
            }
        }
    }

    float CalculateTimeDifferenceInSeconds(DateTime start, DateTime end)
    {
        // Calculate the time span between the two dates
        TimeSpan timeSpan = end - start;

        // Return the total seconds
        return ((float)(timeSpan.TotalSeconds));
    }

    public void UpdateNums()
    {
        FreeMunText.text = curFreeMoney.ToString();
        PreMunText.text = curPreMoney.ToString();
        LifesText.text = AmountOfLife.ToString();
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
            NumOfAbilityTwo = AmountOfAbility2,
            dateTimeAtLogOffData = System.DateTime.Now.ToString("o"),
            //dateTimeAtLogOff = DateTime.Now,
            NumOfLifes = AmountOfLife,
            IsMuted = IsMuted,
            Volume = Volume
            
           
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

    private IEnumerator beginHealthRegen()
    {
        yield return new WaitForSeconds(300);
        AmountOfLife++;
        if(AmountOfLife > MaxAmountOfLife)
        {
            AmountOfLife = MaxAmountOfLife;
        }
        else if(AmountOfLife < MaxAmountOfLife)
        {
            StartCoroutine(beginHealthRegen());
        }
    }

    public void TriggerHealthRegen()
    {
        StartCoroutine(beginHealthRegen());
    }

    public void PlayButtonSound()
    {
        audioManager.Play("ButtonPressed");
    }

    public void BuyPreCur(int val)
    {
        curPreMoney += val;
        UpdateNums();
    }
    private void OnApplicationQuit()
    {
        Debug.Log("gaming quitting");
        SaveTheGame();
    }
}
