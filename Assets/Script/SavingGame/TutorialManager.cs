using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class TutorialStep
{
    [TextArea(3, 10)]
    public string message;      
    public GameObject target;   
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    private bool isTyping = false;
    private string currentFullText = "";
    private Coroutine circleCoroutine;

    [Header("UI Elements")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private RectTransform highlightCircle;
    [SerializeField] private RectTransform Character;
    [Header("Typing Speed")]
    [SerializeField] private float typingSpeed = 0.05f;
    [Header("Bobbing Settings")]
    [SerializeField] private float bobbingAmount = 0.02f; 
    [SerializeField] private float bobbingSpeed = 3f;

    [Header("Settings")]
    [SerializeField] private List<TutorialStep> steps;
    [SerializeField] private float animSpeed = 0.4f;

    private int currentStepIndex = 0;
    private Coroutine typingCoroutine;
    private Vector3 characterOriginalPos;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartTutorial();
    }

    private void Update()
    {
        if(Character != null && tutorialPanel.activeSelf)
        {
            float newY = Character.anchoredPosition.y + Mathf.Sin(Time.unscaledTime * bobbingSpeed) * bobbingAmount;
            Character.anchoredPosition = new Vector2(Character.anchoredPosition.x, newY);
        }
    }

    public void StartTutorial()
    {
        currentStepIndex = 0;
        tutorialPanel.SetActive(true);
        
        SavingGameLogicManager.Instance.SetIsPaused(true);
        
        ShowStep();
    }

    private void ShowStep()
    {
        if (currentStepIndex < steps.Count)
        {
            currentFullText = steps[currentStepIndex].message;

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(currentFullText));

            FocusOnTarget(steps[currentStepIndex].target);
        }
        else
        {
            EndTutorial();
        }
    }
    private IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        tutorialText.text = "";
        foreach (char c in fullText.ToCharArray())
        {
            tutorialText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        isTyping = false;
    }

    private void FocusOnTarget(GameObject target)
    {
        if (target == null)
        {
            highlightCircle.gameObject.SetActive(false);
            return;
        }

        highlightCircle.gameObject.SetActive(true);
        
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position);
        highlightCircle.position = screenPos;

        if (circleCoroutine != null) StopCoroutine(circleCoroutine);
        circleCoroutine = StartCoroutine(AnimateCircle());
    }

    private IEnumerator AnimateCircle()
    {
        highlightCircle.localScale = Vector3.zero;
        float time = 0;
        while (time < animSpeed)
        {
            time += Time.deltaTime;
            float t = time / animSpeed;
            float s = 1f - Mathf.Pow(1f - t, 3f); 
            highlightCircle.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, s);
            yield return null;
        }
        highlightCircle.localScale = Vector3.one;
    }

    public void OnClickNext()
    {
        if(isTyping)
        {
            if(typingCoroutine != null) StopCoroutine(typingCoroutine);
            tutorialText.text = currentFullText;
            isTyping = false;
        }
        else
        {
            currentStepIndex++;
            ShowStep();
        }
    }

    private void EndTutorial()
    {
        tutorialPanel.SetActive(false);
        SavingGameLogicManager.Instance.SetIsPaused(false);
    }
}