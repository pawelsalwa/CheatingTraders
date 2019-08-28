using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UMa
{
    [System.Serializable]
    public class Node
    {
        public string label = "";

        [System.NonSerialized]
        public bool drag = false;

        [System.NonSerialized]
        public bool selected = false;

		  [System.NonSerialized]
		  public Vector2 initialDragPosition = Vector3.zero;

		  [System.NonSerialized]
        public object objectReference = null;

        [SerializeField, HideInInspector]
        public Rect rect;
    }

    public interface INode
    {
        Node node
        {
            get;
        } 
    }

    public interface INodeList
    {
        IList iListNodes
        {
            get;
        }
        void RefreshINodeList();
    }

}
