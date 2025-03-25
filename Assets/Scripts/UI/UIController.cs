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
    public GameObject tentPanel; // Tent paneli
    public Button buySpearButton, buyArrowButton;
    public Button exchangeSpearForCoinButton;
    public Button adSpearButton, adArrowButton, adCoinButton;
    public Button closeButtonBlackSmith, closeButtonTent;
    public Slider healthSlider;

    public Button buyHealthButton; // 40 coin karşılığı can doldurma butonu
    public Button adHealthButton; // Reklam karşılığı can doldurma butonu

    public TMP_Text coinTxt;
    public TMP_Text arrowTxt;
    public TMP_Text spearTxt;
    public TMP_Text adStatusTxt; // Reklam durumu mesajı için

    private RewardedAd rewardedAdSpear;
    private RewardedAd rewardedAdArrow;
    private RewardedAd rewardedAdCoin;
    private RewardedAd rewardedAdHealth; // Reklam karşılığı can doldurma reklamı

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Google Ads SDK başlat
        MobileAds.Initialize(initStatus => { });

        // Butonlara tıklama event'leri bağla
        AssignButtonListeners();

        // Reklamları yükle
        RequestRewardedAds();
    }

    private void AssignButtonListeners()
    {
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

        if (closeButtonBlackSmith != null)
        {
            closeButtonBlackSmith.onClick.AddListener(CloseBlacksmithPanel);
        }
        else
        {
            Debug.LogError("closeButtonBlackSmith is not assigned in the inspector.");
        }

        if (closeButtonTent != null)
        {
            closeButtonTent.onClick.AddListener(CloseTentPanel);
        }
        else
        {
            Debug.LogError("closeButtonTent is not assigned in the inspector.");
        }

        if (buyHealthButton != null)
        {
            buyHealthButton.onClick.AddListener(BuyHealth);
        }
        else
        {
            Debug.LogError("buyHealthButton is not assigned in the inspector.");
        }

        if (adHealthButton != null)
        {
            adHealthButton.onClick.AddListener(GetHealthByAd);
        }
        else
        {
            Debug.LogError("adHealthButton is not assigned in the inspector.");
        }
    }

    private void RequestRewardedAds()
    {
        LoadRewardedAd("ca-app-pub-XXXXXXXXXXXXXXXX/XXXXXXXXXX", ad => rewardedAdSpear = ad);
        LoadRewardedAd("ca-app-pub-XXXXXXXXXXXXXXXX/XXXXXXXXXX", ad => rewardedAdArrow = ad);
        LoadRewardedAd("ca-app-pub-XXXXXXXXXXXXXXXX/XXXXXXXXXX", ad => rewardedAdCoin = ad);
        LoadRewardedAd("ca-app-pub-XXXXXXXXXXXXXXXX/XXXXXXXXXX", ad => rewardedAdHealth = ad); // Reklam karşılığı can doldurma reklamı
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

    public void CloseBlacksmithPanel()
    {
        blacksmithPanel.SetActive(false);
        PlayerMovementController.instance.ResumeMovement(); // Karakteri tekrar hareket ettir
    }

    public void OpenTentPanel()
    {
        tentPanel.SetActive(true);
        PlayerMovementController.instance.StopPlayer(); // Karakteri durdur
    }

    public void CloseTentPanel()
    {
        tentPanel.SetActive(false);
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

    public void BuyHealth()
    {
        Debug.Log("BuyHealth called");
        if (GameManager.instance.coinCount >= 40)
        {
            GameManager.instance.coinCount -= 40;
            if (PlayerHealthController.instance.currentHP < PlayerHealthController.instance.maxHP)
            {
                PlayerHealthController.instance.currentHP += 20; // Canı 20 artır
                if (PlayerHealthController.instance.currentHP > PlayerHealthController.instance.maxHP)
                {
                    PlayerHealthController.instance.currentHP = PlayerHealthController.instance.maxHP;
                }
                SetHealthSlider(PlayerHealthController.instance.currentHP, PlayerHealthController.instance.maxHP);
            }
            UpdateUI();
        }
    }

    public void GetHealthByAd()
    {
        ShowRewardedAd(rewardedAdHealth, () =>
        {
            if (PlayerHealthController.instance.currentHP < PlayerHealthController.instance.maxHP)
            {
                PlayerHealthController.instance.currentHP += 50; // Canı 50 artır
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
