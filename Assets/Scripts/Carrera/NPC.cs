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

    void Update()
    {
        if (carreraLogic != null && carreraLogic.EstaJuegoTerminado()) return;
        // Solo movemos el NPC si la carrera sigue activa
        transform.Translate(Vector2.right * velocidad * Time.deltaTime);
    }
}