using UnityEngine;
using System.Collections;

//<summary>
//Ball movement controlls and simple third-person-style camera
//</summary>
public class RollerBall : MonoBehaviour {

	public Camera ViewCamera;
	public AudioClip JumpSound;
	public AudioClip HitSound;
	public AudioClip CoinSound;
	public Camera otherCamera; 
	public int coinsAccrued = 0; 

	private Rigidbody mRigidBody = null;
	private AudioSource mAudioSource = null;
	private bool mFloorTouched = false;


	void Start () {
		mRigidBody = GetComponent<Rigidbody> ();
		mAudioSource = GetComponent<AudioSource> ();
		 
	}

	void FixedUpdate () {
		if (mRigidBody != null) {
			if (Input.GetButton ("Horizontal")) {
				mRigidBody.AddTorque(Vector3.back * Input.GetAxis("Horizontal")*10);
			}
			if (Input.GetButton ("Vertical")) {
				mRigidBody.AddTorque(Vector3.right * Input.GetAxis("Vertical")*10);
			}
			if (Input.GetButtonDown("Jump")) {
				if(mAudioSource != null && JumpSound != null){
					mAudioSource.PlayOneShot(JumpSound);
				}
				mRigidBody.AddForce(Vector3.up*200);
			}
		}
		if (ViewCamera != null) {
			Vector3 direction = (Vector3.up*2+Vector3.back)*2;
			RaycastHit hit;
			Debug.DrawLine(transform.position,transform.position+direction,Color.red);
			if(Physics.Linecast(transform.position,transform.position+direction,out hit)){
				ViewCamera.transform.position = hit.point;
			}else{
				ViewCamera.transform.position = transform.position+direction;
			}
			ViewCamera.transform.LookAt(transform.position);
		}

	}

	void OnCollisionEnter(Collision coll){
		if (coll.gameObject.tag.Equals ("Floor")) {
			mFloorTouched = true;
			if (mAudioSource != null && HitSound != null && coll.relativeVelocity.y > .5f) {
				mAudioSource.PlayOneShot (HitSound, coll.relativeVelocity.magnitude);
			}
		} else {
			if (mAudioSource != null && HitSound != null && coll.relativeVelocity.magnitude > 2f) {
				mAudioSource.PlayOneShot (HitSound, coll.relativeVelocity.magnitude);
			}
		}
	}

	void OnCollisionExit(Collision coll){
		if (coll.gameObject.tag.Equals ("Floor")) {
			mFloorTouched = false;
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag.Equals ("Coin")) {
			if(mAudioSource != null && CoinSound != null){
				mAudioSource.PlayOneShot(CoinSound);
			}
			Destroy(other.gameObject);
			coinsAccrued++;
			Debug.Log (coinsAccrued);
			UpdateCoins (); 
		}
	}

	void UpdateCoins()
	{
		//GUILayout.Label("Coins: " + coinsAccrued);
		if (coinsAccrued >= 5) {
			Debug.Log ("About to Switch the Cameras"); 
			StartCoroutine(TimedCameraSwitch (6.0f));
		}

			//cameraSwitch(); 
	}
	void cameraSwitch(){
		Debug.Log ("In Camera Switch Function"); 
		if (ViewCamera.enabled) {
			Debug.Log ("Switch Camera");
			//ViewCamera.activeSelf.Equals (false); 
			//otherCamera.activeSelf.Equals (true);
			ViewCamera.enabled = false; 
			otherCamera.enabled = true; 
		} else {
			Debug.Log ("Switch back");
			ViewCamera.enabled = true; 
			otherCamera.enabled = false; 
		}
	}

	IEnumerator TimedCameraSwitch(float duration) {
		float t = 0.0f;
		Debug.Log ("Timer Called");
		cameraSwitch ();
		while (t < duration) {
			t += Time.deltaTime;
			yield return null;
		}
		cameraSwitch ();

	}

}
