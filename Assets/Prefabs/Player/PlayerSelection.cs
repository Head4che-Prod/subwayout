using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    private int currentPlayer;
    private void SelectMyPlayer(int _index)
    {
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
}
