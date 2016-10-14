using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {
	//  1) Inspector-Input Objects	
	    public GameObject head;
	    public GameObject cabin;
		public Animator anim;
	// 2) Inspector-Input Technical Values
	public float deep=25;
	public float position_sync_speed=1;
	public float rotation_sync_speed=1;
	// 3) Inspector-Input Game Values
	public float cabin_rot_speed=30;
	public float head_rot_speed=60;
	public float maxspeed=5;
	public float maxhp=50000;
	public float maxshield=50000;
	public float rot_speed=20;
	public float shield_reg_speed=500;
	public float speed=10;
	// 4) Inspector-Input Textures and Sounds
	// 5) Script-using Objects
	// 6) Script-using Values
	public bool fixed_move=false;
	public bool multiplayer=false; //больше одного игрока или нет
	public bool prev_step_was_right=false;

	int layerMask;

	float hp=0;		
	float shield_value=0;
	float cabin_angle_x=0;
	public int movement=0; //направление и передача

	Vector3 mpos;
	public Vector3 cabin_rotation_vector;
	public Vector3 mech_rotation_vector;
	Vector3 head_look_vector;
	[SyncVar]
	Vector3 realpos;
	[SyncVar]
	Vector3 realrot;

	Quaternion rotateTo;
	RaycastHit camhit;
	// 7) Script-using Textures
	// 8) Scripts references
	public NetworkIdentity network_identity;
	PlayerControllerGUI pc_gui;
	// 9) Debugging variables

		// Use this for initialization
		void Start () {
		//Cursor.visible=false;
			Global.score=0;
			Global.player=gameObject;
			Global.p_controller=this;
			layerMask=1<<8; //terrain caster
			hp=maxhp;
			shield_value=maxshield;
		network_identity=gameObject.GetComponent<NetworkIdentity>();
		if (isLocalPlayer) 
		{
			pc_gui=gameObject.AddComponent<PlayerControllerGUI>();
			pc_gui.incam=Instantiate(Resources.Load<GameObject>("warlord_camera")) as GameObject;
			pc_gui.incam.transform.parent=cabin.transform;
			pc_gui.incam.transform.localPosition=new Vector3(0,2,4);
			pc_gui.incam.transform.localRotation=Quaternion.Euler(Vector3.zero);
			pc_gui.pc=this;
			Global.cam=pc_gui.incam;
			head.SetActive(false);
		}
		}

	void Update() 
	{
		if (Global.network_master) 
		{
			if (Global.network_master.players_count>1||!network_identity.isServer) multiplayer=true;
			else multiplayer=false;
		}
		else multiplayer=false;
		if (Global.pause||!Global.playable) return;
		if (movement!=0) 
		{
			RaycastHit rh;
			if (Physics.Raycast(transform.position,Vector3.down,out rh,100,layerMask)) 
			{
			transform.Translate(Vector3.forward*movement*anim.GetFloat("speed")*speed*Time.deltaTime);
				if (rh.point.y+deep<transform.position.y) transform.Translate(Vector3.down*Time.deltaTime); else transform.Translate(Vector3.up*Time.deltaTime);
			}
		}
		if (head.transform.forward!=head_look_vector) {
			head.transform.rotation=Quaternion.LookRotation(head_look_vector,Vector3.up);
		}
		if (mech_rotation_vector!=Vector3.zero) transform.Rotate(mech_rotation_vector*rot_speed*Time.deltaTime,Space.Self);
		if (cabin_rotation_vector!=Vector3.zero) cabin.transform.Rotate(cabin_rotation_vector*cabin_rot_speed*Time.deltaTime,Space.Self);
		if (isServer) 
		{
			realpos=transform.position;
			realrot=transform.rotation.eulerAngles;
		}
		else 
		{
			float d=(transform.position-realpos).magnitude;
			if (d>=position_sync_speed*Time.deltaTime) 
				transform.position+=(realpos-transform.position).normalized*position_sync_speed*Time.deltaTime;
			d=(transform.rotation.eulerAngles-realrot).magnitude;
			if (d>=rotation_sync_speed*Time.deltaTime)
				transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.Euler(realrot),rotation_sync_speed*Time.deltaTime);
		}
	}

	[Command]
	public void CmdStepToLeft() 
	{
		anim.Play("step_to_left");
		Stop();
		if (multiplayer) RpcStepToLeft();
	}
	[ClientRpc]
	public void RpcStepToLeft () 
	{
		if (network_identity.isServer) return;
		anim.Play("step_to_left");Stop();
	}

	[Command]
	public void CmdStepToRight() 
	{
		anim.Play("step_to_right");Stop();
		if (multiplayer) RpcStepToRight();
	}
	[ClientRpc]
	public void RpcStepToRight() 
	{
		if (network_identity.isServer) return;
		anim.Play("step_to_right");Stop();
	}

	[Command]
	public void CmdAutoMoving() 
	{
		if (movement>0&&fixed_move) 
		{
			fixed_move=false;
			movement=0;
			anim.SetInteger("movement",0);
		}
		else
		{
			movement=1;
			anim.SetInteger("movement",1);
			fixed_move=true;
		}
		if (multiplayer) RpcAutoMoving(movement,fixed_move);
	}
	[ClientRpc]
	public void RpcAutoMoving(int m, bool fmove) 
	{
		if (network_identity.isServer) return;
		movement=m; 
		fixed_move=fmove;
		anim.SetInteger("movement",m);
	}

	[Command]
	public void CmdMoveForward() 
	{
		movement=1;
		anim.SetInteger("movement",1);
		fixed_move=false;
		if (multiplayer) RpcMoveForward();
	}
	[ClientRpc]
	public void RpcMoveForward()
	{
		if (network_identity.isServer) return;
		movement=1;
		anim.SetInteger("movement",1);
		fixed_move=false;
	}

	[Command]
	public void CmdMoveBackward() 
	{
		movement=-1;
		anim.SetInteger("movement",-1);
		fixed_move=false;
		if (multiplayer) RpcMoveBackward();
	}
	[ClientRpc]
	public void RpcMoveBackward() 
	{
		if (network_identity.isServer) return;
		movement=-1;
		anim.SetInteger("movement",-1);
		fixed_move=false;
	}

	[Command]
	public void CmdStopMoving() 
	{
		movement=0;
		fixed_move=false;
		anim.SetInteger("movement",0);
		if (multiplayer) RpcStopMoving();
	}
	[ClientRpc]
	public void RpcStopMoving ()
	{
		if (network_identity.isServer) return;
		movement=0;
		fixed_move=false;
		anim.SetInteger("movement",0);
	}

	[Command]
	public void CmdCabinRotation(bool start,bool right) 
	{
		if (start) 
		{
			if (!right) cabin_rotation_vector=Vector3.down*cabin_rot_speed;
			else cabin_rotation_vector=Vector3.up*cabin_rot_speed;
		}
		else cabin_rotation_vector=Vector3.zero;
		if (multiplayer) RpcCabinRotation(start,right);
	}
	[ClientRpc]
	public void RpcCabinRotation (bool start,bool right) 
	{
		if (network_identity.isServer) return;
		if (start) 
		{
			if (!right) cabin_rotation_vector=Vector3.down*cabin_rot_speed;
			else cabin_rotation_vector=Vector3.up*cabin_rot_speed;
		}
		else cabin_rotation_vector=Vector3.zero;
	}

	[Command]
	public void CmdMechRotation (bool start, bool right) 
	{
		if (start) 
		{
			if (!right) mech_rotation_vector=Vector3.down*rot_speed;
			else mech_rotation_vector=Vector3.up*rot_speed;
		}
		else mech_rotation_vector=Vector3.zero;
		if (multiplayer) RpcMechRotation(start,right);
	}
	[ClientRpc]
	public void RpcMechRotation (bool start,bool right) 
	{
		if (network_identity.isServer) return;
		if (start) 
		{
			if (!right) mech_rotation_vector=Vector3.down*rot_speed;
			else mech_rotation_vector=Vector3.up*rot_speed;
		}
		else mech_rotation_vector=Vector3.zero;
	}

	[Command]
	public void CmdHeadRotation (Vector3 rt) 
	{
		head_look_vector=rt;
		if (multiplayer) RpcHeadRotation(rt);
	}
	[ClientRpc]
	public void RpcHeadRotation (Vector3 rt)
	{
		if (network_identity.isServer) return;
		head_look_vector=rt;
	}

	public void Stop() {
		anim.SetInteger ("movement",0);
		movement=0;
		fixed_move=false;
	}

		public void ApplyDamage (Vector4 v) {
			if (Global.sound&&Global.sm.hit_timer==0) {
				Global.sm.BrotherIAmHit(new Vector3(v.x,v.y,v.z));
			}
			if (Global.invincible||Global.mission_end) return;
			if (shield_value>0) {
				shield_value-=v.w*0.7f;
			}
			else {
				hp-=v.w*0.3f;
				if (hp<0) {
					RaycastHit rc;
					var lm=1<<8;
					Vector3 dir=new Vector3(Random.Range(-1,1)*200,Random.Range(-1,1)*10,Random.Range(-1,1)*200);
					if (Physics.Raycast(transform.position,dir,out rc,500,lm)) {
						Global.cam.transform.position=rc.point+Vector3.up*200;
					}
					else Global.cam.transform.position=transform.position+dir;
					Global.cam.transform.forward=transform.position-Global.cam.transform.position;
					BroadcastMessage("Death",SendMessageOptions.DontRequireReceiver);
					Global.sm.CabinRotation(false);
					Global.bonus=1;
					gameObject.SetActive(false);
					Instantiate(Resources.Load<GameObject>("dead_mech"),transform.position,transform.rotation);
					Global.menu_script.fail=true;
					Destroy(gameObject);
				}
			}
		}


}
