using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance;
   
    [SerializeField] private Examlogic examlogic;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public int GetStarsFromExam()
    {
        if (examlogic == null)
        {
            Debug.LogWarning("Manager: Examlogic not assigned!");
            return 0;
        }

        int heartsLeft = examlogic.playercurrentheart;

        return CalculateStars(heartsLeft);
    }

    private int CalculateStars(int heartsLeft)
    {
        if (heartsLeft <= 0)
        {
            return 0;
        }
        else if (heartsLeft == 1)
        {
            return 1;
        }
        else if (heartsLeft == 2)
        {
            return 2;
        }
        else if (heartsLeft >= 3)
        {
            return 3;
        }
        else
        {
            return 0;
        }
    }
}