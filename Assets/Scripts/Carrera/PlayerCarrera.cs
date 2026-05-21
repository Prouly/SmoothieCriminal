/**
 * Proyecto: Smoothie Criminal
 * Autor: Luis Miguel Muñoz Vega
 * Descripción: Gestiona el movimiento del jugador, bloqueándolo al terminar la carrera.
 * Última modificación: 26/04/2026 (Álvaro Muñoz Adán)
 */
using UnityEngine;

public class PlayerCarrera : MonoBehaviour
{
    #region Variables de Configuración
    [SerializeField] private float distanciaPaso = 0.5f;
    [SerializeField] private Sprite spriteD;
    [SerializeField] private Sprite spriteA;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private CarreraLogic carreraLogic; // Referencia a la lógica
    #endregion

    #region Variables de Estado
    private bool esperaD = true;
    #endregion

    private void Start() 
    {
        // ASIGNACIÓN: Buscamos el script de lógica en la escena
        carreraLogic = Object.FindFirstObjectByType<CarreraLogic>();
        ActualizarSprite();
    }

    void Update()
    {
        // CAMBIO CLAVE: Si no hay lógica o aún no se puede jugar, salimos
        if (carreraLogic == null || !carreraLogic.PuedeMoverse()) return;
        
        if (esperaD && Input.GetKeyDown(KeyCode.D))
        {
            Avanzar();
            esperaD = false;
            ActualizarSprite();
        }
        else if (!esperaD && Input.GetKeyDown(KeyCode.A))
        {
            Avanzar();
            esperaD = true;
            ActualizarSprite();
        }
    }
    
    void Avanzar() => transform.position += new Vector3(distanciaPaso, 0, 0);
    
    void ActualizarSprite() 
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = esperaD ? spriteD : spriteA;
    }
}