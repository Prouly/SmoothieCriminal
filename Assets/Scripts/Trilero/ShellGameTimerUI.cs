using UnityEngine;
using UnityEngine.UI;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Gestión de temporizador en UI Mini Juego Trileros.
 * Última modificación: 26/04/2026
 */

public class ShellGameTimerUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Image imagenPajita;

    [Header("Referencias de Lógica")]
    [SerializeField] private ShellGameLogic logicaTrileros; 

    void Start()
    {
        if (imagenPajita == null) imagenPajita = GetComponent<Image>();
        if (logicaTrileros == null) logicaTrileros = FindAnyObjectByType<ShellGameLogic>();
    }

    void Update()
    {
        if (logicaTrileros != null && imagenPajita != null)
        {
            float max = logicaTrileros.ObtenerTiempoLimite();
            float actual = logicaTrileros.ObtenerTiempoRestante();

            if (max > 0)
            {
                imagenPajita.fillAmount = actual / max;
            }
        }
    }
}