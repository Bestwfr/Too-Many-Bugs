using System;
using System.Collections.Generic;
using System.Linq;
using FlamingOrange.Tools.Components;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace FlamingOrange.Tools
{
    [CustomEditor(typeof(ToolData))]
    public class ToolDataEditor : Editor
    {

        private static List<Type> _dataCompTypes = new List<Type>();
        
        private ToolData _dataSo;

        private bool _showForceUpdateButtons;
        private bool _showAddComponentButtons;

        private void OnEnable()
        {
            _dataSo = target as ToolData;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Set Number of Attacks"))
            {
                foreach (var item in _dataSo.ComponentData)
                {
                    item.InitializeAttackData(_dataSo.NumberOfAttacks);
                }
            }
            
            _showAddComponentButtons = EditorGUILayout.Foldout(_showAddComponentButtons, "Add Component");
            
            if (_showAddComponentButtons)
            {
                foreach (var dataCompType in _dataCompTypes)
                {
                    if (GUILayout.Button(dataCompType.Name))
                    {
                        var comp = Activator.CreateInstance(dataCompType) as ComponentData;

                        if (comp == null) return;

                        comp.InitializeAttackData(_dataSo.NumberOfAttacks);

                        _dataSo.AddData(comp);
                    }
                }
            }
            
            _showForceUpdateButtons = EditorGUILayout.Foldout(_showForceUpdateButtons, "Force Update");
            
            if (_showForceUpdateButtons)
            {
                if (GUILayout.Button("Force Update Component Names"))
                {
                    foreach (var item in _dataSo.ComponentData)
                    {
                        item.SetComponentName();
                    }
                }

                if (GUILayout.Button("Force Update Attack Names"))
                {
                    foreach (var item in _dataSo.ComponentData)
                    {
                        item.SetAttackDataName();
                    }
                }
            }
        }

        [DidReloadScripts]
        private static void OnRecompile()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(assembly => assembly.GetTypes());
            var filteredTypes = types.Where(
                type => type.IsSubclassOf(typeof(ComponentData)) && !type.ContainsGenericParameters && type.IsClass
            );
            _dataCompTypes =  filteredTypes.ToList();
        }
    }
}