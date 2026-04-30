using UnityEngine;
using TMPro;

public class PenaltyMinijuego : MonoBehaviour
{
    private enum EstadoJuego
    {
        ElegirDireccion,
        Disparando,
        Finalizado
    }

    private EstadoJuego estadoActual;

    [Header("Referencias")]
    public Rigidbody2D rbPelota;
    public Transform pelota;
    public Transform flechaDireccion;
    public Transform portero;
    public TMP_Text textoInstruccion;
    public TMP_Text textoResultado;

    [Header("Dirección")]
    public float anguloMaximo = 55f;
    public float velocidadFlecha = 120f;

    [Header("Disparo")]
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

    private float tiempoRestante;
    private float anguloActual;
    private float anguloSeleccionado;
    private int direccionFlecha = 1;

    private bool terminado;
    private Vector2 posicionInicialPelota;

    void Start()
    {
        posicionInicialPelota = pelota.position;
        IniciarJuego();
    }

    void IniciarJuego()
    {
        estadoActual = EstadoJuego.ElegirDireccion;
        terminado = false;

        tiempoRestante = tiempoLimite;

        pelota.position = posicionInicialPelota;
        rbPelota.linearVelocity = Vector2.zero;
        rbPelota.angularVelocity = 0f;
        rbPelota.bodyType = RigidbodyType2D.Kinematic;

        anguloActual = 0f;
        direccionFlecha = 1;

        flechaDireccion.gameObject.SetActive(true);

        textoResultado.text = "";
        textoInstruccion.text = "Pulsa ESPACIO para disparar";
    }

    void Update()
    {
        if (estadoActual == EstadoJuego.Finalizado)
            return;

        ActualizarTimer();

        if (estadoActual == EstadoJuego.Finalizado)
            return;

        MoverPortero();

        if (estadoActual == EstadoJuego.ElegirDireccion)
            ActualizarDireccion();
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

    void ActualizarDireccion()
    {
        anguloActual += velocidadFlecha * direccionFlecha * Time.deltaTime;

        if (anguloActual >= anguloMaximo)
        {
            anguloActual = anguloMaximo;
            direccionFlecha = -1;
        }
        else if (anguloActual <= -anguloMaximo)
        {
            anguloActual = -anguloMaximo;
            direccionFlecha = 1;
        }

        flechaDireccion.rotation = Quaternion.Euler(0, 0, anguloActual + rotacionBaseFlecha);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            anguloSeleccionado = anguloActual;
            Disparar();
        }
    }

    void Disparar()
    {
        estadoActual = EstadoJuego.Disparando;

        flechaDireccion.gameObject.SetActive(false);
        textoInstruccion.text = "";

        rbPelota.bodyType = RigidbodyType2D.Dynamic;

        Vector2 direccion = Quaternion.Euler(0, 0, anguloSeleccionado) * Vector2.up;
        rbPelota.linearVelocity = direccion.normalized * potenciaFija;

        Invoke(nameof(ComprobarResultado), tiempoParaComprobarFuera);
    }

    void MoverPortero()
    {
        Vector3 pos = portero.position;
        pos.x = Mathf.PingPong(Time.time * velocidadPortero, limiteDerecho - limiteIzquierdo) + limiteIzquierdo;
        portero.position = pos;
    }

    void ComprobarResultado()
    {
        if (!terminado)
            Finalizar(false, "¡FUERA!");
    }

    public void Gol()
    {
        Finalizar(true, "¡GOOOL!");
    }

    public void Parada()
    {
        Finalizar(false, "¡PARADA!");
    }

    void Finalizar(bool gana, string mensaje)
    {
        if (terminado) return;

        terminado = true;
        estadoActual = EstadoJuego.Finalizado;

        CancelInvoke(nameof(ComprobarResultado));

        rbPelota.linearVelocity = Vector2.zero;
        rbPelota.angularVelocity = 0f;

        flechaDireccion.gameObject.SetActive(false);
        textoInstruccion.text = "";
        textoResultado.text = mensaje;

        // Más adelante, aquí irá la llamada al GameManager:
        if (gana) GameManager.instancia.Ganar();
        else GameManager.instancia.Perder();
    }

    // Estos métodos sirven para que una UI de timer pueda leer el tiempo si lo necesita.
    public float ObtenerTiempoLimite() => tiempoLimite;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
}