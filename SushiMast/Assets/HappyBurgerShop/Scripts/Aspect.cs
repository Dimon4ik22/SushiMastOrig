using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aspect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Camera.main.aspect = 16.0f / 9.0f; // Пример для соотношения 16:9
    }
}
