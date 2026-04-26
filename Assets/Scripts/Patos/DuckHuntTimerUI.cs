/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Controla visualmente el temporizador del juego de Cazar Patos.
 * Última modificación: 26/04/2026
 */

using UnityEngine;
using UnityEngine.UI;

public class DuckHuntTimerUI : MonoBehaviour
{
    #region Variables de Referencia
    [Header("Referencias UI")]
    [SerializeField] private Image imagenPajita; // La imagen con Fill Method: Horizontal

    [Header("Referencias de Lógica")]
    [SerializeField] private DuckHuntLogic logicaPatos; 
    #endregion

    #region Métodos de Unity
    void Start()
    {
        // Si no se asignó en el inspector, intentamos buscarlo automáticamente
        if (imagenPajita == null) imagenPajita = GetComponent<Image>();
        
        if (logicaPatos == null)
        {
            logicaPatos = FindAnyObjectByType<DuckHuntLogic>();
        }

        // Validación de seguridad para evitar errores en consola
        if (imagenPajita == null || logicaPatos == null)
        {
            Debug.LogWarning($"SISTEMA: Faltan referencias en DuckHuntTimerUI en el objeto {gameObject.name}");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        ActualizarBarra();
    }
    #endregion

    #region Lógica Visual
    private void ActualizarBarra()
    {
        float max = logicaPatos.ObtenerTiempoLimite();
        float actual = logicaPatos.ObtenerTiempoRestante();

        // Calculamos el porcentaje (de 1 a 0)
        if (max > 0)
        {
            imagenPajita.fillAmount = actual / max;
        }
    }
    #endregion
}