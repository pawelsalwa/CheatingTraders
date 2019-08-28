using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace UMa
{

    [AttributeUsage(AttributeTargets.Class)]
    public class CustomNodeEditorAttribute : Attribute
    {
        public Type type
        {
            get;
            private set;
        }

        public CustomNodeEditorAttribute(Type type)
        {
            this.type = type;
        }
    }

    public class CustomNodeEditor
    {

        private Vector2 defaultSize = new Vector2(50, 50);

        public virtual Vector2 size
        {
            get
            {
                return defaultSize;
            }
        }
        
        public virtual Rect DrawNode(System.Object objNode, System.Object objHost, int filter)
        {
            return new Rect();
        }

    }
    
    public static class CustomNodeEditorController
    {
        
        private static Dictionary<Type, CustomNodeEditor> editors = null;
        
        public static CustomNodeEditor GetEditor(Type type)
        {
            if(editors == null)
            {
                editors = new Dictionary<Type, CustomNodeEditor>();
                System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    System.Type[] types = assembly.GetTypes();
                    foreach (var v in types)
                    {
                        var attributes = v.GetCustomAttributes(typeof(CustomNodeEditorAttribute), true);
                        if (attributes.Length > 0)
                        {
                            var attribute = attributes[0] as CustomNodeEditorAttribute;
                            var editorInstance = assembly.CreateInstance(v.FullName);
                            editors.Add(attribute.type, editorInstance as CustomNodeEditor);
                        }
                    }
                }
            }
            var result = null as CustomNodeEditor;
            editors.TryGetValue(type, out result);
            return result;
        }

    }



}