using UnityEngine;

[System.Serializable] 
public class StoryPage 
{
    [TextArea(3,5)]
    public string dialogueText; 
    public string speakerName; 
    public Sprite background;   
    public Sprite character;   
    public bool isChoicePage;  
}