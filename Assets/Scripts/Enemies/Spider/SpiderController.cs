using System.Collections;
using TMPro;
using UnityEngine;

public class SpiderController : MonoBehaviour
{
    public Transform[] positions;
    public float speed;
    public float waitTime;
    public float attackArea;
    private float waitTimeCounter;
    int targetPosIndex;
    bool isAttackable;
    Animator anim;
    Transform targetPlayer;
    BoxCollider2D spiderCollider;
    private void Awake()
    {
        spiderCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        foreach (Transform pos in positions)
        {
            pos.parent = null;
        }
    }
    private void Start()
    {
        isAttackable = true;
        transform.position = positions[targetPosIndex].position;
        targetPlayer = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (!isAttackable) { return; }
        if (targetPlayer.position.x > positions[0].position.x && targetPlayer.position.x < positions[1].position.x)
        {
            waitTimeCounter = 0;  // Bekleme süresini sýfýrla
        }
        if (waitTimeCounter > 0)
        {
            waitTimeCounter -= Time.deltaTime;
            anim.SetBool("isMove", false);
            return;
        }

        MoveToNextPosition();
    }

    void MoveToNextPosition()
    {
        if (targetPlayer.position.x > positions[0].position.x && targetPlayer.position.x < positions[1].position.x)
        {
            
            Vector3 newPosition = new Vector3(targetPlayer.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, newPosition, speed * Time.deltaTime);

            FollowDirection(targetPlayer); // Oyuncuya yönel
            anim.SetBool("isMove", true);
        }
        else
        {
            Transform targetPosition = positions[targetPosIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);

            FollowDirection(targetPosition); // Hedef noktaya yönel
            anim.SetBool("isMove", true);

            // Hedefe ulaþýldýðýnda yeni hedef belirlenir
            if (Vector3.Distance(transform.position, targetPosition.position) < 0.1f)
            {
                waitTimeCounter = waitTime;
                targetPosIndex = (targetPosIndex + 1) % positions.Length;
            }
        }
    }



    void FlipDirection(Transform target)
    {
        if (target.position.x > transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1); // Sola bak
        else
            transform.localScale = Vector3.one; // Saða bak
    }
    void FollowDirection(Transform target)
    {
        if (target.position.x > transform.position.x)
            transform.localScale = Vector3.one; // Saða bak
        else
            transform.localScale = new Vector3(-1, 1, 1); // Sola bak
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < positions.Length; i++)
        {
            Gizmos.DrawWireSphere(positions[i].position, attackArea);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isAttackable)
        {
            isAttackable = false;
            anim.SetTrigger("attack");
            //collision.GetComponent<PlayerMovementController>().BackLeash();
            PlayerMovementController.instance.BackLeash();
            PlayerHealthController.instance.TakeDamage(2);
            StartCoroutine(AttackCooldown());
        }
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(1f);
        isAttackable = true;
    }

}
