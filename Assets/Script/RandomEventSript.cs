using UnityEngine;

[CreateAssetMenu(fileName = "NewEvent", menuName = "Random Event")]
public class Event : ScriptableObject
{
    public string eventName;
    public string description;
    public float probability; // Probability of the event occurring (0 to 1)
    public void TriggerEvent()
    {
        Debug.Log($"Event Triggered: {eventName} - {description}");
        // Implement event effects here
    }
}