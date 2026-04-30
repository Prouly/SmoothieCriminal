using UnityEngine;
using TMPro;

public class BasketMinijuego : MonoBehaviour
{
    private enum EstadoJuego
    {
        EsperandoSalto,
        Saltando,
        Tirando,
        Finalizado
    }

    private EstadoJuego estadoActual;

    [Header("Referencias")]
    public Rigidbody2D rbJugador;
    public Rigidbody2D rbBalon;
    public Transform jugador;
    public Transform balon;
    public TMP_Text textoInstruccion;
    public TMP_Text textoResultado;

    [Header("Salto")]
    public float fuerzaSalto = 9f;
    public float alturaPerfecta = -0.8f;
    public float margenPerfecto = 0.7f;

    [Header("Tiro")]
    public float fuerzaTiroX = 7f;
    public float fuerzaTiroY = 7f;
    public float tiempoParaComprobarFallo = 2f;

    [Header("Timer")]
    public float tiempoLimite = 7f;

    private float tiempoRestante;
    private bool terminado;

    private Vector2 posicionInicialJugador;
    private Vector2 posicionInicialBalon;

    void Start()
    {
        posicionInicialJugador = jugador.position;
        posicionInicialBalon = balon.position;
        IniciarJuego();
    }

    void IniciarJuego()
    {
        estadoActual = EstadoJuego.EsperandoSalto;
        terminado = false;
        tiempoRestante = tiempoLimite;

        jugador.position = posicionInicialJugador;
        balon.position = posicionInicialBalon;

        rbJugador.linearVelocity = Vector2.zero;
        rbJugador.angularVelocity = 0f;
        rbJugador.bodyType = RigidbodyType2D.Dynamic;

        rbBalon.linearVelocity = Vector2.zero;
        rbBalon.angularVelocity = 0f;
        rbBalon.bodyType = RigidbodyType2D.Kinematic;
        rbBalon.gravityScale = 0f;

        textoResultado.text = "";
        textoInstruccion.text = "Pulsa ESPACIO para saltar";
    }

    void Update()
    {
        if (estadoActual == EstadoJuego.Finalizado)
            return;

        ActualizarTimer();

        if (estadoActual == EstadoJuego.Finalizado)
            return;

        MantenerBalonConJugador();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (estadoActual == EstadoJuego.EsperandoSalto)
            {
                Saltar();
            }
            else if (estadoActual == EstadoJuego.Saltando)
            {
                Tirar();
            }
        }
    }

    void ActualizarTimer()
    {
        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0f)
        {
            tiempoRestante = 0f;
            Finalizar(false, "¡TIEMPO!");
        }
    }

    void MantenerBalonConJugador()
    {
        if (estadoActual == EstadoJuego.Tirando)
            return;

        balon.position = new Vector2(jugador.position.x + 0.45f, jugador.position.y + 0.45f);
    }

    void Saltar()
    {
        estadoActual = EstadoJuego.Saltando;

        rbJugador.linearVelocity = Vector2.zero;
        rbJugador.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);

        textoInstruccion.text = "Pulsa ESPACIO otra vez para tirar";
    }

    void Tirar()
    {
        estadoActual = EstadoJuego.Tirando;
        textoInstruccion.text = "";

        float diferenciaAltura = Mathf.Abs(jugador.position.y - alturaPerfecta);

        float precision = 1f - Mathf.Clamp01(diferenciaAltura / margenPerfecto);

        float ajusteVertical = Mathf.Lerp(-2f, 2f, precision);
        float ajusteHorizontal = Mathf.Lerp(-1.5f, 0f, precision);

        rbBalon.bodyType = RigidbodyType2D.Dynamic;
        rbBalon.gravityScale = 1.5f;

        Vector2 fuerzaFinal = new Vector2(fuerzaTiroX + ajusteHorizontal, fuerzaTiroY + ajusteVertical);
        rbBalon.linearVelocity = fuerzaFinal;

        Invoke(nameof(ComprobarFallo), tiempoParaComprobarFallo);
    }

    void ComprobarFallo()
    {
        if (!terminado)
            Finalizar(false, "¡FALLO!");
    }

    public void Canasta()
    {
        Finalizar(true, "¡CANASTA!");
    }

    void Finalizar(bool gana, string mensaje)
    {
        if (terminado) return;

        terminado = true;
        estadoActual = EstadoJuego.Finalizado;

        CancelInvoke(nameof(ComprobarFallo));

        rbJugador.linearVelocity = Vector2.zero;
        rbBalon.linearVelocity = Vector2.zero;

        textoInstruccion.text = "";
        textoResultado.text = mensaje; 
        
        
        if (gana) GameManager.instancia.Ganar();
        else GameManager.instancia.Perder();
    }
    
   

    public float ObtenerTiempoLimite() => tiempoLimite;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
}