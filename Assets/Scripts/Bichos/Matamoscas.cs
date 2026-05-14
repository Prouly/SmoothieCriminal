using UnityEngine;
using System.Collections;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Gestión del matamoscar controlado con ratón para matar los Bichos del minijuego Aplastar Bichos.
 * Última modificación: 26/04/2026
 */

public class Matamoscas : MonoBehaviour
{
    [Header("Sprites de Animación")]
    [SerializeField] private Sprite[] framesMatamoscas; 
    
    [Header("Referencias de Límites")]
    [SerializeField] private Transform objetoLimits; // Objeto padre "Limits"

    [Header("Detección de Golpe")]
    [SerializeField] private Transform puntoImpacto; // Objeto hijo que esté en la red
    [SerializeField] private float radioGolpe = 0.6f; // Tamaño del área de golpeo
        
    private float minX, maxX, minY, maxY;
    private SpriteRenderer sr;
    private BichosLogic logica;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        logica = FindFirstObjectByType<BichosLogic>();
        
        if (framesMatamoscas.Length > 0) sr.sprite = framesMatamoscas[0];

        CalcularLimites();
    }

    void CalcularLimites()
    {
        if (objetoLimits == null) return;

        // Asumimos que Limits tiene 4 hijos: Superior, Inferior, Derecho e Izquierdo
        // Buscamos los valores extremos basados en sus posiciones
        float[] xPositions = new float[objetoLimits.childCount];
        float[] yPositions = new float[objetoLimits.childCount];

        for (int i = 0; i < objetoLimits.childCount; i++)
        {
            xPositions[i] = objetoLimits.GetChild(i).position.x;
            yPositions[i] = objetoLimits.GetChild(i).position.y;
        }

        // Encontramos los bordes
        minX = Mathf.Min(xPositions);
        maxX = Mathf.Max(xPositions);
        minY = Mathf.Min(yPositions);
        maxY = Mathf.Max(yPositions);
    }

    void Update()
    {
        if (logica != null && logica.EstaJuegoTerminado()) return;

        // 1. Obtener posición del ratón
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // 2. Aplicar el CLAMP (Restricción)
        float clampedX = Mathf.Clamp(mousePos.x, minX, maxX);
        float clampedY = Mathf.Clamp(mousePos.y, minY, maxY);

        // 3. Aplicar posición final
        transform.position = new Vector3(clampedX, clampedY, 0);

        if (Input.GetMouseButtonDown(0))
        {
            StopAllCoroutines();
            StartCoroutine(SecuenciaGolpe());
        }
    }
    
    private IEnumerator SecuenciaGolpe()
    {
    sr.sprite = framesMatamoscas[1]; // Frame de golpe

    // CAMBIO AQUÍ: Detectamos en el círculo del punto de impacto, no en el ratón
    Collider2D[] hits = Physics2D.OverlapCircleAll(puntoImpacto.position, radioGolpe);
    
    foreach (Collider2D hit in hits)
    {
        if (hit.CompareTag("Bicho"))
        {
            Bicho bicho = hit.GetComponent<Bicho>();
            if (bicho != null) bicho.Morir();
        }
    }

    yield return new WaitForSeconds(0.1f);
    sr.sprite = framesMatamoscas[0]; // Volver a normal
    }
    
    // Para que puedas ver el área de golpe en el Editor
    private void OnDrawGizmosSelected()
    {
        if (puntoImpacto != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(puntoImpacto.position, radioGolpe);
        }
    }

    /*IEnumerator SecuenciaGolpe()
    {
        sr.sprite = framesMatamoscas[1]; // Levantado
        yield return new WaitForSeconds(0.05f);
        sr.sprite = framesMatamoscas[2]; // Aplastado
        yield return new WaitForSeconds(0.15f);
        sr.sprite = framesMatamoscas[0]; // Reposo
    }*/
}