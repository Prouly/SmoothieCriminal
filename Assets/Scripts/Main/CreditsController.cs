using System.Collections;
using UnityEngine;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Gestión secuencial de créditos con efectos de transición.
 * Última modificación: 08/05/2026
 */

public class CreditsController : MonoBehaviour
{
    [Header("Configuración de Tiempo")]
    [SerializeField] private float tiempoVisible = 3.0f;
    [SerializeField] private float tiempoTransicion = 0.8f;

    [Header("Departamentos (En orden)")]
    [SerializeField] private CanvasGroup[] bloquesCreditos;

    private Coroutine secuenciaActual;

    private void OnEnable()
    {
        // Cada vez que se activa el panel, reiniciamos todo
        PrepararCreditos();
        secuenciaActual = StartCoroutine(ReproducirSecuencia());
    }

    private void OnDisable()
    {
        // Si el usuario vuelve al menú antes de acabar, paramos la corrutina
        if (secuenciaActual != null)
        {
            StopCoroutine(secuenciaActual);
        }
    }

    private void PrepararCreditos()
    {
        // Ocultamos todos los bloques al inicio (Alpha = 0)
        foreach (var bloque in bloquesCreditos)
        {
            bloque.alpha = 0;
            bloque.gameObject.SetActive(false);
        }
    }

    private IEnumerator ReproducirSecuencia()
    {
        foreach (var bloque in bloquesCreditos)
        {
            bloque.gameObject.SetActive(true);

            // 1. Fade In + Efecto de escala pequeño
            yield return StartCoroutine(Fade(bloque, 0, 1));

            // 2. Tiempo que el texto se queda quieto
            yield return new WaitForSeconds(tiempoVisible);

            // 3. Fade Out
            yield return StartCoroutine(Fade(bloque, 1, 0));

            bloque.gameObject.SetActive(false);
        }

        Debug.Log("Fin de los créditos");
    }

    private IEnumerator Fade(CanvasGroup cg, float inicio, float fin)
    {
        float tiempo = 0;
        Vector3 escalaInicio = inicio == 0 ? Vector3.one * 0.9f : Vector3.one;
        Vector3 escalaFin = inicio == 0 ? Vector3.one : Vector3.one * 1.1f;

        while (tiempo < tiempoTransicion)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / tiempoTransicion;
            
            // Cambiamos opacidad
            cg.alpha = Mathf.Lerp(inicio, fin, progreso);
            
            // Añadimos un pequeño efecto de escala para que sea más dinámico
            cg.transform.localScale = Vector3.Lerp(escalaInicio, escalaFin, progreso);
            
            yield return null;
        }
        cg.alpha = fin;
    }
}