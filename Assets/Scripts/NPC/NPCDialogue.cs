using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NPCDialogue", menuName = "Scriptable Objects/NPCDialogue")]
public class NPCDialogue : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    public List<string> dialogueLines;
    public List<bool> autoProgessLines; 
    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.05f;
    public AudioClip voiceSound;
    public float voicePitch = 1.0f;
    

}
