using UnityEngine;
using UnityEngine.UI;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Controla visualmente el temporizador del juego del Globo.
 * Última modificación: 26/04/2026
 */

public class BalloonPopTimerUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Image imagenPajita; 

    [Header("Referencias de Lógica")]
    [SerializeField] private BalloonPop logicaGlobo; 

    void Start()
    {
        if (imagenPajita == null) imagenPajita = GetComponent<Image>();
        
        if (logicaGlobo == null)
        {
            logicaGlobo = FindAnyObjectByType<BalloonPop>();
        }
    }

    void Update()
    {
        if (logicaGlobo != null && imagenPajita != null)
        {
            float max = logicaGlobo.ObtenerTiempoLimite();
            float actual = logicaGlobo.ObtenerTiempoRestante();

            if (max > 0)
            {
                // 1 = Barra llena, 0 = Barra vacía
                imagenPajita.fillAmount = actual / max;
            }
        }
    }
}