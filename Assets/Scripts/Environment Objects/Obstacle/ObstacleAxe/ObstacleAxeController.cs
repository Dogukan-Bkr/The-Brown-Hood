using UnityEngine;

public class ObstacleAxeController : MonoBehaviour
{
    

    float swingSpeed = 150f;
    float zAngle;
    float minAngle = -120f;
    float maxAngle = 120f;
    private void Update()
    {
        zAngle += Time.deltaTime * swingSpeed;
        transform.rotation = Quaternion.Euler(0, 0, zAngle);
        if(zAngle < minAngle)
        {
            swingSpeed = Mathf.Abs(swingSpeed);
        }
        if (zAngle > maxAngle)
        {
            swingSpeed = -Mathf.Abs(swingSpeed);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovementController.instance.BackLeash();
            PlayerHealthController.instance.TakeDamage(1);
            Debug.Log("Player hit by axe");
        }
    }
}
