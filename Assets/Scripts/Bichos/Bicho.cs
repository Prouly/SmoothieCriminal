using UnityEngine;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Gestión del movimiento y muertes de los Bichos del minijuego Aplastar Bichos.
 * Última modificación: 26/04/2026
 */

public class Bicho : MonoBehaviour
{
    [SerializeField] private Sprite[] framesAnimacion; 
    [SerializeField] private float velocidadAnimacion = 0.1f;
    [SerializeField] private float velocidadMovimiento = 2.5f;
    
    [Header("Efecto de Muerte")]
    [SerializeField] private GameObject prefabManchaEspecifica; // prefab de la mancha para ESTE bicho
    
    private SpriteRenderer sr;
    private int frameActual;
    private BichosLogic logica;
    private Vector2 direccion;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        logica = Object.FindFirstObjectByType<BichosLogic>();
        InvokeRepeating(nameof(Animar), 0, velocidadAnimacion);
        CambiarDireccion();
    }

    void Update()
    {
        if (logica != null && logica.EstaJuegoTerminado()) return;
        
        transform.Translate(direccion * velocidadMovimiento * Time.deltaTime, Space.World);
        
        float anguloDeseado = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, anguloDeseado - 90f), Time.deltaTime * 5f);

        if (Random.value < 0.015f) CambiarDireccion();
    }

    void Animar()
    {
        if (framesAnimacion.Length == 0) return;
        frameActual = (frameActual + 1) % framesAnimacion.Length;
        sr.sprite = framesAnimacion[frameActual];
    }

    void CambiarDireccion() => direccion = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

    private void OnMouseDown()
    {
        if (logica == null || logica.EstaJuegoTerminado()) return;

        if (prefabManchaEspecifica != null)
        {
            Instantiate(prefabManchaEspecifica, transform.position, transform.rotation);
        }

        logica.RegistrarBaja();
        Destroy(gameObject);
    }
    
    public void Morir()
    {
        // Instanciamos la mancha configurada para este bicho concreto
        if (prefabManchaEspecifica != null)
        {
            Instantiate(prefabManchaEspecifica, transform.position, Quaternion.identity);
        }

        if (logica != null) logica.RegistrarBaja();
        
        Destroy(gameObject);
    }
}