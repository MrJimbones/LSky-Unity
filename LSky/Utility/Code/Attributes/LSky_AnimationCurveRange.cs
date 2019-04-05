/////////////////////////////////////////////////
/// LSky.
///----------------------------------------------
/// Utility.
///----------------------------------------------
/// Animation Curve Range:
///----------------------------------------------
/// Description: Range for AnimationCurve fields.
/////////////////////////////////////////////////
using System;
using UnityEngine;

namespace Rallec.LSky.Utility
{
    /// <summary> Range values for animation curves </summary>
    public class LSky_AnimationCurveRange : PropertyAttribute
    {

        public readonly float timeStart;
        public readonly float valueStart;
        public readonly float timeEnd;
        public readonly float valueEnd;

        public LSky_AnimationCurveRange(float _timeStart, float _valueStart, float _timeEnd, float _valueEnd)
        {
            this.timeStart  = _timeStart;
            this.valueStart = _valueStart;
            this.timeEnd    = _timeEnd;
            this.valueEnd   = _valueEnd;
        }

    }

}

