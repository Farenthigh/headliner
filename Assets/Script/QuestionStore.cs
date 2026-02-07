using UnityEngine;

[System.Serializable]
public class QuestionStore{
    [TextArea(3,10)]
    public string questionText;
    public string[] choices;
    public int correctchoiceindex;
}