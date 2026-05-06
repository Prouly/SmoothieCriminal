using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla visualmente el temporizador del minijuego de penalti.
/// Lee el tiempo restante desde PenaltyMinijuego y actualiza el fillAmount de la imagen.
/// </summary>
public class PenaltiTimerUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Image imagenTimer;

    [Header("Referencias de Lógica")]
    [SerializeField] private PenaltiMinijuego logicaPenalty;

    void Start()
    {
        if (imagenTimer == null)
            imagenTimer = GetComponent<Image>();

        if (logicaPenalty == null)
            logicaPenalty = FindAnyObjectByType<PenaltiMinijuego>();
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