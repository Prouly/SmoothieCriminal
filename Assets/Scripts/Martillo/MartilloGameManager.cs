using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Luis Miguel Muñoz Vega
 * Descripción: Minijuego de Martillo/Fuerza. El jugador debe detener la barra en la zona verde.
 * Última modificación: 11/05/2026 (Álvaro Muñoz Adán) -> Cambio de lógica para utilizar Image y cambie en función del resultado, se agregan sonido de victoria/derrota
 */

[RequireComponent(typeof(AudioSource))]
public class MartilloGameManager : MonoBehaviour
{
    [Header("Configuración de Juego")]
    [SerializeField] private float timer = 7f;
    [SerializeField] private Scrollbar sliderFuerza;
    [SerializeField] private float velocidad = 1.5f;
    [SerializeField] private RectTransform zonaVerde;
    [SerializeField] private float dimensionZona = 0.2f; //20% de barra

    [Header("Panel de Inicio")]
    public GameObject panelControles;
    public float tiempoEsperaIntro = 4f;
    private bool introFinalizada = false;

    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI textoIndicaciones; // El texto que cambia

    [Header("Visuales del Jugador")]
    [SerializeField] private Image imagenJugador; // La imagen que cambia de sprite
    [SerializeField] private Sprite spriteNormal;
    [SerializeField] private Sprite spriteVictoria;
    [SerializeField] private Sprite spriteDerrota;

    [Header("Audio")]
    private AudioSource audioSource;
    [SerializeField] private AudioClip clipVictoria;
    [SerializeField] private AudioClip clipDerrota;

    private float tiempoRestante;
    private float valorFuerza = 0f;
    private bool subiendo = true;
    private float zonaMin, zonaMax;
    private bool gameFinished = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        tiempoRestante = timer;
        
        if (imagenJugador != null && spriteNormal != null)
            imagenJugador.sprite = spriteNormal;

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
    
    void Update()
    {
        // Bloqueo total hasta que termine la intro o el juego
        if (!introFinalizada || gameFinished) return;

        ManejarTiempo();
        ManejarBarra();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ValidarGolpe();
        }
    }
    
    // Corrutina para gestionar la espera inicial de 4 segundos
    IEnumerator SecuenciaIntro()
    {
        introFinalizada = false;
        panelControles.SetActive(true);
        
        yield return new WaitForSeconds(tiempoEsperaIntro);
        
        panelControles.SetActive(false);
        introFinalizada = true;
        GenerarZona();
    }

    private void ManejarTiempo()
    {
        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0)
        {
            FinalizarJuego(false, true);
        }
    }

    private void ManejarBarra()
    {
        if (subiendo)
        {
            valorFuerza += Time.deltaTime * velocidad;
            if (valorFuerza >= 1f)
            {
                valorFuerza = 1f;
                subiendo = false;
            }
        }
        else
        {
            valorFuerza -= Time.deltaTime * velocidad;
            if (valorFuerza <= 0f)
            {
                valorFuerza = 0f;
                subiendo = true;
            }
        }

        sliderFuerza.value = valorFuerza;
    }
    
    private void GenerarZona()
    {
        zonaMin = Random.Range(0f, 1f - dimensionZona);
        zonaMax = zonaMin + dimensionZona;

        // Ajustar visualmente la zona verde en el Scrollbar
        zonaVerde.anchorMin = new Vector2(zonaMin, 0f);
        zonaVerde.anchorMax = new Vector2(zonaMax, 1f);
    }

    private void ValidarGolpe()
    {
        if (valorFuerza >= zonaMin && valorFuerza <= zonaMax)
        {
            FinalizarJuego(true, false);
        }
        else
        {
            FinalizarJuego(false, false);
        }
    }
    
    private void FinalizarJuego(bool victoria, bool porTiempo)
    {
        if (gameFinished) return;
        gameFinished = true;

        if (victoria)
        {
            Debug.Log("<color=green>¡Has ganado! Golpeaste con el martillo.</color>");
            if (textoIndicaciones != null) textoIndicaciones.text = "GANASTE";
            if (imagenJugador != null && spriteVictoria != null) imagenJugador.sprite = spriteVictoria;
            if (clipVictoria != null) audioSource.PlayOneShot(clipVictoria);
            
            GameManager.instancia.Ganar();
        }
        else
        {
            Debug.Log("<color=red>¡Perdiste!.</color>");
            
            // Cambiar texto según el motivo
            if (textoIndicaciones != null)
                textoIndicaciones.text = porTiempo ? "TIEMPO AGOTADO" : "FALLASTE";

            if (imagenJugador != null && spriteDerrota != null) imagenJugador.sprite = spriteDerrota;
            if (clipDerrota != null) audioSource.PlayOneShot(clipDerrota);
            
            GameManager.instancia.Perder();
        }
    }

    #region Getters para UI
    public float ObtenerTiempoLimite() => timer;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
    #endregion
}