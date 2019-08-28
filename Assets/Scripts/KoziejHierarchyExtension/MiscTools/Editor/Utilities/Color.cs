using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AWI
{
    public static class ColorExtensions
    {
        public static Color SpaceSuit(this Color color)
        {
            return SpaceSuitCalc(color);
        }

        public static Color SpaceSuit(this Color32 color)
        {
            return SpaceSuitCalc(color);
        }

        private static Color SpaceSuitCalc(Color color)
        {
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
            {
                return color.gamma;
            }
            else
            {
                return color;
            }
        }
    }

    public static class ColorPallete
    {
        private static Color m_ui = new Color32(30, 144, 255, 255);
        private static Color m_temp = new Color32(100, 50, 5, 255);

        private static Color m_shadow = new Color32(0, 0, 0, 255);
        private static Color m_water = new Color32(0, 100, 255, 255);
        private static Color m_default = new Color32(128, 128, 128, 255);
        private static Color m_ignore_raycast = new Color32(64, 64, 64, 255);
        private static Color m_ignore_raycast_text = new Color32(128, 128, 128, 255);
        private static Color m_transparent_fx = new Color32(120, 0, 100, 255);
        private static Color m_transparent_fx_text = new Color32(220, 20, 120, 255);

        public static Color[] layers = new Color[]
        {
            new Color32   (166,226,195, 255),
            new Color32   (224,245,234, 255),
            new Color32   (226,166,167, 255),
            new Color32   (84,100,173, 255),
            new Color32   (119,141,68, 255),
            new Color32   (255,150,64, 255),
            new Color32   (230,57,155, 255),
        };

		  private static Color[,] s_layersColors;
		  private static string[] s_layerNames;

		  public static void RemapLayersColors() {
				s_layersColors = new Color[32, 2];
				s_layerNames = new string[32];
				string layerName = "";
				for(int i = 0; i < 32; ++i) {
					 layerName = LayerMask.LayerToName(i);
					 s_layerNames[i] = layerName;
					 RemapLayer(i, layerName, ref s_layersColors[i, 0], ref s_layersColors[i, 1]);
				}
		  }

		  public static void RemapSingleLayerColor(string layerName, int layer) {
				s_layerNames[layer] = layerName;
				RemapLayer(layer, layerName, ref s_layersColors[layer, 0], ref s_layersColors[layer, 1]);
		  }

		  public static void LayerStyle(int layer, string layerName, ref Color backgroundColor, ref Color textColor) {
				if(s_layersColors == null) {
					 RemapLayersColors();
				} else {
					 if(s_layerNames[layer] != layerName) {
						  RemapSingleLayerColor(layerName, layer);
					 }
				}
				backgroundColor = s_layersColors[layer, 0];
				textColor = s_layersColors[layer, 1];
		  }

        private static void RemapLayer(int layer, string layerName, ref Color backgroundColor, ref Color textColor)
        {
            layerName = layerName.ToLower();
            if (layerName.Contains("shadow"))
            {
                backgroundColor = m_shadow;
                textColor = Color.white;
            }
            else if (layerName == "temp")
            {
                backgroundColor = m_temp;
                textColor = Color.black;
            }
            else if (layerName == "default")
            {
                backgroundColor = new Color(0,0,0,0);
                textColor = new Color(0.6f, 0.6f, 0.6f, 1);
            }
            else if (layerName == "water")
            {
                backgroundColor = m_water;
                textColor = Color.cyan;
            }
            else if (layerName == "ignore raycast")
            {
                backgroundColor = m_ignore_raycast;
                textColor = m_ignore_raycast_text;
            }
            else if (layerName == "transparentfx")
            {
                backgroundColor = m_transparent_fx;
                textColor = m_transparent_fx_text;
            }
            else if (layerName == "ui")
            {
                backgroundColor = m_ui;
                textColor = Color.white;
            }
            else if(layer < 8)
            {
                backgroundColor = m_default;
                textColor = Color.white;
            }
            else
            {
                backgroundColor = layers[layer % layers.Length];
                textColor = backgroundColor;
                textColor.r = (1 - textColor.r) * 0.2f;
                textColor.g = (1 - textColor.g) * 0.2f;
                textColor.b = (1 - textColor.b) * 0.2f;
                textColor.a = 1;
            }
        }
    }

    public static class ColorUtilities
    {
        public static Color Multiply(Color color, float intensity)
        {
            return new Color(color.r * intensity, color.g * intensity, color.b * intensity, color.a);
        }

        public static Color RandomColor(int i)
        {
            float r = 0.5f * (Mathf.Sin(1.3f + 8 * 3.14f * i / 10.0f) + 1) + 0.2f;
            float g = 0.5f * (Mathf.Sin(0.5f + 5 * 3.14f * i / 10.0f) + 1) + 0.2f;
            float b = 0.5f * (Mathf.Sin(1.0f + 2 * 3.14f * i / 10.0f) + 1) + 0.2f;
            return new Color(r, g, b, 1);
        }
    }

}