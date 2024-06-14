using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGUI : MonoBehaviour
{
    public Text healthAmt;
    public Text mentalAmt;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        healthAmt.text = SaveScript.health + "%";
        mentalAmt.text=SaveScript.mental.ToString("F0") + "%";
    }
}
