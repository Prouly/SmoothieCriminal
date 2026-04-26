/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Controla visualmente el temporizador de la pajita en la UI.
 * Última modificación: 24/04/2026
 */
using UnityEngine;
using UnityEngine.UI; // Necesario para componentes de UI

public class FindHaznaritoTimerUI : MonoBehaviour
{
    #region Variables de Referencia
    [Header("Referencias UI")]
    [SerializeField] private Image imagenBarraTiempo; // Arrastrar aquí la pajita configurada como Filled

    [Header("Referencias de Lógica")]
    [SerializeField] private FindHaznaritoLogic logicaJuego; // Arrastrar el manager de la escena
    #endregion

    #region Métodos de Unity
    void Start()
    {
        // Validación de componentes
        if (imagenBarraTiempo == null)
        {
            imagenBarraTiempo = GetComponent<Image>();
        }

        // Buscamos la lógica en la escena para no depender del GameManager en testeo independiente
        if (logicaJuego == null)
        {
            logicaJuego = FindAnyObjectByType<FindHaznaritoLogic>();
        }

        // Seguridad
        if (imagenBarraTiempo == null || logicaJuego == null)
        {
            Debug.LogError($"SISTEMA ({gameObject.name}): Faltan referencias en el script FindHaznaritoTimerUI.");
            enabled = false; // Desactivamos el script para evitar spam de errores
            return;
        }

        // Aseguramos que la barra empiece llena
        imagenBarraTiempo.fillAmount = 1f;
    }

    void Update()
    {
        ActualizarBarraVisual();
    }
    #endregion

    #region Lógica Visual
    /// <summary>
    /// Calcula el porcentaje de tiempo restante y actualiza el Fill Amount de la imagen.
    /// </summary>
    private void ActualizarBarraVisual()
    {
        float maxTiempo = logicaJuego.ObtenerTiempoLimite();
        float tiempoActual = logicaJuego.ObtenerTiempoRestante();

        // Calculamos el ratio (valor entre 0 y 1)
        float ratio = 0f;
        if (maxTiempo > 0)
        {
            ratio = tiempoActual / maxTiempo;
        }

        // Actualizamos la UI. El ratio 1 es lleno, ratio 0 es vacío.
        imagenBarraTiempo.fillAmount = ratio;

        // Opcional: Feedback visual de color (ej: ponerse roja al final)
        /*
        if (ratio < 0.25f) // Menos del 25% de tiempo
        {
            imagenBarraTiempo.color = Color.red;
        }
        else
        {
            imagenBarraTiempo.color = Color.white; // Color original
        }
        */
    }
    #endregion
}