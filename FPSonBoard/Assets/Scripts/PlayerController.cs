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
	public float rotateFlag;

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

		MovelikeSkateBoard(moveDir);
		//MovewithRemoteController(moveDir,rotateFlag);
		//SimpleMove(moveDir);
	}

	void SimpleMove(Vector3 vec){
		CC.SimpleMove((transform.rotation*vec)*moveSpeed);
	}

	void MovelikeSkateBoard(Vector3 vec){
		vec = Quaternion.AngleAxis(-90f,Vector3.up) * vec;
		//Debug.Log(vec);
		Vector3 proj = Vector3.Project(vec,Vector3.forward);
		CC.SimpleMove((transform.rotation*proj)*moveSpeed);

		if(Mathf.Abs(vec.x)>0.3f && Mathf.Sin(vec.z)>0f) 
			transform.rotation = Quaternion.Lerp(transform.rotation,
			    transform.rotation*Quaternion.FromToRotation(Vector3.forward,Vector3.right*Mathf.Sign(vec.x*vec.z)),Mathf.Abs(vec.z)*Time.deltaTime);
		//transform.rotation = 

		CC.SimpleMove((transform.rotation*proj)*moveSpeed);
	}

	void MovewithRemoteController(Vector3 vec, float rotate){
		CC.SimpleMove((transform.rotation*vec)*moveSpeed);
		transform.Rotate(Vector3.up,rotate*30f*Time.deltaTime);
	}

	public void Shoot(){
		if(shotTimer>shotInterval){
			GameObject bul = (GameObject)Instantiate (bullet, muzzle.position, muzzle.rotation);
			bul.rigidbody.AddForce(muzzle.parent.forward*shotSpeed,ForceMode.VelocityChange);
			shotTimer=0f;
		}
	}
}
