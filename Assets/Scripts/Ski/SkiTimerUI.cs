using UnityEngine;
using UnityEngine.UI;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Luis Miguel Muñoz Vega
 * Descripción: Controla visualmente el temporizador del juego del Ski.
 * Última modificación: 05/05/2026
 */

public class SkiTimerUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Image imagenPajita; 

    [Header("Referencias de Lógica")]
    [SerializeField] private GameManagerSki logicaSki; 

    void Start()
    {
        if (imagenPajita == null) imagenPajita = GetComponent<Image>();
        
        if (logicaSki == null)
        {
            logicaSki = FindAnyObjectByType<GameManagerSki>();
        }
    }

    void Update()
    {
        if (logicaSki != null && imagenPajita != null)
        {
            float max = logicaSki.ObtenerTiempoLimite();
            float actual = logicaSki.ObtenerTiempoRestante();

            if (max > 0)
            {
                // 1 = Barra llena, 0 = Barra vacía
                imagenPajita.fillAmount = actual / max;
            }
        }
    }
}