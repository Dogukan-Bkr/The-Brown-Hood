using UnityEngine;

public class BoxController : MonoBehaviour
{
    public GameObject boxHitEffect;
    Animator anim;
    int hitCount = 0;
    int randomCount;
    public GameObject coinPrefab;
    Vector2 coinSpawnPos = new Vector2(0, 0);
    private void Awake()
    {
        anim = GetComponent<Animator>();
        randomCount = Random.Range(0, 4);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SwordDamageBox"))
        {
            if(hitCount <= 1)
            {
                anim.SetTrigger("hit");
                Instantiate(boxHitEffect, transform.position, Quaternion.identity);
            }
            else
            {
                anim.SetTrigger("break");
                Destroy(gameObject, 0.5f);
                GetComponent<BoxCollider2D>().enabled = false;
                for (int i = 0; i < randomCount; i++)
                {

                    //coinPrefab = Resources.Load<GameObject>("Prefabs/Objects/CoinWRigidBody");
                    GameObject coin = Instantiate(coinPrefab, (Vector2)transform.position + coinSpawnPos, Quaternion.identity);
                    coinSpawnPos.x += 1;
                    coin.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-100, 100), Random.Range(300, 500)));

                }
            }
            hitCount++;
        }
    }
}
