using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games2Win
{
	public class StumpsControllerScript : MonoBehaviour
	{
		public static StumpsControllerScript instance;
		public GameObject[] stumps; // store all the stumps


		#region Mono Methods
		#endregion

		#region Public Regions
		#endregion

		#region Private Regions
		#endregion


		void Awake()
		{
			instance = this;
		}

		public void ResetStumps()
		{
			
		}
	}

}

