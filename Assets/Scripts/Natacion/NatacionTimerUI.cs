using UnityEngine;
using UnityEngine.UI;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Controla visualmente el temporizador del juego de Natación.
 * Última modificación: 28/04/2026
 */
public class NatacionTimerUI : MonoBehaviour
{
    [SerializeField] private Image imagenPajita;
    [SerializeField] private NatacionLogic logicaNatacion;

    void Update()
    {
        if (logicaNatacion != null && imagenPajita != null)
        {
            // Calculamos el ratio entre el tiempo actual y el límite
            imagenPajita.fillAmount = logicaNatacion.ObtenerTiempoRestante() / logicaNatacion.ObtenerTiempoLimite();
        }
    }
}