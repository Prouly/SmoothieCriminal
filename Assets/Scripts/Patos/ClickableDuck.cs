using UnityEngine;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán (Modificado)
 * Descripción: Controla la animación, movimiento y caída del pato tras ser abatido.
 */
public class ClickableDuck : MonoBehaviour
{
    #region Variables de Configuración
    [Header("Movimiento y Animación")]
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float velocidadCaida = 8f;
    [SerializeField] private Sprite[] spritesVuelo; // 2 sprites para las alas
    [SerializeField] private float tiempoEntreFrames = 0.2f;

    [Header("Estado Abatido")]
    [SerializeField] private Sprite spriteAbatido;
    [SerializeField] private AudioClip sonidoMuerte;
    #endregion

    #region Variables Privadas
    private Vector2 direccion;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private DuckHuntLogic logica;
    private bool estaAbatido = false;
    private int frameActual = 0;
    private float cronometroAnimacion;
    private Collider2D collider;
    #endregion

    #region Métodos de Unity
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        logica = FindFirstObjectByType<DuckHuntLogic>();
        collider = GetComponent<BoxCollider2D>();
        // Dirección inicial aleatoria
        float dirX = Random.Range(0, 2) == 0 ? -1 : 1;
        direccion = new Vector2(dirX, 1).normalized;
    }

    void Update()
    {
        if (estaAbatido || (logica != null && logica.EstaJuegoTerminado())) return;

        ManejarAnimacionVuelo();
    }

    void FixedUpdate()
    {
        if (logica != null && logica.EstaJuegoTerminado() && !estaAbatido)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (estaAbatido)
        {
            // Caída vertical en Y al ser abatido
            rb.linearVelocity = Vector2.down * velocidadCaida;
        }
        else
        {
            // Movimiento normal de vuelo
            rb.linearVelocity = direccion * velocidad;
        }
    }

    private void OnMouseDown()
    {
        // Bloqueo si ya está abatido o el juego terminó[cite: 3]
        if (estaAbatido || logica == null || logica.EstaJuegoTerminado()) return;

        AbatirPato();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        // Si choca con el objeto "Down" mientras cae, desaparece
        if (estaAbatido && collision.gameObject.name == "Down")
        {
            Destroy(gameObject);
            return;
        }

        // Rebote normal con los límites si no está abatido[cite: 3]
        if (!estaAbatido)
        {
            Vector2 normal = collision.contacts[0].normal;
            direccion = Vector2.Reflect(direccion, normal).normalized;
            direccion.x += Random.Range(-0.1f, 0.1f);
            direccion = direccion.normalized;
        }
    }
    #endregion

    #region Lógica de Animación y Estado
    private void ManejarAnimacionVuelo()
    {
        if (spritesVuelo.Length < 2) return;

        cronometroAnimacion += Time.deltaTime;
        if (cronometroAnimacion >= tiempoEntreFrames)
        {
            cronometroAnimacion = 0;
            frameActual = (frameActual + 1) % spritesVuelo.Length;
            sr.sprite = spritesVuelo[frameActual];
        }
    }

    private void AbatirPato()
    {
        estaAbatido = true;
        sr.sprite = spriteAbatido;
        collider.enabled = false;
        // Reproducir sonido de muerte persistente siguiendo el estilo del juego
        if (sonidoMuerte != null)
        {
            AudioSource.PlayClipAtPoint(sonidoMuerte, Camera.main.transform.position);
        }

        logica.RegistrarBaja(); //[cite: 2, 3]
        Debug.Log("SISTEMA: Pato abatido e iniciando caída.");
    }
    #endregion
}