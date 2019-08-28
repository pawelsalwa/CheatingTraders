using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace AWI
{

    [InitializeOnLoad]
    public static class GlobalTimerSceneTools
    {
        static GlobalTimerSceneTools()
        {
            var buttonContent = new GUIContent(IconTextureCache.Get("GlobalTimer"));
            buttonContent.text = "GT";
            Action onDraw = () =>
            {
                GUI.contentColor = GlobalTimer.updateEnabled ? Color.white : Color.red;
                var focus = SceneViewTools.Layout.Button(buttonContent);
                GUI.contentColor = Color.white;
                if (focus)
                {
						  var genericMenu = new GenericMenu();
						  genericMenu.AddItem(new GUIContent("Show Manager"), false, () => { GlobalTimerEditor.Init(); });
						  genericMenu.AddSeparator("");
						  genericMenu.AddItem(new GUIContent("Enabled"), GlobalTimer.updateEnabled, () => { GlobalTimer.updateEnabled = !GlobalTimer.updateEnabled; });
						  genericMenu.ShowAsContext();
					 }
            };
            SceneViewTools.Controller.topBarDrawers.AddListener(onDraw, 10);
        }
    }
    
    public class GlobalTimerEditor : EditorWindow
    {
        protected Vector2 scrollPosition;
        private int selectedToolbarOption = 1;
        private string[] options = new string[] { "high", "normal", "low", "slow", "!play", "tweens" };

		  public struct MethodTickMs {
				public int id;
				public double ms;
				public double ticks;
		  }

        Dictionary<string, MethodTickMs> dic = new Dictionary<string, MethodTickMs>();

		  public static void Init()
        {
            var gt = EditorWindow.GetWindow(typeof(GlobalTimerEditor), true, "global timer") as GlobalTimerEditor;
            gt.Show();
            gt.scrollPosition = Vector2.zero;
            GlobalTimer.debug = true;
        }

        private void OnDestroy()
        {
            GlobalTimer.debug = false;
        }

        public void DrawUpdaterOverview(GlobalTimer.Updater updater, string name)
        {
				GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label(name, EditorStyles.centeredGreyMiniLabel);
            GUIAddons.Layout.InfoLabel("actions count:", updater.count.ToString("00"));
            GUILayout.Space(-8);
            GUIAddons.Layout.InfoLabel("time ms:", updater.update_interval_ms.ToString("00.0000"));
				GUIAddons.Layout.InfoLabel("ticks:", updater.update_interval_ticks.ToString());
				GUILayout.EndVertical();
		  }

        public void DrawDetailedUpdaterOverview(GlobalTimer.Updater updater, Dictionary<string, MethodTickMs> dictionary)
        {
            dictionary.Clear();
            for (int i = 0; i < updater.count; ++i)
            {
                var method = updater.action_list[i].GetInvocationList()[0].Method;
                string name = method.DeclaringType.ToString() + "." + method.Name + "()";
					 if (!dictionary.ContainsKey(name)) {
						  dictionary.Add(name, new MethodTickMs() { id = 1, ms = updater.update_intervals_ms[i], ticks = updater.update_intervals_ticks[i] });
                }
                else {
						  var id = dictionary[name].id + 1;
						  var ms = dictionary[name].ms + updater.update_intervals_ms[i];
						  var ticks = dictionary[name].ticks + updater.update_intervals_ticks[i];
						  dictionary[name] = new MethodTickMs() { id = id, ms = ms, ticks = ticks };
                }
            }
            foreach (var key in dictionary.Keys)
            {
                float fps = (float)(1.0f / (dictionary[key].ms / 1000));
                GUILayout.Label("count: " + dictionary[key].id.ToString("00") 
						  + " time ms: " + dictionary[key].ms.ToString("000.0000")
						  + " ticks: " + dictionary[key].ticks.ToString()
						  + " fps: " + fps.ToString("00000"), EditorStyles.miniLabel);
                GUILayout.Space(-7);
                GUILayout.Label(key, EditorStyles.miniLabel);
                GUILayout.Space(2);
            }
        }
        
        void OnGUI()
        {
            this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, false, true);
            var guiEnabled = GUI.enabled;

            GUILayout.Space(5);
            GUIAddons.Layout.InfoLabel("instances count:", GlobalTimer.instanceCount);

            GUILayout.Space(5);

				GUILayout.BeginVertical(EditorStyles.helpBox);
				GUILayout.Label("timming", EditorStyles.centeredGreyMiniLabel);

            GUIAddons.Layout.InfoLabel("fps", GlobalTimer.fps.ToString("0000.00"));
            GUILayout.Space(-8);
            GUIAddons.Layout.InfoLabel("time since startup", GlobalTimer.timeSinceStartup.ToString("0000000.00"));
            GUILayout.Space(-8);
            GUIAddons.Layout.InfoLabel("real " + SpecChars.delta + "t", GlobalTimer.realDeltaTime.ToString("00.0000"));
            GUILayout.Space(-8);
            GUIAddons.Layout.InfoLabel("real smooth " + SpecChars.delta + "t", GlobalTimer.smoothUnscaledDeltaTime.ToString("00.0000"));
				GUILayout.EndVertical();

            DrawUpdaterOverview(GlobalTimer.InspectiorGetters.update_high, "high");
            DrawUpdaterOverview(GlobalTimer.InspectiorGetters.update, "normal");
            DrawUpdaterOverview(GlobalTimer.InspectiorGetters.update_low, "low");
            DrawUpdaterOverview(GlobalTimer.InspectiorGetters.update_slow, "slow");
            DrawUpdaterOverview(GlobalTimer.InspectiorGetters.update_onApplicationNotPlaying, "!play (if app not playing)");
            DrawUpdaterOverview(GlobalTimer.InspectiorGetters.tweensUpdate, "tweens");

            GUILayout.Space(5);

            using (new GUILayout.VerticalScope())
            {
                selectedToolbarOption = GUILayout.Toolbar(selectedToolbarOption, options, EditorStyles.miniButton);
                GUILayout.Space(5);
                switch (selectedToolbarOption)
                {
                    case 0:
                        DrawDetailedUpdaterOverview(GlobalTimer.InspectiorGetters.update_high, dic);
                        break;
                    case 1:
                        DrawDetailedUpdaterOverview(GlobalTimer.InspectiorGetters.update, dic);
                        break;
                    case 2:
                        DrawDetailedUpdaterOverview(GlobalTimer.InspectiorGetters.update_low, dic);
                        break;
                    case 3:
                        DrawDetailedUpdaterOverview(GlobalTimer.InspectiorGetters.update_slow, dic);
                        break;
                    case 4:
                        DrawDetailedUpdaterOverview(GlobalTimer.InspectiorGetters.update_onApplicationNotPlaying, dic);
                        break;
                    case 5:
                        DrawDetailedUpdaterOverview(GlobalTimer.InspectiorGetters.tweensUpdate, dic);
                        break;
                }
                GUILayout.FlexibleSpace();
            }

            GUILayout.EndScrollView();
            Repaint();
        }
    }
}