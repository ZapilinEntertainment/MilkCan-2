using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkMaster : MonoBehaviour {

	public int players_count=1;
	public NetworkManager network_manager;

	void Awake () 
	{
		Global.network_master=this;
		network_manager=gameObject.GetComponent<NetworkManager>();
	}
}
