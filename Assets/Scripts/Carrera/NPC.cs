/**
 * Proyecto: Smoothie Criminal
 * Autor: Luis Miguel Muñoz Vega
 * Descripción: Gestiona el avance del NPC, deteniéndolo al finalizar la carrera.
 * Última modificación: 26/04/2026 (Álvaro Muñoz Adán)
 */

using UnityEngine;

public class NPC : MonoBehaviour
{
    #region Variables de Configuración
    public float velocidad = 5f;
    private CarreraLogic carreraLogic;
    #endregion

    void Start()
    {
        // Buscamos la referencia al core del juego para saber cuándo empezar
        carreraLogic = Object.FindFirstObjectByType<CarreraLogic>();
    }

    void Update()
    {
        // 1. Si no hay referencia a la lógica, no nos movemos por seguridad
        if (carreraLogic == null) return;

        // 2. Si la intro NO ha finalizado, no nos movemos
        if (!carreraLogic.ObtenerIntroFinalizada()) return;

        // 3. Si el juego ya terminó, nos detenemos
        if (carreraLogic.EstaJuegoTerminado()) return;

        // Movimiento normal
        transform.Translate(Vector2.right * velocidad * Time.deltaTime);
    }
}