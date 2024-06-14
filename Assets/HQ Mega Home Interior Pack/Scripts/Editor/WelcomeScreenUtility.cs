using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class WelcomeScreenUtility
{
    public static Texture2D InvertTexture(Texture2D texture)
    {
        var pixels = texture.GetPixels32();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].r = (byte)(255 - pixels[i].r);
            pixels[i].g = (byte)(255 - pixels[i].g);
            pixels[i].b = (byte)(255 - pixels[i].b);
        }
        texture.SetPixels32(pixels);
        texture.Apply();

        return texture;
    }

    public static IEnumerator Request(string url, DownloadHandler handler, Action<DownloadHandler> callback)
    {
        using (var www = UnityWebRequest.Get(url))
        {
            www.downloadHandler = handler;
            yield return www.SendWebRequest();

            while (www.isDone == false)
            { yield return null; }

            callback(www.downloadHandler);
        }
    }
}