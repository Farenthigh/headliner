using UnityEngine;
using TMPro;
using System.Data;

public class Calculator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private GameObject calculatorPanel;
    
    void Start()
    {
    calculatorPanel.SetActive(false);
    }
    
    public void OpenCalculator()
    {
    calculatorPanel.SetActive(true);
    }

    public void CloseCalculator()
    {
    calculatorPanel.SetActive(false);
    }

    

    private string expression = "";

    // กดตัวเลข
    public void AddNumber(string number)
    {
        expression += number;
        displayText.text = expression;
    }

    // กดเครื่องหมาย
    public void SetOperator(string op)
    {
        if (expression.Length == 0) return;

        char lastChar = expression[expression.Length - 1];

        // กันกดเครื่องหมายซ้ำ
        if (IsOperator(lastChar))
        {
            expression = expression.Substring(0, expression.Length - 1);
        }

        expression += op;
        displayText.text = expression;
    }

    // ปุ่มจุดทศนิยม
    public void AddDot()
    {
        if (expression == "" || IsOperator(expression[expression.Length - 1]))
        {
            expression += "0.";
        }
        else
        {
            string currentNumber = GetCurrentNumber();
            if (!currentNumber.Contains("."))
            {
                expression += ".";
            }
        }

        displayText.text = expression;
    }

    // ลบทั้งหมด
    public void ClearAll()
    {
        expression = "";
        displayText.text = "";
    }

    // ลบตัวสุดท้าย
    public void DeleteLast()
    {
        if (expression.Length > 0)
        {
            expression = expression.Substring(0, expression.Length - 1);
            displayText.text = expression;
        }
    }

    // คำนวณ
    public void CalculateResult()
    {
        if (expression.Length == 0) return;

        try
        {
            var result = new DataTable().Compute(expression, null);
            expression = result.ToString();
            displayText.text = expression;
        }
        catch
        {
            displayText.text = "Error";
            expression = "";
        }
    }

    // เช็คว่าเป็น operator ไหม
    private bool IsOperator(char c)
    {
        return c == '+' || c == '-' || c == '*' || c == '/';
    }

    // ดึงตัวเลขล่าสุด
    private string GetCurrentNumber()
    {
        string number = "";

        for (int i = expression.Length - 1; i >= 0; i--)
        {
            if (IsOperator(expression[i]))
                break;

            number = expression[i] + number;
        }

        return number;
    }
}
