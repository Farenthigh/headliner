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

        // ถ้าเป็นโหมด Saving ให้ซ่อนไปก่อน
        if (currentMode == GameMode.SavingGame)
        {
            myRankBar.SetActive(false); 
            return;
        }

        if (cachedData == null || cachedData.Count == 0)
        {
            myRankBar.SetActive(false);
            return;
        }

        // 2. สร้าง List ใหม่และจัดเรียงคะแนนโหมดภาษีจากมากไปน้อย
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
                    // เรียงเวลาจาก อดีต ไป ปัจจุบัน 
                    // (ใครเวลาเก่ากว่า = ส่งคะแนนเข้า DB ก่อน = ชนะได้ขึ้นแรงค์สูงกว่า)
                    return timeA.CompareTo(timeB); 
                }
            }

            // ถ้าคะแนนไม่เท่ากัน ก็ยึดตามคะแนนปกติได้เลย
            return scoreComparison;
        });

        int myRankIndex = -1;
        int myScore = 0;
        
        // 🚨 ดึงชื่อของเรามาเตรียมเทียบ
        string myName = APIManager.myData.username; 

        // 3. วนลูปสร้าง UI แถวรายชื่อ และเช็กหาชื่อตัวเอง
        for (int i = 0; i < sortedList.Count; i++)
        {
            var data = sortedList[i];
            int currentRank = i + 1; 
            int currentScore = data.tax_game_score;

            // สร้าง UI แต่ละแถว
            var item = Instantiate(rowPrefab, container).GetComponent<LeaderBoardItemUI>();
            item.SetData(currentRank, data.username, currentScore);

            // เช็กว่านี่คือชื่อของเราหรือเปล่า? 
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
        if (currentMode == GameMode.SavingGame)
        {
            cachedData = null;
            UpdateDisplay();
        }
        else if (currentMode == GameMode.TaxGame)
        {
            RefreshData();
        }
    }
}