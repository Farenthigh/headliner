using UnityEngine;

[CreateAssetMenu(fileName = "NewMoneyEvent", menuName = "Random Event/Money Event")]
public class MoneyEvent : Event
{
    [Header("Money Setting")]
    [Tooltip("Amount of money to add or subtract")]
    public int goldAmount;

    public override void TriggerEvent()
    {
        // Call the base method to log the event
        base.TriggerEvent();

        // Example Action: Modify player's gold based on goldAmount
        if (goldAmount >= 0)
        {
            Debug.Log($"<color=green>Player receives {goldAmount} gold. From {eventName}");
        }
        else
        {
            Debug.Log($"<color=red>Player loses {-goldAmount} gold. From {eventName}");
        }

        // Manage Actual Gold Change
        if (SavingGameLogicManager.Instance != null)
        {
            SavingGameLogicManager.Instance.UpdateGold(goldAmount);
        }
    }
}