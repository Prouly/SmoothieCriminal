/**
 * Proyecto: Smoothie Criminal
 * Autor: Luis Miguel Muñoz Vega
 * Descripción: Gestiona la meta y expone el estado de finalización de la carrera.
 * Última modificación: 26/04/2026 (Álvaro Muñoz Adán)
 */
using UnityEngine;

public class Meta : MonoBehaviour
{
    private bool carreraYaTieneGanador = false;
    public Sprite spritePerdedor;
    private CarreraLogic carreraLogic;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Buscamos la lógica si no la tenemos
        if (carreraLogic == null) carreraLogic = Object.FindFirstObjectByType<CarreraLogic>();
    
        if (carreraLogic == null || carreraLogic.EstaJuegoTerminado()) return;

        if (other.CompareTag("Player"))
        {
            CambiarSpriteRival("NPC");
            carreraLogic.FinalizarCarrera(true); // Victoria
        }
        else if (other.CompareTag("NPC"))
        {
            CambiarSpriteRival("Player");
            carreraLogic.FinalizarCarrera(false); // Derrota
        }
    }

    private void CambiarSpriteRival(string tagRival)
    {
        GameObject rival = GameObject.FindGameObjectWithTag(tagRival);
        if (rival != null) rival.GetComponent<SpriteRenderer>().sprite = spritePerdedor;
    }
}