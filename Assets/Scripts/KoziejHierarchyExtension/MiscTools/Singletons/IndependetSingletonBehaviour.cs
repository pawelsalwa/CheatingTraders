using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AWI {
	 [ExecuteInEditMode]
	 public class IndependetSingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour {
		  protected static T _instance;
		  public static bool created = false;
		  void Awake() {
				if (created && _instance != null) {
					 DestroyImmediate(this.gameObject);
					 return;
				} else {
					 _instance = this as T;
					 created = true;
				}
				if (Application.isPlaying) {
					 DontDestroyOnLoad(this);
				}
				AfterCreateInstance();
		  }

		  protected virtual void AfterCreateInstance() {
		  }

		  public static T instance {
				set { _instance = value; }
				get {
					 if (_instance == null) {
						  _instance = (T)FindObjectOfType(typeof(T));
					 }
					 if (_instance == null) {
						  GameObject newInstanceObject = new GameObject("_instance");
						  T newInstance = newInstanceObject.AddComponent<T>();
						  _instance = newInstance;
					 }
					 return _instance;
				}
		  }
	 }
}