using System.ComponentModel;
using UnityEngine;
using TMPro; 
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml.Serialization;


public class LeaderBoardManager : MonoBehaviour
{
    public enum GameMode { SavingGame, TaxGame }
    [SerializeField] private TMP_Dropdown gameDropdown;
    [SerializeField] private Transform container;
    [SerializeField] private GameObject rowPrefab;

    private List<LeaderBoardEntry> cachedData;

    [Header("Panel Referance")]
    [SerializeField] private GameObject leaderBoardPanel;

    [Header("My Rank Bar")]
    [SerializeField] private GameObject myRankBar;
    [SerializeField] private LeaderBoardItemUI myRankItem;

    public void OpenLeaderBoard()
    {
        leaderBoardPanel.SetActive(true);
        RefreshData();
    }

    public void CloseLeaderBoard()
    {
        leaderBoardPanel.SetActive(false);
    }

    private async void RefreshData()
    {
        cachedData = await APIManager.Instance.GetLeaderBoardData();
        UpdateDisplay(gameDropdown.value);
    }

    private async void Start()
    {
        await Task.Yield();
        
        cachedData = await APIManager.Instance.GetLeaderBoardData();
        gameDropdown.value = 0;
        gameDropdown.onValueChanged.AddListener(OnDropdownChanged);
        UpdateDisplay(0);

    }

    public void OnDropdownChanged(int index)
    {
        UpdateDisplay(index);
    }

    public void UpdateDisplay(int index)
    {

        foreach (Transform child in container) Destroy(child.gameObject);

        if (cachedData == null || cachedData.Count == 0) return;

        
        List<LeaderBoardEntry> displayData = new List<LeaderBoardEntry>(cachedData);

        if (index == 0) 
        {
            displayData.Sort((a, b) => b.saving_game_score.CompareTo(a.saving_game_score));
        }
        else 
        {
            displayData.Sort((a, b) => b.tax_game_score.CompareTo(a.tax_game_score));
        }

        
        for (int i = 0; i < displayData.Count; i++)
        {
            var item = Instantiate(rowPrefab, container).GetComponent<LeaderBoardItemUI>();
            int displayScore = (index == 0) ? displayData[i].saving_game_score : displayData[i].tax_game_score;
            item.SetData(i + 1, displayData[i].username, displayScore);
        }

        string myUsername = APIManager.myData.username;
        for(int i = 0; i < displayData.Count; i++)
        {
            if(displayData[i].username == myUsername)
            {
                int myScore = (index == 0)
                    ? displayData[i].saving_game_score
                    : displayData[i].tax_game_score;

                myRankBar.SetActive(true);
                myRankItem.SetData(i + 1, displayData[i].username, myScore);
                return;
            }
        }
        myRankBar.SetActive(false);
    }
}