using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject background1;
    public GameObject background2;

    public float switchTime = 5f;   // thời gian đổi (5 giây)
    private float timer = 0f;

    private bool isBG1Active = true; // đang mở nền 1

    void Start()
    {
        // mặc định mở background1
        background1.SetActive(true);
        background2.SetActive(false);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= switchTime)
        {
            SwitchBackground();
            timer = 0f; // reset timer
        }
    }

    void SwitchBackground()
    {
        if (isBG1Active)
        {
            background1.SetActive(false);
            background2.SetActive(true);
        }
        else
        {
            background1.SetActive(true);
            background2.SetActive(false);
        }

        isBG1Active = !isBG1Active; // đổi trạng thái
    }
}
