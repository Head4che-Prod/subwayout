using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//useful for the buttons
namespace PlayerSelectionMenu
{
    public class PlayerSelection : MonoBehaviour
    {
        public static byte CurrentPlayerSkin { get; private set; } = 0;
        private static PlayerSelectionAnimation[] PlayerModels { get; set; }
        [SerializeField] private Button previousPlayerButton;
        [SerializeField] private Button nextPlayerButton;

        private void Awake()
        {
            int childCount = transform.childCount;
            PlayerModels = new PlayerSelectionAnimation[childCount];
            for (int i = 0; i < childCount; i++)
            {
                PlayerModels[i] = transform.GetChild(i).GetComponent<PlayerSelectionAnimation>();
                if (i == CurrentPlayerSkin) PlayerModels[i].ShowAtStart();
            }
        }
    
        /// <summary>
        /// Changes player skin locally.
        /// </summary>
        /// <param name="change">By what offset the skins should be switched. Is 1 or -1.</param>
        public void ChangeMyPlayer(int change) //for my buttons < and >
        {
            PlayerModels[CurrentPlayerSkin].MoveOut(change == -1);
            CurrentPlayerSkin = (byte)((CurrentPlayerSkin + change + transform.childCount) % transform.childCount); // Changes the current skin and ensures no overflow
            PlayerModels[CurrentPlayerSkin].MoveIn(change == 1);
        }

        public void ChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
