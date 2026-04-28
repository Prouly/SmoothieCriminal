/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Movimiento del jugador mediante alternancia de teclas A/D.
 * Última modificación: 28/04/2026
 */

using UnityEngine;

public class PlayerSwimming : MonoBehaviour
{
    [Header("Configuración de Nado")]
    [SerializeField] private float fuerzaAvance = 0.4f;
    [SerializeField] private float friccionAgua = 2f;
    [SerializeField] private Sprite[] framesNatacion; // 4 frames

    private SpriteRenderer sr;
    private NatacionLogic logica;
    private bool esperarTeclaA = true; 
    private float velocidadActual = 0f;
    private int frameActual = 0;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        logica = Object.FindFirstObjectByType<NatacionLogic>();
        if (framesNatacion.Length > 0) sr.sprite = framesNatacion[0];
    }

    void Update()
    {
        if (logica != null && logica.EstaJuegoTerminado()) return;

        ManejarInput();
        AplicarFisicas();
    }

    private void ManejarInput()
    {
        bool acierto = false;

        if (esperarTeclaA && Input.GetKeyDown(KeyCode.A))
        {
            esperarTeclaA = false;
            acierto = true;
        }
        else if (!esperarTeclaA && Input.GetKeyDown(KeyCode.D))
        {
            esperarTeclaA = true;
            acierto = true;
        }

        if (acierto)
        {
            Avanzar();
            logica.ReproducirChapoteo();
        }
    }

    private void Avanzar()
    {
        velocidadActual += fuerzaAvance;
        
        // Animación manual sincronizada con el esfuerzo
        frameActual = (frameActual + 1) % framesNatacion.Length;
        sr.sprite = framesNatacion[frameActual];
    }

    private void AplicarFisicas()
    {
        transform.Translate(Vector3.right * velocidadActual * Time.deltaTime);
        // Deceleración constante por el agua
        velocidadActual = Mathf.Lerp(velocidadActual, 0, Time.deltaTime * friccionAgua);
    }
}