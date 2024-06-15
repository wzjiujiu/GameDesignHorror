using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUIManager : MonoBehaviour
{
    public GameObject pistolPanel, shotgunPanel, sprayPanel;
    public Text pistolTotalAmmo,pistolCurrentAmmo,shotgunTotalAmmo,shotgunCurrentAmmo;
    private bool panelOn = false;
    void Start()
    {
        pistolPanel.SetActive(false);
        shotgunPanel.SetActive(false);
        sprayPanel.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if(SaveScript.weaponID == 3)
        {
            if(panelOn==false)
            {
                panelOn = true;
                pistolPanel.SetActive(true);

            }
        }
        if (SaveScript.weaponID == 4)
        {
            if (panelOn == false)
            {
                panelOn = true;
                shotgunPanel.SetActive(true);

            }
        }
        if (SaveScript.weaponID == 5)
        {
            if (panelOn == false)
            {
                panelOn = true;
                sprayPanel.SetActive(true);

            }
        }
        if (SaveScript.inventoryOpen==true)
        {
            pistolPanel.SetActive(false);
            shotgunPanel.SetActive(false);
            sprayPanel.SetActive(false);
            panelOn = false;

        }

    }

    private void OnGUI()
    {
        pistolTotalAmmo.text = SaveScript.ammoAmts[0].ToString();
        shotgunTotalAmmo.text = SaveScript.ammoAmts[1].ToString();
        pistolCurrentAmmo.text = SaveScript.currentAmmo[3].ToString();
        shotgunCurrentAmmo.text = SaveScript.currentAmmo[4].ToString();
    }
}
