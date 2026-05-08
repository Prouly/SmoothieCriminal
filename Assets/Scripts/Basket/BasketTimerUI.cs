using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla la barra visual del timer del minijuego de basket.
/// Reduce el fillAmount según el tiempo restante.
/// </summary>
public class BasketTimerUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image imagenTimer;
    [SerializeField] private BasketMinijuego logicaBasket;

    void Start()
    {
        // Si no se asigna manualmente, lo busca automáticamente
        if (imagenTimer == null)
            imagenTimer = GetComponent<Image>();

        if (logicaBasket == null)
            logicaBasket = FindAnyObjectByType<BasketMinijuego>();
    }

    void Update()
    {
        if (imagenTimer == null || logicaBasket == null)
            return;

        float tiempoMax = logicaBasket.ObtenerTiempoLimite();
        float tiempoActual = logicaBasket.ObtenerTiempoRestante();

        if (tiempoMax > 0f)
        {
            imagenTimer.fillAmount = tiempoActual / tiempoMax;
        }
    }
}