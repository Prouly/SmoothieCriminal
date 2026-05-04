using System.Collections;
using UnityEngine;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Luis Miguel Muñoz Vega
 * Descripción: Gestiona las animaciones de los topos, la comunicación con GameManager y el que pueda hacerse click en los topos que se muestran
 * Última modificación: 04/05/2026 (Álvaro Muñoz Adán)
 */

public class ClickableMole : MonoBehaviour
{
    public bool isClickable = false;
    public bool isAnimating = false; // Nueva variable para control de flujo[cite: 12]
    private SpriteRenderer spriteRenderer;
    public Topos gameManager;

    [Header("Configuración de Animación")]
    [SerializeField] private Sprite[] sprites; 
    [SerializeField] private float tiempoEntreFrames = 0.25f;
    [SerializeField] private float tiempoVulnerable = 1f;
    
    private Coroutine animacionActual;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (sprites.Length > 0) spriteRenderer.sprite = sprites[0];
    }

    public void IniciarCiclo()
    {
        isAnimating = true;
        if (animacionActual != null) StopCoroutine(animacionActual);
        animacionActual = StartCoroutine(SecuenciaCompleta());
    }

    private IEnumerator SecuenciaCompleta()
    {
        // Topo clickable desde que sale
        isClickable = true; 

        // 1. Animación de Salida
        for (int i = 1; i < sprites.Length; i++)
        {
            if (i == 3) continue; // Saltamos el frame de golpeado
            spriteRenderer.sprite = sprites[i];
            yield return new WaitForSeconds(tiempoEntreFrames);
        }
        
        // 2. Tiempo de espera en el punto más alto
        yield return new WaitForSeconds(tiempoVulnerable);

        if (isClickable)
        {
            isClickable = false;
            // 3. Animación de Entrada
            for (int i = sprites.Length - 1; i >= 0; i--)
            {
                if (i == 3) continue;
                spriteRenderer.sprite = sprites[i];
                yield return new WaitForSeconds(tiempoEntreFrames);
            }
        }
        isAnimating = false; // Terminó de esconderse sin ser golpeado
    }

    private void Update()
    {
        if (isClickable && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                ProcesarGolpe();
            }
        }
    }

    private void ProcesarGolpe()
    {
        // Confirmación de click en topo
        Debug.Log("<color=yellow>¡Has hecho click en un topo!</color>");

        isClickable = false;
        isAnimating = false; // Detenemos el control de animación normal
        
        if (animacionActual != null) StopCoroutine(animacionActual);

        // Frame 3: El topo golpeado con estrellas
        if (sprites.Length > 3) spriteRenderer.sprite = sprites[3];

        if (gameManager != null) gameManager.MoleClicked();
        
        StartCoroutine(EsconderYResetear());
    }

    private IEnumerator EsconderYResetear()
    {
        yield return new WaitForSeconds(0.3f);
        if (sprites.Length > 0) spriteRenderer.sprite = sprites[0];
    }
}