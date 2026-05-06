using UnityEngine;
using UnityEngine.UI;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Luis Miguel Muñoz Vega
 * Descripción: Controla visualmente el temporizador del juego del Globo.
 * Última modificación: 05/05/2026
 */

public class MartilloTimerUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Image imagenPajita; 

    [Header("Referencias de Lógica")]
    [SerializeField] private MartilloGameManager logicaMartillo; 

    void Start()
    {
        if (imagenPajita == null) imagenPajita = GetComponent<Image>();
        
        if (logicaMartillo == null)
        {
            logicaMartillo = FindAnyObjectByType<MartilloGameManager>();
        }
    }

    void Update()
    {
        if (logicaMartillo != null && imagenPajita != null)
        {
            float max = logicaMartillo.ObtenerTiempoLimite();
            float actual = logicaMartillo.ObtenerTiempoRestante();

            if (max > 0)
            {
                // 1 = Barra llena, 0 = Barra vacía
                imagenPajita.fillAmount = actual / max;
            }
        }
    }
}