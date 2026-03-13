using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectGame : MonoBehaviour
{
    [SerializeField] private Button savinggame;
    [SerializeField] private Button taxgame;
    [SerializeField] private Button achievement;
    [SerializeField] private Button leaderboard;

    private void Start()
    {
        savinggame.onClick.AddListener(() => SelectGameOption("SavingGameMap"));
        taxgame.onClick.AddListener(() => SelectGameOption("TaxGameMap"));
        achievement.onClick.AddListener(() => SelectGameOption("Achievement"));
        leaderboard.onClick.AddListener(() => SelectGameOption("LeaderBoardScene"));
    }
    private void SelectGameOption(string game)
    {
        Debug.Log($"Selected Game: {game}");
        SceneManager.LoadScene(game);
    }
}
