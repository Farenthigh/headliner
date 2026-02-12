using UnityEngine;

public enum EventType {Money, Time, Round}
public enum EventRarity {Common,Uncommon,Rare}

[CreateAssetMenu(fileName = "NewEvent", menuName = "Random Event/General")]
public class Event : ScriptableObject
{
    public EventType eventType;
    public EventRarity rarity;
    public Sprite icon;
    public string eventName;
    [TextArea] public string description; // TextArea ใช้สำหรับข้อความยาว
    

    public virtual void TriggerEvent() // Cannot Override 
    {
        Debug.Log($"Event Triggered: {eventName} - {description} Rarity: {rarity}");
        // Implement event effects here
    }
}