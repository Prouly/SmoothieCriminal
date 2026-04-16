using System.Collections;
using UnityEngine;

public class BalloonPop : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;    
    [SerializeField] private float scaleIncrement = 0.1f;
    [SerializeField] private int pressesToPop = 10;        
    [SerializeField] private float timer = 7f;            
    private int spaceCount = 0;  
    private bool gameFinished = false;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite explosionSprite;
    [SerializeField] private Sprite[] balloonSprites;
    void Start()
    {
        StartCoroutine(TimerCoroutine());
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetObject.GetComponent<SpriteRenderer>().sprite = balloonSprites[0];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && targetObject != null && !gameFinished)
        {
            spaceCount++;
            
            targetObject.transform.localScale += new Vector3(scaleIncrement, scaleIncrement, scaleIncrement);
            
            // Calcular en qué tercio estamos (cambio de sprites)
            int totalSprites = balloonSprites.Length;
            int index = Mathf.FloorToInt((float)spaceCount / pressesToPop * totalSprites);

            // Asegurar que no se pase del índice máximo
            index = Mathf.Clamp(index, 0, totalSprites - 1);

            targetObject.GetComponent<SpriteRenderer>().sprite = balloonSprites[index];
            
            if (spaceCount >= pressesToPop)
            {
                targetObject.GetComponent<SpriteRenderer>().sprite = explosionSprite;
                gameFinished = true;
                GameManager.instancia.Ganar();
                Debug.Log("¡Has ganado!");
            }
        }
    }

    private IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(timer);

        if (!gameFinished)
        {
            gameFinished = true;
            if (targetObject != null && targetObject.activeSelf)
            {
                GameManager.instancia.Perder();
                Debug.Log("¡Has perdido!");
            }
        }
    }
}
