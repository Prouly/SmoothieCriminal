/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Controla el pato y bloquea disparos si el tiempo ha terminado.
 * Última modificación: 15/04/2026
 */

using UnityEngine;

public class ClickableDuck : MonoBehaviour
{
    #region Variables de Configuración
    [SerializeField] private float velocidad = 5f;
    #endregion

    #region Variables Privadas
    private Vector2 direccion;
    private Rigidbody2D rb;
    private DuckHuntLogic logica;
    #endregion

    #region Métodos de Unity
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        logica = FindFirstObjectByType<DuckHuntLogic>();

        float dirX = Random.Range(0, 2) == 0 ? -1 : 1;
        direccion = new Vector2(dirX, 1).normalized;
    }

    void FixedUpdate()
    {
        // Si el juego termina, el pato deja de moverse (opcional, para feedback visual)
        if (logica != null && logica.EstaJuegoTerminado())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = direccion * velocidad;
    }

    private void OnMouseDown()
    {
        // Bloqueo de disparo si el juego ya terminó por tiempo o victoria previa
        if (logica == null || logica.EstaJuegoTerminado()) return;

        logica.RegistrarBaja();
        Debug.Log("SISTEMA: Impacto confirmado.");
        Destroy(gameObject); 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        direccion = Vector2.Reflect(direccion, normal).normalized;
        direccion.x += Random.Range(-0.1f, 0.1f);
        direccion = direccion.normalized;
    }
    #endregion
}