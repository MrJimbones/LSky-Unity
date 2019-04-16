/////////////////////////////////////////////////
/// LSky
///----------------------------------------------
/// Dome.
///----------------------------------------------
/// Dir Light Instantiate.
///----------------------------------------------
/// Description: Instantiate dirrection Light
/// helper
/////////////////////////////////////////////////

using System;
using UnityEngine;

namespace LSky
{
	
	[Serializable] 
	public class LSky_DirLightIntantiate : LSky_EmptyObjectInstantiate
	{
		public Light light;

		/// <summary> Check if all components are asigned. </summary>
		public override bool CheckComponents
		{
			get
			{
				if(light == null) return false;
				return base.CheckComponents;
			}
		}
		
		/// <summary> Instantiate Light. </summary>
		/// <param bame="parentName"> Parent Name</param>
		/// <param bame="lightName"> Light Name </param>
		public void InstantiateLight(string parentName, string lightName)
		{

			Instantiate(parentName, lightName);

			var l = gameObject.GetComponent<Light>();
			if(l != null)
				light = l;
			else	
				light = gameObject.AddComponent<Light>();
			
			light.type = LightType.Directional;
		}       
	}
}
