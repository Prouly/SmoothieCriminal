/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Detecta el clic en Haznarito para ganar el minijuego.
 * Última modificación: 23/04/2026
 */
using UnityEngine;

public class HaznaritoTarget : MonoBehaviour
{
    private FindHaznaritoLogic logicaPrincipal;

    public void Configurar(FindHaznaritoLogic logica) => logicaPrincipal = logica;

    private void OnMouseDown()
    {
        // Solo responde si la lógica principal aún no ha terminado el juego
        logicaPrincipal.FinalizarJuego(true);
    }
}