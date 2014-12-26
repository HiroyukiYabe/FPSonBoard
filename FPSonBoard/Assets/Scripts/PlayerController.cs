/// <summary>
/// Player controller.
/// Attach to Player.
/// </summary>

using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	CharacterController CC;

	[Header("move")]
	public float moveSpeed;
	[HideInInspector]
	public Vector3 moveDir;

	[Header("bullet")]
	public GameObject bullet;
	public Transform muzzle;
	public float shotSpeed;
	public float shotInterval;
	float shotTimer;
	
	// Use this for initialization
	void Awake () {
		CC = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		shotTimer += Time.deltaTime;

		//rigidbody.velocity=moveDir*moveSpeed;
		CC.SimpleMove((transform.rotation*moveDir)*moveSpeed);
	}

	public void Shoot(){
		if(shotTimer>shotInterval){
			GameObject bul = (GameObject)Instantiate (bullet, muzzle.position, muzzle.rotation);
			bul.rigidbody.AddForce(transform.forward*shotSpeed,ForceMode.VelocityChange);
			shotTimer=0f;
		}
	}
}
