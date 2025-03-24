using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using System;

public class UIController : MonoBehaviour
{
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
      private string _adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
      private string _adUnitId = "unused";
#endif
    public static UIController instance;
    public GameObject blacksmithPanel;
    public Button buySpearButton, buyArrowButton;
    public Button exchangeSpearForCoinButton; //exchangeArrowForCoinButton;
    public Button adSpearButton, adArrowButton, adCoinButton;
    public Button closeButton;
    public Slider healthSlider;

    public TMP_Text coinTxt;
    public TMP_Text arrowTxt;
    public TMP_Text spearTxt;
    public TMP_Text adStatusTxt; // Reklam durumu mesajı için

    private RewardedAd rewardedAdSpear;
    private RewardedAd rewardedAdArrow;
    private RewardedAd rewardedAdCoin;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Google Ads SDK başlat
        MobileAds.Initialize(initStatus => { });

        // Butonlara tıklama event'leri bağla
        if (buySpearButton != null)
        {
            buySpearButton.onClick.AddListener(BuySpear);
        }
        else
        {
            Debug.LogError("buySpearButton is not assigned in the inspector.");
        }

        if (buyArrowButton != null)
        {
            buyArrowButton.onClick.AddListener(BuyArrow);
        }
        else
        {
            Debug.LogError("buyArrowButton is not assigned in the inspector.");
        }

        if (exchangeSpearForCoinButton != null)
        {
            exchangeSpearForCoinButton.onClick.AddListener(ExchangeSpearForCoin);
        }
        else
        {
            Debug.LogError("exchangeSpearForCoinButton is not assigned in the inspector.");
        }

        //if (exchangeArrowForCoinButton != null)
        //{
        //    exchangeArrowForCoinButton.onClick.AddListener(ExchangeArrowForCoin);
        //}
        //else
        //{
        //    Debug.LogError("exchangeArrowForCoinButton is not assigned in the inspector.");
        //}

        if (adSpearButton != null)
        {
            adSpearButton.onClick.AddListener(GetSpearByAd);
        }
        else
        {
            Debug.LogError("adSpearButton is not assigned in the inspector.");
        }

        if (adArrowButton != null)
        {
            adArrowButton.onClick.AddListener(GetArrowByAd);
        }
        else
        {
            Debug.LogError("adArrowButton is not assigned in the inspector.");
        }

        if (adCoinButton != null)
        {
            adCoinButton.onClick.AddListener(GetCoinByAd);
        }
        else
        {
            Debug.LogError("adCoinButton is not assigned in the inspector.");
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePanel);
        }
        else
        {
            Debug.LogError("closeButton is not assigned in the inspector.");
        }

        // Reklamları yükle
        RequestRewardedAds();
    }

    private void RequestRewardedAds()
    {
        LoadRewardedAd("ca-app-pub-XXXXXXXXXXXXXXXX/XXXXXXXXXX", ad => rewardedAdSpear = ad);
        LoadRewardedAd("ca-app-pub-XXXXXXXXXXXXXXXX/XXXXXXXXXX", ad => rewardedAdArrow = ad);
        LoadRewardedAd("ca-app-pub-XXXXXXXXXXXXXXXX/XXXXXXXXXX", ad => rewardedAdCoin = ad);
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

    public void OpenBlacksmithPanel()
    {
        blacksmithPanel.SetActive(true);
        PlayerMovementController.instance.StopPlayer(); // Karakteri durdur
    }

    public void ClosePanel()
    {
        blacksmithPanel.SetActive(false);
        PlayerMovementController.instance.ResumeMovement(); // Karakteri tekrar hareket ettir
    }

    public void BuySpear()
    {
        Debug.Log("BuySpear called");
        if (GameManager.instance.coinCount >= 10)
        {
            GameManager.instance.coinCount -= 10;
            GameManager.instance.spearCount++;
            UpdateUI();
        }
    }

    public void BuyArrow()
    {
        Debug.Log("BuyArrow called");
        if (GameManager.instance.coinCount >= 10)
        {
            GameManager.instance.coinCount -= 10;
            GameManager.instance.arrowCount++;
            UpdateUI();
        }
    }

    public void ExchangeSpearForCoin()
    {
        Debug.Log("ExchangeSpearForCoin called");
        if (GameManager.instance.spearCount > 0)
        {
            GameManager.instance.spearCount--;
            GameManager.instance.coinCount += 10;
            UpdateUI();
        }
    }

    public void ExchangeArrowForCoin()
    {
        Debug.Log("ExchangeArrowForCoin called");
        if (GameManager.instance.arrowCount > 0)
        {
            GameManager.instance.arrowCount--;
            GameManager.instance.coinCount += 10;
            UpdateUI();
        }
    }

    public void GetSpearByAd()
    {
        ShowRewardedAd(rewardedAdSpear, () => GameManager.instance.spearCount += 3);
    }

    public void GetArrowByAd()
    {
        ShowRewardedAd(rewardedAdArrow, () => GameManager.instance.arrowCount += 3);
    }

    public void GetCoinByAd()
    {
        ShowRewardedAd(rewardedAdCoin, () => GameManager.instance.coinCount += 30);
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
}

