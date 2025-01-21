using UnityEngine;

public class DoorOpening2 : MonoBehaviour
{
    public Animator DoorAnimOpening2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            DoorAnimOpening2.SetBool("HanoiSolved", true);
        }
    }
}
