/**
 * Proyecto: Smoothie Criminal
 * Autor: Luis Miguel Muñoz Vega
 * Descripción: Gestiona el avance del NPC, deteniéndolo al finalizar la carrera.
 * Última modificación: 23/04/2026 (Álvaro Muñoz Adán)
 */
using UnityEngine;

public class NPC : MonoBehaviour
{
    #region Variables de Configuración
    public float velocidad = 5f;
    #endregion

    void Update()
    {
        // Solo movemos el NPC si la carrera sigue activa
        transform.Translate(Vector2.right * velocidad * Time.deltaTime);
    }
}