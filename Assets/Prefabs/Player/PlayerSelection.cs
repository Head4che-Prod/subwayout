using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; //useful for the buttons
public class PlayerSelection : MonoBehaviour
{
    private static int currentPlayer = 0;
    [SerializeField] private Button previousPlayerButton;
    [SerializeField] private Button nextPlayerButton;
    [SerializeField] public GameObject prefabPlayer;

    private void Awake()
    {
        ChangeMyPlayer(0); //when we start, we are sure that we have one player activated
    }
    
    public void ChangeMyPlayer(int change)
    {
        transform.GetChild(currentPlayer).gameObject.SetActive(false);
        currentPlayer += change + transform.childCount; //will set the index to + change 
        currentPlayer %= transform.childCount;
        transform.GetChild(currentPlayer).gameObject.SetActive(true);
    }

    public void ChangeScene(string sceneName)
    {
        Transform child = prefabPlayer.transform.GetChild(0);
        for (int i = 1; i < child.childCount; i++)
        {
            child.GetChild(currentPlayer + 1).gameObject.SetActive(currentPlayer + 1 == i);
        }
        SceneManager.LoadScene(sceneName);
    }
}
