using System;
using System.Collections.Generic;
using System.Linq;
using FlamingOrange.Tools.Components;
using UnityEngine;

namespace FlamingOrange.Tools
{
    public class ToolGenerator : MonoBehaviour
    {
        [SerializeField] private Tool tool;
        [SerializeField] private ToolData data;
        
        private List<ToolComponent> _componentAlreadyOnTool = new List<ToolComponent>();
        
        private List<ToolComponent> _componentAddedToTool = new List<ToolComponent>();
        
        private List<Type> _componentDependencies = new List<Type>();

        private void Start()
        {
            GenerateTool(data);
        }

        public void GenerateTool(ToolData data)
        {
           tool.SetData(data);
           
           _componentAlreadyOnTool.Clear();
           _componentAddedToTool.Clear();
           _componentDependencies.Clear();

           _componentAlreadyOnTool = GetComponents<ToolComponent>().ToList();

           _componentDependencies = data.GetAllDependencies();

           foreach (var dependency in _componentDependencies)
           {
               if (_componentAddedToTool.FirstOrDefault(component => component.GetType() == dependency))
                   continue;
               var toolComponent 
                   = _componentAlreadyOnTool.FirstOrDefault(component => component.GetType() == dependency);

               if (toolComponent == null)
               {
                   toolComponent = gameObject.AddComponent(dependency) as ToolComponent;
               }
               
               toolComponent.Init();
               
               _componentAddedToTool.Add(toolComponent);
           }
           
           var componentsToRemove = _componentAlreadyOnTool.Except(_componentAddedToTool);
           
           foreach (var toolComponent in componentsToRemove)
           {
               Destroy(toolComponent);
           }
        }
    }
}