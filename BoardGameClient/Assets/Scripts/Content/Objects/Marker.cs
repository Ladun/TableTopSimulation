using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Marker : MonoBehaviour
{
    private Renderer[] renderers;

    private void Awake()
    {

        renderers = GetComponentsInChildren<Renderer>();
    }

    public void SetColor(Color c)
    {

        foreach (var renderer in renderers)
        {

            // Append outline shaders
            renderer.material.color = c;
        }
    }
}
