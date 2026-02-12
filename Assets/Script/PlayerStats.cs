using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Resources")]
    [SerializeField] private int currentGold = 1000;

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

    public void UpdateGold(int amount)
    {
        currentGold += amount;
        if (currentGold < 0) currentGold = 0;
        Debug.Log($"Player Gold Updated: {currentGold}");
    }

    public int GetGold() => currentGold;
}