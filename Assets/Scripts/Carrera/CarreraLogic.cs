using UnityEngine;
using System.Collections;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Gestiona la lógica central, temporizador y estado de la carrera.
 * Última modificación: 07/05/2026 -> Agregados sonidos de pasos, victoria y derrotar
 */

public class CarreraLogic : MonoBehaviour
{
    #region Variables de Configuración
    [Header("Ajustes de Tiempo")]
    [SerializeField] private float tiempoLimite = 7f;

    [Header("Panel de Inicio")]
    public GameObject panelControles; // Arrastra el Panel desde el Inspector
    public float tiempoEsperaIntro = 4f;
    private bool introFinalizada = false;

    [Header("Ajustes de Sonido")]
    [SerializeField] private AudioClip sonidoPaso;      // Sonido al pulsar A o D
    [SerializeField] private AudioClip sonidoVictoria;  // Sonido al ganar
    [SerializeField] private AudioClip sonidoDerrota;   // Sonido al perder
    #endregion

    #region Variables de Estado
    private float tiempoRestante;
    private bool juegoTerminado = false;
    private AudioSource audioSource; // Referencia al componente de audio
    #endregion

    void Start()
    {
        // Inicialización del AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        tiempoRestante = tiempoLimite;

        // --- LÓGICA DE PANEL DE CONTROLES ---
        if (panelControles != null)
        {
            StartCoroutine(SecuenciaIntro());
        }
        else
        {
            introFinalizada = true;
        }
    }

    void Update()
    {
        // Bloqueamos todo el Update hasta que la intro termine o si el juego ya acabó
        if (!introFinalizada || juegoTerminado) return;

        // --- LÓGICA DE SONIDO AL CORRER ---
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            if (sonidoPaso != null)
            {
                audioSource.PlayOneShot(sonidoPaso);
            }
        }

        // Descuento de tiempo fluido (solo ocurre tras la intro)
        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            Debug.Log("RESULTADO: ¡Tiempo agotado! Perdiste la carrera.");
            FinalizarCarrera(false);
        }
    }

    // Corrutina para gestionar la espera inicial
    IEnumerator SecuenciaIntro()
    {
        introFinalizada = false;
        panelControles.SetActive(true);
        
        // Espera de 4 segundos antes de empezar
        yield return new WaitForSeconds(tiempoEsperaIntro);
        
        panelControles.SetActive(false);
        introFinalizada = true;
    }

    #region Control de Partida
    public void FinalizarCarrera(bool victoria)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        // --- LÓGICA DE SONIDO FINAL ---
        if (victoria && sonidoVictoria != null)
        {
            audioSource.PlayOneShot(sonidoVictoria);
        }
        else if (!victoria && sonidoDerrota != null)
        {
            audioSource.PlayOneShot(sonidoDerrota);
        }

        if (GameManager.instancia != null)
        {
            if (victoria) GameManager.instancia.Ganar();
            else GameManager.instancia.Perder();
        }
    }

    public bool PuedeMoverse()
    {
        return introFinalizada && !juegoTerminado;
    }
    
    public bool EstaJuegoTerminado() => juegoTerminado;
    
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
    
    public float ObtenerTiempoLimite() => tiempoLimite;
    
    public bool ObtenerIntroFinalizada() => introFinalizada;
    #endregion
}