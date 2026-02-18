using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public static bool hardUnlocked = false;

    public static void UnlockHard()
    {
        hardUnlocked = true;
    }
}
