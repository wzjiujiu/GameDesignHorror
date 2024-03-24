using System.Collections;
using System.Collections.Generic;
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
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
        bigIcon.sprite = bigIcons[0];
        title.text = titles[0];
        description.text = descriptions[0];

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
    }

    public void ChooseItem(int itemNumber)
    {
        if (itemNumber == 4)
        {
            BIGICON.transform.localPosition = new Vector3(-150.6f, 207.7f, 0f);
        }
        else if (itemNumber == 3)
        {
            BIGICON.transform.localPosition = new Vector3(-81f, 207.7f, 0f);
        }
        else
        {
            BIGICON.transform.localPosition = new Vector3(-25.5f, 207.7f, 0f);
        }
        bigIcon.sprite = bigIcons[itemNumber];
        title.text = titles[itemNumber];
        description.text = descriptions[itemNumber];
        audioPlayer.clip = click;
        audioPlayer.Play();
        choosenItemNumber = itemNumber;

    }

    public void AssignItem()
    {
        SaveScript.itemID = choosenItemNumber;
        audioPlayer.clip = select;
        audioPlayer.Play();
    }
}
