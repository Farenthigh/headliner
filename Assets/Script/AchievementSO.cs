using UnityEngine;

public enum GameType 
{
    TaxGame,       
    SavingGame    
}

[CreateAssetMenu(fileName = "New Achievement", menuName = "Game System/Achievement")]
public class AchievementData : ScriptableObject
{
    public string id;                   
    public string achievementName;      
    [TextArea(2, 4)]
    public string description;          
    public Sprite icon;                 
    public GameType gameType;           

//archievement status
    public bool isUnlocked;             
    public string unlockDate;         
}