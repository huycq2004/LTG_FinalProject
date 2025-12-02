public interface IInteractable
{
    void Interact();
    bool CanInteract();

    
    void EndDialogue()
    {
        // Default implementation (can be overridden)
        return;
    }
}