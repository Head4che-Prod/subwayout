using UnityEngine;

public class DoorOpening1 : MonoBehaviour
{

    public Animator DoorAnimOpening1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            DoorAnimOpening1.SetBool("HanoiSolved", true);
        }
   


    }
}
