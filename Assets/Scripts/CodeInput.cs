using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodeInput : MonoBehaviour
{
    // TextMeshPro TextTransform;
    GameObject ButtonJoin;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // TextTransform = transform.Find("Text Area/Text").GetComponent<TextMeshPro>();
        ButtonJoin = transform.parent.Find("JoinButton").gameObject;
    }

    public void OnTextChange(string text) {
        // bool containsLower = false;
        // foreach (char c in text)
        // {
        //     if (char.IsLower(c))
        //     {
        //         containsLower = true;
        //         break;
        //     }
        // }
        // if (containsLower) {
        //     if (TextTransform == null)
        //     {
        //         TextTransform = transform.Find("Text Area/Text").GetComponent<TextMeshPro>();
        //     }
        //     Debug.Log(TextTransform);
        //     foreach (var component in TextTransform.GetComponents<Component>())
        //     {
        //         Debug.Log(component.name);
        //     }
        //     TextTransform.text = text.ToUpper();
        // }
        // // Set the width of the text box to the width of the text
        // TextTransform.GetComponent<RectTransform>().sizeDelta = new Vector2((text.Length + 1) * 600 / 6, TextTransform.GetComponent<RectTransform>().sizeDelta.y);
        ButtonJoin.GetComponent<Button>().interactable = text.Length == 6;
    }
}
