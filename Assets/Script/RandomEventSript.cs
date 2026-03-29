// using UnityEngine;

// [CreateAssetMenu(fileName = "NewEvent", menuName = "Random Event/General")]
// public class RandomEvent : ScriptableObject
// {
//     public Sprite icon;
//     public string eventName;
//     [TextArea] public string description; // TextArea ใช้สำหรับข้อความยาว
//     public Sprite backcard;
//     public Sprite frontcard;

//     public float probability; // Probability of the event occurring (0 to 1)

//     // public virtual void ExecuteAction() // Can be Override
//     // {
//     //     Debug.Log($"Executing Action for Event: {eventName}");
//     //     // Default implementation of the action
//     // }

//     public virtual void TriggerEvent() // Cannot Override 
//     {
//         Debug.Log($"Event Triggered: {eventName} - {description}");
//         // Implement event effects here
//     }
// }
using UnityEngine;

public enum EventType { Money, Time, Round, Goal }
public enum EventRarity { Common, Uncommon, Rare }
public enum EventValueType { Flat, Percent }

[CreateAssetMenu(fileName = "NewEvent", menuName = "Random Event/General")]
public class Event : ScriptableObject
{
    public EventType eventType;
    public EventValueType eventValueType;
    public float eventValue;
    public EventRarity rarity;
    public Sprite icon;
    public string eventName;
    [TextArea] public string description; // TextArea ใช้สำหรับข้อความยาว


    public virtual void TriggerEvent() // Cannot Override
    {
        Debug.Log($"Event Triggered: {eventName} - {description} Rarity: {rarity}");
        if (eventType == EventType.Money)
        {
            if (eventValueType == EventValueType.Flat)
            {
                SavingGameLogicManager.Instance.AddCash(eventValue);
            }
            else if (eventValueType == EventValueType.Percent)
            {
                SavingGameLogicManager.Instance.AddCash(SavingGameLogicManager.Instance.GetCurrentCash() * (eventValue + 1));
            }
        }
        if (eventType == EventType.Time)
        {
            if (eventValueType == EventValueType.Flat)
            {
                SavingGameLogicManager.Instance.Addtime(-(int)eventValue);
            }
        }
        if (eventType == EventType.Round)
        {
            if (eventValueType == EventValueType.Flat)
            {
                for (int i = 0; i < eventValue; i++)
                {
                    SavingGameLogicManager.Instance.AdvanceMonth();
                }
            }
        }
        if (eventType == EventType.Goal)
        {
            if (eventValueType == EventValueType.Flat)
            {
                SavingGoalManager.Instance.AddGoal(eventValue);
            }
            else if (eventValueType == EventValueType.Percent)
            {
                SavingGoalManager.Instance.AddGoal(SavingGoalManager.Instance.GetGoalAmount() * (eventValue + 1));
            }
        }
        // Implement event effects here
    }
}