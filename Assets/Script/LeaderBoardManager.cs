using System.ComponentModel;
using UnityEngine;
using TMPro; 
using System.Threading.Tasks;
using System.Collections.Generic;


public class LeaderBoardManager : MonoBehaviour
{
    public enum GameMode { SavingGame, TaxGame }
    [SerializeField] private TMP_Dropdown gameDropdown;
    [SerializeField] private Transform container;
    [SerializeField] private GameObject rowPrefab;

    private List<LeaderBoardEntry> cachedData;

    private async void Start()
    {
        cachedData = await APIManager.Instance.GetLeaderBoardData();
        gameDropdown.value = 0;
        UpdateDisplay(0);
    }

    public void OnDropdownChanged(int index)
    {
        UpdateDisplay(index);
    }

    public void UpdateDisplay(int index)
    {
        // clear ui
        foreach (Transform child in container) Destroy(child.gameObject);

        if (cachedData == null || cachedData.Count == 0) return;

        if(index == 0)// Saving Game
            cachedData.Sort((a,b) => b.saving_game_score.CompareTo(a.saving_game_score));
        else
            cachedData.Sort((a,b) => b.tax_game_score.CompareTo(a.tax_game_score));

        // crate ui item
        for(int i = 0; i < cachedData.Count; i++)
        {
            var item = Instantiate(rowPrefab, container).GetComponent<LeaderBoardItemUI>();
            int displayScore = (index == 0) ? cachedData[i].saving_game_score : cachedData[i].tax_game_score;
            item.SetData(i + 1, cachedData[i].username, displayScore);
        }
    }
}