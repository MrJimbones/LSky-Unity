/////////////////////////////////////////////////
/// LSky
///----------------------------------------------
/// Dome
///----------------------------------------------
/// Basic Object Instantiate.
///----------------------------------------------
/// Description: Instantiate basic object helper
/////////////////////////////////////////////////

using System;
using UnityEngine;

namespace Rallec.LSky
{
    /// <summary></summary>
    [Serializable] public class LSky_BasicObjectInstantiate : LSky_EmptyObjectInstantiate
    {

        public MeshFilter meshFilter     = null;
        public MeshRenderer meshRenderer = null;

        /// <summary> Check if all components are asigned. </summary>
        public override bool CheckComponents
        {
            get
            {
                if(meshFilter == null)  return false;
                if(meshRenderer == null)return false;
                return base.CheckComponents;
            }
        }

        /// <summary> Instantiate Object. </summary>
        /// <param bame="parentName"> Parent name </param>
        /// <param bame="name"> Name </param>
        public override void Instantiate(string parentName, string name)
        {

            base.Instantiate(parentName, name);
			
            var mf = gameObject.GetComponent<MeshFilter>();
            if(mf != null)
                meshFilter = mf;
            else	
                meshFilter = gameObject.AddComponent<MeshFilter>();

            var mr = gameObject.GetComponent<MeshRenderer>();
            if(mr != null)
                meshRenderer = mr;
            else	
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

    }
}