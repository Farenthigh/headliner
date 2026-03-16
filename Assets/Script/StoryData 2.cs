using UnityEngine;

[System.Serializable] 
public class StoryPage 
{
    [TextArea(3,5)] public string dialogueText; 
    public string speakerName; 
    public Sprite background;   
    public Sprite speechBubble;
    public Sprite characterLeft;
    public Sprite characterCenter;
    public Sprite characterRight;

    [Header("Dynamic Character Display")]
    public bool usePlayerCharacter;
    public bool useChatbotCharacter;

    [Header("Quiz Settings")]
    public bool isChoicePage;  
} 