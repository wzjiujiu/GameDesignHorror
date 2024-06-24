using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventory : MonoBehaviour
{
    // Start is called before the first frame update

    public Sprite[] bigIcons;
    public Image bigIcon;
    public string[] titles;
    public Text title;

    public string[] descriptions;
    public Text description;

    private AudioSource audioPlayer;
    public AudioClip click, select;
    public GameObject BIGICON;
    private int choosenWeaponNumber;
    public Button[] weaponButtons;

    public GameObject useButton, combineButton;
    public GameObject combinePanel, combineUseButton;
    public Image[] combineItems;
    public Text amtText;
    public GameObject sprayPanel;



    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
        bigIcon.sprite = bigIcons[0];
        title.text = titles[0];
        description.text = descriptions[0];

        combinePanel.SetActive(false);
        combineButton.SetActive(false);
       

    }

    // Update is called once per frame
    void Update()
    {

        
    }

    private void OnEnable()
    {
        int i;
        for (i=0;i<weaponButtons.Length;i++)
        {
            if (SaveScript.weaponPickedUp[i] ==false)
            {
                weaponButtons[i].image.color = new Color(1, 1, 1, 0.06f);
                weaponButtons[i].image.raycastTarget = false;

            }
            if (SaveScript.weaponPickedUp[i] == true)
            {
                weaponButtons[i].image.color = new Color(1, 1, 1, 1);
                weaponButtons[i].image.raycastTarget = true;

            }
        }

        if (choosenWeaponNumber < 5)
        {
            combinePanel.SetActive(false);
            combineButton.SetActive(false);
        }

        if (SaveScript.itemPickedUp[2]==true)
        {
            combineItems[0].color = new Color(1, 1, 1, 1);
        }

        if (SaveScript.itemPickedUp[2] == false)
        {
            combineItems[0].color = new Color(1, 1, 1, 0.06f);
        }

        if (SaveScript.itemPickedUp[3] == true)
        {
            combineItems[1].color = new Color(1, 1, 1, 1);
        }

        if (SaveScript.itemPickedUp[3] == false)
        {
            combineItems[1].color = new Color(1, 1, 1, 0.06f);
        }

        if (SaveScript.weaponAmts[choosenWeaponNumber] <= 0)
        {
            ChooseWeapon(0);
        }

        
    }

    public void ChooseWeapon(int weaponNumber)
    {
        if (weaponNumber == 4)
        {
            BIGICON.transform.localPosition = new Vector3(-150.6f, 207.7f, 0f);
        }
        else if (weaponNumber == 3) 
        {
            BIGICON.transform.localPosition = new Vector3(-81f, 207.7f, 0f);
        }
        else
        {
            BIGICON.transform.localPosition = new Vector3(-25.5f, 207.7f, 0f);
        }
        bigIcon.sprite = bigIcons[weaponNumber];
        title.text = titles[weaponNumber];
        description.text = descriptions[weaponNumber];  
        audioPlayer.clip = click;
        audioPlayer.Play();
        choosenWeaponNumber = weaponNumber;

        if(choosenWeaponNumber >=5)
        {
            combineButton.SetActive(true);
            combinePanel.SetActive(false);
        }
        if (choosenWeaponNumber < 5)
        {
            combinePanel.SetActive(false);
            combineButton.SetActive(false);
        }
        amtText.text = "Amts: " + SaveScript.weaponAmts[weaponNumber];

        if (choosenWeaponNumber == 5)
        {
            useButton.SetActive(false);
        }
        else
        {
            useButton.SetActive(true);
        }


    }
    public void AssignWeapon()
    {
        SaveScript.weaponID = choosenWeaponNumber;
        audioPlayer.clip = select;
        audioPlayer.Play();
    }

    public void CombineAssignWeapon()
    {
        if(choosenWeaponNumber ==5) 
        {
            SaveScript.weaponID=choosenWeaponNumber;
            if (sprayPanel.GetComponent<SprayScript>().sprayAmount <= 0.0f)
            {
                sprayPanel.GetComponent<SprayScript>().sprayAmount = 1.0f;
            }
        }

        if (choosenWeaponNumber == 6)
        {
            SaveScript.weaponID = choosenWeaponNumber+=1;
        }

        audioPlayer.clip = select;
        audioPlayer.Play();

    }

    public void CombineAction()
    {
        combinePanel.SetActive(true );

        if (choosenWeaponNumber == 5)
        {
            combineItems[1].transform.gameObject.SetActive(false);
            if (SaveScript.itemPickedUp[2] == true)
            {
                combineUseButton.SetActive(true);
            }

            if (SaveScript.itemPickedUp[2] == false)
            {
                combineUseButton.SetActive(false);
            }

        }

        if (choosenWeaponNumber == 6)
        {
            combineItems[1].transform.gameObject.SetActive(true);
            if (SaveScript.itemPickedUp[2] == true && SaveScript.itemPickedUp[3] == true)
            {
                combineUseButton.SetActive(true);
            }

            if (SaveScript.itemPickedUp[2] == false || SaveScript.itemPickedUp[3] == false)
            {
                combineUseButton.SetActive(false);
            }

        }

    }
}
