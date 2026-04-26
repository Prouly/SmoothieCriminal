using UnityEngine;
using UnityEngine.UI;

/**
 * Proyecto: Smoothie Criminal
 * Descripción: Controla visualmente el temporizador del juego de Topos.
 * Autor: Álvaro Muñoz Adán
 * Última modificación: 26/04/2026
 */

public class ToposTimerUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Image imagenPajita; 

    [Header("Referencias de Lógica")]
    [SerializeField] private Topos logicaTopos; 

    void Start()
    {
        if (imagenPajita == null) imagenPajita = GetComponent<Image>();
        if (logicaTopos == null) logicaTopos = FindAnyObjectByType<Topos>();
    }

    void Update()
    {
        if (logicaTopos != null && imagenPajita != null)
        {
            float max = logicaTopos.ObtenerTiempoLimite();
            float actual = logicaTopos.ObtenerTiempoRestante();

            if (max > 0)
            {
                imagenPajita.fillAmount = actual / max;
            }
        }
    }
}