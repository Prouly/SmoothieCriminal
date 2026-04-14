using UnityEngine;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Luismi Muñoz
 * Descripción: Gestiona la lógica del NPC del minijuego de Carrera
 * Última modificación: 14/04/2026
 */
public class NPC : MonoBehaviour
{
    // Velocidad a la que se moverá el objeto
    public float velocidad = 5f;

    void Update()
    {
        // Mover el objeto hacia la derecha
        transform.Translate(Vector2.right * velocidad * Time.deltaTime);
    }
}