using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HomeMenu
{
	public class DynamicButton : MonoBehaviour, IPointerEnterHandler, IDeselectHandler, ISelectHandler
	{
		private Selectable _selectable;
		private GameObject _childToAppear; // The child GameObject to toggle
    
		private void Awake()
		{
			// Automatically retrieve the Selectable component (e.g., Button, Toggle) attached to the same GameObject
			_selectable = GetComponent<Selectable>();
			_childToAppear = (transform.Find("AppearOnSelect") ?? transform.GetChild(0).GetChild(0)).gameObject;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (gameObject.GetComponent<Button>().interactable)
				_selectable.Select();
		}

		public void OnDeselect(BaseEventData eventData)
		{
			_childToAppear.SetActive(false);
		}

		public void OnSelect(BaseEventData eventData)
		{
			_childToAppear.SetActive(true);
		}
	}
}
