using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ensure that the entered join code has 6 characters.
/// </summary>
public class CodeInput : MonoBehaviour
{
    GameObject ButtonJoin;
    void Start()
    {
        ButtonJoin = transform.parent.Find("JoinButton").gameObject;
    }

    public void OnTextChange(string text) {
        ButtonJoin.GetComponent<Button>().interactable = text.Length == 6;
    }
}
