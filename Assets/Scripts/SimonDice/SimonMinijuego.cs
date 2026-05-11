using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Yeray Fernández Jimenez
 * Descripción: El jugador debe repetir la secuencia correcta antes de que termine el tiempo.
 * Última modificación: 10/05/2026 Álvaro Muñoz Adán -> Cambio en la lógica para utilizar spriteRenderers y utilización de sonidos para representar cuando se acierta o se falla
 */

[RequireComponent(typeof(AudioSource))]
public class SimonMinijuego : MonoBehaviour
{
    private enum EstadoJuego { EsperandoInput, Finalizado }
    private EstadoJuego estadoActual;

    [Header("Referencias Visuales")]
    public SpriteRenderer[] renderersFlechas;    // 4 flechas
    public SpriteRenderer[] renderersPersonajes; // 3 personajes (Izq, Centro, Der)

    [Header("Sprites de Flechas (Arrays de 3: 0=N, 1=E, 2=A)")]
    public Sprite[] spritesArriba;
    public Sprite[] spritesAbajo;
    public Sprite[] spritesIzquierda;
    public Sprite[] spritesDerecha;

    [Header("Sprites de Personajes (Arrays de 3: 0=N, 1=E, 2=A)")]
    public Sprite[] spritesPjIzquierda;
    public Sprite[] spritesPjCentro; // El de los dos carteles
    public Sprite[] spritesPjDerecha;

    [Header("UI")]
    public TMP_Text textoIntroduccion;
    public TMP_Text textoResultado;

    [Header("Audio")]
    private AudioSource miAudioSource;
    public AudioClip sonidoVictoria;
    public AudioClip sonidoDerrota;

    [Header("Configuración")]
    public int longitudSecuencia = 4;
    public float tiempoLimite = 7f;

    private List<KeyCode> secuencia = new List<KeyCode>();
    private int indiceActual;
    private float tiempoRestante;
    private bool terminado;

    #region Getters para UI
    public float ObtenerTiempoLimite() => tiempoLimite;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
    #endregion

    void Awake() => miAudioSource = GetComponent<AudioSource>();

    void Start() => IniciarJuego();

    void IniciarJuego()
    {
        estadoActual = EstadoJuego.EsperandoInput;
        terminado = false;
        indiceActual = 0;
        tiempoRestante = tiempoLimite;

        if (textoResultado != null) textoResultado.gameObject.SetActive(false);
        if (textoIntroduccion != null) textoIntroduccion.gameObject.SetActive(true);
        if (textoIntroduccion != null) textoIntroduccion.text = "REPITE LA SECUENCIA";

        GenerarSecuencia();
        SetEstadoInicial();
    }

    void GenerarSecuencia()
    {
        secuencia.Clear();
        KeyCode[] opciones = { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };
        for (int i = 0; i < longitudSecuencia; i++)
            secuencia.Add(opciones[Random.Range(0, opciones.Length)]);
    }

    void SetEstadoInicial()
    {
        // Flechas a estado 0 (Normal) y con su icono de dirección
        for (int i = 0; i < renderersFlechas.Length; i++)
        {
            renderersFlechas[i].sprite = GetSpriteFlecha(secuencia[i], 0);
            renderersFlechas[i].color = Color.white;
        }

        // Personajes a estado 0 (Normal)
        if (renderersPersonajes.Length >= 3)
        {
            renderersPersonajes[0].sprite = spritesPjIzquierda[0];
            renderersPersonajes[1].sprite = spritesPjCentro[0];
            renderersPersonajes[2].sprite = spritesPjDerecha[0];
        }
    }

    Sprite GetSpriteFlecha(KeyCode tecla, int estado)
    {
        // estado: 0 normal, 1 error, 2 acierto
        switch (tecla)
        {
            case KeyCode.UpArrow: return spritesArriba[estado];
            case KeyCode.DownArrow: return spritesAbajo[estado];
            case KeyCode.LeftArrow: return spritesIzquierda[estado];
            case KeyCode.RightArrow: return spritesDerecha[estado];
            default: return null;
        }
    }

    void Update()
    {
        if (terminado) return;
        ManejarTiempo();
        if (estadoActual == EstadoJuego.EsperandoInput) ManejarEntrada();
    }

    void ManejarTiempo()
    {
        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0) Finalizar(false, "TIEMPO AGOTADO");
    }

    void ManejarEntrada()
    {
        KeyCode tecla = DetectarTeclaFlecha();
        if (tecla == KeyCode.None) return;

        if (tecla == secuencia[indiceActual])
        {
            // ACIERTO PARCIAL
            CambiarEstadoElemento(indiceActual, 2);
            indiceActual++;

            if (indiceActual >= secuencia.Count) Finalizar(true, "CONSEGUIDO");
        }
        else
        {
            // ERROR
            CambiarEstadoElemento(indiceActual, 1);
            Finalizar(false, "FALLASTE");
        }
    }

    void CambiarEstadoElemento(int idxFlecha, int estado)
    {
        // 1. Cambiar la flecha
        renderersFlechas[idxFlecha].sprite = GetSpriteFlecha(secuencia[idxFlecha], estado);

        // 2. Cambiar el personaje correspondiente
        int idxPj = 0;
        if (idxFlecha == 0) idxPj = 0;      // Izq
        else if (idxFlecha == 1 || idxFlecha == 2) idxPj = 1; // Centro (Cartel 2 y 3)
        else if (idxFlecha == 3) idxPj = 2; // Der

        if (idxPj == 0) renderersPersonajes[0].sprite = spritesPjIzquierda[estado];
        else if (idxPj == 1) renderersPersonajes[1].sprite = spritesPjCentro[estado];
        else if (idxPj == 2) renderersPersonajes[2].sprite = spritesPjDerecha[estado];
    }

    void Finalizar(bool gana, string mensaje)
    {
        if (terminado) return;
        terminado = true;
        estadoActual = EstadoJuego.Finalizado;

        // UI: Ocultar instrucción y mostrar resultado
        if (textoIntroduccion != null) textoIntroduccion.gameObject.SetActive(false);
        if (textoResultado != null)
        {
            textoResultado.gameObject.SetActive(true);
            textoResultado.text = mensaje;
        }

        if (gana)
        {
            if (sonidoVictoria != null) miAudioSource.PlayOneShot(sonidoVictoria);
            SetTodosLosPersonajes(2); // Todos felices
        }
        else
        {
            if (sonidoDerrota != null) miAudioSource.PlayOneShot(sonidoDerrota);
            SetTodosLosPersonajes(1); // Todos tristes
        }

        if (gana) GameManager.instancia?.Ganar();
        else GameManager.instancia?.Perder();
    }

    void SetTodosLosPersonajes(int estado)
    {
        if (renderersPersonajes.Length < 3) return;
        renderersPersonajes[0].sprite = spritesPjIzquierda[estado];
        renderersPersonajes[1].sprite = spritesPjCentro[estado];
        renderersPersonajes[2].sprite = spritesPjDerecha[estado];
    }

    KeyCode DetectarTeclaFlecha()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) return KeyCode.UpArrow;
        if (Input.GetKeyDown(KeyCode.DownArrow)) return KeyCode.DownArrow;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) return KeyCode.LeftArrow;
        if (Input.GetKeyDown(KeyCode.RightArrow)) return KeyCode.RightArrow;
        return KeyCode.None;
    }
}