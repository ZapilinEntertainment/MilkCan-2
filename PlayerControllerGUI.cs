using UnityEngine;
using System.Collections;

public class PlayerControllerGUI : MonoBehaviour {
	// 1) Inspector-Input Objects	
	public GameObject outcam;
	public GameObject incam;
	// 2) Inspector-Input Technical Values
	// 3) Inspector-Input Game Values
	// 4) Inspector-Input Textures and Sounds
	// 5) Script-using Objects
	// 6) Script-using Values
	bool inside_camera=true;

	int k=16;
	int sw;
	int sh;
	// 7) Script-using Textures
	// 8) Scripts references
	public PlayerController pc;
	// 9) Debugging variables

	void Start () 
	{
		sw=Screen.width;
		sh=Screen.height;
		if (inside_camera) pc.head.SetActive(false);
	}

	void Update () {
		if (Global.pause||!Global.playable||pc.network_identity==null) return;

		if (Input.GetKeyDown("v")) {
			if (inside_camera) {
				Global.cam=outcam;
				outcam.SetActive(true);
				incam.SetActive(false);
				pc.head.SetActive(true);
				inside_camera=false;
			}
			else {
				Global.cam=incam;
				outcam.SetActive(false);
				incam.SetActive(true);
				inside_camera=true;
				pc.head.SetActive(false);
			}
		}

		if (Input.GetKeyDown("z")) {pc.CmdStepToLeft();}
		if (Input.GetKeyDown("c")) {pc.CmdStepToRight();}
		if (Input.GetKeyDown("r")) pc.CmdAutoMoving();
		if (Input.GetKey("w")) {
			if (pc.movement==0||pc.fixed_move) 
			{
				if (pc.network_identity.isClient) pc.CmdMoveForward();
				else pc.RpcMoveForward();
			}
		}
		else 
		{
			if (Input.GetKeyUp("w")&&pc.movement>0&&!pc.fixed_move) 
			{
				if (pc.network_identity.isClient) pc.CmdStopMoving();
				else pc.RpcStopMoving();
			}
			else 
			{
				if (Input.GetKey("s")) 
				{
					if (pc.movement==0||pc.fixed_move) 
					{
						if (pc.network_identity.isClient) pc.CmdMoveBackward();
						else pc.RpcMoveBackward();
					}
				}
				else 
				{
					if (Input.GetKeyUp("s")&&pc.movement<0) 
					{
						if (pc.network_identity.isClient) pc.CmdStopMoving();
						else pc.RpcStopMoving();
					}
				}
			}
		}

		// MECH ROTATION
		if (Input.GetKey("a")) 
		{
			if (pc.mech_rotation_vector==Vector3.zero||pc.mech_rotation_vector.y>0) 
			{
				if (pc.network_identity.isClient) pc.CmdMechRotation(true,false);
				else pc.RpcMechRotation(true,false);
			}
		}
		else {
			if (Input.GetKeyUp("a")) 	
			{
				if (pc.network_identity.isClient) pc.CmdMechRotation(false,false);
				else pc.RpcMechRotation(false,false);
			}
			else 
			{
				if (Input.GetKey("d")) 
				{
					if (pc.mech_rotation_vector==Vector3.zero||pc.mech_rotation_vector.y<0) 
					{
						if (pc.network_identity.isClient) pc.CmdMechRotation(true,true);
						else pc.RpcMechRotation(true,true);
					}
				}
				else 
				{
					if (Input.GetKeyUp("d")) 
					{
						if (pc.network_identity.isClient) pc.CmdMechRotation(false,false);
						else pc.RpcMechRotation(false,false);
					}
				}
			}
		}

		//CABIN ROTATION
		if (Input.GetKey("q")) 
		{
			if (pc.cabin_rotation_vector==Vector3.zero||pc.cabin_rotation_vector.y>0) {
				if (pc.network_identity.isClient) pc.CmdCabinRotation(true,false);
				else pc.RpcCabinRotation(true,false);
			}
		}
		else {
			if (Input.GetKeyUp("q")) 
			{
				if (pc.network_identity.isClient) pc.CmdCabinRotation(false,false);
				else pc.RpcCabinRotation(false,false);
			}
			else 
			{
			if (Input.GetKey("e")) 
				{
					if (pc.cabin_rotation_vector==Vector3.zero||pc.cabin_rotation_vector.y<0) 
					{
						if (pc.network_identity.isClient) pc.CmdCabinRotation(true,true);
						else pc.RpcCabinRotation(true,true);
					}
				}
				else 
				{
					if (Input.GetKeyUp("e")) {
						if (pc.network_identity.isClient) pc.CmdCabinRotation(false,false);
						else pc.RpcCabinRotation(false,false);
					}
				}
			}
		}

		if (inside_camera) 
		{
			float mx=Input.mousePosition.x/Screen.width;
			float my=Input.mousePosition.y/Screen.height;
			Vector3 head_rotateTo=new Vector3(-180*my+90,180*mx-90,0);
			if (head_rotateTo!=pc.head.transform.localRotation.eulerAngles) 
			{
				incam.transform.localRotation=Quaternion.Euler(head_rotateTo);
				if (pc.network_identity.isClient) pc.CmdHeadRotation(incam.transform.forward);
				else pc.RpcHeadRotation(incam.transform.forward);
			}
		}
	}


	void OnGUI () 
	{
		k=Global.gui_piece;

	}
}
