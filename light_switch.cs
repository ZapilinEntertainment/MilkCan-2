using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class light_switch : MonoBehaviour {

	// 1) Inspector-Input Objects	
	public Light main_light;

	public Camera cin_cam;

	public Transform focus;
	public Transform cam;
	public Transform right_pauldron;
	public Transform left_pauldron;

	public Renderer[] rrs;
	// 2) Inspector-Input Technical Values
	public Vector3 cam_menu_pos=new Vector3(35.5f,20,34);
	public Vector3 cam_menu_rot=new Vector3(355,246,0);
	public Vector3 cam_custom_fwd_pos=new Vector3(0,42,28);
	public Vector3 cam_custom_fwd_rot=new Vector3(17,180,0);
	public Vector3 cam_custom_back_pos=new Vector3(10,46,-25);
	public Vector3 cam_custom_back_rot=new Vector3(17,0,0);
	// 3) Inspector-Input Game Values
	public int lastmission=10;
	// 4) Inspector-Input Textures and Sounds
	public AudioClip mech_activated;
	public AudioClip mech_deactivated;

	public Texture start_button_tx;
	public Texture settings_button_tx;
	public Texture help_button_tx;
	public Texture credits_button_tx;
	public Texture exit_button_tx;
	public Texture return_button_tx;
	public Texture weaponry_button_tx;
	public Texture chosen_frame_tx;
	public Texture back_tx;
	public Texture module_empty;
	public Texture kitty_icon;
	public Texture start_mission_tx;
	public Texture missions_back_tx;
	public Texture logo_tx;
	// 5) Script-using Objects
	GameObject[] modules;
	// 6) Script-using Values
	public string mechcode="0102";
	public string gamedata_string="0010";
	string console_text="";
	string address_string="";


	bool console=false;
	bool cinematic=false;
	bool gui_set=false;
	bool help=false;
	bool settings =false;
	bool step_traces=false;
	bool weaponry=false;
	public bool powered=false;

	public byte chosen_module=255;
	byte chosen_mission=0;
	byte i=0;
	byte quality_lvl=5;
	byte mission_menu=0;

	public float speed=10;

	int gui_state=0;
	int k=16;
	int sh=600;
	int sw=800;

	Rect r1,r2,r3,r4,r5;

	// 7) Script-using Textures
	// 8) Scripts references
	public textor txtr;
	// 9) Debugging variables


	void Awake () {
		Global.nongamescene=true;
		Global.cam=cam.gameObject;
		Global.pause=false;
		if (PlayerPrefs.HasKey("gdata")) {
			gamedata_string=PlayerPrefs.GetString("gdata");
			txtr.SetLanguage((byte)(int.Parse(gamedata_string.Substring(0,1))));
			if (gamedata_string.Length>1) {
				if (gamedata_string[1]=='1') step_traces=true; else step_traces=false;
			} else step_traces=false;
			if (gamedata_string.Length>2) {
				if (gamedata_string[2]=='1') Global.sound=true;
				else Global.sound=false;
			}
			else Global.sound=true;
		}
		else {
			gui_state=3;
		}
		quality_lvl=(byte)QualitySettings.GetQualityLevel();
	}

	void Start () {
		sh=Screen.height;
		sw=Screen.width;
		k=sh/9;
		int sti=sw-2*k;
		r1=new Rect(sti,2*k,2*k,k);
		r2=new Rect(sti,r1.y+k,2*k,k);
		r3=new Rect(sti,r2.y+k,2*k,k);
		r4=new Rect(sti,r3.y+k,2*k,k);
		r5=new Rect(sti,r4.y+k,2*k,k);

		cam.position=cam_menu_pos;
		cam.rotation=Quaternion.Euler(cam_menu_rot);

		if (PlayerPrefs.HasKey("mechcode")) {
			mechcode=PlayerPrefs.GetString("mechcode");
			if (mechcode.Length<20) mechcode=mechcode+new string ('0',20-mechcode.Length);
		}
		else mechcode="0102";

		modules=new GameObject[10];
		for (byte i=0;i<mechcode.Length/2;i++) {
			//SpawnModule(mechcode.Substring(i*2,2),i);
		}
		Global.pause=false;
		Time.timeScale=1;

		Global.ind_green_tx=Resources.Load<Texture>("ind_green");
		Global.ind_red_tx=Resources.Load<Texture>("ind_red");

		if (PlayerPrefs.HasKey("walkthrough")) {
			lastmission=int.Parse(PlayerPrefs.GetString("walkthrough").Substring(0,2));
		}
		else {
			lastmission=1;
			PlayerPrefs.SetString("walkthrough","01");
		}

		if (Global.to_hangar) {
			Global.to_hangar=false;
			gui_state=1;
			r1=new Rect(sw-2*k,0,2*k,k/2);
			r5=new Rect(sw-2*k,0,2*k,k);
			r3=new Rect(sw-2*k,k,2*k,k);
			LightsOn();
			chosen_module=255;
			chosen_mission=Global.lastmission;
			settings=false;
			Global.cam.GetComponent<Camera>().pixelRect=new Rect(0,0,sw/2+k,sh);
			Global.cam.transform.LookAt(focus.position);
		}

		//lastmission=10;
	}

	void Update () {
	if (Input.GetKey("c")) {
			if (Input.GetKey("a")) {
				if (Input.GetKeyDown("t")) {
					if (!console) console=true;
				}
				}}
		//	if (!cinematic) {
		//		Global.cam.SetActive(false);
		//		cin_cam.enabled=true;
		//		cinematic=true;
		//	}
		//	else {
		//		Global.cam.SetActive(true);
		//		cin_cam.enabled=false;
		//		cinematic=false;
	//		}

	}

	void LateUpdate() {
		if (!cinematic&&gui_state==1) {
			Vector2 mdir=new Vector2(Input.GetAxis("Mouse X"),Input.GetAxis("Mouse Y"));
			if (Input.GetMouseButton(0)) {
				Vector3 rdir=transform.TransformDirection(Vector3.up);
				rdir.x=0;
				Global.cam.transform.RotateAround(focus.position,rdir,mdir.x*200*Time.deltaTime);
				//rdir=Vector3.right;
				//Global.cam.transform.RotateAround(focus.position,rdir,mdir.y*140*Time.deltaTime);
			}
		}
	}

	void SpawnModule (string code, byte index) {
		if (code=="00") return;
		GameObject x=modules[index];
		if (x!=null) Destroy(x);
		bool right=true;
		switch (code) {
		case "00": break;
		case "01":
			x=Instantiate(Resources.Load<GameObject>("plasmagun")) as GameObject;
			if (index!=0) {
				x.transform.rotation=Quaternion.Euler(x.GetComponent<moduleInfo>().rot_lf);
				right=false;
			}
			break;
		case "02":
			x=Instantiate(Resources.Load<GameObject>("chaingun")) as GameObject;
			if (index!=0) right=false;
			break;
		case "03":
			x=Instantiate(Resources.Load<GameObject>("rocket_launcher")) as GameObject;
			if (index%2==0) right=true; else right=false;
			break;
		case "04":break;
		case "05":
			x=Instantiate(Resources.Load<GameObject>("pvogun")) as GameObject;
			if (index%2==0) right=true; else right=false;
			break;
		case "06":
			x=Instantiate(Resources.Load<GameObject>("artbattery")) as GameObject;
			if (index%2==0) right=true; else right=false;
			break;
		case "07":
			x=Instantiate(Resources.Load<GameObject>("pvo_launcher")) as GameObject;
			if (index%2==0) right=true; else right=false;
			break;
		case "08":
			x=Instantiate(Resources.Load<GameObject>("shlauncher")) as GameObject;
			if (index%2==0) right=true; else right=false;
			break;
		case "09":
			x=Instantiate(Resources.Load<GameObject>("shield_gen")) as GameObject;
			if (index%2==0) right=true; else right=false;
			break;	
		}

		Vector3 pos=Vector3.zero;
		Vector3 slot_correction=Vector3.zero;
		if (right) {pos=right_pauldron.position;x.transform.parent=right_pauldron;}
		else {pos=left_pauldron.position;x.transform.parent=left_pauldron;}
		if (index>1) {
			switch (index) {
			case 2: slot_correction=new Vector3(-2,3,3.5f); break;
			case 3: slot_correction=new Vector3(2,3,3.5f); break;
			case 4: slot_correction=new Vector3(1.5f,3,3.5f); break;
			case 5: slot_correction=new Vector3(-1.5f,3,3.5f); break;
			case 6: slot_correction=new Vector3(-2,3,-3.5f); break;
			case 7: slot_correction=new Vector3(2f,3,-3.5f); break;
			case 8: slot_correction=new Vector3(1.5f,3,-3.5f); break;
			case 9: slot_correction=new Vector3(-1.5f,3,-3.5f); break;
			}
		}
		modules[index]=x;
		x.transform.position=pos;
		x.transform.localPosition+=slot_correction+x.GetComponent<moduleInfo>().correction_vector;
		if (index<6) x.transform.localRotation=Quaternion.Euler(0,0,0);else x.transform.localRotation=Quaternion.Euler(0,180,0);
		if (index!=mechcode.Length/2-1) mechcode=mechcode.Substring(0,index*2)+x.GetComponent<moduleInfo>().code.Substring(0,2)+mechcode.Substring((index+1)*2,mechcode.Length-(index+1)*2);
		else {mechcode=mechcode.Substring(0,index*2)+x.GetComponent<moduleInfo>().code.Substring(0,2);}
	}

	public void LightsOn() {
		foreach (Renderer r in rrs) {
			r.material.SetColor("_EmissionColor",Color.white);
		}
		powered=true;
		if (Global.sound) Global.cam.GetComponent<AudioSource>().PlayOneShot(mech_activated);
	}
	public void LightsOff() {
		foreach (Renderer r in rrs) {
			r.material.SetColor("_EmissionColor",Color.black);
		}
		powered=false;
		if (Global.sound) Global.cam.GetComponent<AudioSource>().PlayOneShot(mech_deactivated);
	}

	void OnGUI () {
		if (cinematic) return;
		if (!gui_set) {
			GUI.skin.GetStyle("Button").fontSize=k/4;
			GUI.skin.GetStyle("Label").fontSize=k/4;
			GUI.skin.label.clipping=TextClipping.Overflow;
			GUI.skin.button.clipping=TextClipping.Overflow;
			gui_set=true;
		}
		switch (gui_state) {
		case 0: 
			GUI.DrawTexture(new Rect(sw-2*k,0,2*k,2*k),logo_tx,ScaleMode.StretchToFill);
			if (GUI.Button(r1,start_button_tx)) {
				gui_state=1;
				r1=new Rect(sw-2*k,0,2*k,k/2);
				r5=new Rect(sw-2*k,0,2*k,k);
				r3=new Rect(sw-2*k,k,2*k,k);
				//LightsOn();
				chosen_module=255;
				chosen_mission=0;
				settings=false;
				help=false;
				Global.cam.GetComponent<Camera>().pixelRect=new Rect(0,0,sw/2+k,sh);
				Global.cam.transform.LookAt(focus.position);
		}
			if (GUI.Button(r2,settings_button_tx)) {
				settings=!settings;
				help=false;
			}
			if (GUI.Button(r3,help_button_tx)) {
				help=!help;
				settings=false;
			}
			//if (GUI.Button(r4,credits_button_tx)) {				
			//}
			if (GUI.Button(r5,exit_button_tx)) {
				Application.Quit();
			}
		if (settings) {
				if (GUI.Button(new Rect(sw-5*k,0,3*k,k/2),txtr.data_clean_button)) {
					gui_state=4;
				}
				if (GUI.Button(new Rect(sw-5*k,k/2,3*k,k/2),txtr.language_button)) {
					PlayerPrefs.DeleteKey("walkthrough");
					gui_state=3;
				}

				GUI.Label(new Rect(r2.x-3*k,r2.y-k,2*k,k/2),txtr.sound_button);
				if (Global.sound) {
					if (GUI.Button(new Rect(r2.x-k/2,r2.y-k,k/2,k/2),Global.ind_green_tx)) {
						Global.sound=false;
						if (PlayerPrefs.HasKey("gdata")) {
							if (gamedata_string.Length>3) gamedata_string=gamedata_string.Substring(0,2)+'0'+gamedata_string.Substring(3,gamedata_string.Length-3);
							else {
								if (gamedata_string.Length<2) gamedata_string="000";
								else gamedata_string=gamedata_string.Substring(0,2)+'0';
							}
						}
						else {
							gamedata_string="000";
						}
						PlayerPrefs.SetString("gdata",gamedata_string);
					}
				}
				else {
					if (GUI.Button(new Rect(r2.x-k/2,r2.y-k,k/2,k/2),Global.ind_red_tx)) {
						Global.sound=true;
						if (PlayerPrefs.HasKey("gdata")) {
							if (gamedata_string.Length>3) gamedata_string=gamedata_string.Substring(0,2)+'1'+gamedata_string.Substring(3,gamedata_string.Length-3);
							else {
								if (gamedata_string.Length<2) gamedata_string="001";
								else gamedata_string=gamedata_string.Substring(0,2)+'1';
							}
						}
						else {
							gamedata_string="001";
						}
						PlayerPrefs.SetString("gdata",gamedata_string);
					}
				}

				GUI.Label(new Rect(r2.x-3*k,r2.y,2*k,k/2),txtr.step_traces_button);
				if (step_traces) {
					if (GUI.Button(new Rect(r2.x-k/2,r2.y,k/2,k/2),Global.ind_green_tx)) {
						step_traces=false;
						gamedata_string=PlayerPrefs.GetString("gdata");
						if (gamedata_string.Length>2) PlayerPrefs.SetString("gdata",gamedata_string.Substring(0,1)+'0'+gamedata_string.Substring(2,gamedata_string.Length-2));
						else PlayerPrefs.SetString("gdata",gamedata_string.Substring(0,1)+'0');
					}
				}
				else {
					if (GUI.Button(new Rect(r2.x-k/2,r2.y,k/2,k/2),Global.ind_red_tx)) {
						step_traces=true;
						string s2=PlayerPrefs.GetString("gdata");
						if (s2.Length>2) PlayerPrefs.SetString("gdata",s2.Substring(0,1)+'1'+s2.Substring(2,s2.Length-2));
						else PlayerPrefs.SetString("gdata",s2.Substring(0,1)+'1');
					}
				}
				GUI.Label(new Rect(r2.x-3*k,r2.y+k,2*k,k/2),txtr.graphics_quality_button);
				if (GUI.Button(new Rect(r2.x-3*k,r2.y+2*k,2*k,k/2),"Fastest")) {QualitySettings.SetQualityLevel(0);quality_lvl=0;}
				if (GUI.Button(new Rect(r2.x-3*k,r2.y+2.5f*k,2*k,k/2),"Fast")) {QualitySettings.SetQualityLevel(1);quality_lvl=1;}
				if (GUI.Button(new Rect(r2.x-3*k,r2.y+3*k,2*k,k/2),"Simple")) {QualitySettings.SetQualityLevel(2);quality_lvl=2;}
				if (GUI.Button(new Rect(r2.x-3*k,r2.y+3.5f*k,2*k,k/2),"Good")) {QualitySettings.SetQualityLevel(3);quality_lvl=3;}
				if (GUI.Button(new Rect(r2.x-3*k,r2.y+4*k,2*k,k/2),"Beautiful")) {QualitySettings.SetQualityLevel(4);quality_lvl=4;}
				if (GUI.Button(new Rect(r2.x-3*k,r2.y+4.5f*k,2*k,k/2),"Fantastic")) {QualitySettings.SetQualityLevel(5);quality_lvl=5;}
				Rect cr=new Rect(r2.x-3*k,r2.y+2*k,2*k,k/2);
				switch (quality_lvl) {
				case 1:cr.y+=k/2;break; 
				case 2:cr.y+=k;break;
				case 3: cr.y+=1.5f*k;break;
				case 4: cr.y+=2*k;break;
				case 5: cr.y+=2.5f*k;break;
				}
				GUI.DrawTexture(cr,chosen_frame_tx,ScaleMode.StretchToFill);
				GUI.DrawTexture(r2,chosen_frame_tx,ScaleMode.StretchToFill);
		}
			if (console) {
				console_text=GUILayout.TextField(console_text);
				if (GUILayout.Button("Submit")) {
					switch (console_text) {
					case "missions": lastmission=10;break;
					case "too hard": Global.invincible=true;break;	
					}
					console=false;
				}
			}
			if (help) {
				GUI.DrawTexture(new Rect(r1.x-sh/2,0,sh/2,sh),txtr.help_tx);
				GUI.DrawTexture(r3,chosen_frame_tx,ScaleMode.StretchToFill);
			}
			break;
		case 1:  //Экран миссий
			GUI.DrawTexture(new Rect(sw/2+k,0,sw/2-k,sh),missions_back_tx);
			int d=k;

			switch (mission_menu) {
			case 0: //выбор
				if (GUI.Button(new Rect(sw-2*k,k,2*k,k),"Сюжетные миссии")) mission_menu=1;
				if (GUI.Button(new Rect(sw-2*k,2*k,2*k,k),"Игра по сети")) 
				{
					mission_menu=2; 
					if (PlayerPrefs.HasKey("last_conn_info")) address_string=PlayerPrefs.GetString("last_conn_info");
					else address_string="127.0.0.1|4444";
				}
				break;
			case 1: //миссии кампании
			int py=(int)(r1.y+k);
			for (i=1;i<=lastmission;i++) {
				if (i!=chosen_mission) {
				    if (GUI.Button(new Rect(r1.x,py,r1.width,r1.height),"mission"+i.ToString())) {chosen_mission=i;}
				}
				else {
					if (GUI.Button(new Rect(r1.x,py,r1.width,r1.height),start_mission_tx)) {
						PlayerPrefs.SetString("mechcode",mechcode);
						SceneManager.LoadScene(i);
					}
				}
				py+=(int)(k/2);
				}
			if (chosen_mission!=0) {
				GUI.DrawTexture(new Rect(r1.x,chosen_mission*k/2+r1.y+k/2,r1.width,r1.height),chosen_frame_tx,ScaleMode.StretchToFill);
				Rect mbr=new Rect(sw/2+k,6*k,sw/2-k,3*k);
				switch (chosen_mission) {
				case 1: GUI.Label(mbr,txtr.m1_briefing);break;
				case 2: GUI.Label(mbr,txtr.m2_briefing);break;
				case 3: GUI.Label(mbr,txtr.m3_briefing);break;
				case 4: GUI.Label(mbr,txtr.m4_briefing);break;
				case 5: GUI.Label(mbr,txtr.m5_briefing);break;
				case 6: GUI.Label(mbr,txtr.m6_briefing);break;
				case 7: GUI.Label(mbr,txtr.m7_briefing);break;
				case 8: GUI.Label(mbr,txtr.m8_briefing);break;
				case 9: GUI.Label(mbr,txtr.m9_briefing);break;
				case 10: GUI.Label(mbr,txtr.m10_briefing);break;
				}
				GUI.DrawTexture(new Rect(sw/2+k,3*k,3*k,3*k),kitty_icon,ScaleMode.StretchToFill);
			}
				
			int lm=(int)(1.5f*k);
			moduleInfo mi=null;
			Texture module_picture=module_empty;
				if (modules[1]!=null) {
				mi=modules[1].GetComponent<moduleInfo>();
				}

			d=k;

			if (mi!=null) module_picture=mi.picture; else module_picture=module_empty;
			if 	(GUI.Button(new Rect(r1.x-3*k,r1.y+k/2+d,k,k),module_picture)) chosen_module=1;

			mi=null;
			if (modules[0]!=null) {mi=modules[0].GetComponent<moduleInfo>();	}
			if (mi!=null) module_picture=mi.picture; else module_picture=module_empty;
			if 	(GUI.Button(new Rect(r1.x-k,r1.y+k/2+d,k,k),module_picture)) chosen_module=0;

			if (lastmission>1) {
			mi=null;
			if (modules[2]!=null) {mi=modules[2].GetComponent<moduleInfo>();	}
			if (mi!=null) module_picture=mi.picture; else module_picture=module_empty;
			if 	(GUI.Button(new Rect(r1.x-k,r1.y+d,k/2,k/2),module_picture)) chosen_module=2;

			mi=null;
			if (modules[3]!=null) {mi=modules[3].GetComponent<moduleInfo>();}
			if (mi!=null) module_picture=mi.picture; else module_picture=module_empty;
			if 	(GUI.Button(new Rect(r1.x-2.5f*k,r1.y+d,k/2,k/2),module_picture)) chosen_module=3;
				if (lastmission>2) {
			mi=null;
			if (modules[4]!=null) {mi=modules[4].GetComponent<moduleInfo>();}
			if (mi!=null) module_picture=mi.picture; else module_picture=module_empty;
			if 	(GUI.Button(new Rect(r1.x-0.5f*k,r1.y+d,k/2,k/2),module_picture)) chosen_module=4;

			mi=null;
			if (modules[5]!=null) {mi=modules[5].GetComponent<moduleInfo>();}
			if (mi!=null) module_picture=mi.picture; else module_picture=module_empty;
			if 	(GUI.Button(new Rect(r1.x-3*k,r1.y+d,k/2,k/2),module_picture)) chosen_module=5;
					if (lastmission>3) {
			mi=null;
			if (modules[6]!=null) {mi=modules[6].GetComponent<moduleInfo>();}
			if (mi!=null) module_picture=mi.picture; else module_picture=module_empty;
			if 	(GUI.Button(new Rect(r1.x-k,r1.y+1.5f*k+d,k/2,k/2),module_picture)) chosen_module=6;

			mi=null;
			if (modules[7]!=null) {mi=modules[7].GetComponent<moduleInfo>();}
			if (mi!=null) module_picture=mi.picture; else module_picture=module_empty;
			if 	(GUI.Button(new Rect(r1.x-2.5f*k,r1.y+1.5f*k+d,k/2,k/2),module_picture)) chosen_module=7;
						if (lastmission>4) {
			mi=null;
			if (modules[8]!=null) {mi=modules[8].GetComponent<moduleInfo>();}
			if (mi!=null) module_picture=mi.picture; else module_picture=module_empty;
			if 	(GUI.Button(new Rect(r1.x-k/2,r1.y+1.5f*k+d,k/2,k/2),module_picture)) chosen_module=8;

			mi=null;
			if (modules[9]!=null) {mi=modules[9].GetComponent<moduleInfo>();}
			if (mi!=null) module_picture=mi.picture; else module_picture=module_empty;
			if 	(GUI.Button(new Rect(r1.x-3*k,r1.y+1.5f*k+d,k/2,k/2),module_picture)) chosen_module=9;
		}}}}
				break;
			case 2:
				GUI.Label(new Rect(sw-2*k,k,k/2,k/2),"IP:");
				int ix=address_string.IndexOf('|');
				string ips=address_string.Substring(0,ix);
				ips=GUI.TextField(new Rect(sw-1.5f*k,k,1.5f*k,k/2),ips);
				int a=ips.IndexOf('|');
				while (a>0) 
				{
					ips=ips.Substring(0,a)+ips.Substring(a+1,ips.Length-a-1);
					a=ips.IndexOf('|');
				}
				address_string=ips+address_string.Substring(ix,address_string.Length-ix);
				GUI.Label(new Rect(sw-2*k,2*k,k/2,k/2),"Port:");
				string port_s=address_string.Substring(ix+1,address_string.Length-1-ix);
				port_s=GUI.TextField(new Rect(sw-1.5f*k,2*k,k*1.5f,k/2),port_s);
				a=port_s.IndexOf('|');
				while (a>0) 
				{
					port_s=port_s.Substring(0,a)+port_s.Substring(a+1,port_s.Length-a-1);
					a=port_s.IndexOf('|');
				}
				address_string=address_string.Substring(0,ix+1)+port_s;

				if (GUI.Button(new Rect(sw-2*k,3*k,k,k/2),"Connect")) 
				{
					int port=4444;
					NetworkManager nm=Global.network_master.network_manager;
					if (int.TryParse(port_s,out port)) 
					{
						nm.networkAddress=ips;
						nm.networkPort=port;
						nm.onlineScene="city_map0";
						nm.autoCreatePlayer=true;
						PlayerPrefs.SetString("last_conn_info",address_string);
						nm.StartClient();
					}
				}
				if (GUI.Button(new Rect(sw-k,3*k,k,k/2),"Host")) 
				{
					int port=4444;
					NetworkManager nm=Global.network_master.network_manager;
					if (int.TryParse(port_s,out port)) 
					{
						nm.networkAddress=ips;
						nm.networkPort=port;
						nm.onlineScene="city_map0";
						nm.autoCreatePlayer=true;
						PlayerPrefs.SetString("last_conn_info",address_string);
						nm.StartHost();
					}
				}
				break;
			}
			//end of case
			Rect ch_rect=new Rect(-k,-k,k,k);
			if (chosen_module!=255) {
				if (chosen_module<2) {
					if (GUI.Button(new Rect(r1.x-5.5f*k,r1.y,2.5f*k,k/2),txtr.plasmagun_name)) SpawnModule("01",chosen_module);
					if (GUI.Button(new Rect(r1.x-5.5f*k,r1.y+k/2,2.5f*k,k/2),txtr.chaingun_name)) SpawnModule("02",chosen_module);
				if (lastmission>2) {
						if (GUI.Button(new Rect(r1.x-5.5f*k,r1.y+k,2.5f*k,k/2),txtr.rlauncher_name)) SpawnModule("03",chosen_module);
				}
				}
				else {
					Rect amr=new Rect(r1.x-5.5f*k,r1.y,2.5f*k,k/2);
					if (GUI.Button(amr,txtr.pvogun_name)) SpawnModule("05",chosen_module); amr.y+=k/2;
					if (lastmission>4) {if (GUI.Button(amr,txtr.pvorockets_name)) SpawnModule("07",chosen_module); amr.y+=k/2;}
					if (lastmission>2)	{if (GUI.Button(amr,txtr.shield_gen_name)) SpawnModule("09",chosen_module); amr.y+=k/2;}

					if (chosen_module<6) {
						if (GUI.Button(amr,txtr.artillery_name)) SpawnModule("06",chosen_module); amr.y+=k/2;
						if (lastmission>3) {if (GUI.Button(amr,txtr.sh_rockets)) SpawnModule("08",chosen_module); amr.y+=k/2;}
					}
				}
			
				switch (chosen_module) {
				case 0: ch_rect=new Rect(r1.x-k,r1.y+k/2+d,k,k);break;
				case 1: ch_rect=new Rect(r1.x-3*k,r1.y+k/2+d,k,k); break;
				case 2: ch_rect=new Rect(r1.x-k,r1.y+d,k/2,k/2);break;
				case 3: ch_rect=new Rect(r1.x-2.5f*k,r1.y+d,k/2,k/2); break;
				case 4: ch_rect=new Rect(r1.x-0.5f*k,r1.y+d,k/2,k/2);break;
				case 5: ch_rect=new Rect(r1.x-3*k,r1.y+d,k/2,k/2); break;
				case 6: ch_rect=new Rect(r1.x-k,r1.y+1.5f*k+d,k/2,k/2); break;
				case 7: ch_rect=new Rect(r1.x-2.5f*k,r1.y+1.5f*k+d,k/2,k/2); break;
				case 8: ch_rect=new Rect(r1.x-k/2,r1.y+1.5f*k+d,k/2,k/2); break;
				case 9: ch_rect=new Rect(r1.x-3*k,r1.y+1.5f*k+d,k/2,k/2); break;
				}
				GUI.DrawTexture(ch_rect,chosen_frame_tx,ScaleMode.StretchToFill);
			}
			if (GUI.Button(new Rect(sw-2*k,0,2*k,k/2),"В главное меню")) {
				gui_state=0;
				r1=new Rect(sw-2*k,2*k,2*k,k);
				r5=new Rect(sw-2*k,6*k,2*k,k);
				r3=new Rect(sw-2*k,4*k,2*k,k);
				if (weaponry)
				{weaponry=false;
				cam.position=cam_menu_pos;
				cam.rotation=Quaternion.Euler(cam_menu_rot);
					PlayerPrefs.SetString("mechcode",mechcode);
				}
				main_light.enabled=false;
				Global.cam.GetComponent<Camera>().pixelRect=new Rect(0,0,sw,sh);
				Global.cam.transform.rotation=Quaternion.Euler(cam_menu_rot);
				Global.cam.transform.position=cam_menu_pos;
				//LightsOff();
			}
			if (mission_menu!=0) if (GUI.Button(new Rect(sw-2*k,k/2,2*k,k/2),"К режимам игры")) {mission_menu=0;}
			break;
			case 3: //choosing language
			if (GUI.Button(new Rect(sw/2-k,sh/2-k/2,2*k,k/2),"Русский язык")) {txtr.SetLanguage(1);gui_state=0;}
			if (GUI.Button(new Rect(sw/2-k,sh/2,2*k,k/2),"English")) {txtr.SetLanguage(0);gui_state=0;}
			break;
		   case 4://clear accept
			GUI.DrawTexture(new Rect(sw/2-3*k,sh/2-k,6*k,3*k),missions_back_tx);
			GUI.Label(new Rect(sw/2-3*k,sh/2-k,6*k,2*k),txtr.clear_request_label);
			if (GUI.Button(new Rect(sw/2-3*k,sh/2+k,3*k,k), txtr.data_clean_button)) {
				PlayerPrefs.DeleteKey("walkthrough");
				PlayerPrefs.DeleteKey("gdata");
				PlayerPrefs.DeleteKey("mechcode");
				SceneManager.LoadScene(0);
			}
			if (GUI.Button(new Rect(sw/2,sh/2+k,3*k,k), txtr.to_menu_button)) {
				gui_state=0;
			}
			break;
		}
	}
}
