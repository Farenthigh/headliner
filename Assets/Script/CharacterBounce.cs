using UnityEngine;


public class CharacterBounce : MonoBehaviour
{
   private RectTransform rect;
   private float speed = 3f;
   private float height = 15f;
   private Vector2 startPos;


   void Start()
   {
       rect = GetComponent<RectTransform>();
       startPos = rect.anchoredPosition;
   }


   void Update()
   {
       float y = Mathf.Sin(Time.time * speed) * height;
       rect.anchoredPosition = startPos + new Vector2(0, y);
   }


   void OnEnable()
   {
       rect = GetComponent<RectTransform>();
       startPos = rect.anchoredPosition;
   }


}

