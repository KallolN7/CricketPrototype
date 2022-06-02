using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Games2Win
{
	/// <summary>
	/// Script resposible for swipe detection for batting  
	/// </summary>
	public class BatSwipePanelScript : MonoBehaviour, IBeginDragHandler, IDragHandler
	{
    	[SerializeField]private GameData gameData;  //GameData scriptable object
		private Vector2 startTouchPosition; // the touch's start position
		private Vector2 newTouchPosition; // the current touch's position 
		private bool IsBatSwinged;


		#region Mono Methods

		void Awake()
		{
			Input.multiTouchEnabled = false; // switch multitouch off
		}

		/// <summary>
		/// Subscribing methods to events
		/// </summary>
		private void OnEnable()
		{
			EventManager.AddListener(EventID.Reset, OnReset);
		}

		/// <summary>
		/// Un-Subscribing methods from events
		/// </summary>
		private void OnDisable()
		{
			EventManager.RemoveListener(EventID.Reset, OnReset);
		}

		#endregion


		#region Public Regions

		/// <summary>
		/// Implementing the OnDrag method from IDragHandler interface to drag and calculate the swipe direction
		/// and passing the value via event 
		/// </summary>
		/// <param name="eventData"></param>
		public void OnDrag(PointerEventData eventData)
		{
			newTouchPosition = eventData.position; // set newTouchPosition to current drag position

			// if the bat has not been swinged i.e the player has not tried hitting the ball before 
			// and the drag length is greated than the minimum drag length required then call the
			// BatControllerScript's HitTheBall function with dragAngle passed as the parameter
			if (!IsBatSwinged && Vector2.Distance(newTouchPosition, startTouchPosition) >= gameData.SwipeData.minDrag)
			{
				IsBatSwinged = true;
				Vector2 dragDirection = newTouchPosition - startTouchPosition; // direction vector of the drag
				float dragAngle = Mathf.Atan2(dragDirection.y, dragDirection.x) * Mathf.Rad2Deg; // angle of the direction vector

				// reset the dragAngle to match that of the world's angle
				dragAngle += 90;
				dragAngle *= -1;

				// trigger BallHit event with dragAngle passed as the parameter
				EventManager.TriggerEvent(EventID.BallHit, dragAngle);
			}
		}

		#endregion

		#region Private Regions

		/// <summary>
		/// Implementing the OnBeginDrag method from IBeginDragHandler interface to begin swiping to hit ball in desired direction
		/// </summary>
		/// <param name="eventData"></param>
		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			startTouchPosition = eventData.position; // set startTouchPosition to the touch's start position
		}

		/// <summary>
		/// Method called on Reset event.
		/// </summary>
		/// <param name="obj"></param>
		private void OnReset(object arg)
		{
			IsBatSwinged = false;
		}

		#endregion
	}
}


