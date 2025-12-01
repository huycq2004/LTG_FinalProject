using UnityEngine;
using System.Collections;

public class SimpleSpawnEffect : MonoBehaviour
{
    public GameObject player;        // object người chơi
    public GameObject spawnEffect;   // object effect khói
    public float effectDuration = 1.0f; // thời gian hiệu ứng tồn tại

    private void Start()
    {
        StartCoroutine(PlaySpawn());
    }

    IEnumerator PlaySpawn()
    {
        // 1. Ẩn player
        player.SetActive(false);

        // 2. Hiện hiệu ứng spawn
        spawnEffect.SetActive(true);

        // 3. Chờ hiệu ứng chạy xong
        yield return new WaitForSeconds(effectDuration);

        // 4. Ẩn hiệu ứng
        spawnEffect.SetActive(false);

        // 5. Hiện player
        player.SetActive(true);
    }
}
