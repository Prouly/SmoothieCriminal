using UnityEngine;
using TMPro;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Yeray Jimenez Fernández
 * Descripción: Minijuego de Basket con sistema de 4 frames y audio avanzado.
 * Última modificación: 12/05/2026 (Álvaro Muñoz Adán) -> Retraso de sonido de victoria y ocultación de texto.
 */

[RequireComponent(typeof(AudioSource))]
public class BasketMinijuego : MonoBehaviour
{
    private enum EstadoJuego { EsperandoSalto, Saltando, Tirando, Finalizado }
    private EstadoJuego estadoActual;

    [Header("Referencias")]
    public Rigidbody2D rbJugador;
    public Rigidbody2D rbBalon;
    public Transform jugador;
    public Transform balon;
    public Transform objetivoAro;
    public SpriteRenderer jugadorRenderer;
    public Collider2D colJugador; 
    public Collider2D colBalon;   
    public TMP_Text textoInstruccion, textoResultado;

    [Header("Sprites del Jugador (4 Frames)")]
    public Sprite spriteNormal;      
    public Sprite spriteSalto;       
    public Sprite spriteLanzamiento; 
    public Sprite spriteCaida;       

    [Header("Audio")]
    private AudioSource miAudioSource;
    public AudioClip clipVictoria;
    public AudioClip clipDerrota;
    public AudioClip clipCanasta;
    public AudioClip clipTablero;

    [Header("Ajustes de Parábola")]
    public float fuerzaSalto = 10f;
    public float arcoVertical = 12f;  
    public float fuerzaHorizontal = 6f; 
    public float alturaIdealTiro = 1.5f;
    [Range(0.1f, 1f)] public float sensibilidadTiming = 0.5f;

    [Header("Timer")]
    public float tiempoLimite = 7f;
    private float tiempoRestante;
    private bool terminado;

    #region Getters para UI
    public float ObtenerTiempoLimite() => tiempoLimite;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
    #endregion

    void Awake() => miAudioSource = GetComponent<AudioSource>();

    void Start() {
        if (colJugador != null && colBalon != null) {
            Physics2D.IgnoreCollision(colJugador, colBalon, true);
        }
        IniciarJuego();
    }

    void IniciarJuego() {
        estadoActual = EstadoJuego.EsperandoSalto;
        terminado = false;
        tiempoRestante = 7f;
        balon.gameObject.SetActive(false);
        if (jugadorRenderer != null) jugadorRenderer.sprite = spriteNormal;
        textoInstruccion.text = "PULSA ESPACIO PARA SALTAR Y TIRAR";
        textoResultado.text = "";
    }

    void Update() {
        if (terminado) return;
        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0) Finalizar(false, "TIEMPO AGOTADO");

        if (estadoActual == EstadoJuego.EsperandoSalto && Input.GetKeyDown(KeyCode.Space)) Saltar();
        else if (estadoActual == EstadoJuego.Saltando && Input.GetKeyDown(KeyCode.Space)) Tirar();
    }

    void Saltar() {
        estadoActual = EstadoJuego.Saltando;
        rbJugador.linearVelocity = Vector2.up * fuerzaSalto;
        if (jugadorRenderer != null) jugadorRenderer.sprite = spriteSalto;
    }

    void Tirar() {
        estadoActual = EstadoJuego.Tirando;
        if (jugadorRenderer != null) jugadorRenderer.sprite = spriteLanzamiento;
        
        // El texto de instrucción desaparece al tirar
        if (textoInstruccion != null) textoInstruccion.text = ""; 

        balon.position = jugador.position + new Vector3(0.5f, 0.5f, 0); 
        balon.gameObject.SetActive(true);
        rbBalon.bodyType = RigidbodyType2D.Dynamic;

        float direccionX = (objetivoAro.position.x > balon.position.x) ? 1 : -1;
        float diferenciaY = Mathf.Abs(jugador.position.y - alturaIdealTiro);
        float error = diferenciaY * sensibilidadTiming;

        Vector2 impulsoFinal = new Vector2(
            (fuerzaHorizontal * direccionX) - (error * 0.5f), 
            arcoVertical - error 
        );

        rbBalon.linearVelocity = impulsoFinal;
        rbBalon.angularVelocity = -500f; 

        Invoke(nameof(ComprobarFallo), 2.5f);
    }

    // --- NUEVA LÓGICA DE CANASTA CON RETRASO ---
    public void Canasta() { 
        if(!terminado) { 
            terminado = true; // Bloqueamos inmediatamente para evitar múltiples detecciones
            CancelInvoke(nameof(ComprobarFallo)); // Cancelamos el posible fallo por tiempo
            
            if(clipCanasta != null) miAudioSource.PlayOneShot(clipCanasta); 
            
            // Llamamos a la victoria tras retraso de 0.7s
            Invoke(nameof(EjecutarVictoriaRetrasada), 0.7f);
        } 
    }

    void EjecutarVictoriaRetrasada() {
        FinalizarProceso(true, "CANASTA");
    }

    public void ReproducirTablero() { if(clipTablero != null) miAudioSource.PlayOneShot(clipTablero); }
    
    void ComprobarFallo() { if(!terminado) Finalizar(false, "FALLASTE"); }

    // Método estándar para casos de fallo o tiempo agotado
    void Finalizar(bool gana, string mensaje) {
        if (terminado) return;
        terminado = true;
        FinalizarProceso(gana, mensaje);
    }

    // Núcleo de la finalización (donde ocurre lo visual y sonoro)
    void FinalizarProceso(bool gana, string mensaje) {
        if (textoInstruccion != null) textoInstruccion.text = "";
        if (jugadorRenderer != null) jugadorRenderer.sprite = spriteCaida;
        textoResultado.text = mensaje;

        if (gana) {
            if (clipVictoria != null) miAudioSource.PlayOneShot(clipVictoria);
            GameManager.instancia?.Ganar();
        } else {
            if (clipDerrota != null) miAudioSource.PlayOneShot(clipDerrota);
            GameManager.instancia?.Perder();
        }
    }
}