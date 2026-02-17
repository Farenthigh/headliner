using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private List<RandomEvent> events;
    [SerializeField] private int maxEventsPerMonth = 1;
    private List<RandomEvent> activeEvents = new List<RandomEvent>();
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
        if (eventTriggeredThisMonth) return;
        eventTriggeredThisMonth = true;
        activeEvents.Clear();
        int eventsTriggered = 0;
        foreach (RandomEvent randomEvent in events)
        {
            if (eventsTriggered >= maxEventsPerMonth)
                break;

            float roll = Random.Range(0f, 1f);
            if (roll <= randomEvent.probability)
            {
                randomEvent.TriggerEvent();

                if (EventDisplay.Instance != null)
                {
                    EventDisplay.Instance.ShowEvent(randomEvent);
                }
                activeEvents.Add(randomEvent);
                eventsTriggered++;
            }
        }
    }
    public void ResetEventTrigger()
    {
        eventTriggeredThisMonth = false;
    }
}
