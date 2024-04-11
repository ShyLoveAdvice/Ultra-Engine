using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Ultra.LevelEditor
{
    public class UScaleDraggerManager : MonoBehaviour
    {
        //public Vector3Int CurrentCellPos;
        //[HideInInspector] public ULevelEditorGUIManager GUIManager;
        //[HideInInspector] public USelection Selection;
        //[HideInInspector] public RectTransform RectTransform;
        //[HideInInspector] public ULevelEditorInputManager InputManager;
        //private USelection.ScaleDraggersData _currentScaleDraggerData;
        //public enum ScaleDraggerDirections { LeftBottom, LeftTop, RightBottom, RightTop }
        //private Dictionary<ScaleDraggerDirections, UScaleDragger> _directionalScaleDraggerDict = new Dictionary<ScaleDraggerDirections, UScaleDragger>();
        //private UScaleDragger[] _scaleDraggers;
        //private bool _scaleDraggersTurnedOn;
        //private ScaleDraggerDirections _oppositeDir;
        //public void InitializeScaleDraggerManager(ULevelEditorGUIManager GUIManager)
        //{
        //    this.GUIManager = GUIManager;
        //    Selection = FindObjectOfType<USelection>();
        //    RectTransform = GetComponent<RectTransform>();
        //    _scaleDraggers = GetComponentsInChildren<UScaleDragger>();

        //    for (int i = 0; i < _scaleDraggers.Length; i++)
        //    {
        //        _scaleDraggers[i].InitializeScaleDragger(this);
        //    }

        //    TurnOffScaleDraggers();
        //}
        //public void UpdateScaleDraggerManager()
        //{
        //    if(_scaleDraggersTurnedOn)
        //    {
        //        if(_directionalScaleDraggerDict != null && _directionalScaleDraggerDict.Count == Enum.GetNames(typeof(ScaleDraggerDirections)).Length)
        //        {
        //            for (int i = 0; i < _directionalScaleDraggerDict.Count; i++)
        //            {
        //                _directionalScaleDraggerDict[(ScaleDraggerDirections)i].UpdateScaleDragger(GetWorldPosAtDirection(_currentScaleDraggerData, (ScaleDraggerDirections)i), (ScaleDraggerDirections)i);
        //            }
        //        }
        //    }
        //}
        //public void TurnOnScaleDraggers()
        //{
        //    foreach (var scaleDragger in _scaleDraggers)
        //    {
        //        scaleDragger.gameObject.SetActive(true);
        //    }

        //    _scaleDraggersTurnedOn = true;
        //}
        //public void TurnOffScaleDraggers()
        //{
        //    foreach (var scaleDragger in _scaleDraggers)
        //    {
        //        scaleDragger.gameObject.SetActive(false);
        //    }

        //    _scaleDraggersTurnedOn = false;
        //}
        //public void SetScaleDraggersWorldPositions(USelection.ScaleDraggersData scaleDraggersData)
        //{
        //    _currentScaleDraggerData = scaleDraggersData;

        //    if (_scaleDraggersTurnedOn)
        //    {
        //        _directionalScaleDraggerDict.Clear();

        //        for (int i = 0; i < _scaleDraggers.Length; i++)
        //        {
        //            _directionalScaleDraggerDict.Add((ScaleDraggerDirections)i, _scaleDraggers[i]);
        //        }
        //    }
        //}
        //public void SetOppositeScaleDragger(ScaleDraggerDirections dir)
        //{
        //    ScaleDraggerDirections oppositeDirection = ScaleDraggerDirections.RightTop;

        //    switch (dir)
        //    {
        //        case ScaleDraggerDirections.LeftBottom: oppositeDirection = ScaleDraggerDirections.RightTop; break;
        //        case ScaleDraggerDirections.LeftTop: oppositeDirection = ScaleDraggerDirections.RightBottom; break;
        //        case ScaleDraggerDirections.RightBottom: oppositeDirection = ScaleDraggerDirections.LeftTop; break;
        //        case ScaleDraggerDirections.RightTop: oppositeDirection = ScaleDraggerDirections.LeftBottom; break;
        //    }
        //}
        //public void DragScaleDragger(ScaleDraggerDirections dir)
        //{

        //}
        //private Vector3 GetWorldPosAtDirection(USelection.ScaleDraggersData scaleDraggersData, ScaleDraggerDirections dir)
        //{
        //    switch(dir)
        //    {
        //        case ScaleDraggerDirections.LeftBottom: return scaleDraggersData.leftBottom; break;
        //        case ScaleDraggerDirections.LeftTop: return scaleDraggersData.leftTop; break;
        //        case ScaleDraggerDirections.RightBottom: return scaleDraggersData.rightBottom; break;
        //        case ScaleDraggerDirections.RightTop: return scaleDraggersData.rightTop; break;
        //    }

        //    return Vector3.zero;
        //}
    }
}
