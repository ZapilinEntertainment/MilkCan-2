using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class menu : MonoBehaviour {
	public ushort shot_effect_count=30;
	public ushort shot_expl_effect_count=30;
	public ushort dust_effect_count=30;
	GameObject effects;
	Transform fireplaces;
	int lastscore=0;
	float deltascore=0;
	public float t=0;
	public byte stage=1;
	public int stage_score=5000;
	public float score_fall_speed=20;
	public Texture frame_tx;
	public Texture h_line_tx;
	Texture h_line_blue;
	public bool fail=false;
	Texture fail_tx;
	public string gdatas;
	int g=16;
	int sci=0;
	public int last_mission=0;

	void Start () {
		Global.pause=false;
		Global.menu_script=this;
		sci=SceneManager.GetActiveScene().buildIndex;
		if (PlayerPrefs.HasKey("gdata")) {
			gdatas=PlayerPrefs.GetString("gdata");
			if (gdatas.Length>2) {
				if (gdatas[2]=='1') Global.sound=true;
				else Global.sound=false;
			}
			else Global.sound=true;
		}
		else {
			gdatas="0010";
			Global.sound=true;
		}

		textor t=gameObject.AddComponent<textor>();
		Global.txtr=t;
		Global.txtr.SetLanguage((byte)(int.Parse(gdatas.Substring(0,1))));

		if (PlayerPrefs.HasKey("walkthrough")) {
			last_mission=int.Parse(PlayerPrefs.GetString("walkthrough").Substring(0,2));
		}

		fireplaces=new GameObject("fireplaces").transform;
		effects=new GameObject("effects");
		GameObject x=Resources.Load<GameObject>("shot_effect");
		Global.r_shot_effect=new GameObject[shot_effect_count];
		ushort i=0;
		for (i=0;i<shot_effect_count;i++) {
			Global.r_shot_effect[i]=Instantiate(x) as GameObject;
			Global.r_shot_effect[i].SetActive(false);
			Global.r_shot_effect[i].transform.parent=effects.transform;
		}
		x=Resources.Load<GameObject>("shot_expl_effect");
		Global.r_shot_expl_effect=new GameObject[shot_expl_effect_count];
		for (i=0;i<shot_expl_effect_count;i++) {
			Global.r_shot_expl_effect[i]=Instantiate(x) as GameObject;
			Global.r_shot_expl_effect[i].SetActive(false);
			Global.r_shot_expl_effect[i].transform.parent=effects.transform;
		}
		x=Resources.Load<GameObject>("dust_effect");
		Global.r_dust_effect=new GameObject[dust_effect_count];
		for (i=0;i<dust_effect_count;i++) {
			Global.r_dust_effect[i]=Instantiate(x) as GameObject;
			Global.r_dust_effect[i].SetActive(false);
			Global.r_dust_effect[i].transform.parent=effects.transform;
		}
		Global.r_flatten_tank0=Resources.Load<GameObject>("flatten_tank0");
		Global.r_flatten_bunker0=Resources.Load<GameObject>("flatten_bunker0");
		Global.r_flatten_tree=Resources.Load<GameObject>("flatten_tree");
		Global.r_fired_place=Resources.Load<GameObject>("fired_ground");
		Global.r_dead_tank0=Resources.Load<GameObject>("dead_tank0");
		Global.r_dead_bunker0=Resources.Load<GameObject>("dead_bunker0");
		Global.r_dead_flatten_tank0=Resources.Load<GameObject>("dead_flatten_tank0");
		Global.r_rocket=Resources.Load<GameObject>("rocket");
		Global.r_homing_missile=Resources.Load<GameObject>("homing_missile");
		Global.r_tank0_lod_sprite=Resources.Load<GameObject>("tank0_lod_sprite");
		Global.r_simple_rocket=Resources.Load<GameObject>("simpleRocket");
		Global.r_simple_hmissile=Resources.Load<GameObject>("simple_hmissile");
		Global.r_fire=Resources.Load<GameObject>("fire1");

		Global.small_explosion=Instantiate(Resources.Load<ParticleSystem>("expl0")) as ParticleSystem;
		Global.concrete_dust=Instantiate(Resources.Load<ParticleSystem>("concrete_dust")) as ParticleSystem;
		Global.artillery_strike=Instantiate(Resources.Load<ParticleSystem>("artillery_strike")) as ParticleSystem;
		Global.bigdust=Instantiate(Resources.Load<ParticleSystem>("bigdust")) as ParticleSystem;
		if (Global.water_on_level) Global.water_expl=Instantiate(Resources.Load<ParticleSystem>("water_explosion")) as ParticleSystem;

		Global.aim_tx_right=Resources.Load<Texture>("aim_right");
		Global.aim_tx_left=Resources.Load<Texture>("aim_left");
		Global.aim_slider_tx=Resources.Load<Texture>("aim_slider");
		Global.ind_red_tx=Resources.Load<Texture>("ind_red");
		Global.ind_green_tx=Resources.Load<Texture>("ind_green");
		Global.chosen_frame_tx=Resources.Load<Texture>("chosen_frame");
		Global.quality=QualitySettings.GetQualityLevel();

		frame_tx=Resources.Load<Texture>("horizontal_bar_frame");
		h_line_tx=Resources.Load<Texture>("horizontal_red_line");
		fail_tx=Resources.Load<Texture>("fail_tx");
		h_line_blue=Resources.Load<Texture>("horizontal_blue_line");

	}
	// Update is called once per frame

	public void AddFireplace(Vector3 pos) {
		float d=0;
		Transform t;
		if (fireplaces.childCount>0) {
		for (int i=0;i<fireplaces.childCount;i++) {
			t=fireplaces.GetChild(i);
			d=Vector3.Distance(t.position,pos);
			if (d<=5) {				
					return;
			}
			}}
		RaycastHit rh;
		var layerMask = 1 << 11;
		if (Physics.Raycast(new Vector3(pos.x,pos.y+100,pos.z),Vector3.down,out rh,200,layerMask)) {
			rh.collider.SendMessage("WaterSplash",rh.point,SendMessageOptions.DontRequireReceiver);
			return;
		}
		else {
			layerMask=1<<8;
		if (Physics.Raycast(new Vector3(pos.x,pos.y+5,pos.z),Vector3.down,out rh,6,layerMask)) {
			GameObject fp=Instantiate(Global.r_fired_place,rh.point+Vector3.up*0.1f,Quaternion.Euler(0,Random.value*360,0)) as GameObject;
			fp.transform.parent=fireplaces;
		}
		}
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (Global.pause) {
				Global.pause=false;
				Time.timeScale=1;
			}
			else {
				Global.pause=true;
			     Time.timeScale=0;
			}
		}
		Global.gui_piece=Screen.height/24;
		if (Global.pause) return;
		if (t>0) {t-=Time.deltaTime*stage;if (t<=0) {deltascore=0;lastscore=Global.score;stage=1;Global.bonus=1;}}
		if (Global.score!=lastscore) {
			deltascore+=Global.score-lastscore;
			if (deltascore>100000*stage) {stage++;Global.bonus=stage;}
			lastscore=Global.score;
			t=10;
		}
	}
		

	void OnGUI () {
		if (!Global.playable) return;
		g=Global.gui_piece;
		if (t>0) {
			GUI.Label(new Rect(0,Screen.height-2*g,g*6,g),deltascore.ToString()+" ["+stage.ToString()+"]");
			float x=t/10;
			x*=Global.gui_piece*6;
			GUI.DrawTexture(new Rect(0,Screen.height-g/2,x,g/2),h_line_tx,ScaleMode.StretchToFill);
			GUI.DrawTexture(new Rect(0,Screen.height-g/2,6*g,g/2),frame_tx,ScaleMode.StretchToFill);
			x=deltascore/(100000*stage);
			x*=Global.gui_piece*6;
			GUI.DrawTexture(new Rect(0,Screen.height-g,x,g/2),h_line_blue,ScaleMode.StretchToFill);
			GUI.DrawTexture(new Rect(0,Screen.height-g,6*g,g/2),frame_tx,ScaleMode.StretchToFill);
		}
		if (fail) {
			GUI.DrawTexture(new Rect(Screen.width/2-4*g,Screen.height/2-8*g,8*g,8*g),fail_tx,ScaleMode.StretchToFill);
			if (GUI.Button(new Rect(Screen.width/2-2*g,Screen.height/2,4*g,g),Global.txtr.try_again_button)) {SceneManager.LoadScene(sci);}
			if (GUI.Button(new Rect(Screen.width/2-2*g,Screen.height/2+g,4*g,g),Global.txtr.to_menu_button)) {SceneManager.LoadScene(0);}
		}
		if (GUI.Button(new Rect(Screen.width-2*g,0,2*g,g),"Disconnect")) 
		{if (Global.player.GetComponent<NetworkIdentity>().isServer) 	Global.network_master.network_manager.StopHost();
		else Global.network_master.network_manager.StopClient();
		}
	}
}
