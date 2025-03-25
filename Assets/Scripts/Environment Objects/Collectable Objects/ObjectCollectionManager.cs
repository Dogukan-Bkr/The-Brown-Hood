using UnityEngine;

public class ObjectController : MonoBehaviour
{
    // Toplanabilir nesnelerin türlerini belirten enum
    public enum CollectableType { Coin, Arrow, Spear, HealthPotion }

    // Bu nesnenin hangi türde olduðunu belirler
    public CollectableType objectType;

    // Toplanacak miktar
    public int amount = 1;

    // Nesnenin toplanýp toplanmadýðýný kontrol eden deðiþken
    private bool isCollectable = false;

    // Oyuncu nesneye temas ettiðinde çalýþýr
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Eðer çarpýþan nesne "Player" (Oyuncu) ise ve nesne daha önce toplanmamýþsa
        if (collision.gameObject.CompareTag("Player") && !isCollectable)
        {
            isCollectable = true; // Nesnenin toplandýðýný iþaretle

            // Nesne türüne göre farklý iþlemler yap
            switch (objectType)
            {
                case CollectableType.Coin:
                    GameManager.instance.coinCount += amount;  // Altýn sayýsýný artýr
                    UIController.instance.UpdateUI();  // UI'yi güncelle
                    break;

                case CollectableType.Arrow:
                    GameManager.instance.arrowCount += amount; // Ok sayýsýný artýr
                    UIController.instance.UpdateUI(); // UI'yi güncelle
                    break;

                case CollectableType.Spear:
                    GameManager.instance.spearCount += amount; // Mýzrak sayýsýný artýr
                    UIController.instance.UpdateUI(); // UI'yi güncelle
                    break;

                case CollectableType.HealthPotion:
                    PlayerHealthController.instance.currentHP += amount; // Oyuncunun canýný artýr
                    PlayerHealthController.instance.GetHeal(); // UI'yi güncelle
                    break;
            }

            // Nesneyi sahneden kaldýr
            Destroy(gameObject);
        }
    }
}
