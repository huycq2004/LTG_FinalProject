using UnityEngine;

public class BackgroundSwitcher : MonoBehaviour
{
    public GameObject backgroundA;  // background mặc định
    public GameObject backgroundB;  // background mới khi đến checkpoint

    private bool hasSwitched = false; // để tránh đổi nhiều lần


    public void Start()
    {
        backgroundB.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasSwitched) return;

        if (collision.CompareTag("Player"))
        {
            hasSwitched = true;

            backgroundA.SetActive(false);
            backgroundB.SetActive(true);

            Debug.Log("Đã đổi background!");
        }
    }
}
