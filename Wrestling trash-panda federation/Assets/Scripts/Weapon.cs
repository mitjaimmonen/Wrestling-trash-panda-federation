using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public Player player;
    public Collider hitCollider;
    public int damage;
    public float chargeTime;
    public float hitTime;
    public bool oneTimeUse;
    public bool breakable;
    public float breakForce;
    public Collider[] colliders;
    public GameObject brokenVersion;
    public GameObject regularVersion;

    public bool grabbed;
    private Transform grabPoint;

    void Awake()
    {        
        if (!grabPoint)
            grabPoint = transform.Find("GrabPoint");
        if (!hitCollider)
            hitCollider = GetComponentInChildren<Collider>();
    }

    public void ToBeGrabbed(GameObject grabPosition)
    {
        OnHeld(grabPosition.transform.parent.gameObject);
        gameObject.transform.SetParent(grabPosition.transform);
        StartCoroutine(LerpToHand());
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public void ToBeDropped()
    {
        grabbed = false;
        transform.parent = null;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
        Break();
    }

    IEnumerator LerpToHand()
    {
        while (transform.localPosition != Vector3.zero)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, 0.5f);
            yield return null;
        }
    }

    public void OnHeld(GameObject _player)
    {
        player = _player.GetComponent<Player>();
        grabbed = true;

    }

    public void Break()
    {
        //break
    }

    void OnTriggerEnter(Collider other)
    {
        //if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        //{
        //	Player enemy = other.GetComponentInParent<Player>();
        //	if (enemy)
        //	{
        //		enemy.GetHit(damage);

        //	}
        //}
        //if (!grabbed)
        //{
        //    if (other.gameObject.tag =="Player")
        //    {
        //        ToBeGrabbed(other.gameObject.GetComponentInParent<Player>().grabPoint);        

        //    }
        //}
    }
}
