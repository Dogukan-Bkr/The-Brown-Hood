using UnityEngine;

public class CheckPointController : MonoBehaviour
{
    public SpriteRenderer checkPointRenderer;
    public Sprite checkPointOn, checkPointOff;
    private bool isActivated = false;

    private void Start()
    {
        checkPointRenderer.sprite = checkPointOff;  // Baþlangýçta checkpoint kapalý
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActivated)
        {
            // Checkpoint'i aktif et
            GameManager.instance.lastCheckPointPos = transform.position;
            Debug.Log("Checkpoint activated!");
            // Canýn yarýsýný kaydet
            int halfHP = Mathf.Max(1, PlayerHealthController.instance.currentHP / 2);
            GameManager.instance.lastCheckPointHP = halfHP;

            checkPointRenderer.sprite = checkPointOn;  // Checkpoint'i aktif hale getir
            isActivated = true;

            // Daha önce aktive olmuþ tüm checkpoint'leri kapat (Opsiyonel)
            CheckPointController[] allCheckpoints = FindObjectsByType<CheckPointController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (CheckPointController cp in allCheckpoints)
            {
                if (cp != this)
                {
                    cp.DeactivateCheckpoint(); // Diðer checkpoint'leri kapat
                }
            }
        }
    }

    public void DeactivateCheckpoint()
    {
        checkPointRenderer.sprite = checkPointOff; // Checkpoint'i kapat
        isActivated = false;
    }
}
