using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Minijuego Simon Dice.
/// El jugador debe repetir la secuencia correcta antes de que termine el tiempo.
/// </summary>
public class SimonMinijuego : MonoBehaviour
{
    private enum EstadoJuego
    {
        EsperandoInput,
        Finalizado
    }

    private EstadoJuego estadoActual;

    [Header("UI")]
    public Image[] flechas;

    public TMP_Text textoInstruccion;
    public TMP_Text textoResultado;

    [Header("Sprites")]
    public Sprite spriteNormal;
    public Sprite spriteCorrecto;
    public Sprite spriteIncorrecto;

    [Header("Configuración")]
    public int longitudSecuencia = 4;
    public float tiempoLimite = 7f;

    private List<KeyCode> secuencia = new List<KeyCode>();

    private int indiceActual;
    private float tiempoRestante;
    private bool terminado;

    void Start()
    {
        IniciarJuego();
    }

    void IniciarJuego()
    {
        estadoActual = EstadoJuego.EsperandoInput;
        terminado = false;

        indiceActual = 0;
        tiempoRestante = tiempoLimite;

        textoResultado.text = "";
        textoInstruccion.text = "Repite la secuencia";

        // Reset visual
        foreach (Image flecha in flechas)
        {
            flecha.sprite = spriteNormal;
            flecha.color = Color.white;
        }

        GenerarSecuencia();
        MostrarSecuenciaEnPantalla();
    }

    void Update()
    {
        if (estadoActual == EstadoJuego.Finalizado)
            return;

        ActualizarTimer();
        DetectarInput();
    }

    void ActualizarTimer()
    {
        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0f)
        {
            tiempoRestante = 0f;
            Finalizar(false, "TIEMPO");
        }
    }

    void GenerarSecuencia()
    {
        secuencia.Clear();

        KeyCode[] opciones =
        {
            KeyCode.UpArrow,
            KeyCode.DownArrow,
            KeyCode.LeftArrow,
            KeyCode.RightArrow
        };

        for (int i = 0; i < longitudSecuencia; i++)
        {
            KeyCode direccionAleatoria = opciones[Random.Range(0, opciones.Length)];
            secuencia.Add(direccionAleatoria);
        }
    }

    void MostrarSecuenciaEnPantalla()
    {
        for (int i = 0; i < flechas.Length; i++)
        {
            if (i < secuencia.Count)
            {
                flechas[i].gameObject.SetActive(true);

                flechas[i].rectTransform.localRotation =
                    Quaternion.Euler(0, 0, ObtenerRotacion(secuencia[i]));
            }
            else
            {
                flechas[i].gameObject.SetActive(false);
            }
        }
    }

    void DetectarInput()
    {
        KeyCode tecla = ObtenerTeclaPulsada();

        if (tecla == KeyCode.None)
            return;

        // Correcto
        if (tecla == secuencia[indiceActual])
        {
            flechas[indiceActual].sprite = spriteCorrecto;

            indiceActual++;

            if (indiceActual >= secuencia.Count)
            {
                Finalizar(true, "CORRECTO");
            }
        }
        // Incorrecto
        else
        {
            flechas[indiceActual].sprite = spriteIncorrecto;

            Finalizar(false, "INCORRECTO");
        }
    }

    KeyCode ObtenerTeclaPulsada()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            return KeyCode.UpArrow;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            return KeyCode.DownArrow;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            return KeyCode.LeftArrow;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            return KeyCode.RightArrow;

        return KeyCode.None;
    }

    float ObtenerRotacion(KeyCode tecla)
    {
        // El sprite original apunta a la derecha
        switch (tecla)
        {
            case KeyCode.RightArrow:
                return 0f;

            case KeyCode.UpArrow:
                return 90f;

            case KeyCode.LeftArrow:
                return 180f;

            case KeyCode.DownArrow:
                return -90f;

            default:
                return 0f;
        }
    }

    void Finalizar(bool gana, string mensaje)
    {
        if (terminado) return;

        terminado = true;
        estadoActual = EstadoJuego.Finalizado;

        textoResultado.text = mensaje;
        textoInstruccion.text = "";

        
        if (GameManager.instancia != null)
        {
            if (gana)
                GameManager.instancia.Ganar();
            else
                GameManager.instancia.Perder();
        }
        
    }

    // Timer UI
    public float ObtenerTiempoLimite() => tiempoLimite;

    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
}