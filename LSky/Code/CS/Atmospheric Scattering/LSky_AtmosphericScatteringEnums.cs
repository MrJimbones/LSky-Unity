/////////////////////////////////////////////////
/// LSky
///----------------------------------------------
/// Atmospheric Scattering.
///----------------------------------------------
/// Atmospheric Scattering enums.
///----------------------------------------------
/// Description: Enums for atmospheric scattering.
/////////////////////////////////////////////////

using System;
using UnityEngine;

namespace LSky
{

    /// <summary>
    /// The way a celestial body affects 
    /// the rayleigh effect of the atmosphere.
    /// </summary>
    public enum LSky_CelestialRayleighMode
    {
        Off                   = 0,
        OpossiteSun           = 1,
        CelestialContribution = 2,
    }

}
