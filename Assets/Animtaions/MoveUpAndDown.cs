using UnityEngine;

public class MoveUpAndDown : MonoBehaviour
{
    public float moveDistance = 2f;      // how far to move up/down
    public float moveSpeed = 2f;         // speed of movement
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Use sine wave for smooth up and down movement
        float newY = startPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
