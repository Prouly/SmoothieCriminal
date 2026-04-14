/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Detecta disparos y bloquea la interacción si el juego ha finalizado.
 * Última modificación: 14/04/2026
 */

using UnityEngine;

public class TargetShoot : MonoBehaviour
{
    #region Variables de Configuración
    [SerializeField] private bool esBandido = true;
    #endregion

    #region Variables Privadas
    private VaqueroLogic logica;
    private SpriteRenderer spriteRenderer;
    #endregion

    #region Métodos de Unity
    void Start()
    {
        logica = FindFirstObjectByType<VaqueroLogic>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        AjustarProfundidad();
    }

    /// <summary>
    /// Detecta el click del ratón. Bloquea el disparo si el juego ya terminó.
    /// </summary>
    private void OnMouseDown()
    {
        // Si no hay lógica o el juego ya ha finalizado, no permitimos más disparos
        if (logica == null || logica.EstaJuegoTerminado()) return;

        if (esBandido)
        {
            logica.BandidoEliminado();
        }
        else
        {
            logica.InocenteDisparado();
        }

        // El personaje desaparece solo si el disparo fue válido
        gameObject.SetActive(false);
    }
    #endregion

    #region Ajustes Visuales
    /// <summary>
    /// Ajusta el Order in Layer según la posición Y para evitar solapamientos.
    /// </summary>
    private void AjustarProfundidad()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
        }
    }
    #endregion
}