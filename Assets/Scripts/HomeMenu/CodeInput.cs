using UnityEngine;
using UnityEngine.UI;

namespace HomeMenu
{
    /// <summary>
    /// Ensure that the entered join code has 6 characters.
    /// </summary>
    public class CodeInput : MonoBehaviour
    {
        GameObject _buttonJoin;
        void Start()
        {
            _buttonJoin = transform.parent.Find("JoinButton").gameObject;
        }

        public void OnTextChange(string text) {
            _buttonJoin.GetComponent<Button>().interactable = text.Length == 6;
        }
    }
}
