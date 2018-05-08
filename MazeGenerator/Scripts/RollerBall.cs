using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//<summary>
//Ball movement controlls and simple third-person-style camera
//</summary>
public class RollerBall : MonoBehaviour {

	public Camera ViewCamera;
	public AudioClip HitSound;
	public AudioClip CoinSound;
    public AudioClip MeditationAwake;
	public Camera otherCamera; 
	public int coinsAccrued = 0; 
	public GameObject MindWave;
    public Text CoinCounter;
    public Text FeedbackText;
	private DisplayData controller; 

	private Rigidbody mRigidBody = null;
	private AudioSource mAudioSource = null;
	private bool mFloorTouched = false;


	void Start () {
		controller = MindWave.GetComponent<DisplayData> (); 
		mRigidBody = GetComponent<Rigidbody> ();
		mAudioSource = GetComponent<AudioSource> ();
		 
	}

	void FixedUpdate () {
		if (mRigidBody != null && CheckGameOver() == false && Input.GetKey(KeyCode.Space)) { //change space bar to controller.attention1 >= 70
            FeedbackText.text = "Good work! Keep up your focus!";
            if (Input.GetButton ("Horizontal")) {
				mRigidBody.AddTorque(Vector3.back * Input.GetAxis("Horizontal")*12);
			}
			if (Input.GetButton ("Vertical")) {
				mRigidBody.AddTorque(Vector3.right * Input.GetAxis("Vertical")*12);
			}
		} else if(CheckGameOver()==true) {
            FeedbackText.text = "Congratulations, you've won!";
        } else {
            FeedbackText.text = "Focus your attention to move the ball!";
        }
		if (ViewCamera != null) {
			Vector3 direction = (Vector3.up*2+Vector3.back)*2;
			Debug.DrawLine(transform.position,transform.position+direction,Color.red);
            ViewCamera.transform.position = transform.position + direction;
            ViewCamera.transform.LookAt(transform.position);
		}
        if (Input.GetKey(KeyCode.Return)) //change enter key to controller.meditation1 >=70 (or another value TBD) 
        {
            if (mAudioSource != null && MeditationAwake != null)
            {
                mAudioSource.PlayOneShot(MeditationAwake);
            }
            StartCoroutine(TimedCameraSwitch(6.0f));
        }
        CheckGameOver();

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
            CoinCounter.text = "Coins: " + coinsAccrued;
			Debug.Log (coinsAccrued);
		}
	}

    bool CheckGameOver() {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        Debug.Log(coins);
        if (coins.Length < 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
	//void UpdateCoins()
	//{
	//	//GUILayout.Label("Coins: " + coinsAccrued)
	//	//HERE IS WHERE WE TEST WHETHER OR NOT TO SWITCH THE CAMERA
	//	if (Input.GetKeyDown(KeyCode.Return)) {
	//		StartCoroutine(TimedCameraSwitch (6.0f));
	//	}
				
	//}
	void cameraSwitch(){
		
		if (ViewCamera.enabled) {
			ViewCamera.enabled = false; 
			otherCamera.enabled = true; 
		} else {
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
