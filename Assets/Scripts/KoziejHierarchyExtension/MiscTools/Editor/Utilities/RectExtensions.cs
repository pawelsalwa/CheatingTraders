using UnityEngine;
using UnityEditor;

namespace AWI
{
	 /// <summary>
	 /// Extension for creating new rects
	 /// </summary>
    public static class RectExtensions
    {
		  /// <summary>
		  /// Return new adding border
		  /// </summary>
		  /// <param name="source"></param>
		  /// <param name="left"></param>
		  /// <param name="right"></param>
		  /// <param name="top"></param>
		  /// <param name="bottom"></param>
		  /// <returns></returns>
        public static Rect Add(this Rect source, int left, int right, int top, int bottom)
        {
				var result = new Rect(source);
				result.x -= left;
				result.width += left + right;
				result.y -= top;
				result.height += top + bottom;
				return result;
        }

		  /// <summary>
		  /// return new rect, same size but right to the old one
		  /// </summary>
		  /// <param name="rect"></param>
		  /// <returns></returns>
        public static Rect NextRight(this Rect rect)
        {
            rect.x += rect.width;
            return rect;
        }

		  /// <summary>
		  /// new rect with new x
		  /// </summary>
		  /// <param name="rect"></param>
		  /// <param name="x"></param>
		  /// <returns></returns>
        public static Rect CopyX(this Rect rect, int x)
        {
            rect.x = x;
            return rect;
        }

		  /// <summary>
		  /// new rect with new y
		  /// </summary>
		  /// <param name="rect"></param>
		  /// <param name="y"></param>
		  /// <returns></returns>
        public static Rect CopyY(this Rect rect, int y)
        {
            rect.y = y;
            return rect;
        }

		  /// <summary>
		  /// new rect with new width
		  /// </summary>
		  /// <param name="rect"></param>
		  /// <param name="width"></param>
		  /// <returns></returns>
        public static Rect CopyWidth(this Rect rect, float width)
        {
            rect.width = width;
            return rect;
        }

		  /// <summary>
		  /// new rect with new height
		  /// </summary>
		  /// <param name="rect"></param>
		  /// <param name="height"></param>
		  /// <returns></returns>
        public static Rect CopyHeight(this Rect rect, float height)
        {
            rect.height = height;
            return rect;
        }

    }

}

