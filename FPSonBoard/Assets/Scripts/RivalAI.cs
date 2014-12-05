//高低差のあるNavmeshに対応したキャラクタ移動AI
//Attach to Character


using UnityEngine;
using System.Collections;

// Require these components when using this script
//[RequireComponent(typeof (Animator))]
//[RequireComponent(typeof (CapsuleCollider))]
//[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class RivalAI : MonoBehaviour
{
	
	//public float animSpeed = 1.5f;				// a public setting for overall animator animation speed
	//public float lookSmoother = 3f;				// a smoothing setting for camera motion
	
	//private Animator anim;							// a reference to the animator on the character
	//private AnimatorStateInfo currentBaseState;			// a reference to the current state of the animator, used for base layer
	//private AnimatorStateInfo layer2CurrentState;	// a reference to the current state of the animator, used for layer 2
	private CapsuleCollider col;					// a reference to the capsule collider of the character
	
	[HideInInspector]
	public NavMeshAgent nav;					// Reference to the nav mesh agent.
	float speedDampTime = 0.1f;				// Damping time for the Speed parameter.
	float deadZone = 3f;					// The number of degrees for which the rotation isn't controlled by Mecanim.
	public float MinSpeed = 0.2f;
	public float MaxSpeed = 1.0f;

	float timer=0f;
	bool invokeFlag=false;

	public float area;
	
	[SerializeField]
	Vector3 dest;
	[SerializeField]
	float remain;

	public bool findPlayer;
	Transform player;
	float shotTimer;
	float shotInterval= 1.0f;

	public GameObject bullet;
	public GameObject Muzzle;
	public float speed;
		

	void Start ()
	{
		player= GameObject.FindWithTag("Player").transform;
		nav = GetComponent<NavMeshAgent> ();
		//nav.updateRotation = false;	// Making sure the rotation is controlled by Mecanim.
		//nav.updatePosition = false;	// Making sure the position is controlled by Mecanim.
		Invoke ("SetNav", 1f);
		findPlayer = false;
	}
	
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		shotTimer += Time.deltaTime;

		if (!findPlayer && (transform.position - nav.destination).sqrMagnitude <= nav.stoppingDistance*nav.stoppingDistance || timer > 10f) {
			if (!invokeFlag) {
				Invoke ("SetNav", Random.Range (1f, 3f));
				invokeFlag = true;
				nav.speed=0f;
			}
		}

		if (findPlayer) {
			nav.Stop();
			transform.rotation = Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(player.position-transform.position),5f);
			if(shotTimer>shotInterval){
				Shoot();
				shotTimer=0f;
			}
		}
		        
		
		remain = nav.remainingDistance;
		//NavAnimSetup ();
	}
	
	
	void SetNav(){
		Vector3 pos = RandomPosition ();
		//NavMeshHit hit;
		//NavMesh.Raycast (pos + Vector3.up * 30, pos, out hit, -1);
		RaycastHit hit;
		while (true) {
			if(Physics.Raycast (pos + Vector3.up * 30, -Vector3.up, out hit))
				break;
		}
		Vector3 dest = hit.point;
		//Debug.Log (pos);
		//Debug.Log (dest);
		nav.SetDestination (dest);
		nav.speed = Random.Range (MinSpeed, MaxSpeed);
		dest = nav.destination;
			
			//if(nav.speed>5.0f) nav.updatePosition = true;
			//else nav.updatePosition = false;
		
		timer=0f;
		invokeFlag = false;
	}
	
	
	//To Do  mottorandamu
	Vector3 RandomPosition(){
		float x = Random.Range (-area,area);
		float z = Random.Range (-area,area);
		//Vector3 toCenter = (center - transform.position).normalized;
		return transform.position + new Vector3 (x, 0f, z);  // +toCenter*Random.Range(3f,10f);
	}

	
	
	void NavAnimSetup ()
	{
		// Create the parameters to pass to the helper function.
		float speed;
		float angle;
		
		// ... and the angle is the angle between forward and the desired velocity.
		angle = FindAngle(transform.forward, nav.desiredVelocity, transform.up);
		
		if (nav.remainingDistance > nav.stoppingDistance) {
			// Otherwise the speed is a projection of desired velocity on to the forward vector...
			speed = nav.speed;//Vector3.Project (nav.desiredVelocity, transform.forward).magnitude + 0.1f;
		} else
			speed = 0f;
		
		
		// If the angle is within the deadZone...
		if(Mathf.Abs(angle) < deadZone)
		{
			// ... set the direction to be along the desired direction and set the angle to be zero.
			transform.LookAt(transform.position + nav.desiredVelocity);
			angle = 0f;
		}
		
		// Call the Setup function of the helper class with the given parameters.
		Setup(speed, angle);
	}
	
	
	float FindAngle (Vector3 fromVector, Vector3 toVector, Vector3 upVector)
	{
		// If the vector the angle is being calculated to is 0...
		if(toVector == Vector3.zero)
			// ... the angle between them is 0.
			return 0f;
		
		// Create a float to store the angle between the facing of the enemy and the direction it's travelling.
		float angle = Vector3.Angle(fromVector, toVector);
		
		// Find the cross product of the two vectors (this will point up if the velocity is to the right of forward).
		Vector3 normal = Vector3.Cross(fromVector, toVector);
		
		// The dot product of the normal with the upVector will be positive if they point in the same direction.
		angle *= Mathf.Sign(Vector3.Dot(normal, upVector));
		
		return angle;
	}
	
	
	public void Setup(float speed, float angle)
	{
		// Set the mecanim parameters and apply the appropriate damping to them.
		//anim.SetFloat("Speed", speed, speedDampTime, Time.deltaTime);
		//anim.SetFloat ("Direction", angle / 90f);//, angularSpeedDampTime, Time.deltaTime);
	}	

	void Shoot(){
		GameObject bul = (GameObject)Instantiate (bullet, Muzzle.transform.position, Muzzle.transform.rotation);
		bul.rigidbody.AddForce(transform.forward*speed,ForceMode.VelocityChange);
	}
	
}