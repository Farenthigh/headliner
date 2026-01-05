using UnityEngine;
using UnityEngine.UI;

public class SelectGame : MonoBehaviour
{
    [SerializeField] private Button savinggame;
    [SerializeField] private Button taxgame;
    [SerializeField] private Button investgame;
    
    
    private void Start()
    {
        savinggame.onClick.AddListener(() => SelectGameOption("Saving Game"));
        taxgame.onClick.AddListener(() => SelectGameOption("Tax Game"));
        investgame.onClick.AddListener(() => SelectGameOption("Investment Game"));
    }
    private void SelectGameOption(string game)
    {
        Debug.Log($"Selected Game: {game}");
    }
}
