using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DynamicButton : MonoBehaviour, IPointerEnterHandler, IDeselectHandler, ISelectHandler
{
    private Selectable selectable;
	private GameObject childToAppear; // The child GameObject to toggle
    
	private void Awake()
    {
        // Automatically retrieve the Selectable component (e.g., Button, Toggle) attached to the same GameObject
        selectable = GetComponent<Selectable>();
		childToAppear = (transform.Find("AppearOnSelect") ?? transform.GetChild(0).GetChild(0)).gameObject;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (gameObject.GetComponent<Button>().interactable)
			selectable.Select();
	}

	public void OnDeselect(BaseEventData eventData)
    {
		childToAppear.SetActive(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
		childToAppear.SetActive(true);
    }
}
