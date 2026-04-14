<<<<<<< HEAD
using System.Collections;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] private float timer = 6f;    
    private bool gameFinished = false;

    void Start()
    {
        StartCoroutine(TimerCoroutine());
    }
    
    
    private IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(timer);

        if (!gameFinished)
        {
            gameFinished = true;
            Debug.Log("¡Has perdido!");
            
        }
    }
}
=======
using System.Collections;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] private float timer = 6f;    
    private bool gameFinished = false;

    void Start()
    {
        StartCoroutine(TimerCoroutine());
    }
    
    
    private IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(timer);

        if (!gameFinished)
        {
            gameFinished = true;
            Debug.Log("¡Has perdido!");
            
        }
    }
}
>>>>>>> 6582b7c27b7bb627f7c1a50f0e38a056e4d593a0
