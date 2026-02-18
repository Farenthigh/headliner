using UnityEngine;

// สร้างตัวเลือกประเภทคำถาม
public enum QuestionType 
{
    MultipleChoice, // แบบ 4 ตัวเลือก
    TextInput       // แบบพิมพ์ตอบ
}

[System.Serializable]
public class QuestionStore 
{
    [TextArea]
    public string questionText;
    public QuestionType type; 
    public string[] choices;
    public int correctChoiceIndex;
    public string correctStringAnswer;
}