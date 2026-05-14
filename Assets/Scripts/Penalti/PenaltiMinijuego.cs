using System.Collections;
using UnityEngine;
using TMPro;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Yeray
 * Descripción: Minijuego de penaltis con sonidos, mensajes de consola y cambio de sprites.
 * Última modificación: 10/05/2026 (Álvaro Muñoz Adán) -> Implementado cambio de Sprites en caso de Victoria o Derrota, agregados console.log y sonidos
 */

[RequireComponent(typeof(AudioSource))]
public class PenaltiMinijuego : MonoBehaviour
{
    private enum EstadoJuego
    {
        ElegirDireccion,
        Disparando,
        Finalizado
    }

    private EstadoJuego estadoActual;

    [Header("Referencias UI")]
    public TMP_Text textoInstruccion;
    public TMP_Text textoResultado;

    [Header("Referencias Objetos")]
    public Rigidbody2D rbPelota;
    public Transform pelota;
    public Transform flechaDireccion;
    public SpriteRenderer porteroRenderer;

    [Header("Configuración Visual (Sprites)")]
    public Sprite spritePorteroNormal;
    public Sprite spritePorteroVictoria; 
    public Sprite spritePorteroDerrota;  

    [Header("Audio (AudioClips Directos)")]
    private AudioSource miAudioSource;
    public AudioClip clipDisparo;
    public AudioClip clipVictoria;
    public AudioClip clipDerrota;

    [Header("Configuración de Juego")]
    public float anguloMaximo = 55f;
    public float velocidadFlecha = 120f;
    public float potenciaFija = 10f;
    public float tiempoParaComprobarFuera = 1.5f;

    [Header("Portero")]
    public float velocidadPortero = 3f;
    public float limiteIzquierdo = -2.2f;
    public float limiteDerecho = 2.2f;

    [Header("Timer")]
    public float tiempoLimite = 7f;

    [Header("Ajuste visual de la flecha")]
    public float rotacionBaseFlecha = 90f;
    
    [Header("Panel de Inicio")]
    public GameObject panelControles;
    public float tiempoEsperaIntro = 4f;
    private bool introFinalizada = false;

    private float tiempoRestante;
    private float anguloActual;
    private int direccionFlecha = 1;
    private bool terminado = false;

    #region Getters para UI
    public float ObtenerTiempoLimite() => tiempoLimite;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
    #endregion

    void Awake()
    {
        miAudioSource = GetComponent<AudioSource>();
        miAudioSource.playOnAwake = false;
    }

    void Start()
    {
        estadoActual = EstadoJuego.ElegirDireccion;
        tiempoRestante = tiempoLimite;
        anguloActual = 0f;
        
        if(spritePorteroNormal != null) porteroRenderer.sprite = spritePorteroNormal;
        
        if(textoInstruccion != null) textoInstruccion.text = "¡ESPACIO PARA DISPARAR!";
        if(textoResultado != null) textoResultado.text = "";
        
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
        // Si el juego ha terminado, no hacemos nada más
        if (!introFinalizada || terminado) return;
        
        // Lógica según el estado
        switch (estadoActual)
        {
            case EstadoJuego.ElegirDireccion:
                OscilarFlecha();
                if (Input.GetKeyDown(KeyCode.Space)) Disparar();
                break;

            case EstadoJuego.Disparando:
                // Aquí ya no hace falta poner MoverPortero porque está arriba
                break;
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
        
        // Manejar el tiempo (siempre corre mientras no termine)
        ManejarTiempo();

        // MOVER PORTERO
        MoverPortero();
    }

    void ManejarTiempo()
    {
        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0)
        {
            Debug.Log("PERDIDO: Se agotó el tiempo.");
            Finalizar(false, "¡TIEMPO AGOTADO!");
        }
    }

    void OscilarFlecha()
    {
        anguloActual += direccionFlecha * velocidadFlecha * Time.deltaTime;
        if (Mathf.Abs(anguloActual) >= anguloMaximo)
        {
            direccionFlecha *= -1;
            anguloActual = Mathf.Clamp(anguloActual, -anguloMaximo, anguloMaximo);
        }
        flechaDireccion.localRotation = Quaternion.Euler(0, 0, anguloActual + rotacionBaseFlecha);
    }

    void Disparar()
    {
        estadoActual = EstadoJuego.Disparando;
        if (clipDisparo != null) miAudioSource.PlayOneShot(clipDisparo);

        Vector2 direccion = Quaternion.Euler(0, 0, anguloActual) * Vector2.up;
        rbPelota.linearVelocity = direccion.normalized * potenciaFija;

        Invoke(nameof(ComprobarResultadoFuera), tiempoParaComprobarFuera);
    }

    void MoverPortero()
    {
        Vector3 pos = porteroRenderer.transform.position;
        pos.x = Mathf.PingPong(Time.time * velocidadPortero, limiteDerecho - limiteIzquierdo) + limiteIzquierdo;
        porteroRenderer.transform.position = pos;
    }

    void ComprobarResultadoFuera()
    {
        if (!terminado)
        {
            Debug.Log("PERDIDO: El balón se fue fuera.");
            Finalizar(false, "¡FUERA!");
        }
    }

    public void Gol()
    {
        Debug.Log("GANADO: ¡Gol marcado!");
        Finalizar(true, "¡GOOOL!");
    }

    public void Parada()
    {
        Debug.Log("PERDIDO: El portero ha parado el balón.");
        Finalizar(false, "¡PARADA!");
    }

    void Finalizar(bool gana, string mensaje)
    {
        if (terminado) return;
        terminado = true;
        estadoActual = EstadoJuego.Finalizado;

        CancelInvoke(nameof(ComprobarResultadoFuera));

        rbPelota.linearVelocity = Vector2.zero;
        rbPelota.angularVelocity = 0f;
        flechaDireccion.gameObject.SetActive(false);
        
        if(textoInstruccion != null) textoInstruccion.text = "";
        if(textoResultado != null) textoResultado.text = mensaje;

        if (gana)
        {
            if (clipVictoria != null) miAudioSource.PlayOneShot(clipVictoria);
            if (spritePorteroVictoria != null) porteroRenderer.sprite = spritePorteroVictoria;
            if (GameManager.instancia != null) GameManager.instancia.Ganar();
        }
        else
        {
            if (clipDerrota != null) miAudioSource.PlayOneShot(clipDerrota);
            if (spritePorteroDerrota != null) porteroRenderer.sprite = spritePorteroDerrota;
            if (GameManager.instancia != null) GameManager.instancia.Perder();
        }
    }
}