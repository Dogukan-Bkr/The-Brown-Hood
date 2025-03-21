using UnityEngine;

public class DummyController : MonoBehaviour
{
    public int health = 10000000;
    public GameObject damageEffect;

    public void TakeDamage(int damage)
    {
        health -= damage;
        ShowDamageEffect();

        if (health <= 0)
        {
            Die();
        }
    }

    private void ShowDamageEffect()
    {
        if (damageEffect != null)
        {
            Instantiate(damageEffect, transform.position, Quaternion.identity);
        }
    }

    private void Die()
    {
        // Handle the dummy's death
        Destroy(gameObject);
    }
}
