/////////////////////////////////////////////////
/// LSky.
///----------------------------------------------
/// Utility.
///----------------------------------------------
/// Common Editor:
///----------------------------------------------
/// Description: Common for custom inspectos.
/////////////////////////////////////////////////

using System;
using UnityEngine;
using UnityEditor;

namespace Rallec.LSky.Utility
{
    public abstract class LSky_CommonEditor : Editor
    {

        // Set Obj.
        //-----------------------------------------------------------
        protected SerializedObject serObj;

        // Title Styule.
        //-----------------------------------------------------------
        protected GUIStyle TextTitleStyle
        {
            get
            {
                GUIStyle s  = new GUIStyle(EditorStyles.label);
                s.fontStyle = FontStyle.Bold;
                s.fontSize  = 20;
                return s;
            }
        }

        protected GUIStyle TextTabTitleStyle
        {
            get
            {
                GUIStyle s  = new GUIStyle(EditorStyles.label);
                s.fontStyle = FontStyle.Bold;
                s.fontSize  = 10;
                return s;
            }
        }

        // Title.
        //-----------------------------------------------------------
        protected virtual string Title{ get{ return "New Class"; } }

        // Initialized.
        //-----------------------------------------------------------
        protected virtual void OnEnable()
        {
            serObj = new SerializedObject(target);
        }

        // On Inspector GUI.
        //------------------------------------------------------------
        public override void OnInspectorGUI()
        {
            serObj.Update();

            LSky_EditorGUIUtility.ShurikenHeader(Title, TextTitleStyle, LSky_ShurikenStyle.TitleHeader);

            _OnInspectorGUI();

            serObj.ApplyModifiedProperties();

        }

        protected abstract void _OnInspectorGUI();

    }

}
