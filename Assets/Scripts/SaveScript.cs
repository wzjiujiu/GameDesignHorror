using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class SaveScript : MonoBehaviour
{
    public static bool inventoryOpen = false;
    public static bool OptionOpen = false;
    public static int weaponID = 0;
    public static int itemID = 0;
    public static bool [] weaponPickedUp = new bool[7];
    public static bool[] itemPickedUp = new bool[12];
    public static bool pausePanelOpen = false;
    public static int[] weaponAmts = new int[7];
    public static int[] itemAmts = new int[12];
    public static int[] ammoAmts = new int[3];
    public static int[] currentAmmo = new int[8];
    public static bool change = false;
    public static float mental;
    public static int health;
    public static GameObject doorObject;
    public static float movementspeed = FirstPersonController.walkSpeedAmt;

    public static List<GameObject> monsterChasing= new List<GameObject>();



    // Start is called before the first frame update
    void Start()
    {
        mental = FirstPersonController.FPSmental;
        health = 99;
        weaponAmts[0] = 1;
        weaponPickedUp[0] = true;
        itemPickedUp[0 ] = true;
        itemPickedUp[1 ] = true;
        itemAmts[0] = 1;
        itemAmts[1] = 1;
        ammoAmts[0] = 12;
        ammoAmts[1] = 4;
        movementspeed = FirstPersonController.walkSpeedAmt;

        for (int i = 0;i<currentAmmo.Length;i++)
        {
            currentAmmo[i] = 2;
        }
        currentAmmo[3] = 12;
        currentAmmo[5] = 0;
       

    }

    // Update is called once per frame
    void Update()
    {
        FirstPersonController.FPSmental = mental;
        FirstPersonController.FPShealth = health;
        if(FirstPersonController.inventorySwitchedOn==true)
        {
            inventoryOpen = true;
        }
        if (FirstPersonController.inventorySwitchedOn == false)
        {
            inventoryOpen = false;
        }

        if (FirstPersonController.pausePanelSwitchedOn == true)
        {
            OptionOpen = true;
        }
        if (FirstPersonController.pausePanelSwitchedOn == false)
        {
            OptionOpen = false;
        }

        if (Input.GetAxis("Vertical")!=0 && Input.GetKey(KeyCode.LeftShift) && FirstPersonController.FPSmental>0.0f)
        {
            FirstPersonController.FPSmental-=5*Time.deltaTime;
            mental=FirstPersonController.FPSmental;
        }
        if (mental < 100)
        {
            FirstPersonController.FPSmental += 2.5f * Time.deltaTime;
            mental = FirstPersonController.FPSmental;

        }

        if(change==true)
        {
            change = false;
            for(int i = 1;i<weaponAmts.Length;i++)
            {
                if (weaponAmts[i] > 0)
                {
                    weaponPickedUp[i] = true;
                }
                else if (weaponAmts[i] == 0) 
                {
                    weaponPickedUp[i] = false;
                        
                }
            }

            for (int i = 2; i < itemAmts.Length; i++)
            {
                if (itemAmts[i] > 0)
                {
                    itemPickedUp[i] = true;
                }
                else if (itemAmts[i] == 0)
                {
                    itemPickedUp[i] = false;

                }
            }
        }

    }
}
