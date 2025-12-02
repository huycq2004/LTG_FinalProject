using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if(dialogueData == null)
        {
            Debug.LogWarning("NPC: No dialogue data assigned!");
            return;
        }

        if(isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        nameText.text = dialogueData.npcName;
        portraitImage.sprite = dialogueData.npcPortrait;

        dialoguePanel.SetActive(true);

        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";

        foreach(char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }
        isTyping = false;

        if(dialogueData.autoProgessLines[dialogueIndex] && dialogueData.autoProgessLines.Count > dialogueIndex)
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    void NextLine()
    {
        if(isTyping) {
            StopAllCoroutines();
            dialogueText.text = dialogueData.dialogueLines[dialogueIndex];
            isTyping = false;
            
        }
        else if (++dialogueIndex < dialogueData.dialogueLines.Count)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
    }
}
