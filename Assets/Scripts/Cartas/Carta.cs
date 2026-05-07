using System.Collections;
using UnityEngine;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Configuración de cada Carta con efecto visual de volteo (Fake 3D).
 * Última modificación: 07/05/2026
 */

public class Carta : MonoBehaviour
{
    public int idPareja;
    private SpriteRenderer spriteRenderer;
    private Sprite spriteFrontal;
    [SerializeField] private Sprite spriteReverso;
    
    [Header("Ajustes de Animación")]
    [SerializeField] private float tiempoGiro = 0.2f; // Velocidad del efecto

    private CartasGameLogic gameController;
    private bool estaRevelada = false;
    private Vector3 escalaOriginal;

    public void Configurar(int id, Sprite frontal, CartasGameLogic controller)
    {
        idPareja = id;
        spriteFrontal = frontal;
        gameController = controller;
        spriteRenderer = GetComponent<SpriteRenderer>();
        escalaOriginal = transform.localScale;
        
        // Empezamos mostrando el frontal durante la fase de memorización
        spriteRenderer.sprite = spriteFrontal;
        estaRevelada = true;
    }

    public void MostrarFrontal()
    {
        if (!estaRevelada)
        {
            StopAllCoroutines();
            StartCoroutine(AnimacionVoltear(spriteFrontal, true));
        }
    }

    public void Ocultar()
    {
        if (estaRevelada)
        {
            StopAllCoroutines();
            StartCoroutine(AnimacionVoltear(spriteReverso, false));
        }
    }

    private IEnumerator AnimacionVoltear(Sprite nuevoSprite, bool estadoFinal)
    {
        float tiempo = 0;
        
        // Fase 1: Encoger la carta (Eje X -> 0)
        while (tiempo < tiempoGiro / 2)
        {
            tiempo += Time.deltaTime;
            float escalaX = Mathf.Lerp(escalaOriginal.x, 0, tiempo / (tiempoGiro / 2));
            transform.localScale = new Vector3(escalaX, escalaOriginal.y, escalaOriginal.z);
            yield return null;
        }

        // Cambio de Sprite justo cuando no se ve nada (escala 0)
        spriteRenderer.sprite = nuevoSprite;
        estaRevelada = estadoFinal;

        // Fase 2: Estirar la carta (Eje X -> Original)
        tiempo = 0;
        while (tiempo < tiempoGiro / 2)
        {
            tiempo += Time.deltaTime;
            float escalaX = Mathf.Lerp(0, escalaOriginal.x, tiempo / (tiempoGiro / 2));
            transform.localScale = new Vector3(escalaX, escalaOriginal.y, escalaOriginal.z);
            yield return null;
        }

        transform.localScale = escalaOriginal; // Aseguramos escala final exacta
    }

    private void OnMouseDown()
    {
        if (gameController.PuedeJugar && !estaRevelada)
        {
            MostrarFrontal();
            gameController.CartaSeleccionada(this);
        }
    }
}