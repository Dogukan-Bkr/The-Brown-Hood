using UnityEngine;

public class CameraController : MonoBehaviour
{
    PlayerMovementController player;

    private void Awake()
    {
        player = Object.FindFirstObjectByType<PlayerMovementController>();
    }
    void Update()
    {
        if(player != null)
        {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        }
    }
}
