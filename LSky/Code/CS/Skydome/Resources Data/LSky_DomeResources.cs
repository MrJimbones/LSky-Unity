/////////////////////////////////////////////////
/// LSky
///----------------------------------------------
/// Sky Dome.
///----------------------------------------------
/// Dome Resources.
///----------------------------------------------
/// Description: Resources for skydome.
/////////////////////////////////////////////////

using System.IO;
using UnityEngine;

namespace Rallec.LSky
{

    [CreateAssetMenu(fileName = "LSky_DomeResources", menuName = "Rallec/LSky/Skydome/Resources", order = 1)]
    public partial class LSky_DomeResources : ScriptableObject
    {

        #region [Meshes]

        [Header("Sphere Meshes")]
        public Mesh sphereLOD0;
        public Mesh sphereLOD1;
        public Mesh sphereLOD2;
        public Mesh sphereLOD3;
        //--------------------------

        [Header("Atmosphere Meshes")]
        public Mesh atmosphereLOD0;
        public Mesh atmosphereLOD1;
        public Mesh atmosphereLOD2;
        public Mesh atmosphereLOD3;
        //---------------------------

        [Header("Hemisphere Meshes")]
        public Mesh hemisphereLOD0;
        public Mesh hemisphereLOD1;
        public Mesh hemisphereLOD2;
        public Mesh hemisphereLOD3;
        //---------------------------

        [Header("Other Meshes")]
        public Mesh QuadMesh;
        //---------------------------
        #endregion

        #region [Shaders]

        [Header("Atmosphere Shaders")]
        public Shader atmosphereShader;

        [Header("Deep Space Shaders")]
        public Shader galaxyBackgroundShader;
        public Shader starsFieldShader;

        [Header("Near Space Shaders")]
        public Shader sunShader;
        public Shader moonShader;

        [Header("Ambient Skybox")]
        public Shader ambientSkyboxShader;

        #endregion

        #region [Materials]

        [Header("Atmosphere Materials")]
        public Material atmosphereMaterial;

        [Header("Deep Space Materials")]
        public Material galaxyBackgroundMaterial;
        public Material starsFieldMaterial;

        [Header("Near Space Materials")]
        public Material sunMaterial;
        public Material moonMaterial;

        [Header("Ambient Skybox")]
        public Material ambientSkyboxMaterial;

        #endregion

    }
}