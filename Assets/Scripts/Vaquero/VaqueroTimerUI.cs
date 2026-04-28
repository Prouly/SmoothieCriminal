using UnityEngine;
using UnityEngine.UI;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Controla visualmente el temporizador del juego de Vaqueros.
 * Última modificación: 26/04/2026
 */

public class VaqueroTimerUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Image imagenPajita; 

    [Header("Referencias de Lógica")]
    [SerializeField] private VaqueroLogic logicaVaqueros; 

    void Start()
    {
        if (imagenPajita == null) imagenPajita = GetComponent<Image>();
        
        if (logicaVaqueros == null)
        {
            logicaVaqueros = FindAnyObjectByType<VaqueroLogic>();
        }
    }

    void Update()
    {
        if (logicaVaqueros != null && imagenPajita != null)
        {
            float max = logicaVaqueros.ObtenerTiempoLimite();
            float actual = logicaVaqueros.ObtenerTiempoRestante();

            if (max > 0)
            {
                // Actualiza el relleno de la imagen (1 = llena, 0 = vacía)
                imagenPajita.fillAmount = actual / max;
            }
        }
    }
}