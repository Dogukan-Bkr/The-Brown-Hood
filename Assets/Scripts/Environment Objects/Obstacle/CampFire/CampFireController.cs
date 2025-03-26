using UnityEngine;

public class CampFireController : MonoBehaviour
{
    [SerializeField] private int damage = 2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovementController.instance.BackLeash();
            PlayerHealthController.instance.TakeDamage(damage);
            Debug.Log("Player burn by campfire");
        }
    }
}
