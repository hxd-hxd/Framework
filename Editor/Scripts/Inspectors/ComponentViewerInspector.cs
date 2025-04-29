// -------------------------
// 创建日期：2023/4/11 15:27:24
// -------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Framework.Editor
{

    [CustomEditor(typeof(ComponentViewer))]
    public class ComponentViewerInspector : UnityEditor.Editor
    {
        protected Object script;// 脚本资产
        protected SerializedProperty cTarget;

        protected GenericsTypeGUI cTargetGUI;
        protected GenericsTypeGUI _numGUI;

        bool cTargetSettingsFoldout { get => my.cTargetSettingsFoldout; set => my.cTargetSettingsFoldout = value; }
        bool showNonsupportMember { get => my.showNonsupportMember; set => my.showNonsupportMember = value; }
        bool showInheritRelation { get => my.showInheritRelation; set => my.showInheritRelation = value; }
        int maxDepth { get => my.maxDepth; set => my.maxDepth = value; }
        int minTextLine { get => my.minTextLine; set => my.minTextLine = value; }
        int maxTextLine { get => my.maxTextLine; set => my.maxTextLine = value; }

        ComponentViewer my => (ComponentViewer)target;

        // 此函数在脚本启动时调用
        protected virtual void Awake()
        {
            //Debug.Log($"{nameof(ComponentViewerInspector)}.Awake");

            script = MonoScript.FromMonoBehaviour(my);

            cTarget = serializedObject.FindProperty("_target");

            cTargetGUI = new GenericsTypeGUI(my.target, my, my.GetType().GetField("_target"
                , BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
            cTargetGUI.foldout = true;
            //cTargetGUI.showInheritRelation = false;
            cTargetGUI.setValueStartEvent = (_, v, _info) =>
            {
                Undo.RecordObject(my.target.gameObject, $"修改组件查看器目标 {_info?.MemberType} {_info?.Name}");
                Undo.RecordObject(my.target, $"修改组件查看器目标 {_info?.MemberType} {_info?.Name}");
                EditorUtility.SetDirty(my.target);
            };

            _numGUI = new GenericsTypeGUI(my._num, my, my.GetType().GetField("_num", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
            _numGUI.foldout = true;
            _numGUI.setValueStartEvent = (_, v, _info) =>
            {
                Undo.RecordObject(my, $"修改组件查看器 {_info.MemberType} {_info.Name}");
                EditorUtility.SetDirty(my);
            };

            Init();
        }

        protected virtual void OnEnable()
        {
            //Debug.Log(script);

        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            OnEditorTargetScriptGUI();

            OnTitleGUI();

            OnCTargetGUI();

            //_numGUI.OnGUI();

            OnHelpGUI();

            // 字段、属性
            OnCTargetMemberGUI();
        }

        protected virtual void Init()
        {
            cTargetGUI.target = my.target;
        }

        // 编辑器目标脚本文件
        protected virtual void OnEditorTargetScriptGUI()
        {
            if (script)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), true);
                EditorGUI.EndDisabledGroup();
            }
        }

        // 标题
        protected virtual void OnTitleGUI()
        {
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("组件成员查看器", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space(2);
        }

        // 温馨提示
        protected virtual void OnHelpGUI()
        {
            EditorGUILayout.HelpBox("将暴露所有 “非公开的” 字段和属性，请谨慎修改！\r\n部分属性及字段修改将不能被 Unity 序列化！数据会丢失，且不支持撤回！\r\n有关 Unity 序列化规则可自行查阅资料，比较简单的的判断方式是：能在原组件的 Inspector 中显示的都是可序列化的（本组件除外）。", MessageType.Warning);
        }

        // 目标 gui
        protected virtual void OnCTargetGUI()
        {
            EditorGUI.BeginChangeCheck();
            Object oldTarget = cTarget.objectReferenceValue;
            EditorGUILayout.PropertyField(cTarget);
            bool change_cTarget = EditorGUI.EndChangeCheck();
            //change_cTarget = change_cTarget || oldTarget != cTarget.objectReferenceValue;

            // 切换目标
            if (change_cTarget)
            {
                // 先保存
                serializedObject.ApplyModifiedProperties();
                //Debug.Log(my.target as object == null);
                // 再初始化
                Init();
                //Debug.Log($"更改目标为：{(cTarget.objectReferenceValue as Component)?.gameObject.name}<{cTarget.objectReferenceValue?.GetType().Name}>。原目标：{(oldTarget as Component)?.gameObject.name}<{oldTarget?.GetType().Name}>");
            }

            OnCTargetScriptGUI();
        }
        // 查看器目标脚本文件
        protected virtual void OnCTargetScriptGUI()
        {
            if (my.target is MonoBehaviour monoScript)
            {
                Object cTargetScript = MonoScript.FromMonoBehaviour(monoScript);
                if (cTargetScript)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField("Target Script", cTargetScript, typeof(MonoScript), true);
                    EditorGUI.EndDisabledGroup();
                }
            }

        }

        // 目标组件的成员（字段、属性） gui
        protected virtual void OnCTargetMemberGUI()
        {
            OnCTargetSettingsGUI();

            CheckCTargetChange();

            if (my.target == null)
            {
                EditorGUILayout.HelpBox("设置 Target 以操作字段、属性！", MessageType.Info);
            }
            else
            {
                cTargetGUI.OnGUI(true, false);
            }
        }
        // 检查目标变化，有变化则自动更新目标成员信息
        protected virtual void CheckCTargetChange()
        {
            /* 检查已有字段、属性的所属实例与现有目标是否相同
                用于解决类似以下问题：
                ArgumentException: Field _target defined on type Framework.ComponentViewer is not a field on the target object which is of type UnityEngine.Transform.
            */
            bool targetChange = cTargetGUI.CheckTargetChange(GetType(my.target));
            if (targetChange)
            {
                Init();
            }
        }
        // 设置查看器目标的 gui
        protected virtual void OnCTargetSettingsGUI()
        {
            cTargetSettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(cTargetSettingsFoldout, "设置");
            if (cTargetSettingsFoldout)
            {
                EditorGUI.indentLevel++;

                showNonsupportMember = EditorGUILayout.Toggle("显示不支持类型的成员", showNonsupportMember);

                showInheritRelation = EditorGUILayout.Toggle("显示类型成员的继承关系", showInheritRelation);

                maxDepth = EditorGUILayout.IntSlider("GUI 最大深度", maxDepth, 2, 100);

                //float minTextLine = cTargetGUI.minTextLine, maxTextLine = cTargetGUI.maxTextLine;
                //EditorGUI.MinMaxSlider("文本显示行数", ref minTextLine, ref maxTextLine, 2, 100);
                //cTargetGUI.minTextLine = (int)minTextLine;
                //cTargetGUI.maxTextLine = (int)maxTextLine;
                minTextLine = EditorGUILayout.IntSlider("最小文本显示行数", minTextLine, 2, 100);
                maxTextLine = EditorGUILayout.IntSlider("最大文本显示行数", maxTextLine, 2, 100);

                //if (GUILayout.Button("展开所有成员"))
                //{

                //}

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            cTargetGUI.showNonsupportMember = showNonsupportMember;
            cTargetGUI.showInheritRelation = showInheritRelation;

            cTargetGUI.maxDepth = maxDepth;
            maxDepth = cTargetGUI.maxDepth;

            cTargetGUI.minTextLine = minTextLine;
            cTargetGUI.maxTextLine = maxTextLine;
            minTextLine = cTargetGUI.minTextLine;
            maxTextLine = cTargetGUI.maxTextLine;
        }

        /// <summary>
        /// 获取类型，如果是可 <c>null</c> 类型且值为 <c>null</c>，则获取非实例类型
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Type GetType<TIn>(TIn obj) => GenericsTypeGUI.GetType(obj);

    }

}