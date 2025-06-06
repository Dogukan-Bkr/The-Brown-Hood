﻿using GoogleMobileAds.Api;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public GameObject blacksmithPanel;
    public GameObject tentPanel;
    public GameObject pausePanel;

    public TMP_Text coinTxt;
    public TMP_Text arrowTxt;
    public TMP_Text spearTxt;
    public TMP_Text adStatusTxt;
    public Slider healthSlider;

    public Button buySpearButton;
    public Button buyArrowButton;
    public Button exchangeSpearForCoinButton;

    public Button adSpearButton;
    public Button adArrowButton;
    public Button adCoinButton;

    public Button closeButtonBlackSmith;
    public Button closeButtonTent;
    public Button buyHealthButton;
    public Button adHealthButton;

    private RewardedAd rewardedAdSpear;
    private RewardedAd rewardedAdArrow;
    private RewardedAd rewardedAdCoin;
    private RewardedAd rewardedAdHealth;

    private AdmobRewardedIDs adIds;
    private string testId;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ConfigData config = ConfigLoader.LoadConfig();

        if (config == null || config.admob == null)
        {
            Debug.LogError("AdMob konfigürasyonu yüklenemedi!");
            return;
        }

        adIds = config.admob.rewarded;
        testId = config.admob.testId;

        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("AdMob SDK initialized.");
            var adapterStatusMap = initStatus.getAdapterStatusMap();
            foreach (var adapter in adapterStatusMap)
            {
                Debug.Log($"Adapter: {adapter.Key}, Status: {adapter.Value.InitializationState}");
            }

            if (adapterStatusMap.Values.All(status => status.InitializationState == AdapterState.Ready))
            {
                Debug.Log("AdMob SDK initialized successfully.");
            }
            else
            {
                Debug.LogError("AdMob SDK initialization failed for some adapters.");
            }
        });

        AssignButtonListeners();
        RequestRewardedAds();
    }

    private void AssignButtonListeners()
    {
        buySpearButton?.onClick.AddListener(BuySpear);
        buyArrowButton?.onClick.AddListener(BuyArrow);
        exchangeSpearForCoinButton?.onClick.AddListener(ExchangeSpearForCoin);

        adSpearButton?.onClick.AddListener(GetSpearByAd);
        adArrowButton?.onClick.AddListener(GetArrowByAd);
        adCoinButton?.onClick.AddListener(GetCoinByAd);

        closeButtonBlackSmith?.onClick.AddListener(CloseBlacksmithPanel);
        closeButtonTent?.onClick.AddListener(CloseTentPanel);
        buyHealthButton?.onClick.AddListener(BuyHealth);
        adHealthButton?.onClick.AddListener(GetHealthByAd);
    }

    private void RequestRewardedAds()
    {
        // Gerçek ID'leri yayınlandığında aktif hale getir:
        // LoadRewardedAd(adIds?.spear, ad => rewardedAdSpear = ad);
        // LoadRewardedAd(adIds?.arrow, ad => rewardedAdArrow = ad);
        // LoadRewardedAd(adIds?.coin, ad => rewardedAdCoin = ad);
        // LoadRewardedAd(adIds?.health, ad => rewardedAdHealth = ad);

        // Test ID'ler aktif:
        LoadRewardedAd(testId, ad => rewardedAdSpear = ad);
        LoadRewardedAd(testId, ad => rewardedAdArrow = ad);
        LoadRewardedAd(testId, ad => rewardedAdCoin = ad);
        LoadRewardedAd(testId, ad => rewardedAdHealth = ad);
    }

    private void LoadRewardedAd(string adUnitId, Action<RewardedAd> callback)
    {
        AdRequest request = new AdRequest();
        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError($"Ad Load Failed: {error?.GetMessage()}");
                return;
            }

            ad.OnAdFullScreenContentClosed += () => LoadRewardedAd(adUnitId, callback);
            callback.Invoke(ad);
        });
    }

    public void GetHealthByAd()
    {
        ShowRewardedAd(rewardedAdHealth, () =>
        {
            if (PlayerHealthController.instance.currentHP < PlayerHealthController.instance.maxHP)
            {
                PlayerHealthController.instance.currentHP += 50;
                if (PlayerHealthController.instance.currentHP > PlayerHealthController.instance.maxHP)
                {
                    PlayerHealthController.instance.currentHP = PlayerHealthController.instance.maxHP;
                }
                SetHealthSlider(PlayerHealthController.instance.currentHP, PlayerHealthController.instance.maxHP);
            }
        });
    }

    public void GetSpearByAd()
    {
        ShowRewardedAd(rewardedAdSpear, () => GameManager.instance.spearCount += 3);
    }

    public void GetArrowByAd()
    {
        ShowRewardedAd(rewardedAdArrow, () => GameManager.instance.arrowCount += 5);
    }

    public void GetCoinByAd()
    {
        ShowRewardedAd(rewardedAdCoin, () => GameManager.instance.coinCount += 25);
    }

    void ShowRewardedAd(RewardedAd ad, Action rewardAction)
    {
        if (ad != null && ad.CanShowAd())
        {
            ad.Show((Reward reward) =>
            {
                rewardAction.Invoke();
                UpdateUI();
            });
        }
        else
        {
            Debug.LogError("Rewarded ad not ready.");
            if (adStatusTxt != null)
            {
                adStatusTxt.text = "Reklam yüklenmedi, lütfen daha sonra tekrar deneyin.";
            }
        }
    }

    public void UpdateUI()
    {
        coinTxt.text = GameManager.instance.coinCount.ToString();
        arrowTxt.text = GameManager.instance.arrowCount.ToString();
        spearTxt.text = GameManager.instance.spearCount.ToString();
    }

    public void SetHealthSlider(int currentHP, int maxHP)
    {
        healthSlider.maxValue = maxHP;
        healthSlider.value = currentHP;
    }

    public void DecreaseArrowCount()
    {
        GameManager.instance.arrowCount--;
        UpdateUI();
    }

    public void DecreaseSpearCount()
    {
        GameManager.instance.spearCount--;
        UpdateUI();
    }

    public void OpenBlacksmithPanel()
    {
        blacksmithPanel.SetActive(true);
        PlayerMovementController.instance.StopPlayer();
    }

    public void OpenPausePanel()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ClosePausePanel()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void Replay()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void CloseBlacksmithPanel()
    {
        if (blacksmithPanel.activeSelf) blacksmithPanel.SetActive(false);
        PlayerMovementController.instance.ResumeMovement();
    }

    public void OpenTentPanel()
    {
        tentPanel.SetActive(true);
        PlayerMovementController.instance.StopPlayer();
    }

    public void CloseTentPanel()
    {
        tentPanel.SetActive(false);
        PlayerMovementController.instance.ResumeMovement();
    }

    public void BuySpear()
    {
        if (GameManager.instance.coinCount >= 30)
        {
            GameManager.instance.coinCount -= 30;
            GameManager.instance.spearCount += 2;
            UpdateUI();
        }
    }

    public void BuyArrow()
    {
        if (GameManager.instance.coinCount >= 16)
        {
            GameManager.instance.coinCount -= 16;
            GameManager.instance.arrowCount += 2;
            UpdateUI();
        }
    }

    public void ExchangeSpearForCoin()
    {
        if (GameManager.instance.spearCount > 0)
        {
            GameManager.instance.spearCount--;
            GameManager.instance.coinCount += 10;
            UpdateUI();
        }
    }

    public void BuyHealth()
    {
        if (PlayerHealthController.instance.currentHP >= PlayerHealthController.instance.maxHP)
        {
            Debug.Log("Sağlık zaten maksimumda, satın alma işlemi yapılamaz.");
            return;
        }

        if (GameManager.instance.coinCount >= 30)
        {
            GameManager.instance.coinCount -= 30;

            int healthToAdd = Mathf.CeilToInt(PlayerHealthController.instance.maxHP * 0.2f);
            PlayerHealthController.instance.currentHP += healthToAdd;

            if (PlayerHealthController.instance.currentHP > PlayerHealthController.instance.maxHP)
            {
                PlayerHealthController.instance.currentHP = PlayerHealthController.instance.maxHP;
            }

            SetHealthSlider(PlayerHealthController.instance.currentHP, PlayerHealthController.instance.maxHP);
            UpdateUI();
        }
        else
        {
            Debug.Log("Yeterli miktarda coin yok.");
        }
    }
}
