using System;
using System.Collections.Generic;
using UnityEngine;

namespace AWI {
	 public static class TransformUtilities {

		  public static Transform FindChildInHierarhy(this Transform transform, string name) {
				Transform result = transform.Find(name);
				for (int i = 0, count = transform.childCount; i < count && result == null; ++i) {
					 result = transform.GetChild(i).FindChildInHierarhy(name);
				}
				return result;
		  }

		  public static List<T> FindShallowChildren<T>(Transform transform) where T : Component {
				var result = new List<T>();
				var childCount = transform.childCount;
				for (int i = 0; i < childCount; ++i) {
					 var children = transform.GetChild(i).GetComponents<T>();
					 if (children != null) {
						  result.AddRange(children);
					 }
				}
				return result;
		  }

		  public static List<Component> FindShallowChildren(Type type, Transform transform) {
				var result = new List<Component>();
				var childCount = transform.childCount;
				for (int i = 0; i < childCount; ++i) {
					 var children = transform.GetChild(i).GetComponents(type);
					 if (children != null) {
						  result.AddRange(children);
					 }
				}
				return result;
		  }

		  public static T FindParent<T>(Transform transform) where T : Component {
				if (transform == null) {
					 return null;
				}
				return FindParentIncludingSelf<T>(transform.parent);
		  }

		  public static Component FindParent(Type type, Transform transform) {
				if (transform == null) {
					 return null;
				}
				return FindParentIncludingSelf(type, transform.parent);
		  }

		  private static T FindParentIncludingSelf<T>(Transform transform) where T : Component {
				if (transform == null) {
					 return null;
				} else {
					 var component = transform.GetComponent<T>();
					 if (component != null) {
						  return component;
					 } else {
						  return FindParentIncludingSelf<T>(transform.parent);
					 }
				}
		  }

		  private static Component FindParentIncludingSelf(Type type, Transform transform) {
				if (transform == null) {
					 return null;
				} else {
					 var component = transform.GetComponent(type);
					 if (component != null) {
						  return component;
					 } else {
						  return FindParentIncludingSelf(type, transform.parent);
					 }
				}
		  }
	 }
}