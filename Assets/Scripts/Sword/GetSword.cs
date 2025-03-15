using UnityEngine;

public class GetSword : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovementController.instance.normalPlayer.SetActive(false);
            PlayerMovementController.instance.swordPlayer.SetActive(true);
            Destroy(gameObject);
        }
    }
}
