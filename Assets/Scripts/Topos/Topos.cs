using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using UnityEngine.UI;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Luis Miguel Muñoz Vega
 * Descripción: Gestión del minijuego de topos con temporizador fluido para UI.
 * Última modificación: 26/04/2026 (Álvaro Muñoz Adán) -> Timer UI
 */

public class Topos : MonoBehaviour
{
    #region Variables de Configuración
    [SerializeField] private GameObject[] vasosTopos;
    [SerializeField] private int number; // Cantidad de topos a salir
    [SerializeField] private float timer;  // Tiempo límite (máximo)
    [SerializeField] private Sprite newSpriteMole; 
    #endregion

    #region Variables de Estado
    private List<GameObject> seleccionados;
    private int clickeadosCount = 0;
    private bool gameFinished = false;
    private float tiempoRestante; // Variable para la pajita UI
    #endregion
    
    private void Start()
    {
        // Inicialización de tiempo
        tiempoRestante = timer;

        seleccionados = new List<GameObject>();
        List<GameObject> copia = new List<GameObject>(vasosTopos);

        for (int i = 0; i < number && copia.Count > 0; i++)
        {
            int index = Random.Range(0, copia.Count);
            seleccionados.Add(copia[index]);
            copia.RemoveAt(index);
        }
        
        StartCoroutine(CambiarSprites());
        // El Timer ya no usa Coroutine para poder ser leído frame a frame por la UI
    }

    private void Update()
    {
        if (gameFinished) return;

        // Descuento de tiempo fluido
        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            FinalizarJuego(false);
            Debug.Log("Lose");

        }
    }
    
    private IEnumerator CambiarSprites()
    {
        float intervalo = timer / number;

        foreach (GameObject obj in seleccionados)
        {
            if (gameFinished) yield break;

            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = newSpriteMole;
                ClickableMole mole = obj.GetComponent<ClickableMole>();
                if (mole != null) mole.isClickable = true;
            }
            yield return new WaitForSeconds(intervalo);
        }
    }

    public void MoleClicked()
    {
        if (gameFinished) return;

        clickeadosCount++;

        if (clickeadosCount >= number)
        {
            FinalizarJuego(true);
            Debug.Log("Win");
        }
    }

    private void FinalizarJuego(bool victoria)
    {
        if (gameFinished) return;
        gameFinished = true;

        if (GameManager.instancia != null)
        {
            if (victoria)
            {
                GameManager.instancia.Ganar();
            }
            else
            {
                GameManager.instancia.Perder();
            }
        }
    }

    #region Getters para UI
    public float ObtenerTiempoLimite() => timer;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
    #endregion
}