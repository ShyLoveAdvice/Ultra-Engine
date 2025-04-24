using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.LevelEditor
{
    public class UPrefabPlaceTool : ULevelEditorTool
    {
        public GameObject prefabBoundingBoxPrefab;
        private LineRenderer _boundingBoxLr;

        private Vector3Int _startCellPos;
        
        protected override void OnSelected()
        {
            Debug.Log("Selected");
            _startCellPos = CurrentMouseCellPos;
            _boundingBoxLr = GameObject.Instantiate(prefabBoundingBoxPrefab, CurrentMouseCellPos, Quaternion.identity).GetComponent<LineRenderer>();
            
        }

        // Draw bounding box
        protected override void PersistUpdate()
        {
            if (CurrentMouseCellPos != LastCellPos || CurrentMouseCellPos == _startCellPos)
            {
                
            }
        }
    }
}
