using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CutsceneMover : MonoBehaviour
{
    public float autoMoveSpeed = 3f;
    public bool isAutoMoving = false;
    private Vector3 targetPos;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private SoldierController controller;
    private Animator animator;

    public string sceneToLoad = "END GAME";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        controller = GetComponent<SoldierController>();
        animator = GetComponent<Animator>();
    }

    public void MoveTo(Vector3 destination)
    {
        targetPos = destination;
        isAutoMoving = true;

        // Khóa điều khiển player
        controller.enabled = false;

        // Bật animation đi bộ
        if (animator != null)
            animator.SetBool("isWalking", true);

        // BẮT ĐẦU ĐẾM GIÂY → SAU 3 GIÂY CHUYỂN SCENE
        StartCoroutine(AutoEndCutscene());
    }

    IEnumerator AutoEndCutscene()
    {
        yield return new WaitForSeconds(3f); // luôn chờ 3 giây

        // tắt auto move
        isAutoMoving = false;

        // dừng nhân vật
        rb.linearVelocity = Vector2.zero;

        if (animator != null)
            animator.SetBool("isWalking", false);

        // chuyển scene
        SceneManager.LoadScene(sceneToLoad);
    }

    void Update()
    {
        if (!isAutoMoving) return;

        float dir = targetPos.x > transform.position.x ? 1 : -1;

        sr.flipX = dir < 0;

        rb.linearVelocity = new Vector2(dir * autoMoveSpeed, rb.linearVelocity.y);
    }
}
