using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatars : MonoBehaviour {

	public Mesh[] avatarMeshes = new Mesh[4];
	public MeshFilter[] avatarMeshFilters = new MeshFilter[4];

	public int [] indexes = new int[4];

    public int meshNumber (int playerNumber)
    {
        return indexes[playerNumber];
    }

	void Awake()
	{
		GamepadStateHandler gamepad = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GamepadStateHandler>();
		gamepad.avatars = this;
	}

	public void ChangeAvatar(int playerNumber, int plusOrMinus)
	{
		Debug.Log("Change avatar: " + playerNumber);
		indexes[playerNumber] += plusOrMinus;
		if (indexes[playerNumber] > 3)
			indexes[playerNumber] = 0;
		if (indexes[playerNumber] < 0)
			indexes[playerNumber] = 3;
		avatarMeshFilters[playerNumber].mesh = avatarMeshes[indexes[playerNumber]];
	}

    

}
