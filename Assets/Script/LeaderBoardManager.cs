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

    private List<LeaderboardEntry> cachedData;
    private GameMode currentMode;

    [Header("Panel Referance")]
    [SerializeField] private GameObject leaderboardPanel;

    [Header("My Rank Bar")]
    [SerializeField] private GameObject myRankBar;
    [SerializeField] private LeaderBoardItemUI myRankItem;

    
    public APIManager apiManager;

    public async void OpenLeaderBoard()
    {
        leaderboardPanel.SetActive(true);
        // ตอนนี้โค้ดจะรู้จัก apiManager แล้วครับ
        cachedData = await apiManager.GetStageLeaderBoard();
        var myRank = await apiManager.GetMyRank();
        ShowMyRank(myRank);
        UpdateDisplay();
    }

    public void CloseLeaderBoard()
    {
        leaderboardPanel.SetActive(false);
    }



    private async void RefreshData()
    {
        cachedData = await APIManager.Instance.GetStageLeaderBoard();

        var myRank = await APIManager.Instance.GetMyRank();
        ShowMyRank(myRank);

        UpdateDisplay();
    }

    private async void Start()
    {
        await Task.Yield();

        cachedData = await APIManager.Instance.GetStageLeaderBoard();

        var myRank = await APIManager.Instance.GetMyRank();

        ShowMyRank(myRank);
        currentMode = GameMode.SavingGame;

        gameDropdown.onValueChanged.AddListener(OnDropdownChanged);
        gameDropdown.value = 0; // default Saving

        UpdateDisplay();
    }


   public void UpdateDisplay()
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);

            //  ถ้าเป็น Saving → ว่างเลย
        if (currentMode == GameMode.SavingGame)
        {
            myRankBar.SetActive(false); // ซ่อน rank ตัวเองด้วย
            return;
        }

            if (cachedData == null || cachedData.Count == 0)
            return;

        foreach (var data in cachedData)
        {
            var item = Instantiate(rowPrefab, container).GetComponent<LeaderBoardItemUI>();
            item.SetData(data.rank, data.username, data.total_stars);
        }
    }
    private void ShowMyRank(LeaderboardEntry myRank)
        {
            if (!string.IsNullOrEmpty(myRank.username))
            {
                myRankBar.SetActive(true);
                myRankItem.SetData(myRank.rank, myRank.username, myRank.total_stars);
            }
            else
            {
                myRankBar.SetActive(false);
            }
        }


       public void OnDropdownChanged(int index)
    {
        Debug.Log("Dropdown changed: " + index);

        currentMode = (GameMode)index;
        Debug.Log("CurrentMode: " + currentMode);

        if (currentMode == GameMode.SavingGame)
        {
            Debug.Log("Saving mode");
            cachedData = null;
            UpdateDisplay();
        }
        else if (currentMode == GameMode.TaxGame)
        {
            Debug.Log("Tax mode");
            RefreshData();
        }
    }


}