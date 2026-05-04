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
 * Última modificación: 04/04/2026 (Álvaro Muñoz Adán) -> Utilización de todos los frames de animación y aceleración del spawn de siguiente topo
 */

public class Topos : MonoBehaviour
{
    [SerializeField] private GameObject[] vasosTopos;
    [SerializeField] private int number; 
    [SerializeField] private float timer;

    private List<GameObject> seleccionados;
    private int clickeadosCount = 0;
    private bool gameFinished = false;
    private float tiempoRestante;
    private bool avanzarInmediatamente = false; // Flag para acelerar el flujo

    private void Start()
    {
        tiempoRestante = timer;
        ConfigurarSeleccion();
        StartCoroutine(GestionarOleada());
    }

    private void ConfigurarSeleccion()
    {
        seleccionados = new List<GameObject>();
        List<GameObject> copia = new List<GameObject>(vasosTopos);

        for (int i = 0; i < number && copia.Count > 0; i++)
        {
            int index = Random.Range(0, copia.Count);
            seleccionados.Add(copia[index]);
            copia.RemoveAt(index);
        }
    }

    private void Update()
    {
        if (gameFinished) return;
        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0) FinalizarJuego(false);
    }
    
    private IEnumerator GestionarOleada()
    {
        // Mientras no hayamos golpeado todos los necesarios y quede tiempo
        while (clickeadosCount < number && !gameFinished)
        {
            avanzarInmediatamente = false;
            
            // Elegimos el topo actual de la lista de seleccionados basado en cuántos llevamos
            GameObject objActual = seleccionados[clickeadosCount];
            ClickableMole mole = objActual.GetComponent<ClickableMole>();
            
            if (mole != null) mole.IniciarCiclo();

            // Esperamos a que: o lo golpeen, o se esconda solo
            while (!avanzarInmediatamente && mole.isAnimating)
            {
                yield return null;
            }

            // Si se escondió solo (no hubo click), esperamos un pelín y reiniciamos el mismo
            if (!avanzarInmediatamente)
            {
                yield return new WaitForSeconds(0.2f);
                continue; // El bucle vuelve a empezar con el mismo clickeadosCount (mismo topo)
            }

            // Si llegamos aquí es porque avanzarInmediatamente es true (hubo click)
            yield return new WaitForSeconds(0.1f); 
        }
    }

    public void MoleClicked()
    {
        if (gameFinished) return;
        
        clickeadosCount++;
        avanzarInmediatamente = true; // Forzamos la salida del siguiente topo

        if (clickeadosCount >= number) FinalizarJuego(true);
    }

    private void FinalizarJuego(bool victoria)
    {
        if (gameFinished) return;
        gameFinished = true;

        if (victoria)
        {
            // LOG: Victoria por puntos
            Debug.Log("<color=green>¡Has ganado! Todos los topos han sido golpeados.</color>");
            if (GameManager.instancia != null) GameManager.instancia.Ganar();
        }
        else
        {
            // LOG: Derrota por tiempo
            Debug.Log("<color=red>¡Se ha acabado el tiempo! Has perdido el minijuego.</color>");
            if (GameManager.instancia != null) GameManager.instancia.Perder();
        }
    }

    public float ObtenerTiempoLimite() => timer;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
}