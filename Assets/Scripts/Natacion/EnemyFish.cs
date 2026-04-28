using UnityEngine;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: IA del pez perseguidor.
 * Última modificación: 28/04/2026
 */

public class EnemyFish : MonoBehaviour
{
    [Header("Ajustes de Persecución")]
    [SerializeField] private float velocidadBase = 2.0f;
    [SerializeField] private Sprite[] framesPez; // 4 frames
    [SerializeField] private float velocidadAnim = 0.12f;

    private SpriteRenderer sr;
    private int frameIndex;
    private NatacionLogic logica;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        logica = Object.FindFirstObjectByType<NatacionLogic>();
        InvokeRepeating(nameof(AnimarPez), 0, velocidadAnim);
    }

    void Update()
    {
        if (logica != null && logica.EstaJuegoTerminado()) return;

        // Movimiento constante hacia el jugador
        transform.Translate(Vector3.right * velocidadBase * Time.deltaTime);
    }

    void AnimarPez()
    {
        if (framesPez.Length == 0 || (logica != null && logica.EstaJuegoTerminado())) return;
        frameIndex = (frameIndex + 1) % framesPez.Length;
        sr.sprite = framesPez[frameIndex];
    }
}