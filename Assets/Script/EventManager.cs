using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private List<Event> events;
    [SerializeField] private int maxEventsPerMonth = 1;
    private List<Event> activeEvents = new List<Event>();
    public static EventManager Instance;

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
        activeEvents.Clear();
        int eventsTriggered = 0;
        foreach (Event randomEvent in events)
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
}
