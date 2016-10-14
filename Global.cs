using UnityEngine;
using System.Collections;

public static class Global {
	// 5) Script-using Objects
	public static GameObject cam;
	public static GameObject player;
	public static GameObject[] r_shot_effect;
	public static GameObject[] r_shot_expl_effect;
	public static GameObject[] r_dust_effect;
	public static GameObject r_flatten_tank0;
	public static GameObject r_dead_tank0;
	public static GameObject r_dead_flatten_tank0;
	public static GameObject r_flatten_tree;
	public static GameObject r_fired_place;
	public static GameObject r_rocket;
	public static GameObject r_simple_rocket;
	public static GameObject r_homing_missile;
	public static GameObject r_simple_hmissile;
	public static GameObject r_flatten_bunker0;
	public static GameObject r_dead_bunker0;
	public static GameObject r_tank0_lod_sprite;
	public static GameObject r_fire;

	public static ParticleSystem small_explosion;
	public static ParticleSystem concrete_dust;
	public static ParticleSystem artillery_strike;
	public static ParticleSystem water_expl;
	public static ParticleSystem bigdust;
	// 6) Script-using Values
	public static bool invincible=false;
	public static bool mission_end=false;
	public static bool pause;
	public static bool sound=true;
	public static bool nongamescene=false;
	public static bool playable=true;
	public static bool to_hangar=false;
	public static bool water_on_level=false;

	public static byte lastmission=0;

	public static float bonus=1;

	public static int gui_piece;
	public static int score;
	public static int quality;

	static ushort shot_i=0;
	static ushort shot_expl_i=0;
	static ushort dust_i=0;

	public static Vector3 aim;
	public static Vector3 endpoint;
	// 7) Script-using Textures
	public static Texture aim_tx_right;
	public static Texture aim_tx_left;
	public static Texture aim_slider_tx;
	public static Texture ind_red_tx;
	public static Texture ind_green_tx;
	public static Texture chosen_frame_tx;
	// 8) Scripts references
	public static menu menu_script;
	public static PlayerController p_controller;
	public static soundMaster sm;
	public static textor txtr;
	public static NetworkMaster network_master;
	// 9) Debugging variables


	public static void WaterExpl(Vector3 pos) {
		water_expl.transform.position=pos;
		water_expl.Emit(10);
	} 

	public static void ArtilleryStrikeAt (Vector3 pos) {
		artillery_strike.transform.position=pos;
		artillery_strike.Emit(50);
		menu_script.AddFireplace(pos);
		SmallExplosionRequest(pos);
	}

	public static void BigDustRequest (Vector3 pos) {
		bigdust.transform.position=pos;
		bigdust.Emit(10);
	}
	public static void ConcreteDustRequest (Vector3 pos) {
		concrete_dust.transform.position=pos;
		concrete_dust.Emit(10);
	}
	public static void SmallExplosionRequest (Vector3 pos) {
		small_explosion.transform.position=pos;
		small_explosion.Emit(40);
		if (!sm.outside_off) sm.Explosion(pos);
	}
	public static void DustEffectRequest(Vector3 pos) {
		r_dust_effect[dust_i].transform.position=pos;
		r_dust_effect[dust_i].SetActive(true);
		r_dust_effect[dust_i].SendMessage("Reboot",SendMessageOptions.DontRequireReceiver);
		dust_i++;
		if (dust_i>=r_dust_effect.Length) dust_i=0;
	}

	public static void ShotEffectRequest(Vector3 pos) {
		r_shot_effect[shot_i].transform.position=pos;
		r_shot_effect[shot_i].SetActive(true);
		r_shot_effect[shot_i].SendMessage("Reboot",SendMessageOptions.DontRequireReceiver);
		shot_i++;
		if (shot_i>=r_shot_effect.Length) shot_i=0;
	}

	public static void ShotExplEffectRequest(Vector3 pos) {
		r_shot_expl_effect[shot_expl_i].transform.position=pos;
		r_shot_expl_effect[shot_expl_i].SetActive(true);
		r_shot_expl_effect[shot_expl_i].SendMessage("Reboot",SendMessageOptions.DontRequireReceiver);
		shot_expl_i++;
		if (shot_expl_i>=r_shot_expl_effect.Length) shot_expl_i=0;
	}
}
