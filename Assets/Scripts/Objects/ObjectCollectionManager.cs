using UnityEngine;

public class ObjectController : MonoBehaviour
{
    bool isCoin;
    bool isCollectable;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isCollectable)
        {
          isCollectable = true;
          Destroy(gameObject);
          
        }
    }
}
