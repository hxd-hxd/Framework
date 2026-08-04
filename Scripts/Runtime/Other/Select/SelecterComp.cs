using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(SelecterComp))]
    public class SelecterCompInspector : UnityEditor.Editor
    {
        SelecterComp my => (SelecterComp)target;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            //EditorGUILayout.LabelField("监视器", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                
            }
        }
    }
#endif

    public class SelecterComp : MonoBehaviour
    {
        public Selecter _selecter;

        void Start()
        {
            _selecter.Init();

            // 检查是否选中的
            if(_selecter.curItem == null)
            {
                for (int i = 0; i < _selecter.Count; i++)
                {
                    if (_selecter[i].isSelect)
                    {
                        _selecter.curItem = _selecter[i];
                        break;
                    }
                }
            }
        }

    }
}
