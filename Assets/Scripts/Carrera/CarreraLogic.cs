using UnityEngine;

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
    }

    void Update()
    {
        if (juegoTerminado) return;

        // --- LÓGICA DE SONIDO AL CORRER ---
        // Detecta si se pulsa la tecla A o la tecla D en este frame
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            if (sonidoPaso != null)
            {
                // Usamos PlayOneShot para que los sonidos puedan solaparse si se pulsa muy rápido
                audioSource.PlayOneShot(sonidoPaso);
            }
        }

        // Descuento de tiempo fluido
        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            Debug.Log("RESULTADO: ¡Tiempo agotado! Perdiste la carrera.");
            FinalizarCarrera(false);
        }
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

    public bool EstaJuegoTerminado() => juegoTerminado;
    #endregion

    #region Getters para UI
    public float ObtenerTiempoLimite() => tiempoLimite;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
    #endregion
}