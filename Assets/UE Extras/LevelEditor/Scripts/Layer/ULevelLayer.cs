using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace Ultra.LevelEditor
{
    public class ULevelLayer : SerializedMonoBehaviour
    {
        public TileBase TileToSet;
        [SerializeField] protected Dictionary<Vector3Int, UTileData> _tileDataDict = new Dictionary<Vector3Int, UTileData>();
        [SerializeField] protected Dictionary<Vector3Int, UTileData> _previewTileDataDict = new Dictionary<Vector3Int, UTileData>();
        public ULayerData LayerData { get => _layerData; }
        protected ULayerData _layerData = new ULayerData();
        protected List<UTileData> _tileDataList = new List<UTileData>();
        protected Tilemap LayerTileMap { get; private set; }
        private void Awake()
        {
            LayerTileMap = GetComponent<Tilemap>();
        }
        public void DrawTilesLine(Vector3Int lineStart, Vector3Int lineEnd, TileBase tile)
        {
            DrawTiles(UShapeGetter.GetLine(lineStart, lineEnd), tile);
        }
        public void DrawTiles(Vector3Int[] poses, TileBase tile)
        {
            SetTiles(poses, tile);
            SaveTiles(poses, tile);
        }
        public void DrawTiles(Vector3Int[] poses, TileBase[] tileBases)
        {
            SetTiles(poses, tileBases);
            SaveTiles(poses, tileBases);
        }
        public void DrawTilesInDict(Vector3Int[] poses)
        {
            for (int i = 0; i < poses.Length; i++)
            {
                if (_tileDataDict.ContainsKey(poses[i]))
                {
                    LayerTileMap.SetTile(poses[i], _tileDataDict[poses[i]].TileBase);
                }
            }
        }
        public void DrawTilesPreview(Vector3Int[] poses, TileBase tile)
        {
            SetTiles(poses, tile);
            SavePreviewTiles(poses, tile);
        }
        public void DrawTilesPreview(Vector3Int[] poses, TileBase[] tileBases)
        {
            SetTiles(poses, tileBases);
            SavePreviewTiles(poses, tileBases);
        }

        public void EraseTilePreview(Vector3Int pos)
        {
            LayerTileMap.SetTile(pos, null);
        }
        public void EraseTilesPreview(Vector3Int[] poses)
        {
            for (int i = 0; i < poses.Length; i++)
            {
                EraseTilePreview(poses[i]);
            }
        }
        public void ErasePreviewTilesNotInTileDataDict(Vector3Int[] poses)
        {
            for (int i = 0; i < poses.Length; i++)
            {
                if (!_tileDataDict.ContainsKey(poses[i]))
                {
                    EraseTilePreview(poses[i]);
                }
            }
            RemovePreviewTiles(poses);
        }
        public void EraseTiles(Vector3Int[] poses)
        {
            EraseTilesPreview(poses);
            RemoveTiles(poses);
        }

        public void ClearTiles(Vector3Int[] cellPoses)
        {
            RemoveTiles(cellPoses);
            LayerTileMap.SetTiles(cellPoses, null);
        }
        public void ClearAllTiles()
        {
            RemoveAllTiles();
            LayerTileMap.ClearAllTiles();
        }
        public void ClearAllPreviewTiles()
        {
            Vector3Int[] _savedPreviewTilePoses = GetAllPreviewTilePoses();
            RemovePreviewTiles(_savedPreviewTilePoses);
            ClearAllDrawnPreviewTiles(_savedPreviewTilePoses);
        }
        private void ClearAllDrawnPreviewTiles(Vector3Int[] previewTilePoses)
        {
            for (int i = 0; i < previewTilePoses.Length; i++)
            {
                if (!_tileDataDict.ContainsKey(previewTilePoses[i]))
                {
                    LayerTileMap.SetTile(previewTilePoses[i], null);
                }
            }
        }

        public Vector3Int[] GetTilePoses(Vector3Int[] cellPoses)
        {
            List<Vector3Int> tilePoses = new List<Vector3Int>();
            for (int i = 0; i < cellPoses.Length; i++)
            {
                if (_tileDataDict.ContainsKey(cellPoses[i]))
                {
                    tilePoses.Add(cellPoses[i]);
                }
            }
            return tilePoses.ToArray();
        }
        public TileBase[] GetTileBases(Vector3Int[] cellPoses)
        {
            TileBase[] tileBases = new TileBase[cellPoses.Length];
            for (int i = 0; i < cellPoses.Length; i++)
            {
                if (_tileDataDict.ContainsKey(cellPoses[i]))
                {
                    tileBases[i] = _tileDataDict[cellPoses[i]].TileBase;
                }
            }
            return tileBases;
        }
        public TileBase[] GetPreviewTileBases(Vector3Int[] cellPoses)
        {
            TileBase[] tileBases = new TileBase[cellPoses.Length];
            for (int i = 0; i < cellPoses.Length; i++)
            {
                if (_previewTileDataDict.ContainsKey(cellPoses[i]))
                {
                    tileBases[i] = _previewTileDataDict[cellPoses[i]].TileBase;
                }
            }
            return tileBases;
        }
        public Vector3Int[] GetPreviewTilePoses(Vector3Int[] cellPoses)
        {
            List<Vector3Int> tilePoses = new List<Vector3Int>();
            for (int i = 0; i < cellPoses.Length; i++)
            {
                if (_previewTileDataDict.ContainsKey(cellPoses[i]))
                {
                    tilePoses.Add(cellPoses[i]);
                }
            }
            return tilePoses.ToArray();
        }
        private Vector3Int[] GetAllPreviewTilePoses()
        {
            if (_previewTileDataDict != null)
            {
                return _previewTileDataDict.Keys.ToArray();
            }
            return null;
        }

        private void SetTiles(Vector3Int[] poses, TileBase tileToSet)
        {
            if (poses != null && poses.Length > 0)
            {
                TileBase[] tileBases = new TileBase[poses.Length];
                for (int i = 0; i < tileBases.Length; i++)
                {
                    tileBases[i] = tileToSet;
                }
                LayerTileMap.SetTiles(poses, tileBases);
            }
        }
        private void SetTiles(Vector3Int[] poses, TileBase[] tileBasesToSet)
        {
            if (poses != null && poses.Length > 0)
            {
                LayerTileMap.SetTiles(poses, tileBasesToSet);
            }
        }
        private void SaveTiles(Vector3Int[] poses, TileBase tileToSave)
        {
            for (int i = 0; i < poses.Length; i++)
            {
                if (!_tileDataDict.ContainsKey(poses[i]))
                {
                    _tileDataDict.Add(poses[i], new UTileData(poses[i], tileToSave));
                }
                else
                {
                    _tileDataDict[poses[i]] = new UTileData(poses[i], tileToSave);
                }
            }
        }
        private void SaveTiles(Vector3Int[] poses, TileBase[] tileBasesToSave)
        {
            for (int i = 0; i < poses.Length; i++)
            {
                if (!_tileDataDict.ContainsKey(poses[i]))
                {
                    _tileDataDict.Add(poses[i], new UTileData(poses[i], tileBasesToSave[i]));
                }
                else
                {
                    _tileDataDict[poses[i]] = new UTileData(poses[i], tileBasesToSave[i]);
                }
            }
        }
        private void SavePreviewTiles(Vector3Int[] previewTilePoses, TileBase previewTileBase)
        {
            for (int i = 0; i < previewTilePoses.Length; i++)
            {
                if (!_previewTileDataDict.ContainsKey(previewTilePoses[i]))
                {
                    _previewTileDataDict.Add(previewTilePoses[i], new UTileData(previewTilePoses[i], previewTileBase));
                }
                else
                {
                    _previewTileDataDict[previewTilePoses[i]] = new UTileData(previewTilePoses[i], previewTileBase);
                }
            }
        }
        private void SavePreviewTiles(Vector3Int[] previewTilePoses, TileBase[] previewTileBases)
        {
            for (int i = 0; i < previewTilePoses.Length; i++)
            {
                if (!_previewTileDataDict.ContainsKey(previewTilePoses[i]))
                {
                    _previewTileDataDict.Add(previewTilePoses[i], new UTileData(previewTilePoses[i], previewTileBases[i]));
                }
                else
                {
                    _previewTileDataDict[previewTilePoses[i]] = new UTileData(previewTilePoses[i], previewTileBases[i]);
                }
            }
        }



        private void RemoveTiles(Vector3Int[] poses)
        {
            for (int i = 0; i < poses.Length; i++)
            {
                if (_tileDataDict.ContainsKey(poses[i]))
                {
                    _tileDataDict.Remove(poses[i]);
                }
            }
        }
        private void RemovePreviewTiles(Vector3Int[] previewTilePoses)
        {
            for (int i = 0; i < previewTilePoses.Length; i++)
            {
                if (_previewTileDataDict.ContainsKey(previewTilePoses[i]))
                {
                    _previewTileDataDict.Remove(previewTilePoses[i]);
                }
                
            }
        }
        private void RemoveAllTiles()
        {
            _tileDataDict.Clear();
        }
    }
}
