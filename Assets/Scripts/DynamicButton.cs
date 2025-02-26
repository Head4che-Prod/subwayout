using UnityEngine;

public class DynamicButton : MonoBehaviour
{
	private GameObject _childToAppear; // The child GameObject to toggle
    
	private void Awake()
    {
        // Automatically retrieve the Selectable component (e.g., Button, Toggle) attached to the same GameObject
		_childToAppear = transform.Find("AppearOnSelect").gameObject;
	}

	public void Activate()
    {
		_childToAppear.SetActive(true);
    }

    public void Deactivate()
    {
		_childToAppear.SetActive(false);
    }
}
