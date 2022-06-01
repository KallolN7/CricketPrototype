using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Games2Win
{
	public class BallControllerScript : MonoBehaviour
	{
		[SerializeField] private Vector3 defaultPosition; // ball's default beginning position
		[SerializeField] private GameObject marker; // stores the marker game object
		[SerializeField] private float ballSpeed; // speed of the ball
		[SerializeField] private GameObject ball; // stores the ball game object
		[SerializeField] private float bounceScalar; // the bounce scalar value to scale the bounce angle after the ball hits the ground
		[SerializeField] private float spinScalar; // the ball's spin scalar value
		[SerializeField] private float realWorldBallSpeed; // the ball's speed to display on the UI which corresponds to the real world units(kmph)
		[SerializeField] private GameObject trajectoryHolder; // the holder game object to parent each trajectory ball object to
		[SerializeField] private int ballType; // the balls type; 0 - straight, 1 - leg spin, 2 - off spin

		private float angle; // the bounce angle of the ball after the ball hits the ground for the first time
		private Vector3 startPosition; // ball's startPosition for the lerp function
		private Vector3 targetPosition; // ball's targetPosition for the lerp function
		private Vector3 direction; // the direction vector the ball is going in
		private Rigidbody rb; // rigidbody of the ball
		private float spinBy; // value to spin the ball by

		private bool firstBounce; // whether ball's hit the ground once or not
		private bool isBallThrown; // whether the ball is thrown or not
		private bool isBallHit; // whether the bat hitted the ball
		private bool isTrajectoryEnabled; // whether the trajectory is enabled or disabled
		private BatSwingData batSwingData;
		public float BallSpeed { set { ballSpeed = value; } }
		public int BallType { set { ballType = value; } }
		public bool IsBallThrown { get { return isBallThrown; } }
		public bool IsTrajectoryEnabled { set { isTrajectoryEnabled = value; } get { return isTrajectoryEnabled; } }
		public bool IsBallHit { get { return isBallHit; } }

		#region Mono Methods

		void Awake()
		{
			marker = GameObject.Find("Marker");
			defaultPosition = transform.position; // set defaultPosition to the balls beginning position
			rb = gameObject.GetComponent<Rigidbody>();
			startPosition = transform.position;  // set the startPosition to the balls beginning position
		}

		private void OnEnable()
		{
			EventManager.AddListener(EventID.Reset, OnReset);
			EventManager.AddListener(EventID.BallBowled, BowlBall);
			EventManager.AddListener(EventID.BatSwing, OnHitTheBall);
			EventManager.AddListener(EventID.UpdateBallSpeed, OnUpdateBallSpeed);
			EventManager.AddListener(EventID.UpdateBallType, OnUpdateBallType);
		}

		private void OnDisable()
		{
			EventManager.RemoveListener(EventID.Reset, OnReset);
			EventManager.RemoveListener(EventID.BallBowled, BowlBall);
			EventManager.RemoveListener(EventID.BatSwing, OnHitTheBall);
			EventManager.RemoveListener(EventID.UpdateBallSpeed, OnUpdateBallSpeed);
			EventManager.RemoveListener(EventID.UpdateBallType, OnUpdateBallType);
		}

		void Update()
		{
			// if the isTrajectoryEnabled is set to true and the ball's velocity is greater than 0 
			// i.e its in motion then instantiate trajectory balls at each frame
			if (rb.velocity.magnitude > 0 && isTrajectoryEnabled)
			{
				GameObject trajectoryBall = Instantiate(ball, transform.position, Quaternion.identity) as GameObject;
				trajectoryBall.transform.SetParent(trajectoryHolder.transform); // set the instantiated trajectory ball's parent to the trajectoryHolder object
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.tag == "HitZone")
			{
				EventManager.TriggerEvent(EventID.BallEnterHitZone);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.tag == "HitZone")
			{
				EventManager.TriggerEvent(EventID.BallExitHitZone);
			}
		}

		void OnCollisionEnter(Collision collision)
		{
			if (!isBallHit && collision.gameObject.CompareTag("Ground"))
			{ // if the ball is not hit by the bat and it collides with the ground then the expression returns true
				switch (ballType)
				{ // check the ballType and set the spinBy value depending on the ball's speed
					case 0:
						spinBy = direction.x; // don't change spinBy 
						break;
					case 1:
						spinBy = spinScalar / ballSpeed; // change spinBy to a positive value based on the spinScalar value and the ball's speed
						break;
					case 2:
						spinBy = -spinScalar / ballSpeed; // change spinBy to a negative value based on the spinScalar value and the ball's speed
						break;
				}

				if (!firstBounce)
				{ // if firstBounce is false i.e. when the ball hits the ground for the first time then the expression returns true 
					firstBounce = true; // set the firstBounce bool to true
					rb.useGravity = true; // allow the gravity to affect the ball

					// change the y value of the direction to the negative of it's present value multiplied by the bounceScalar and ball's speed
					// of the ball i.e. the bounce will be more if the ball's speed is more compared to a slower one
					direction = new Vector3(spinBy, -direction.y * (bounceScalar * ballSpeed), direction.z);
					direction = Vector3.Normalize(direction); // normalize the direction value

					angle = Mathf.Atan2(direction.y, direction.z) * Mathf.Rad2Deg; // calculte the bounce angle from the direction vector

					// Add an instant force impulse in the direction vector multiplied by ballSpeed to the ball considering its mass
					rb.AddForce(direction * ballSpeed, ForceMode.Impulse);
					//rb.velocity = direction * ballSpeed; // update the balls velocity
					CanvasManagerScript.instance.UpdateBallsBounceAngleUI(angle); // Update the balls bounce angle ui text
				}
				AudioManagerScript.instance.PlayBounceAudio(); // play the ball bounce sound
			}

			if (collision.gameObject.CompareTag("Stump"))
			{ // if the ball has hit the stump then the expression returns true
				AudioManagerScript.instance.PlayBatHitAudio(); // play the same sound as the bat hit sound.
				collision.gameObject.GetComponent<Rigidbody>().useGravity = true; // set the stump's rigidbody to be affected by gravity
			}
		}


		#endregion

		#region Public Regions
		#endregion

		#region Private Regions

		private void OnReset(System.Object arg)
		{
			firstBounce = false;
			isBallHit = false;
			isBallThrown = false;
			rb.velocity = Vector3.zero;
			rb.useGravity = false;
			transform.position = defaultPosition;

			// Destroy trajectory balls if childCount of the trajectory holder is greater than 0
			int childCount = trajectoryHolder.transform.childCount;
			if (childCount > 0)
			{ // if the trajectory was enabled, destroy the trajectoryHolder's children i.e each trajectorBall 
				for (int i = childCount - 1; i >= 0; i--)
				{
					Destroy(trajectoryHolder.transform.GetChild(i).gameObject);
				}
			}
		}

		private  void BowlBall(System.Object arg)
		{
			if (!IsBallThrown)
			{ // if the ball is not thrown, throw the ball
				isBallThrown = true;
				CanvasManagerScript.instance.EnableBatSwipePanel(); // Enable the bat swipe panel 
				targetPosition = marker.transform.position; // make the balls target position to the markers position
				direction = Vector3.Normalize(targetPosition - startPosition); // calculate the direction vector
				rb.AddForce(direction * ballSpeed, ForceMode.Impulse); // Add an instant force impulse in the direction vector multiplied by ballSpeed to the ball considering its mass
			}
		}

		private void OnHitTheBall(System.Object obj)
		{
			batSwingData = (BatSwingData)obj;
			Debug.Log("OnHitTheBall, hitdirection = " + batSwingData.hitDirection + " | batspeed = " + batSwingData.batSpeed);
			isBallHit = true; // set the is ball hit to true
			rb.velocity = Vector3.zero; // set the ball's velocity to zero to stop the ball
			direction = Vector3.Normalize(batSwingData.hitDirection); // normalize the hit direction of the bat
			float hitSpeed = (ballSpeed / 2) + batSwingData.batSpeed; // calculate the balls return speed based on the bats speed and the balls speed
			rb.AddForce(-direction * hitSpeed, ForceMode.Impulse); // Add an instant force impulse in the negative direction vector multiplied by ballSpeed to the ball considering its mass
			if (!firstBounce)
			{ // if the ball has never hitted the ground then set the ball's rigidbody to be affected by gravity
				rb.useGravity = true;
			}
		}

		private void SwitchSide(System.Object obj)
		{
			transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z); // negate the x value of balls position to change the side
			defaultPosition = transform.position; // reset the default position to new balls position
			startPosition = transform.position; // reset the default position to new balls position
		}

		private void OnUpdateBallSpeed(System.Object obj)
        {
			ballSpeed = (float)obj;	
        }

		private void OnUpdateBallType(System.Object obj)
		{
			ballType = (int)obj;
		}

		#endregion
	}
}

