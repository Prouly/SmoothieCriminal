using UnityEngine;
using UnityEngine.UI;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Controla visualmente el temporizador del juego de Carrera.
 * Última modificación: 26/04/2026
 */

public class CarreraTimerUI : MonoBehaviour
{
    [SerializeField] private Image imagenPajita;
    [SerializeField] private CarreraLogic logicaCarrera;

    void Start()
    {
        if (imagenPajita == null) imagenPajita = GetComponent<Image>();
        if (logicaCarrera == null) logicaCarrera = Object.FindFirstObjectByType<CarreraLogic>();
    }

    void Update()
    {
        if (logicaCarrera != null && imagenPajita != null)
        {
            float max = logicaCarrera.ObtenerTiempoLimite();
            float actual = logicaCarrera.ObtenerTiempoRestante();
            imagenPajita.fillAmount = (max > 0) ? actual / max : 0;
        }
    }
}