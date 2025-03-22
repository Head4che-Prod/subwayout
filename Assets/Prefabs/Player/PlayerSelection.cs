using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; //useful for the buttons
public class PlayerSelection : MonoBehaviour
{
    private int currentPlayer;
    [SerializeField] private Button previousPlayerButton;
    [SerializeField] private Button nextPlayerButton;
    [SerializeField] private string sceneName;

    private void Awake()
    {
        SelectMyPlayer(0); //when we start, we are sure that we have one player activated
    }
    private void SelectMyPlayer(int _index)
    {
        previousPlayerButton.interactable = (_index != 0); //set the "previous" button to interactable only if we're not on the first model
        nextPlayerButton.interactable = (_index != transform.childCount-1); //set the "next" button to interactable only if we're not on the last model
        for (int i = 0; i < transform.childCount; i++) //for loop that iterates until no children
        {
            transform.GetChild(i).gameObject.SetActive(i == _index); // will activate the child of the index
        }
        
    }

    public void ChangeMyPlayer(int _change)
    {
        currentPlayer += _change; //will set the index to + change 
        SelectMyPlayer(currentPlayer); //will immediately call the function with the new index
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
