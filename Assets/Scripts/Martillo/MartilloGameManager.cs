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

    private float zonaMin;
    private float zonaMax;
    
    #region Variables de Estado
    private bool gameFinished = false;
    private float tiempoRestante; 
    private bool subiendo = true;
    private float valorFuerza = 0f;
    #endregion

    #region Getters para UI
    public float ObtenerTiempoLimite() => timer;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
    #endregion

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        tiempoRestante = timer;
        gameFinished = false;
        
        // Estado inicial
        if (textoIndicaciones != null) textoIndicaciones.text = "Para la barra en la zona verde";
        if (imagenJugador != null && spriteNormal != null) imagenJugador.sprite = spriteNormal;

        GenerarZona();
    }

    private void Update()
    {
        if (gameFinished) return;
        
        ActualizarBarra();
        ManejarEntrada();
        
        tiempoRestante -= Time.deltaTime;
        
        if (tiempoRestante <= 0f)
        {
            tiempoRestante = 0f;
            FinalizarJuego(false, true); // Perder por tiempo
        }
    }

    private void ManejarEntrada()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Comprobar si el valor está dentro de la zona verde
            bool acierto = (valorFuerza >= zonaMin && valorFuerza <= zonaMax);
            FinalizarJuego(acierto, false);
        }
    }

    private void ActualizarBarra()
    {
        if (subiendo)
        {
            valorFuerza += velocidad * Time.deltaTime;
            if (valorFuerza >= 1f)
            {
                valorFuerza = 1f;
                subiendo = false;
            }
        }
        else
        {
            valorFuerza -= velocidad * Time.deltaTime;
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
    
    private void FinalizarJuego(bool victoria, bool porTiempo)
    {
        if (gameFinished) return;
        gameFinished = true;

        if (victoria)
        {
            Debug.Log("SISTEMA: ¡Ganaste!");
            if (textoIndicaciones != null) textoIndicaciones.text = "GANASTE";
            if (imagenJugador != null && spriteVictoria != null) imagenJugador.sprite = spriteVictoria;
            if (clipVictoria != null) audioSource.PlayOneShot(clipVictoria);
            
            GameManager.instancia.Ganar();
        }
        else
        {
            Debug.Log("SISTEMA: Perdiste");
            
            // Cambiar texto según el motivo
            if (textoIndicaciones != null)
                textoIndicaciones.text = porTiempo ? "TIEMPO AGOTADO" : "FALLASTE";

            if (imagenJugador != null && spriteDerrota != null) imagenJugador.sprite = spriteDerrota;
            if (clipDerrota != null) audioSource.PlayOneShot(clipDerrota);
            
            GameManager.instancia.Perder();
        }
    }
}