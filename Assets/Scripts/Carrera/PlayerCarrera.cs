using System;
using UnityEngine;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Luismi Muñoz
 * Descripción: Gestiona la lógica de Jugador del minijuego de Carrera
 * Última modificación: 14/04/2026
 */
public class PlayerCarrera : MonoBehaviour
{
    [SerializeField] private float distanciaPaso = 0.5f;   //Cuánto avanza por paso
    private bool esperaD = true;//Empieza esperando D
    [SerializeField] private Sprite spriteD;
    [SerializeField] private Sprite spriteA;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Start()
    {
        ActualizarSprite();
    }

    void Update()
    {
        //Alterna entre presionar D y A para avanzar
        if (esperaD && Input.GetKeyDown(KeyCode.D))
        {
            transform.position += new Vector3(distanciaPaso, 0, 0);
            esperaD = false; // Ahora espera A
            ActualizarSprite();
        }
        else if (!esperaD && Input.GetKeyDown(KeyCode.A))
        {
            transform.position += new Vector3(distanciaPaso, 0, 0);
            esperaD = true; //Ahora espera D
            ActualizarSprite();
        }
    }
    
    void ActualizarSprite()
    {
        spriteRenderer.sprite = esperaD ? spriteD : spriteA;
    }
}