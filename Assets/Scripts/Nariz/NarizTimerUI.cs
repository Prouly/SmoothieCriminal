using UnityEngine;
using UnityEngine.UI;

public class NarizTimerUI : MonoBehaviour
{
    [SerializeField] private Image imagenPajita;
    private NarizGameLogic logica;

    void Start()
    {
        logica = FindAnyObjectByType<NarizGameLogic>();
    }

    void Update()
    {
        if (logica != null && imagenPajita != null)
        {
            imagenPajita.fillAmount = logica.ObtenerTiempoRestante() / logica.ObtenerTiempoLimite();
        }
    }
}