using System;
using UnityEngine;

[Serializable()]
public class Setting
{
    public GameObject setting;
    public Color ambientColor;
    public Material skybox;
    public float rotation;
    public float exposure;
}
public class Setting_Switcher : MonoBehaviour
{
    #pragma warning disable IDE0052
    [SerializeField] Setting[] settings = new Setting[0];
    #pragma warning restore IDE0052

    [SerializeField] new int enabled = 0;
}