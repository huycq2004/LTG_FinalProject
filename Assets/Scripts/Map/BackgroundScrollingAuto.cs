using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxAutoController : MonoBehaviour
{
    GameObject[] backgrounds;
    Material[] mat;
    float[] backSpeed;

    float farthestBack;

    [Range(0.01f, 0.1f)]
    public float parallaxSpeed = 0.03f;

    // Tốc độ nền tự chạy sang phải
    public float autoScrollSpeed = 0.1f;

    void Start()
    {
        int backCount = transform.childCount;

        mat = new Material[backCount];
        backSpeed = new float[backCount];
        backgrounds = new GameObject[backCount];

        for (int i = 0; i < backCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            mat[i] = backgrounds[i].GetComponent<Renderer>().material;
        }

        BackSpeedCalculate(backCount);
    }

    void BackSpeedCalculate(int backCount)
    {
        // Tìm background xa nhất
        for (int i = 0; i < backCount; i++)
        {
            if (backgrounds[i].transform.position.z > farthestBack)
            {
                farthestBack = backgrounds[i].transform.position.z;
            }
        }

        // Gán tốc độ hiệu ứng theo độ sâu
        for (int i = 0; i < backCount; i++)
        {
            backSpeed[i] = 1 - (backgrounds[i].transform.position.z / farthestBack);
        }
    }

    private void Update()
    {
        // Auto chạy sang phải
        float distance = autoScrollSpeed * Time.deltaTime;

        for (int i = 0; i < backgrounds.Length; i++)
        {
            float speed = backSpeed[i] * parallaxSpeed;

            // Cộng dồn offset để chạy vô hạn
            Vector2 offset = mat[i].GetTextureOffset("_MainTex");
            offset.x += distance * speed;
            mat[i].SetTextureOffset("_MainTex", offset);
        }
    }
}
