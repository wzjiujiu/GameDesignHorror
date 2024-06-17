using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemsInventory : MonoBehaviour
{
    public Sprite[] bigIcons;
    public Image bigIcon;
    public string[] titles;
    public Text title;

    public string[] descriptions;
    public Text description;

    private AudioSource audioPlayer;
    public AudioClip click, select;
    public GameObject BIGICON;
    private int choosenItemNumber;
    public Button[] itemButtons;

    public Text amtText;
    private int updatehealth;
    private float updatemental;

    private bool addHealth = false;
    private bool addMental = false;


    public GameObject useButton;
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
        bigIcon.sprite = bigIcons[0];
        title.text = titles[0];
        description.text = descriptions[0];
        useButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {


    }
    private void OnEnable()
    {
        int i;
        for (i = 0; i < itemButtons.Length; i++)
        {
            if (SaveScript.itemPickedUp[i] == false)
            {
                itemButtons[i].image.color = new Color(1, 1, 1, 0.06f);
                itemButtons[i].image.raycastTarget = false;

            }
            if (SaveScript.itemPickedUp[i] == true)
            {
                itemButtons[i].image.color = new Color(1, 1, 1, 1);
                itemButtons[i].image.raycastTarget = true;

            }
        }
        if (SaveScript.itemAmts[choosenItemNumber]<=0)
        {
            ChooseItem(0);
        }
        ChooseItem(choosenItemNumber);
    }

    public void ChooseItem(int itemNumber)
    {

        if (itemNumber < 2)
        {
            useButton.SetActive(false);
        }
        else if (itemNumber == 4)
        {
            BIGICON.transform.localPosition = new Vector3(-150.6f, 207.7f, 0f);
            useButton.SetActive(true);
        }
        else if (itemNumber == 3)
        {
            BIGICON.transform.localPosition = new Vector3(-81f, 207.7f, 0f);
            useButton.SetActive(true);
        }
        else
        {
            BIGICON.transform.localPosition = new Vector3(-25.5f, 207.7f, 0f);
            useButton.SetActive(true);
        }

        if (SaveScript.itemAmts[itemNumber] == 0)
        {
            SaveScript.itemPickedUp[choosenItemNumber] = false;
            useButton.SetActive(false);

        }

        bigIcon.sprite = bigIcons[itemNumber];
        title.text = titles[itemNumber];
        description.text = descriptions[itemNumber];
        if (audioPlayer != null)
        {
            audioPlayer.clip = click;
            audioPlayer.Play();
        }
        choosenItemNumber = itemNumber;


        amtText.text="Amts: " + SaveScript.itemAmts[itemNumber];
        

    }

    public void AssignItem()
    {
        SaveScript.itemID = choosenItemNumber;
        audioPlayer.clip = select;
        audioPlayer.Play();
        if (choosenItemNumber != 9 && choosenItemNumber != 10)
        {
            SaveScript.itemAmts[choosenItemNumber]--;
            ChooseItem(choosenItemNumber);
            if (SaveScript.itemAmts[choosenItemNumber] == 0)
            {
                SaveScript.itemPickedUp[choosenItemNumber] = false;
                useButton.SetActive(false);

            }
        }

        if (addHealth == true)
        {
            addHealth = false;
            if (SaveScript.health < 100)
            {
                SaveScript.health += updatehealth;
               
            }
            if (SaveScript.health > 100)
            {
                SaveScript.health =100;
            }

        }
        if (addMental == true)
        {
            addMental = false;
            if (SaveScript.mental < 100)
            {
                SaveScript.mental += updatemental;
            }
            if (SaveScript.mental > 100)
            {
                SaveScript.mental =100;
            }
        }

        if (choosenItemNumber == 9)
        {
            if(SaveScript.doorObject!=null)
            {
                if((int)SaveScript.doorObject.GetComponent<DoorType>().chooseDoor==1)
                {
                    if (SaveScript.doorObject.GetComponent<DoorType>().locked == true)
                    {
                        SaveScript.doorObject.GetComponent<DoorType>().locked = false;

                    }
                }
            }
        }

        if (choosenItemNumber == 10)
        {
            if (SaveScript.doorObject != null)
            {
                if ((int)SaveScript.doorObject.GetComponent<DoorType>().chooseDoor == 2)
                {
                    if (SaveScript.doorObject.GetComponent<DoorType>().locked == true)
                    {
                        SaveScript.doorObject.GetComponent<DoorType>().locked = false;

                    }
                }
            }
        }
    }

    public void AddHealth(int healthupdate)
    {
        updatehealth = healthupdate;
        addHealth = true;
    }

    public void AddMental(int mentalhupdate)
    {
        updatemental = mentalhupdate;
        addMental = true;
    }
}
