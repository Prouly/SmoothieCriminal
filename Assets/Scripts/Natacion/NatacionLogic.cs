using UnityEngine;
using System.Collections;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Gestión del minijuego de natación. Control de meta, tiempo y colisión con el pez.
 * Última modificación: 28/04/2026
 */

public class NatacionLogic : MonoBehaviour
{
    #region Variables de Configuración
    [Header("Ajustes de Tiempo")]
    [SerializeField] private float tiempoLimite = 7f;

    [Header("Ajustes de Carrera")]
    [SerializeField] private float xMeta = 8.5f; 
    [SerializeField] private Transform jugador;
    [SerializeField] private Transform pezEnemigo;

    [Header("Efectos de Sonido")]
    [SerializeField] private AudioClip sonidoChapoteo;
    [SerializeField] private AudioClip sonidoVictoria; // Nuevo: Sonido al ganar
    [SerializeField] private AudioClip sonidoDerrota;  // Nuevo: Sonido al perder
    
    [Header("Panel de Inicio")]
    public GameObject panelControles;
    public float tiempoEsperaIntro = 4f;
    private bool introFinalizada = false;
    #endregion

    #region Variables de Estado
    private float tiempoRestante;
    private bool juegoTerminado = false;
    private AudioSource audioSource; // Referencia interna
    #endregion

    #region Métodos de Unity
    void Start()
    {
        // Configuramos el AudioSource automáticamente
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
        
        tiempoRestante = tiempoLimite;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        if (!introFinalizada || juegoTerminado) return;

        ManejarCronometro();
        VerificarCondiciones();
    }
    
    // Corrutina para gestionar la espera inicial de 4 segundos
    IEnumerator SecuenciaIntro()
    {
        introFinalizada = false;
        panelControles.SetActive(true);
        
        yield return new WaitForSeconds(tiempoEsperaIntro);
        
        panelControles.SetActive(false);
        introFinalizada = true;
    }
    #endregion

    #region Lógica de Juego
    private void ManejarCronometro()
    {
        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            Debug.Log("SISTEMA: El tiempo se agotó. ¡El pez te atrapó!");
            FinalizarPartida(false);
        }
    }

    private void VerificarCondiciones()
    {
        if (jugador.position.x >= xMeta)
        {
            FinalizarPartida(true);
        }

        if (pezEnemigo.position.x >= jugador.position.x)
        {
            FinalizarPartida(false);
        }
    }

    public void ReproducirChapoteo()
    {
        if (sonidoChapoteo != null)
        {
            // Mantenemos el método original por si lo usas desde animaciones
            audioSource.PlayOneShot(sonidoChapoteo);
        }
    }

    private void FinalizarPartida(bool victoria)
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

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (GameManager.instancia != null)
        {
            if (victoria) GameManager.instancia.Ganar();
            else GameManager.instancia.Perder();
        }
    }
    
    public bool ObtenerIntroFinalizada() => introFinalizada;
    #endregion

    #region Getters para UI
    public float ObtenerTiempoLimite() => tiempoLimite;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
    public bool EstaJuegoTerminado() => juegoTerminado;
    #endregion
}