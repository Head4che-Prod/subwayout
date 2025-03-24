using Prefabs.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//useful for the buttons
namespace Prefabs.UI
{
    public class PlayerSelection : MonoBehaviour
    {
        public static int CurrentPlayer { get; private set; } = 0;
        public static GameObject[] PlayerModels { get; private set; }
        [SerializeField] private Button previousPlayerButton;
        [SerializeField] private Button nextPlayerButton;

        private void Awake()
        {
            int childCount = transform.childCount;
            PlayerModels = new GameObject[childCount];
            for (int i = 0; i < childCount; i++)
            {
                PlayerModels[i] = transform.GetChild(i).gameObject;
                PlayerModels[i].SetActive(i == CurrentPlayer);
            }
        }
    
        public void ChangeMyPlayer(int change) //for my buttons < and >
        {
            PlayerModels[CurrentPlayer].SetActive(false);
            CurrentPlayer += change + transform.childCount; //will set the index to + change 
            CurrentPlayer %= transform.childCount;
            PlayerModels[CurrentPlayer].SetActive(true);
        }

        public void ChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
