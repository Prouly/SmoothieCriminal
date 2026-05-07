using UnityEngine;
using UnityEngine.UI;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Controla visualmente el temporizador de la pajita en la UI en el mini juego de Cartas.
 * Última modificación: 07/05/2026
 */

public class CartasTimerUI : MonoBehaviour
{
    [SerializeField] private Image imagenPajita; 
    private CartasGameLogic logicaCartas; 

    void Start()
    {
        if (imagenPajita == null) imagenPajita = GetComponent<Image>();
        logicaCartas = FindAnyObjectByType<CartasGameLogic>();
    }

    void Update()
    {
        if (logicaCartas != null && imagenPajita != null)
        {
            float max = logicaCartas.ObtenerTiempoLimite();
            float actual = logicaCartas.ObtenerTiempoRestante();
            imagenPajita.fillAmount = actual / max;
        }
    }
}