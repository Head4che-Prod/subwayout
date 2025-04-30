using System.Collections.Generic;
using Objects;
using UnityEngine;


public class BagManager: ObjectActionable
{
    [SerializeField] private GameObject _bagOpen;
    [SerializeField] private GameObject _bagClose;
    [SerializeField] private List<GameObject> objectsInBag;

    protected override void Action()
    {
        Debug.Log("Action played");
        _bagOpen.SetActive(true);
        _bagClose.SetActive(false);
        foreach (GameObject obj in objectsInBag)
        {
            obj.SetActive(true);
            Instantiate(obj, _bagOpen.transform.position+ new Vector3(0,10,0), Quaternion.identity);
                obj.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-10,10),Random.Range(30,50),Random.Range(-10,10)), ForceMode.Impulse);
        }
    }
}
