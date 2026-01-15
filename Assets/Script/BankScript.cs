using Unity.VisualScripting;
using UnityEngine;

public class BankScript : MonoBehaviour
{
    // [SerializeField] private string bankName;
    private void OnMouseDown()
    {
        if (SavingGameUIManager.Instance.GetBankPanel() != true)
            SavingGameUIManager.Instance.OnOpenBankPanel(gameObject.name);
    }
}