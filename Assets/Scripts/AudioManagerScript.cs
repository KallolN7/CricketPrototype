using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games2Win
{
	/// <summary>
	/// Script resposible for audio
	/// </summary>
	public class AudioManagerScript : MonoBehaviour
	{

        [SerializeField] private GameData gameData;  //GameData scriptable object
		[SerializeField] AudioSource audioSource;

		#region Mono Methods

		/// <summary>
		/// Subscribing methods to events
		/// </summary>
		private void OnEnable()
		{
			EventManager.AddListener(EventID.BallHit, PlayBatHitAudio);
			EventManager.AddListener(EventID.BallBounce, PlayBounceAudio);
			EventManager.AddListener(EventID.HitStumps, PlayBatHitAudio);
		}


		/// <summary>
		/// Un-Subscribing methods from events
		/// </summary>
		private void OnDisable()
		{
			EventManager.RemoveListener(EventID.BallHit, PlayBatHitAudio);
			EventManager.RemoveListener(EventID.BallBounce, PlayBounceAudio);
			EventManager.RemoveListener(EventID.HitStumps, PlayBatHitAudio);
		}

		#endregion


		#region Private Regions

		private void PlayBounceAudio(object obj)
		{
			audioSource.PlayOneShot(gameData.AudioData.bounceClip);
		}

		public void PlayBatHitAudio(object obj)
		{
			audioSource.PlayOneShot(gameData.AudioData.hitClip);
		}

		#endregion



	}

}


