using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Utils
{
    public static IEnumerator CoLoadTexture(string path, System.Action<Texture2D> action=null)
    {
        // Start a download of the given URL
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);

        // Wait for download to complete
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            // assign texture
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            if( action != null)
            {
                action.Invoke(texture);
            }
        }
    }
}
