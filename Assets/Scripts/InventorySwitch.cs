using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySwitch : MonoBehaviour
{
    public GameObject weaponPanel, itemsPanel;

    void Start()
    {
        weaponPanel.SetActive(true);
        itemsPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchItemsOn()
    {
        weaponPanel.SetActive(false);
        itemsPanel.SetActive(true);
    }

    public void SwitchWeaponsOn()
    {
        weaponPanel.SetActive(true);
        itemsPanel.SetActive(false);
    }
}
