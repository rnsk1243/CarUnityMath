using UnityEngine;
using System.Collections;

public class CarInitSpeed : MonoBehaviour {

    Rigidbody rig;

	// Use this for initialization
	void Start () {
        rig = GetComponent<Rigidbody>();
        rig.velocity = Vector3.forward * 5.0f;
	}
	
	//// Update is called once per frame
	//void Update () {
	
	//}
}
