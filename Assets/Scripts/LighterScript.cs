using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LighterScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject lightObj;

    void OnEnable()
    {
        lightObj.SetActive(true);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     void OnDisable()
    {
        lightObj.SetActive(false);
    }
}
