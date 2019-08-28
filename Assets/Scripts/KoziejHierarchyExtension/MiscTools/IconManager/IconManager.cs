using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AWI
{
#if UNITY_EDITOR
	 public static class IconTextureCache {
		  private static readonly string[] extensions = { ".png", ".psd" };
		  private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

		  private static List<string> s_GizmoPaths;
		  private static List<string> gizmoPaths {
				get {
					 if(s_GizmoPaths == null) {
						  FindGizmos();
					 }
					 return s_GizmoPaths;
				}
		  }

		  private static Texture2D GetAtFinal(string path) {
				var result = null as Texture2D;
				if (path.Contains(".")) {
					 result = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
				} else for(int i = 0; i < extensions.Length; ++i) {
						  result = (Texture2D)AssetDatabase.LoadAssetAtPath(path + extensions[i], typeof(Texture2D));
						  if (result != null) {
								break;
						  }
				}
				return result;
		  }

		  public static Texture2D Get(string name) {
				var result = null as Texture2D;
				var pathedName = "";
				if (!textures.TryGetValue(name, out result)) {
					 if (name.Contains("Assets/")) {
						  pathedName = name;
						  result = GetAtFinal(pathedName);
					 } else {
						  var prefixedName = name;
						  if (!prefixedName.StartsWith("ET_")) {
								prefixedName = "ET_" + prefixedName;
						  }
						  foreach(var gizmoPath in gizmoPaths) {
								pathedName = gizmoPath + prefixedName;
								result = GetAtFinal(pathedName);
								if(result != null) {
									 break;
								}
						  }
					 }
					 if(result != null) {
						  textures.Add(name, result);
					 } else {
						  //Debug.Log("cant find: " + name);
					 }
				}
				return result;
		  }

		  private static void FindGizmos() {
				s_GizmoPaths = new List<string>();
				var appPath = Application.dataPath;
				FindGizmoFolders(appPath);
				for (int i = 0; i < gizmoPaths.Count; ++i) {
					 gizmoPaths[i] = "Assets" + s_GizmoPaths[i].Substring(appPath.Length);
					 gizmoPaths[i] = s_GizmoPaths[i].Replace('\\', '/');
				}
		  }
		  
		  private static void FindGizmoFolders(string path, int depth = 0) {
				if (depth > 8) {
					 return;
				}
				string[] dir = System.IO.Directory.GetDirectories(path);
				foreach (var directory in dir) {
					 if (directory.EndsWith("Gizmos")) {
						  gizmoPaths.Add(directory + "/");
					 } else if (directory.EndsWith("Gizmos/")) {
						  gizmoPaths.Add(directory);
					 } else {
						  FindGizmoFolders(directory, depth + 1);
					 }
				}
		  }

		  public static void Reset() {
				textures.Clear();
				s_GizmoPaths = null;
				Init();
		  }

		  public static void Init() {
				if(s_GizmoPaths == null) {
					 FindGizmos();
				}
		  }
		  
	 };
#endif
}// end of namespace
