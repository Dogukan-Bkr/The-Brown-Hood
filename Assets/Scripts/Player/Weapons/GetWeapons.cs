using UnityEngine;
using static PlayerMovementController;

public class GetWeapons : MonoBehaviour
{
    public WeaponType weaponType; // Toplanacak silah t�r�

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Mevcut silahlar� devre d��� b�rak
            PlayerMovementController.instance.normalPlayer.SetActive(false);
            PlayerMovementController.instance.swordPlayer.SetActive(false);
            PlayerMovementController.instance.spearPlayer.SetActive(false);
            PlayerMovementController.instance.bowPlayer.SetActive(false);

            // Yeni silah� aktif hale getir
            switch (weaponType)
            {
                case WeaponType.Sword:
                    PlayerMovementController.instance.swordPlayer.SetActive(true);
                    PlayerMovementController.instance.currentWeapon = WeaponType.Sword;
                    break;
                case WeaponType.Spear:
                    PlayerMovementController.instance.spearPlayer.SetActive(true);
                    PlayerMovementController.instance.currentWeapon = WeaponType.Spear;
                    break;
                case WeaponType.Bow:
                    PlayerMovementController.instance.bowPlayer.SetActive(true);
                    PlayerMovementController.instance.currentWeapon = WeaponType.Bow;
                    break;
            }
            Destroy(gameObject);
        }
    }
}
