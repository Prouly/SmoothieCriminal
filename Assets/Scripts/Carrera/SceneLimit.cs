/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Detiene el movimiento de cualquier objeto que toque este límite.
 * Última modificación: 23/04/2026
 */
using UnityEngine;

public class SceneLimit : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Al chocar con el límite, desactivamos los scripts de movimiento
        PlayerCarrera player = collision.gameObject.GetComponent<PlayerCarrera>();
        if (player != null) player.enabled = false;

        NPC npc = collision.gameObject.GetComponent<NPC>();
        if (npc != null) npc.enabled = false;
        
        Debug.Log($"Advertencia: {collision.gameObject.name} ha alcanzado el límite de la escena.");
    }
}