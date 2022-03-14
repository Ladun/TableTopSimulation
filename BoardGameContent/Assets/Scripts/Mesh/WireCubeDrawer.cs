using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireCubeDrawer : MonoBehaviour
{

    private LineRenderer[] surfaceRenderer;
    public float lineWidth = 0.02f;
    public Material mat;
    public Color matColor;


    private void Start()
    {
        Init();
    }

    private void Init()
    {
        surfaceRenderer = new LineRenderer[4];

        for(int i = 0; i <4; i++)
        {
            GameObject go = new GameObject("surface" + i);
            go.transform.SetParent(transform);

            surfaceRenderer[i] = go.AddComponent<LineRenderer>();
            surfaceRenderer[i].loop = true;
            surfaceRenderer[i].startWidth = surfaceRenderer[i].endWidth = lineWidth;
            surfaceRenderer[i].numCornerVertices = 4;
            surfaceRenderer[i].material = mat;
            surfaceRenderer[i].material.color = matColor;
        }

        SetSize(1, 1, 1);
    }

    public void SetSize(Vector3 size)
    {
        SetSize(size.x, size.y, size.z);
    }

    public void SetSize(float x, float y, float z)
    {
        x /= 2;
        y /= 2;
        z /= 2;


        SetPositions(0, new Vector3[]{
            new Vector3(x, y, -z),
            new Vector3(x, -y, -z),
            new Vector3(-x, -y, -z),
            new Vector3(-x, y, -z)
        });

        SetPositions(1, new Vector3[]{
            new Vector3(x, y, z),
            new Vector3(x, -y, z),
            new Vector3(-x, -y, z),
            new Vector3(-x, y, z)
        });

        SetPositions(2, new Vector3[]{
            new Vector3(-x, y, z),
            new Vector3(-x, -y, z),
            new Vector3(-x, -y, -z),
            new Vector3(-x, y, -z)
        });
        SetPositions(3, new Vector3[]{
            new Vector3(x, y, z),
            new Vector3(x, -y, z),
            new Vector3(x, -y, -z),
            new Vector3(x, y, -z)
        });
    }

    private void SetPositions(int idx, Vector3[] points)
    {
        surfaceRenderer[idx].positionCount = points.Length;
        surfaceRenderer[idx].SetPositions(points);
    }
}
