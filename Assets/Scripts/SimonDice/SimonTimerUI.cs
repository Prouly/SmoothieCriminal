using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla visualmente el timer del minijuego Simon Dice.
/// Reduce el fillAmount de la imagen según el tiempo restante.
/// </summary>
public class SimonTimerUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image imagenTimer;
    [SerializeField] private SimonMinijuego logicaSimon;

    void Start()
    {
        // Busca automáticamente la imagen si no está asignada
        if (imagenTimer == null)
            imagenTimer = GetComponent<Image>();

        // Busca automáticamente el SimonManager
        if (logicaSimon == null)
            logicaSimon = FindAnyObjectByType<SimonMinijuego>();
    }

    void Update()
    {
        if (imagenTimer == null || logicaSimon == null)
            return;

        float tiempoMax = logicaSimon.ObtenerTiempoLimite();
        float tiempoActual = logicaSimon.ObtenerTiempoRestante();

        if (tiempoMax > 0f)
        {
            imagenTimer.fillAmount = tiempoActual / tiempoMax;
        }
    }
}