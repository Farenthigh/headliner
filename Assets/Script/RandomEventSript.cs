using UnityEngine;

[CreateAssetMenu(fileName = "NewEvent", menuName = "Random Event/General")]
public class Event : ScriptableObject
{
    public Sprite icon;
    public string eventName;
    [TextArea] public string description; // TextArea ใช้สำหรับข้อความยาว
    public float probability; // Probability of the event occurring (0 to 1)

    // public virtual void ExecuteAction() // Can be Override
    // {
    //     Debug.Log($"Executing Action for Event: {eventName}");
    //     // Default implementation of the action
    // }

    public virtual void TriggerEvent() // Cannot Override 
    {
        Debug.Log($"Event Triggered: {eventName} - {description}");
        // Implement event effects here
    }
}