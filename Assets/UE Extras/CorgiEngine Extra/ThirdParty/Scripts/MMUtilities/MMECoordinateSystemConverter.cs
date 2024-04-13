using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools
{
    public class MMECoordinateSystemConverter
    {
        public static Vector2 WorldPointToScreenPoint(Vector3 worldPoint)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPoint);
            return screenPoint;
        }
        public static Vector2 WorldPointToUILocalPoint(Vector2 worldPoint, Camera uiCamera, RectTransform parentRT)
        {
            Vector2 screenPoint = WorldPointToScreenPoint(worldPoint);
            Vector3 uiPoint = ScreenPointToUILocalPoint(screenPoint, uiCamera, parentRT);
            return uiPoint;
        }
        public static Vector2 WorldPointToUIPoint(Vector2 worldPoint, Camera uiCamera, RectTransform rt)
        {
            Vector3 screenPos = WorldPointToScreenPoint(worldPoint);
            Vector3 uiPoint = ScreenPointToUIPoint(screenPos, uiCamera, rt);
            Debug.Log(uiPoint);
            return uiPoint;
        }
        public static Vector3 ScreenPointToWorldPoint(Vector2 screenPoint, float planeZ)
        {
            Vector3 position = new Vector3(screenPoint.x, screenPoint.y, planeZ);
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(position);
            return worldPoint;
        }
        public static Vector2 UIPointToScreenPoint(Vector3 worldPoint, Camera uiCamera)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, worldPoint);
            return screenPoint;
        }
        public static Vector3 ScreenPointToUIPoint(Vector2 screenPoint, Camera uiCamera, RectTransform rt)
        {
            Vector3 uiPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, screenPoint, uiCamera, out uiPos);
            return uiPos;
        }
        public static Vector3 ScreenPointToUIPoint(Vector2Int screenPoint, Camera uiCamera, RectTransform rt)
        {
            Vector3 uiPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, screenPoint, uiCamera, out uiPos);
            return uiPos;
        }
        public static Vector2 ScreenPointToUILocalPoint(Vector2 screenPoint, Camera uiCamera, RectTransform parentRT)
        {
            Vector2 uiLocalPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRT, screenPoint, uiCamera, out uiLocalPos);
            return uiLocalPos;
        }

        
    }
}
