using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AWI {
	 public static class MaterialUtilities {
		  public static readonly GUIContent showEditorWindow = new GUIContent("Show Material Editor...");
		  public static readonly GUIContent closeEditorWindow = new GUIContent("Hide Material Editor");

		  private static MethodInfo shaderPopup = typeof(MaterialEditor).GetMethod("OnHeaderControlsGUI", BindingFlags.NonPublic | BindingFlags.Instance);
		  private static FieldInfo m_IsVisible = typeof(MaterialEditor).GetField("m_IsVisible", BindingFlags.NonPublic | BindingFlags.Instance);

		  private static Dictionary<Material, MaterialEditor> materialEditors = new Dictionary<Material, MaterialEditor>();

		  //private static Dictionary<Material, Texture> previewsTorus64 = new Dictionary<Material, Texture>();
		  //private static Dictionary<Material, Texture> previewsTorus32 = new Dictionary<Material, Texture>();

		  //private static Dictionary<Material, Texture> previesSphere64 = new Dictionary<Material, Texture>();
		  //private static Dictionary<Material, Texture> previesSphere32 = new Dictionary<Material, Texture>();

		  private static Mesh m_torusMesh;
		  private static Mesh m_sphereMesh;

		  public static Mesh torusMesh {
				get {
					 return m_torusMesh = m_torusMesh == null ? GetMaterialMesh("torus") : m_torusMesh;
				}
		  }

		  public static Mesh sphereMesh {
				get {
					 return m_sphereMesh = m_sphereMesh == null ? GetMaterialMesh("sphere") : m_sphereMesh;
				}
		  }

		  private static Mesh GetMaterialMesh(string name) {
				GameObject gameObject = (GameObject)EditorGUIUtility.LoadRequired("Previews/PreviewMaterials.fbx");
				gameObject.SetActive(false);
				foreach (Transform transform in gameObject.transform) {
					 if (transform.name == name)
						  return transform.GetComponent<MeshFilter>().sharedMesh;
				}
				return (Mesh)null;
		  }

		  public static MaterialEditor GetMaterialEditor(Material material) {
				var result = null as MaterialEditor;
				if (!materialEditors.TryGetValue(material, out result)) {
					 materialEditors.Add(material, result = MaterialEditor.CreateEditor(material, typeof(MaterialEditor)) as MaterialEditor);
				}
				return result;
		  }

		  public static void ShaderPopup(Editor editor) {
				shaderPopup.Invoke(editor, null);
		  }

		  public static void SetForceVisible(Editor editor, bool value) {
				m_IsVisible.SetValue(editor, true);
		  }

		  public static void DrawEditor(MaterialEditor materialEditor) {
				if (materialEditor != null) {
					 ShaderPopup(materialEditor);
					 SetForceVisible(materialEditor, true);
					 materialEditor.OnInspectorGUI();
				} else {
					 GUILayout.Label("Nothing to edit", EditorStyles.centeredGreyMiniLabel);
				}
		  }

		  public static bool DrawEditorWithOpenOption(MaterialEditor materialEditor, bool open) {
				GUIContent content = (open) ? closeEditorWindow : showEditorWindow;
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button(content, EditorStyles.miniButton, new GUILayoutOption[] { GUILayout.Width(150f) })) 
				{
					 open = !open;
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
				if (open) {
					 using (new GUILayout.VerticalScope(EditorStyles.helpBox)) {
						  if (materialEditor != null) {
								ShaderPopup(materialEditor);
								SetForceVisible(materialEditor, true);
								materialEditor.OnInspectorGUI();
						  } else {
								GUILayout.Label("Nothing to edit", EditorStyles.centeredGreyMiniLabel);
						  }
					 }
				}
				return open;
		  }
	 };
}// end of namespace

