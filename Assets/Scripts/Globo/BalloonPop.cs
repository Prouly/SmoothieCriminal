using System.Collections;
using UnityEngine;

/**
 * Proyecto: Smoothie Criminal
 * Descripción: Minijuego de inflar el globo con control de tiempo fluido para la UI.
 * Autor: Luis Miguel Muñoz Vega
 * Última modificación: 04/05/2026 (Álvaro Muñoz Adán) -> Corrección Timer
 */

public class BalloonPop : MonoBehaviour
{
    #region Variables de Configuración
    [Header("Ajustes del Globo")]
    [SerializeField] private GameObject Balloon;    
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
    private float tiempoRestante; 
    #endregion

    void Start()
    {
        tiempoRestante = timer;
        
        if (Balloon != null && balloonSprites.Length > 0)
        {
            Balloon.GetComponent<SpriteRenderer>().sprite = balloonSprites[0];
        }
    }

    void Update()
    {
        if (gameFinished) return;

        // Restar el tiempo real cada frame
        tiempoRestante -= Time.deltaTime;

        // Comprobar si se acabó el tiempo
        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            FinalizarJuego(false);
            return;
        }

        // Lógica de inflar el globo
        if (Input.GetKeyDown(KeyCode.Space) && Balloon != null)
        {
            spaceCount++;
            Balloon.transform.localScale += new Vector3(scaleIncrement, scaleIncrement, scaleIncrement);
            
            int totalSprites = balloonSprites.Length;
            int index = Mathf.FloorToInt((float)spaceCount / pressesToPop * totalSprites);
            index = Mathf.Clamp(index, 0, totalSprites - 1);
            Balloon.GetComponent<SpriteRenderer>().sprite = balloonSprites[index];
            
            if (spaceCount >= pressesToPop)
            {
                FinalizarJuego(true);
            }
        }
    }

    private void FinalizarJuego(bool victoria)
    {
        gameFinished = true;
        if (victoria)
        {
            Balloon.GetComponent<SpriteRenderer>().sprite = explosionSprite;
            GameManager.instancia.Ganar();
            Debug.Log("¡Has ganado!");
        }
        else
        {
            GameManager.instancia.Perder();
            Debug.Log("¡Has perdido por tiempo!");
        }
    }

    #region Getters para UI
    public float ObtenerTiempoLimite() => timer;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
    #endregion
}