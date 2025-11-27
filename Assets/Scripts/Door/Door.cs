using UnityEngine;

public class Door : MonoBehaviour
{
    public float openOffset = 6f;   // move up by 6 units
    public float moveSpeed = 2f;    // speed of door movement
    private Vector3 closedPosition; // original door position
    private Vector3 openPosition;   // target open position
    private Vector3 targetPosition; // current target position
    private bool isOpen = false;

    private void Start()
    {
        // Save the original position (closed)
        closedPosition = transform.position;

        // Calculate the open position
        openPosition = closedPosition + new Vector3(0, openOffset, 0);
        targetPosition = openPosition;
        OpenDoor();
    }

    private void Update()
    {
        // Move towards target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            targetPosition = openPosition;
            isOpen = true;
        }
    }

    public void CloseDoor()
    {
        if (isOpen)
        {
            targetPosition = closedPosition;
            isOpen = false;
        }
    }
}
