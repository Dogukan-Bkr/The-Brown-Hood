using UnityEngine;
using DG.Tweening;
public class FadeController : MonoBehaviour
{
    public static FadeController instance; 
    public GameObject fadeImg; // Karartma resmi

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
       
    }
    private void Start()
    {
        FadeOut(); // Oyun baþladýðýnda karartmayý kaldýr
    }

    public void FadeIn()
    {
        fadeImg.GetComponent<CanvasGroup>().alpha = 0f; // Karartma resminin alfasýný sýfýrla
        fadeImg.GetComponent<CanvasGroup>().DOFade(1f, 1f); // Karartma resmini belirtilen sürede tamamen karart
    }
    public void FadeOut()
    {
        fadeImg.GetComponent<CanvasGroup>().alpha = 1f; // Karartma resminin alfasýný tam yap
        fadeImg.GetComponent<CanvasGroup>().DOFade(0f, 1f);
    }

}
