using System.Collections;
using UnityEngine;

<<<<<<< HEAD
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
=======
/**
 * Proyecto: Smoothie Criminal
 * Descripción: Minijuego de inflar el globo con control de tiempo fluido para la UI.
 * Autor: Luis Miguel Muñoz Vega
 * Última modificación: 26/04/2026 (Álvaro Muñoz Adán)
 */

public class BalloonPop : MonoBehaviour
{
    #region Variables de Configuración
    [Header("Ajustes del Globo")]
    [SerializeField] private GameObject targetObject;    
    [SerializeField] private float scaleIncrement = 0.1f;
    [SerializeField] private int pressesToPop = 10;        
    [SerializeField] private Sprite explosionSprite;
    [SerializeField] private Sprite[] balloonSprites;

    [Header("Ajustes de Tiempo")]
    [SerializeField] private float timer = 7f; 
    #endregion

    #region Variables de Estado
    private int spaceCount = 0;  
    private bool gameFinished = false;
    private float tiempoRestante; // Control para la pajita UI
    #endregion

    void Start()
    {
        tiempoRestante = timer;
        
        if (targetObject != null && balloonSprites.Length > 0)
        {
            targetObject.GetComponent<SpriteRenderer>().sprite = balloonSprites[0];
        }
>>>>>>> 4783fef24e3419b985ad333cedda73547493c94a
    }

    void Update()
    {
<<<<<<< HEAD
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
=======
        if (gameFinished) return;

        // 1. Gestión del Tiempo
        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            FinalizarJuego(false);
            return;
        }

        // 2. Lógica de Inflado
        if (Input.GetKeyDown(KeyCode.Space) && targetObject != null)
        {
            InflarGlobo();
        }
    }

    private void InflarGlobo()
    {
        spaceCount++;
        targetObject.transform.localScale += new Vector3(scaleIncrement, scaleIncrement, scaleIncrement);
        
        // Cambio de sprites basado en progreso
        int totalSprites = balloonSprites.Length;
        int index = Mathf.FloorToInt((float)spaceCount / pressesToPop * totalSprites);
        index = Mathf.Clamp(index, 0, totalSprites - 1);
        targetObject.GetComponent<SpriteRenderer>().sprite = balloonSprites[index];
        
        if (spaceCount >= pressesToPop)
        {
            targetObject.GetComponent<SpriteRenderer>().sprite = explosionSprite;
            FinalizarJuego(true);
        }
    }

    private void FinalizarJuego(bool victoria)
    {
        if (gameFinished) return;
        gameFinished = true;

        if (GameManager.instancia != null)
        {
            if (victoria)
            {
                Debug.Log("¡Has ganado!");
                GameManager.instancia.Ganar();
            }
            else
            {
                Debug.Log("¡Has perdido por tiempo!");
                GameManager.instancia.Perder();
            }
        }
    }

    #region Getters para UI
    public float ObtenerTiempoLimite() => timer;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
    #endregion
}
>>>>>>> 4783fef24e3419b985ad333cedda73547493c94a
