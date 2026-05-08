using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainMenuScriptTest : MonoBehaviour
{
    public float extraWidth = 40f;
    public float extraHeight = 20f;
    public float smoothSpeed = 10f;
    
    [Header("Panels")]
    public GameObject controlesImage;
    public GameObject creditosImage;
    private Button[] buttons;
    private RectTransform hoveredButton;

    private Dictionary<RectTransform, Vector2> originalSizes = new();

    void Start()
    {
        buttons = GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            RectTransform rect = button.GetComponent<RectTransform>();

            originalSizes[rect] = rect.sizeDelta;

            AddHoverEvents(button);
        }
    }

    void Update()
    {
        foreach (Button button in buttons)
        {
            RectTransform rect = button.GetComponent<RectTransform>();

            Vector2 baseSize = originalSizes[rect];

            Vector2 targetSize =
                (rect == hoveredButton)
                ? new Vector2(
                    baseSize.x + extraWidth,
                    baseSize.y + extraHeight
                )
                : baseSize;

            rect.sizeDelta = Vector2.Lerp(
                rect.sizeDelta,
                targetSize,
                Time.deltaTime * smoothSpeed
            );
        }
    }

    void AddHoverEvents(Button button)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener((data) =>
        {
            hoveredButton = button.GetComponent<RectTransform>();
        });

        EventTrigger.Entry exit = new EventTrigger.Entry();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener((data) =>
        {
            hoveredButton = null;
        });

        trigger.triggers.Add(enter);
        trigger.triggers.Add(exit);
    }
    
    public void OnClickButtonMaraton()
    {
        SceneManager.LoadScene("Main");
    }

    public void OnClickButtonNiveles()
    {
        SceneManager.LoadScene("SelectorNiveles");
    }

    public void OnClickButtonControles()
    {
        controlesImage.SetActive(true);
    }
    
    public void OnClickButtonSalirControles()
    {
        controlesImage.SetActive(false);
    }

    public void OnClickButtonCreditos()
    {
        creditosImage.SetActive(true);
    }
    
    public void OnClickButtonSalirCreditos()
    {
        creditosImage.SetActive(false);
    }

    public void OnClickButtonSalir()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}