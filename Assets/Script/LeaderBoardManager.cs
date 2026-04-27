using UnityEngine;
using TMPro; 
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

    public async void OpenLeaderBoard()
    {
        leaderboardPanel.SetActive(true);
        // ดึงข้อมูลรายชื่อทั้งหมดแค่เส้นเดียวพอ
        cachedData = await APIManager.Instance.GetStageLeaderBoard();
        
        if (this == null) return;
        
        UpdateDisplay();
    }

    public void CloseLeaderBoardToTaxGameMap()
    {
        LoadingManager.Instance.LoadScene("TaxGameMap");
    }
    public void CloseLeaderBoardToSelectGame()
    {
        LoadingManager.Instance.LoadScene("SelectGame");
    }
    public void CloseLeaderBoardToSavingGameMap()
    {
        LoadingManager.Instance.LoadScene("SavingGameStageInfo");
    }
    


    private async void RefreshData()
    {
        cachedData = await APIManager.Instance.GetStageLeaderBoard();
        UpdateDisplay();
    }

    private async void Start()
    {
        await Task.Yield();
        cachedData = await APIManager.Instance.GetStageLeaderBoard();
        currentMode = GameMode.SavingGame;

        gameDropdown.onValueChanged.AddListener(OnDropdownChanged);
        gameDropdown.value = 0; // default Saving

        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        // 1. ล้างของเก่า
        foreach (Transform child in container)
            Destroy(child.gameObject);

        if (cachedData == null || cachedData.Count == 0)
        {
            myRankBar.SetActive(false);
            return;
        }

        // 2. สร้าง List ใหม่และจัดเรียงคะแนนจากมากไปน้อยตาม mode
        List<LeaderboardEntry> sortedList = new List<LeaderboardEntry>(cachedData);
        sortedList.Sort((a, b) => 
        {
            int scoreComparison;
            
            // สเต็ป 1: เทียบคะแนน (ใครคะแนนมากกว่า ชนะ)
            if (currentMode == GameMode.TaxGame) {
                scoreComparison = b.tax_game_score.CompareTo(a.tax_game_score);
            } else {
                scoreComparison = b.saving_game_score.CompareTo(a.saving_game_score);
            }

            // สเต็ป 2: ถ้าคะแนนเกิด "เท่ากันเป๊ะ" (scoreComparison == 0) ให้เทียบเวลา
            if (scoreComparison == 0) {
                System.DateTime timeA, timeB;
                bool isTimeAGood = System.DateTime.TryParse(a.updated_at, out timeA);
                bool isTimeBGood = System.DateTime.TryParse(b.updated_at, out timeB);
                
                if (isTimeAGood && isTimeBGood) {
                    return timeA.CompareTo(timeB); 
                }
            }

            return scoreComparison;
        });

        int myRankIndex = -1;
        int myScore = 0;
        
        string myName = APIManager.myData.username; 

        // 3. วนลูปสร้าง UI แถวรายชื่อ และเช็กหาชื่อตัวเอง
        for (int i = 0; i < sortedList.Count; i++)
        {
            var data = sortedList[i];
            int currentRank = i + 1;
            // ✅ เลือก score ตาม mode ที่กำลังดูอยู่
            int currentScore = currentMode == GameMode.TaxGame
                ? data.tax_game_score
                : data.saving_game_score;

            var item = Instantiate(rowPrefab, container).GetComponent<LeaderBoardItemUI>();
            item.SetData(currentRank, data.username, currentScore);

            if (!string.IsNullOrEmpty(myName) && data.username == myName)
            {
                myRankIndex = currentRank;
                myScore = currentScore;
            }
        }

        // 4. อัปเดตแถบ My Rank ด้านบนสุด
        if (myRankIndex != -1)
        {
            myRankBar.SetActive(true);
            myRankItem.SetData(myRankIndex, myName, myScore);
            Debug.Log($"✅ เจออันดับของฉันแล้ว! อันดับที่ {myRankIndex} คะแนน {myScore}");
        }
        else
        {
            myRankBar.SetActive(false);
            Debug.LogWarning($"❌ หาชื่อ '{myName}' ในกระดานไม่เจอ! (ถ้าชื่อว่างเปล่าแปลว่าลืมผ่านหน้า Login)");
        }
    }

    public void OnDropdownChanged(int index)
    {
        currentMode = (GameMode)index;
        RefreshData(); // ดึงข้อมูลใหม่และแสดงผลทุก mode
    }
}