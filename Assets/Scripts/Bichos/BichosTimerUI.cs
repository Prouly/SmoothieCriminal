using UnityEngine;
using UnityEngine.UI;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Controla visualmente el temporizador del juego de Aplastar Bichos.
 * Última modificación: 26/04/2026
 */
public class BichosTimerUI : MonoBehaviour
{
    [SerializeField] private Image imagenPajita;
    [SerializeField] private BichosLogic logicaBichos;

    void Update()
    {
        if (logicaBichos != null && imagenPajita != null)
        {
            imagenPajita.fillAmount = logicaBichos.ObtenerTiempoRestante() / logicaBichos.ObtenerTiempoLimite();
        }
    }
}