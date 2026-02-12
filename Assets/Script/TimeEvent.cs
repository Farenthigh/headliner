using UnityEngine;

[CreateAssetMenu(fileName = "NewTimeEvent", menuName = "Random Event/Time Event")]
public class TimeEvent : Event
{
    [Header("Time Setting")]
    public float timeChange;

    public override void TriggerEvent()
    {
        base.TriggerEvent();

        if(SavingGameLogicManager.Instance != null)
        {
            SavingGameLogicManager.Instance.ModifyTime(timeChange);
            if(timeChange > 0)
            {
                Debug.Log($"Time Advanced by {timeChange} seconds. From {eventName}");
            }
            else{
                Debug.Log($"Time Moved Backward by {-timeChange} seconds. From {eventName}");
            }
        }
        else{
            Debug.LogWarning("SavingGameLogicManager Instance is null. Cannot modify time.");
        }
    }
}