using UnityEngine;

public class BirdController : MonoBehaviour
{
    public Transform[] positions;  // Kuþun gideceði noktalar
    public float speed = 2f;       // Hareket hýzý
    public float waitTime = 1f;    // Noktada bekleme süresi

    private int currentPosIndex = 0;
    private float waitTimer = 0;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        
        foreach (Transform pos in positions)
        {
            pos.parent = null;
        }
    }

    private void Start()
    {
        transform.position = positions[currentPosIndex].position;
    }

    private void Update()
    {
        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            anim.SetBool("isFlying", false);
            return;
        }

        MoveToNextPosition();
    }

    void MoveToNextPosition()
    {
        Transform targetPosition = positions[currentPosIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);

        anim.SetBool("isFlying", true);

        // Hedefe ulaþýldýðýnda yeni hedef belirlenir
        if (Vector3.Distance(transform.position, targetPosition.position) < 0.1f)
        {
            waitTimer = waitTime;
            currentPosIndex = (currentPosIndex + 1) % positions.Length;  // Dizinin dýþýna çýkmasýný engelliyoruz
            FlipDirection(targetPosition);
        }
    }

    void FlipDirection(Transform target)
    {
        if (target.position.x > transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1); // Sola bak
        else
            transform.localScale = new Vector3(1, 1, 1); // Saða bak
    }
}
