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

namespace LSky.Utility
{
    /// <summary> Range values for animation curves </summary>
    public class LSky_AnimationCurveRange : PropertyAttribute
    {

        public readonly float timeStart;
        public readonly float valueStart;
        public readonly float timeEnd;
        public readonly float valueEnd;

        public readonly int colorIndex;

        public LSky_AnimationCurveRange(float _timeStart, float _valueStart, float _timeEnd, float _valueEnd, int _colorIndex)
        {
            this.timeStart  = _timeStart;
            this.valueStart = _valueStart;
            this.timeEnd    = _timeEnd;
            this.valueEnd   = _valueEnd;
            this.colorIndex = _colorIndex;
        }

    }

}

