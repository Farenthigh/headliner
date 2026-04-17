using UnityEngine;
using UnityEngine.Video; // 👈 เพิ่มบรรทัดนี้ด้วย

public enum SpeakerPosition
{
        None,
        Left,
        Center,
        Right
    }
[System.Serializable] 
public class StoryPage 
{
    [TextArea(3,5)] public string dialogueText; 
    public string speakerName; 
    public SpeakerPosition speakerPosition;
    public Sprite background;
    public VideoClip videoBackground; // 👈 เพิ่มตรงนี้ 
    public Sprite speechBubble;
    public Sprite characterLeft;
    public Sprite characterCenter;
    public Sprite characterRight;

    [Header("Quiz Settings")]
    public bool isChoicePage;

    public bool isNamingPage;


}