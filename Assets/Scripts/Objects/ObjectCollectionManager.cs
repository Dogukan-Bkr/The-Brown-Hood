using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public enum CollectableType { Coin, Arrow, Spear } 
    public CollectableType objectType; 
    private bool isCollectable = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isCollectable)
        {
            isCollectable = true;

            switch (objectType)
            {
                case CollectableType.Coin:
                    GameManager.instance.coinCount++;
                    UIController.instance.SetCoinCount();
                    break;

                case CollectableType.Arrow:
                    GameManager.instance.arrowCount++;
                    UIController.instance.SetArrowCount();
                    break;

                case CollectableType.Spear:
                    GameManager.instance.spearCount++;
                    UIController.instance.SetSpearCount();
                    break;
            }

            Destroy(gameObject);
        }
    }
}
