using UnityEngine;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Gestión de mancha tras la muerte de los Bichos del minijuego Aplastar Bichos.
 * Última modificación: 26/04/2026
 */
public class Mancha : MonoBehaviour
{
    [SerializeField] private float tiempoVida = 1f;

    void Start()
    {
        // La mancha se destruirá automáticamente tras X segundos
        Destroy(gameObject, tiempoVida);
    }
}