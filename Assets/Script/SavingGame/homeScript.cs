using UnityEngine;
using UnityEngine.EventSystems;

public class HomeScript : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) 
        {
            return;
        }
        
        if (HomePanel.Instance.GetHomePanel() || BankPanelUI.Instance.GetBankPanel()) return;
        HomePanel.Instance.OnOpenHomePanel();
    }
}