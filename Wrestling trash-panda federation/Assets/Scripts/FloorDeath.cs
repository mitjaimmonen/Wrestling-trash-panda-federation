using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorDeath : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {      
        if (other.gameObject.transform.parent.tag == "Player")
        {           
            other.gameObject.GetComponentInParent<Player>().TakeOut();
        }
    }
}
