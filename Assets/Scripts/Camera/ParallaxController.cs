using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    // Ana kamera referansý
    Transform cam;

    // Kameranýn baþlangýç konumu
    Vector3 camStartPos;

    // Kamera hareket mesafesi (x ve y ekseni)
    float distanceX;
    float distanceY;

    // Arka plan nesneleri ve materyalleri
    GameObject[] backgrounds;
    Material[] mat;

    // Arka plan hareket hýzlarý
    float[] backSpeed;

    // En uzaktaki arka planýn derinliði
    float farthestBack;

    // Parallax efektinin hýzýný ayarlar (0.01 - 0.05 arasýnda)
    [Range(0.01f, 0.05f)]
    public float parallaxSpeed;

    void Start()
    {
        // Ana kamerayý bul ve baþlangýç pozisyonunu kaydet
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

        // Arka plan hýzlarýný hesapla
        BackSpeedCalculate(backCount);
    }

    // Arka plan hýzlarýný hesaplayan fonksiyon
    void BackSpeedCalculate(int backCount)
    {
        // En uzak arka planý belirle
        for (int i = 0; i < backCount; i++)
        {
            float depth = backgrounds[i].transform.position.z - cam.position.z;
            if (depth > farthestBack)
            {
                farthestBack = depth;
            }
        }

        // Her arka planýn hýzýný hesapla (uzaklýk arttýkça hýz azalýr)
        for (int i = 0; i < backCount; i++)
        {
            backSpeed[i] = 1 - (backgrounds[i].transform.position.z - cam.position.z) / farthestBack;
        }
    }

    // Kamera hareketine göre arka planlarý kaydýr
    private void LateUpdate()
    {
        // Kameranýn hareket mesafesini hesapla
        distanceX = cam.position.x - camStartPos.x;
        distanceY = cam.position.y - camStartPos.y;

        // Arka plan nesnelerini kamerayla ayný hizada tut
        transform.position = new Vector3(cam.position.x, transform.position.y, 0);

        // Her arka plan için paralaks efektini uygula
        for (int i = 0; i < backgrounds.Length; i++)
        {
            float speed = backSpeed[i] * parallaxSpeed;

            // Arka plan dokusunu hem x hem de y ekseninde kaydýr
            mat[i].SetTextureOffset("_MainTex", new Vector2(distanceX, distanceY) * speed);
        }
    }
}
