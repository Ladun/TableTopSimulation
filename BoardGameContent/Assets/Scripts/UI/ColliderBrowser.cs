using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ColliderBrowser : MonoBehaviour
{

    public WireCubeDrawer drawer;

    public TMP_InputField[] sizeInput;
    public TMP_InputField[] centerInput;

    private CustomData currentData;


    private void Start()
    {
        Close();
#if UNITY_EDITOR
        if (sizeInput.Length != 3)
        {
            Debug.LogError("Size input field length must be 3");
        }
        if (centerInput.Length != 3)
        {
            Debug.LogError("Size input field length must be 3");
        }
#endif
    }

    public void Open()
    {
        gameObject.SetActive(true);
        drawer.gameObject.SetActive(true);
        currentData = ContentManager.instance.GetActiveObject();

        sizeInput[0].text = currentData.colliderSize.x.ToString();
        sizeInput[1].text = currentData.colliderSize.y.ToString();
        sizeInput[2].text = currentData.colliderSize.z.ToString();
        drawer.SetSize(currentData.colliderSize);

        centerInput[0].text = currentData.colliderCenter.x.ToString();
        centerInput[1].text = currentData.colliderCenter.y.ToString();
        centerInput[2].text = currentData.colliderCenter.z.ToString();
    }

    public void ChangeSize()
    {
        float x = float.Parse(sizeInput[0].text);
        float y = float.Parse(sizeInput[1].text);
        float z = float.Parse(sizeInput[2].text);

        drawer.SetSize(x, y ,z);
        currentData.colliderSize = new Vector3(x, y, z);
    }
    public void ChangeCenter()
    {

    }

    public void Close()
    {
        drawer.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
