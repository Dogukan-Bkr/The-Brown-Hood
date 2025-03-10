using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    // Ana kamera referans�
    Transform cam;

    // Kameran�n ba�lang�� konumu
    Vector3 camStartPos;

    // Kamera hareket mesafesi (x ve y ekseni)
    float distanceX;
    float distanceY;

    // Arka plan nesneleri ve materyalleri
    GameObject[] backgrounds;
    Material[] mat;

    // Arka plan hareket h�zlar�
    float[] backSpeed;

    // En uzaktaki arka plan�n derinli�i
    float farthestBack;

    // Parallax efektinin h�z�n� ayarlar (0.01 - 0.05 aras�nda)
    [Range(0.01f, 0.05f)]
    public float parallaxSpeed;

    void Start()
    {
        // Ana kameray� bul ve ba�lang�� pozisyonunu kaydet
        cam = Camera.main.transform;
        camStartPos = cam.position;

        // Arka plan nesnelerini al
        int backCount = transform.childCount;
        mat = new Material[backCount];
        backSpeed = new float[backCount];
        backgrounds = new GameObject[backCount];

        // Her arka plan nesnesinin materyalini al
        for (int i = 0; i < backCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            mat[i] = backgrounds[i].GetComponent<Renderer>().material;
        }

        // Arka plan h�zlar�n� hesapla
        BackSpeedCalculate(backCount);
    }

    // Arka plan h�zlar�n� hesaplayan fonksiyon
    void BackSpeedCalculate(int backCount)
    {
        // En uzak arka plan� belirle
        for (int i = 0; i < backCount; i++)
        {
            float depth = backgrounds[i].transform.position.z - cam.position.z;
            if (depth > farthestBack)
            {
                farthestBack = depth;
            }
        }

        // Her arka plan�n h�z�n� hesapla (uzakl�k artt�k�a h�z azal�r)
        for (int i = 0; i < backCount; i++)
        {
            backSpeed[i] = 1 - (backgrounds[i].transform.position.z - cam.position.z) / farthestBack;
        }
    }

    // Kamera hareketine g�re arka planlar� kayd�r
    private void LateUpdate()
    {
        // Kameran�n hareket mesafesini hesapla
        distanceX = cam.position.x - camStartPos.x;
        distanceY = cam.position.y - camStartPos.y;

        // Arka plan nesnelerini kamerayla ayn� hizada tut
        transform.position = new Vector3(cam.position.x, transform.position.y, 0);

        // Her arka plan i�in paralaks efektini uygula
        for (int i = 0; i < backgrounds.Length; i++)
        {
            float speed = backSpeed[i] * parallaxSpeed;

            // Arka plan dokusunu hem x hem de y ekseninde kayd�r
            mat[i].SetTextureOffset("_MainTex", new Vector2(distanceX, distanceY) * speed);
        }
    }
}
