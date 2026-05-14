using System.Collections;
using UnityEngine;

/**
 * Proyecto: Smoothie Criminal
 * Descripción: Minijuego de inflar el globo con control de tiempo fluido para la UI.
 * Autor: Luis Miguel Muñoz Vega
 * Última modificación: 07/05/2026 (Álvaro Muñoz Adán) -> Agregado efectos de sonido para inflado y explosion
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

    [Header("Panel de Inicio")]
    public GameObject panelControles; // Arrastra el Panel desde el Inspector
    public float tiempoEsperaIntro = 4f;
    private bool introFinalizada = false;

    [Header("Ajustes de Sonido")]
    [SerializeField] private AudioClip inflateSound; // Sonido al pulsar espacio
    [SerializeField] private AudioClip popSound;     // Sonido al explotar
    [SerializeField] private AudioClip loseSound;    // Sonido al perder por tiempo (AÑADIDO)
    #endregion

    #region Variables de Estado
    private int spaceCount = 0;  
    private bool gameFinished = false;
    private float tiempoRestante; 
    private AudioSource audioSource; // Referencia interna al componente
    #endregion

    void Start()
    {
        tiempoRestante = timer;

        // Inicialización del AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Lógica de inicio con panel
        if (panelControles != null)
        {
            StartCoroutine(SecuenciaIntro());
        }
        else
        {
            introFinalizada = true;
        }
    }

    // Corrutina para gestionar la espera inicial
    IEnumerator SecuenciaIntro()
    {
        introFinalizada = false;
        panelControles.SetActive(true);
        
        yield return new WaitForSeconds(tiempoEsperaIntro);
        
        panelControles.SetActive(false);
        introFinalizada = true;
    }

    void Update()
    {
        // Bloqueo hasta que termine la intro o el juego
        if (!introFinalizada || gameFinished) return;

        // Gestión del tiempo
        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            FinalizarJuego(false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            spaceCount++;

            // --- EFECTO DE SONIDO: INFLADO ---
            if (inflateSound != null)
            {
                audioSource.PlayOneShot(inflateSound);
            }

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
        if (gameFinished) return; // Evita llamadas múltiples
        gameFinished = true;

        if (victoria)
        {
            // --- EFECTO DE SONIDO: EXPLOSIÓN ---
            if (popSound != null)
            {
                audioSource.PlayOneShot(popSound);
            }

            Balloon.GetComponent<SpriteRenderer>().sprite = explosionSprite;
            GameManager.instancia.Ganar();
            Debug.Log("¡Has ganado!");
        }
        else
        {
            // --- EFECTO DE SONIDO: DERROTA (AÑADIDO) ---
            if (loseSound != null)
            {
                audioSource.PlayOneShot(loseSound);
            }

            GameManager.instancia.Perder();
            Debug.Log("¡Has perdido por tiempo!");
        }
    }

    #region Getters para UI
    public float ObtenerTiempoLimite() => timer;

    public float ObtenerTiempoRestante()
    {
        return Mathf.Max(0f, tiempoRestante);
    }
    #endregion
}