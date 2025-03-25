using UnityEngine;

public class ObjectController : MonoBehaviour
{
    // Toplanabilir nesnelerin t�rlerini belirten enum
    public enum CollectableType { Coin, Arrow, Spear, HealthPotion }

    // Bu nesnenin hangi t�rde oldu�unu belirler
    public CollectableType objectType;

    // Toplanacak miktar
    public int amount = 1;

    // Nesnenin toplan�p toplanmad���n� kontrol eden de�i�ken
    private bool isCollectable = false;

    // Oyuncu nesneye temas etti�inde �al���r
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // E�er �arp��an nesne "Player" (Oyuncu) ise ve nesne daha �nce toplanmam��sa
        if (collision.gameObject.CompareTag("Player") && !isCollectable)
        {
            isCollectable = true; // Nesnenin topland���n� i�aretle

            // Nesne t�r�ne g�re farkl� i�lemler yap
            switch (objectType)
            {
                case CollectableType.Coin:
                    GameManager.instance.coinCount += amount;  // Alt�n say�s�n� art�r
                    UIController.instance.UpdateUI();  // UI'yi g�ncelle
                    break;

                case CollectableType.Arrow:
                    GameManager.instance.arrowCount += amount; // Ok say�s�n� art�r
                    UIController.instance.UpdateUI(); // UI'yi g�ncelle
                    break;

                case CollectableType.Spear:
                    GameManager.instance.spearCount += amount; // M�zrak say�s�n� art�r
                    UIController.instance.UpdateUI(); // UI'yi g�ncelle
                    break;

                case CollectableType.HealthPotion:
                    PlayerHealthController.instance.currentHP += amount; // Oyuncunun can�n� art�r
                    PlayerHealthController.instance.GetHeal(); // UI'yi g�ncelle
                    break;
            }

            // Nesneyi sahneden kald�r
            Destroy(gameObject);
        }
    }
}
