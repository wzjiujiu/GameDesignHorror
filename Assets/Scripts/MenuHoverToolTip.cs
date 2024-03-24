using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuHoverToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button buttonHover;
    private TMP_Text tooltipText;
    private GameObject tooltipPanel;

    private void Start()
    {
        tooltipPanel = GameObject.Find("ToolTipPanel");
        tooltipText = tooltipPanel.GetComponentInChildren<TMP_Text>();

        // Add EventTrigger component to handle hover events
        EventTrigger trigger = buttonHover.gameObject.AddComponent<EventTrigger>();

        // Add listener for PointerEnter event
        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
        pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
        pointerEnterEntry.callback.AddListener((eventData) => { OnPointerEnter((PointerEventData)eventData); });
        trigger.triggers.Add(pointerEnterEntry);

        // Add listener for PointerExit event
        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
        pointerExitEntry.eventID = EventTriggerType.PointerExit;
        pointerExitEntry.callback.AddListener((eventData) => { OnPointerExit((PointerEventData)eventData); });
        trigger.triggers.Add(pointerExitEntry);
    }

    private void OnDestroy()
    {
        // Cleanup EventTrigger component
        Destroy(buttonHover.GetComponent<EventTrigger>());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }

    private void ShowTooltip()
    {
        tooltipText.gameObject.SetActive(true);
        switch (buttonHover.name)
        {
            case "ResumeButton":
                tooltipText.text = "Continue playing game";
                break;
            case "SaveButton":
                tooltipText.text = "Save your current progress";
                break;
            case "LoadButton":
                tooltipText.text = "Load previously saved game";
                break;
            case "QuitButton":
                tooltipText.text = "Exit game";
                break;

            default:
                tooltipText.text = "Default Tooltip";
                break;
        }
    }

    private void HideTooltip()
    {
        tooltipText.gameObject.SetActive(false);
    }
}
