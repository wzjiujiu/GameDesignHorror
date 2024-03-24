using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class SaveScript : MonoBehaviour
{
    public static bool inventoryOpen = false;
    public static int weaponID = 0;
    public static int itemID = 0;
    public static bool [] weaponPickedUp = new bool[7];
    public static bool[] itemPickedUp = new bool[12];
    public static bool pausePanelOpen = false;


    // Start is called before the first frame update
    void Start()
    {
        weaponPickedUp[0] = true;
        weaponPickedUp[1] = true;
        weaponPickedUp[2] = true;
        weaponPickedUp[3] = true;
        weaponPickedUp[4] = true;
        weaponPickedUp[5] = true;
        itemPickedUp[0 ] = true;
        itemPickedUp[1 ] = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(FirstPersonController.inventorySwitchedOn==true)
        {
            inventoryOpen = true;
        }
        if (FirstPersonController.inventorySwitchedOn == false)
        {
            inventoryOpen = false;
        }

    }
}
