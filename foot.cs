using UnityEngine;
using System.Collections;

public class foot : MonoBehaviour {

	void OnTriggerEnter (Collider c) {
		if (c.transform.root.tag=="unit"||c.transform.root.tag=="decoration") 
		{			
			c.transform.root.SendMessage("Flatten",transform.position,SendMessageOptions.DontRequireReceiver);
		}
		if (c.gameObject.layer==8) Global.sm.StepSound(Global.p_controller.prev_step_was_right);
	}
}
