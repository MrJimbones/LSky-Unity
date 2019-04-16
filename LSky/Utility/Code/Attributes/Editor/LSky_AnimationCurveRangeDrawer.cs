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

namespace LSky.Utility
{
    [CustomPropertyDrawer(typeof(LSky_AnimationCurveRange))]
    public class LSky_AnimationCurveRangeDrawer: PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LSky_AnimationCurveRange attr = attribute as LSky_AnimationCurveRange;

            Color col = Color.black;
            switch(attr.colorIndex)
            {
                case 0 : col = Color.blue; break;
                case 1 : col = Color.cyan; break;
                case 2 : col = Color.gray; break;
                case 3 : col = Color.green; break;
                case 4 : col = Color.magenta; break;
                case 5 : col = Color.red; break;
                case 6 : col = Color.white; break;
                case 7 : col = Color.yellow; break;
            }

            if(property.propertyType == SerializedPropertyType.AnimationCurve)
                EditorGUI.CurveField(position, property, col, new Rect(attr.timeStart, attr.valueStart, attr.timeEnd, attr.valueEnd));
            else
                EditorGUI.HelpBox(position, "Only work with AnimationCurve", MessageType.Warning);
        }

    }
}