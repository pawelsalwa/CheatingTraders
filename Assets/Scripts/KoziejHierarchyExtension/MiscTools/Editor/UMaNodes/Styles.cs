using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AWI;

namespace UMa
{
    public static class Styles
    {
        public static GUIStyle gray_centerBoldMidLabel = CenterBold(6, new Color(0,0,0,0.5f));
		  public static GUIStyle centerBoldMiniLabel = CenterBold(6, Color.black);
		  public static GUIStyle centerBoldMidLabel = CenterBold(8, Color.black);
        public static GUIStyle centerBoldLargeLabel = CenterBold(10, Color.black);
        public static GUIStyle blackField = Bordered("Capsule_Field.psd", new RectOffset(8, 8, 8, 8));
        public static GUIStyle selectionFrame = Bordered("Frame.psd", new RectOffset(4, 4, 4, 4));
		  public static GUIStyle greenSelectionFrame = Bordered("GreenFrame.psd", new RectOffset(4, 4, 4, 4));
		  public static GUIStyle emptyStyle = new GUIStyle();

        private static GUIStyle CenterBold(int size, Color color)
        {
            var style = new GUIStyle(EditorStyles.miniBoldLabel);
            style.fontSize = size;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = color;
            style.contentOffset = Vector2.zero;
            style.wordWrap = true;
            return style;
        }

        private static GUIStyle Bordered(string texture, RectOffset border)
        {
            var style = new GUIStyle(EditorStyles.whiteLabel);
            style.normal.background = IconTextureCache.Get(texture);
            style.normal.textColor = Color.white;
            style.border = border;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 9;
            return style;
        }
    }
}
