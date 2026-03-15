using UnityEngine;

public class HomeScript : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (HomePanel.Instance.GetHomePanel() || BankPanelUI.Instance.GetBankPanel()) return;
        HomePanel.Instance.OnOpenHomePanel();
    }
}