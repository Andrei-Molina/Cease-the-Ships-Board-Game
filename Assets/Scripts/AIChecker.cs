using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChecker : MonoBehaviour
{
    private static AIChecker instance;
    private static bool AIEnabled = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Make this object persist between scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }
}
