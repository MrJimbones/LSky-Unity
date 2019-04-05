/////////////////////////////////////////////////
/// LSky.
///----------------------------------------------
/// Utility.
///----------------------------------------------
/// Animation Curve Range Drawer:
///----------------------------------------------
/// Description: Range for AnimationCurve fields.
/////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;

namespace Rallec.LSky.Utility
{
    [CustomPropertyDrawer(typeof(LSky_AnimationCurveRange))]
    public class LSky_AnimationCurveRangeDrawer: PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LSky_AnimationCurveRange attr = attribute as LSky_AnimationCurveRange;

            if(property.propertyType == SerializedPropertyType.AnimationCurve)
                EditorGUI.CurveField(position, property, Color.white, new Rect(attr.timeStart, attr.valueStart, attr.timeEnd, attr.valueEnd));
            else
                EditorGUI.HelpBox(position, "Only work with AnimationCurve", MessageType.Warning);
        }

    }
}