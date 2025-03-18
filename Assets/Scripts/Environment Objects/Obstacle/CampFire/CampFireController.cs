using UnityEngine;

public class CampFireController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovementController.instance.BackLeash();
            PlayerHealthController.instance.TakeDamage(1);
            Debug.Log("Player burn by campfire");
        }
    }
}
