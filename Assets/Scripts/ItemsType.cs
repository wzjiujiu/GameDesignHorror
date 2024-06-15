using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsType : MonoBehaviour
{
    // Start is called before the first frame update
    public enum typeOfItem
    {
        flashlight,
        nightvision,
        lighter,
        rags,
        healthkit,
        pills,
        waterbottle,
        apple,
        battery,
        housekey,
        cabinkey,
        jerrycan
    }

    public typeOfItem chooseItem;
}
