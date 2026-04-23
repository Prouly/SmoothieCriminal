/**
 * Proyecto: Smoothie Criminal
 * Autor: Luis Miguel Muñoz Vega
 * Descripción: Gestiona el movimiento del jugador, bloqueándolo al terminar la carrera.
 * Última modificación: 23/04/2026 (Álvaro Muñoz Adán)
 */
using UnityEngine;

public class PlayerCarrera : MonoBehaviour
{
    #region Variables de Configuración
    [SerializeField] private float distanciaPaso = 0.5f;
    [SerializeField] private Sprite spriteD;
    [SerializeField] private Sprite spriteA;
    [SerializeField] private SpriteRenderer spriteRenderer;
    #endregion

    #region Variables de Estado
    private bool esperaD = true;
    #endregion

    private void Start() => ActualizarSprite();

    void Update()
    {

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
    
    void ActualizarSprite() => spriteRenderer.sprite = esperaD ? spriteD : spriteA;
}