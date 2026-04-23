/**
 * Proyecto: Smoothie Criminal
 * Autor: Luis Miguel Muñoz Vega
 * Descripción: Gestiona la meta y expone el estado de finalización de la carrera.
 * Última modificación: 23/04/2026 (Álvaro Muñoz Adán)
 */
using UnityEngine;

public class Meta : MonoBehaviour
{
    private bool carreraYaTieneGanador = false;
    public Sprite spritePerdedor;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si ya alguien cruzó la meta, ignoramos el resto para el resultado, 
        // pero permitimos que sigan moviéndose.
        if (carreraYaTieneGanador) return;

        carreraYaTieneGanador = true;

        if (other.CompareTag("Player"))
        {
            Debug.Log("¡El Player llegó primero!");
            CambiarSpriteRival("NPC");
            GameManager.instancia.Ganar();
        }
        else if (other.CompareTag("NPC"))
        {
            Debug.Log("El NPC llegó primero.");
            CambiarSpriteRival("Player");
            GameManager.instancia.Perder();
        }
    }

    private void CambiarSpriteRival(string tagRival)
    {
        GameObject rival = GameObject.FindGameObjectWithTag(tagRival);
        if (rival != null) rival.GetComponent<SpriteRenderer>().sprite = spritePerdedor;
    }
}