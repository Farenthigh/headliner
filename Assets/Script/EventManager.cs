using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private List<Event> events;
    // [SerializeField] private int maxEventsPerMonth = 1;
    private List<Event> activeEvents = new List<Event>();
    public static EventManager Instance;
    bool eventTriggeredThisMonth = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RandomEvent()
    {
        // Randomly select an event based on rarity
        float roll = Random.Range(0f, 1f);
        EventRarity selectedRarity;

        if (roll <= 0.05f)
        {
            selectedRarity = EventRarity.Rare;
        }
        else if (roll <= 0.35f)
        {
            selectedRarity = EventRarity.Uncommon;
        }
        else
        {
            selectedRarity = EventRarity.Common;
        }

        // Create a pool of events matching the selected rarity
        List<Event> pool = events.FindAll(e => e.rarity == selectedRarity);

        if (pool.Count > 0)
        {
            int randomIndex = Random.Range(0, pool.Count);
            Event finalEvent = pool[randomIndex];

            if (EventDisplay.Instance != null)
            {
                EventDisplay.Instance.ShowEvent(finalEvent);
            }
        }
        else
        {
            Debug.LogWarning($"No events found for rarity: {selectedRarity}");
            SavingGameLogicManager.Instance.SetIsPaused(false);
        }
    }
    public void ResetEventTrigger()
    {
        eventTriggeredThisMonth = false;
    }
}