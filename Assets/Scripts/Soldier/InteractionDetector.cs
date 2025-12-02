using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    public IInteractable interactableInRange = null;
    public GameObject interactIcon;

    void Start()
    {
        if (interactIcon != null)
            interactIcon.SetActive(false);
    }

    void Update()
    {
        HandleInteractionInput();
    }

    public void HandleInteractionInput()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("InteractionDetector: E key pressed");
            ActiveInteraction();
        }
    }

    public void ActiveInteraction()
    {
        if (interactableInRange != null)
        {
            interactIcon.SetActive(false);
            interactableInRange.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && interactable.CanInteract())
        {
            interactableInRange = interactable;
            if (interactIcon != null)
                interactIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && interactable == interactableInRange)
        {
            interactable.EndDialogue();
            interactableInRange = null;
            if (interactIcon != null)
                interactIcon.SetActive(false);
        }
    }
}
