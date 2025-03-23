using System.Collections.Generic;
using Prefabs.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//useful for the buttons
namespace Prefabs.UI
{
    public class PlayerSelection : MonoBehaviour
    {
        private int _currentPlayer = 0;
        private GameObject[] _playerModels;
        [SerializeField] private Button previousPlayerButton;
        [SerializeField] private Button nextPlayerButton;
        [SerializeField] private PlayerObject prefabPlayer;

        private void Awake()
        {
            int childCount = transform.childCount;
            _playerModels = new GameObject[childCount];
            for (int i = 0; i < childCount; i++)
            {
                _playerModels[i] = transform.GetChild(i).gameObject;
                _playerModels[i].SetActive(i == _currentPlayer);
            }
        }
    
        public void ChangeMyPlayer(int change) //for my buttons < and >
        {
            _playerModels[_currentPlayer].SetActive(false);
            _currentPlayer += change + transform.childCount; //will set the index to + change 
            _currentPlayer %= transform.childCount;
            _playerModels[_currentPlayer].SetActive(true);
        }

        public void ChangeScene(string sceneName)
        {
            Transform child = prefabPlayer.transform.GetChild(0);
            Debug.Log(_currentPlayer);
            for (int i = 1; i < child.childCount; i++)
            {
                _playerModels[_currentPlayer].SetActive(_currentPlayer + 1 == i);
            }
            SceneManager.LoadScene(sceneName);
        }
    }
}
