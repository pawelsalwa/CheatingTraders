using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UMa {

	 public class SocketList {

		  public List<Socket> sockets = new List<Socket>(20);

		  [System.NonSerialized]
		  public int count = 0;
		  [System.NonSerialized]
		  public int enumerator = 0;

		  public void AddSocketToEnd(ref Socket socket) {
				if (enumerator > sockets.Capacity) {
					 sockets.Capacity = sockets.Capacity * 2;
				}
				if (enumerator >= sockets.Count) {
					 sockets.Add(new Socket());
				}
				socket = sockets[enumerator];
				enumerator++;
		  }
		  public Socket this[int i] {
				get {
					 return sockets[i];
				}
		  }
	 }

	 public class Socket {

		  public Node node;
		  public Vector2 direction;
		  public Rect rect;
		  public bool useDirection = false;
		  public bool isInput;
		  public System.Action<object> action;

		  public Rect dropRect {
				get {
					 return new RectOffset(4, 4, 4, 4).Add(new Rect(rect));
				}
		  }

		  public IList connections;
	 }
}