using Prefabs.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//useful for the buttons
namespace Prefabs.UI
{
    public class PlayerSelection : MonoBehaviour
    {
        public static byte CurrentPlayerSkin { get; private set; } = 0;
        private static GameObject[] PlayerModels { get; set; }
        [SerializeField] private Button previousPlayerButton;
        [SerializeField] private Button nextPlayerButton;

        private void Awake()
        {
            int childCount = transform.childCount;
            PlayerModels = new GameObject[childCount];
            for (int i = 0; i < childCount; i++)
            {
                PlayerModels[i] = transform.GetChild(i).gameObject;
                PlayerModels[i].SetActive(i == CurrentPlayerSkin);
            }
        }
    
        /// <summary>
        /// Changes player skin locally.
        /// </summary>
        /// <param name="change">By what offset the skins should be switched. Is 1 or -1.</param>
        public void ChangeMyPlayer(int change) //for my buttons < and >
        {
            PlayerModels[CurrentPlayerSkin].SetActive(false);
            CurrentPlayerSkin = (byte)((CurrentPlayerSkin + change + transform.childCount) % transform.childCount); // Changes the current skin and ensures no overflow
            PlayerModels[CurrentPlayerSkin].SetActive(true);
        }

        public void ChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
