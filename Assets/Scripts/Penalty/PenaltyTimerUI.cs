using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla visualmente el temporizador del minijuego de penalti.
/// Lee el tiempo restante desde PenaltyMinijuego y actualiza el fillAmount de la imagen.
/// </summary>
public class PenaltyTimerUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Image imagenTimer;

    [Header("Referencias de Lógica")]
    [SerializeField] private PenaltyMinijuego logicaPenalty;

    void Start()
    {
        if (imagenTimer == null)
            imagenTimer = GetComponent<Image>();

        if (logicaPenalty == null)
            logicaPenalty = FindAnyObjectByType<PenaltyMinijuego>();
    }

    void Update()
    {
        if (logicaPenalty != null && imagenTimer != null)
        {
            float max = logicaPenalty.ObtenerTiempoLimite();
            float actual = logicaPenalty.ObtenerTiempoRestante();

            if (max > 0)
            {
                imagenTimer.fillAmount = actual / max;
            }
        }
    }
}