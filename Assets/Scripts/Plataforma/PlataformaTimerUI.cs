using UnityEngine;
using UnityEngine.UI;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Luis Miguel Muñoz Vega
 * Descripción: Controla visualmente el temporizador del juego del Plataforma.
 * Última modificación: 05/05/2026
 */

public class PlataformaTimerUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Image imagenPajita; 

    [Header("Referencias de Lógica")]
    [SerializeField] private PlataformaGameManager logicaPlataforma; 

    void Start()
    {
        if (imagenPajita == null) imagenPajita = GetComponent<Image>();
        
        if (logicaPlataforma == null)
        {
            logicaPlataforma = FindAnyObjectByType<PlataformaGameManager>();
        }
    }

    void Update()
    {
        if (logicaPlataforma != null && imagenPajita != null)
        {
            float max = logicaPlataforma.ObtenerTiempoLimite();
            float actual = logicaPlataforma.ObtenerTiempoRestante();

            if (max > 0)
            {
                // 1 = Barra llena, 0 = Barra vacía
                imagenPajita.fillAmount = actual / max;
            }
        }
    }
}