using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoType : MonoBehaviour
{
    // Start is called before the first frame update
    public enum typeOfAmmo
    {
      pistolammo,
      shotgunammo
    }

    public typeOfAmmo chooseAmmo;
}