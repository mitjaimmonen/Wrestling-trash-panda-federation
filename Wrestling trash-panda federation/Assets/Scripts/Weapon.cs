using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	public Player player;
	public Collider hitCollider;
	public int damage;
	public float chargeTime;
	public float hitTime;
	public bool oneTimeUse;
	public bool breakable;
	public float breakForce;

	void Awake()
	{
		if (!hitCollider)
			hitCollider = GetComponent<Collider>();
	}
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			Player enemy = other.GetComponentInParent<Player>();
			if (enemy && enemy != player)
			{
				enemy.GetHit(damage);

			}
		}
	}
}
