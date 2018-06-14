using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

[System.Serializable]
public class Playerdata {

	public int characterIndex;
	public PlayerIndex gamepadPlayerIndex;
	public int meshNumber; //Used to spawn one of four different meshes, chosen at main menu -- Dunno if it should be here or Player

}
