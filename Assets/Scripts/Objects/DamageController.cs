using UnityEngine;

public class DamageController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealthController.instance.TakeDamage(1);
            PlayerMovementController.instance.BackLeash();
        }
    }
}
