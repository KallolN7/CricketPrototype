using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Games2Win
{
	/// <summary>
	/// Script responsible for handling the dragging and position of ball pitch marker
	/// </summary>
	public class MarkerDraggerScript : MonoBehaviour, IBeginDragHandler, IDragHandler
	{

		[SerializeField] private GameData gameData; //GameData scriptable object
		private GameObject marker; // marker game object
		private Vector3 defaultPosition;
		private bool IsBallThrown;
		private Vector2 startTouchPosition; // start touch position of the drag
		private Vector3 markerStartTouchPosition; // marker's position at the start of the drag
		private Vector2 newTouchPosition; // new touch position i.e. the current touch position
		private int touchId;

   
        #region Mono Methods

        void Awake()
		{
			marker = GameObject.Find("Marker");  //Using Find instead of direct referencing in scene so that this MarkerTouchPanel gameobject can be used as a prefab and instantiated when necessary or downloaded as Addressables. 
			Input.multiTouchEnabled = false; // disable multitouch
			defaultPosition = marker.transform.position;
#if PLATFORM_ANDROID // if the platform is android change the scaleDownDragBy value
			gameData.MarkerDragData.scaleDownDragBy = 0.05f;
#endif
		}

		/// <summary>
		/// Subscribing methods to events
		/// </summary>
		private void OnEnable()
		{
			EventManager.AddListener(EventID.Reset, OnReset);
			EventManager.AddListener(EventID.BallBowled, OnBallBowled);
		}

		/// <summary>
		/// Un-Subscribing methods from events
		/// </summary>
		private void OnDisable()
		{
			EventManager.RemoveListener(EventID.Reset, OnReset);
			EventManager.RemoveListener(EventID.BallBowled, OnBallBowled);
		}

        private void Start()
        {
			EventManager.TriggerEvent(EventID.DragMarker, marker.transform.position);
        }

		#endregion

		#region Public Regions


		/// <summary>
		/// Implementing the OnDrag method from IDragHandler interface to drag the ball pitch marker to desired position
		/// and passing the value via event 
		/// </summary>
		/// <param name="eventData"></param>
		public void OnDrag(PointerEventData eventData)
		{
			if (!IsBallThrown)
			{ // if the ball is not thrown the marker can be moved
				newTouchPosition = eventData.position; // update it the current position of the touch

				// set the marker's position based on the drag value of the touch
				marker.transform.position = new Vector3(markerStartTouchPosition.x + (newTouchPosition.x - startTouchPosition.x) * gameData.MarkerDragData.scaleDownDragBy,
			    marker.transform.position.y,
				markerStartTouchPosition.z + (newTouchPosition.y - startTouchPosition.y) * gameData.MarkerDragData.scaleDownDragBy);

				// clamp the marker's position within it's reach bounds
				marker.transform.position = new Vector3(Mathf.Clamp(marker.transform.position.x, -gameData.MarkerDragData.boundaryPointX, gameData.MarkerDragData.boundaryPointX),
				marker.transform.position.y,
				Mathf.Clamp(marker.transform.position.z, gameData.MarkerDragData.minBoundaryPointZ, gameData.MarkerDragData.maxBoundaryPointZ));

				//Triggering event to update all classes subsribed to this event, that the marker position has changed.
				EventManager.TriggerEvent(EventID.DragMarker, marker.transform.position);
			}
		}

		#endregion

		#region Private Regions

		/// <summary>
		/// Method called on BallBowled event.
		/// </summary>
		/// <param name="obj"></param>
		private void OnBallBowled(System.Object obj)
        {
			IsBallThrown = true;
        }

		/// <summary>
		/// Method called on Reset event.
		/// </summary>
		/// <param name="obj"></param>
		private void OnReset(object arg)
		{
			IsBallThrown = false;
			marker.transform.position = defaultPosition;
			//Triggering event to update all classes subsribed to this event, that the marker position has changed.
			EventManager.TriggerEvent(EventID.DragMarker, marker.transform.position);

		}

		/// <summary>
		/// Implementing the OnBeginDrag method from IBeginDragHandler interface to begin dragging the ball pitch marker to desired position
		/// </summary>
		/// <param name="eventData"></param>
		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			startTouchPosition = eventData.position; // set the startTouchPosition to the touch's start position 
			markerStartTouchPosition = marker.transform.position; // set the marker's start position
		}
		#endregion

	}
}


