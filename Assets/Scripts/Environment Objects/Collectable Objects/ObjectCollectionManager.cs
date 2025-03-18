using UnityEngine;

public class ObjectController : MonoBehaviour
{
    // Toplanabilir nesnelerin türlerini belirten enum
    public enum CollectableType { Coin, Arrow, Spear, HealthPotion }

    // Bu nesnenin hangi türde olduðunu belirler
    public CollectableType objectType;

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
                    GameManager.instance.coinCount++;  // Altýn sayýsýný artýr
                    UIController.instance.SetCoinCount();  // UI'yi güncelle
                    break;

                case CollectableType.Arrow:
                    GameManager.instance.arrowCount++; // Ok sayýsýný artýr
                    UIController.instance.SetArrowCount(); // UI'yi güncelle
                    break;

                case CollectableType.Spear:
                    GameManager.instance.spearCount++; // Mýzrak sayýsýný artýr
                    UIController.instance.SetSpearCount(); // UI'yi güncelle
                    break;
                case CollectableType.HealthPotion:
                    PlayerHealthController.instance.currentHP += 20; // Oyuncunun canýný artýr
                    PlayerHealthController.instance.GetHeal(); // UI'yi güncelle
                    break;
            }

            // Nesneyi sahneden kaldýr
            Destroy(gameObject);
        }
    }
}
