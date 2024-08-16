using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using System.Collections;


namespace Ultra.LevelEditor
{
    public class ULevelLayer : SerializedMonoBehaviour
    {
        protected ULevelEditor LevelEditor { get; private set; }
        [SerializeField] protected Dictionary<Vector3Int, UTileData> _tileDataDict = new Dictionary<Vector3Int, UTileData>();
        public ULayerData LayerData { get => _layerData; }
        protected ULayerData _layerData;
        protected Tilemap LayerTileMap { get; private set; }
        public void InitializeLevelLayer(ULevelEditor levelEditor)
        {
            LevelEditor = levelEditor;

            _layerData = new ULayerData();

            for (int x = 0; x <= ULevelEditor.Instance.LevelSize.x; x++)
            {
                for (int y = 0; y <= ULevelEditor.Instance.LevelSize.y; y++)
                {
                    _tileDataDict.Add(levelEditor.LevelStartPos + new Vector3Int(x, y), new UTileData());
                }
            }

            LayerTileMap = GetComponent<Tilemap>();
        }
        public void SaveLayer()
        {
            UTileDataSave[] saveTiles = new UTileDataSave[_tileDataDict.Count];
            int index = 0;
            foreach (var pair in _tileDataDict)
            {
                saveTiles[index] = new UTileDataSave(pair.Key, pair.Value.TileBase);
                index++;
            }
            _layerData = new ULayerData(LayerTypes.Tile, LevelEditor.GridDrawerData.CellSize, saveTiles);
        }

        public void DrawTilesLine(Vector3Int lineStart, Vector3Int lineEnd, TileBase tile)
        {
            DrawTiles(UShapeGetter.GetLine(lineStart, lineEnd), tile);
        }
        public void DrawTile(Vector3Int pos, UTileData tileData)
        {
            SetTile(pos, tileData);
            SaveTile(pos, tileData);
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
        public void DrawTiles(Vector3Int[] poses, UTileData[] tileDatas)
        {
            SetTiles(poses, tileDatas);
            SaveTiles(poses, tileDatas);
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

        public Vector3Int[] GetFloodFillPoses(Vector3Int startPos)
        {
            UTileData originalTileData = GetTileData(startPos);
            Queue<Vector3Int> poses = new Queue<Vector3Int>();
            HashSet<Vector3Int> result = new HashSet<Vector3Int>();
            poses.Enqueue(startPos);
            int count = 0;
            while (poses.Count > 0 && count < 30000)
            {
                Vector3Int pos = poses.Dequeue();
                count++;
                if (pos.x < LevelEditor.LevelStartPos.x || pos.x > LevelEditor.LevelEndPos.x
                || pos.y < LevelEditor.LevelStartPos.y || pos.y > LevelEditor.LevelEndPos.y
                || result.Contains(pos)
                || _tileDataDict[pos] != originalTileData)
                {
                    continue;
                }
                else
                {
                    result.Add(pos);
                    poses.Enqueue(pos + Vector3Int.down);
                    poses.Enqueue(pos + Vector3Int.right);
                    poses.Enqueue(pos + Vector3Int.up);
                    poses.Enqueue(pos + Vector3Int.left);
                }
            }

            Vector3Int[] arrayResult = new Vector3Int[result.Count];
            result.CopyTo(arrayResult);

            return arrayResult;
        }
        public void FloodFill(Vector3Int startPos, UTileData newTileData)
        {
            UTileData originalTileData = GetTileData(startPos);

            if (originalTileData == newTileData)
            {
                return;
            }
            Queue<Vector3Int> poses = new Queue<Vector3Int>();
            poses.Enqueue(startPos);
            while(poses.Count > 0)
            {
                Vector3Int pos = poses.Dequeue();
                if (pos.x < LevelEditor.LevelStartPos.x || pos.x > LevelEditor.LevelEndPos.x
                || pos.y < LevelEditor.LevelStartPos.y || pos.y > LevelEditor.LevelEndPos.y
                || _tileDataDict[pos] != originalTileData)
                {
                    continue;
                }
                else
                {
                    DrawTile(pos, newTileData);
                    poses.Enqueue(pos + Vector3Int.down);
                    poses.Enqueue(pos + Vector3Int.right);
                    poses.Enqueue(pos + Vector3Int.up);
                    poses.Enqueue(pos + Vector3Int.left);
                }
            }
        }
        public void FloodFill(Vector3Int startPos, UTileData newTileData, USelection selection)
        {
            UTileData originalTileData = GetTileData(startPos);

            if (originalTileData == newTileData)
            {
                return;
            }

            Queue<Vector3Int> poses = new Queue<Vector3Int>();
            poses.Enqueue(startPos);

            while (poses.Count > 0)
            {
                Vector3Int pos = poses.Dequeue();
                if (pos.x < LevelEditor.LevelStartPos.x || pos.x > LevelEditor.LevelEndPos.x
                || pos.y < LevelEditor.LevelStartPos.y || pos.y > LevelEditor.LevelEndPos.y
                || _tileDataDict[pos] != originalTileData
                || !selection.Contains(pos))
                {
                    continue;
                }
                else
                {
                    DrawTile(pos, newTileData);
                    poses.Enqueue(pos + Vector3Int.down);
                    poses.Enqueue(pos + Vector3Int.right);
                    poses.Enqueue(pos + Vector3Int.up);
                    poses.Enqueue(pos + Vector3Int.left);
                }
            }
        }

        public void EraseTile(Vector3Int pos)
        {
            LayerTileMap.SetTile(pos, null);
            RemoveTile(pos);
        }
        public void EraseTiles(Vector3Int[] poses)
        {
            foreach (var pos in poses)
            {
                EraseTile(pos);
            }
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

        public Vector3Int? GetTilePos(Vector3Int cellPos)
        {
            if (_tileDataDict.ContainsKey(cellPos))
            {
                return cellPos;
            }
            return null;
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
        public Vector3Int[] GetTilePoses(Vector3Int[] poses, UTileData[] tileDatas)
        {
            List<Vector3Int> tilePoses = new List<Vector3Int>();
            for (int i = 0; i < tileDatas.Length; i++)
            {
                tilePoses.Add(poses[i]);
            }
            return tilePoses.ToArray();
        }
        public UTileData GetTileData(Vector3Int cellPos)
        {
            if (_tileDataDict.ContainsKey(cellPos))
            {
                return _tileDataDict[cellPos];
            }
            return new UTileData();
        }
        public UTileData[] GetTileDatas(Vector3Int[] cellPoses)
        {
            List<UTileData> tileDatas = new List<UTileData>();
            for (int i = 0; i < cellPoses.Length; i++)
            {
                if (_tileDataDict.ContainsKey(cellPoses[i]))
                {
                    tileDatas.Add(_tileDataDict[cellPoses[i]]);
                }
            }
            return tileDatas.ToArray();
        }
        public TileBase GetTileBase(Vector3Int cellPos)
        {
            if (_tileDataDict.ContainsKey(cellPos))
            {
                return _tileDataDict[cellPos].TileBase;
            }
            return null;
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
        public TileBase[] GetTileBases(UTileData[] tileDatas)
        {
            TileBase[] tileBases = new TileBase[tileDatas.Length];
            for (int i = 0; i < tileDatas.Length; i++)
            {
                tileBases[i] = tileDatas[i].TileBase;
            }
            return tileBases;
        }

        private void SetTile(Vector3Int pos, UTileData tileData)
        {
            if (tileData.Initialized)
            {
                LayerTileMap.SetTile(pos, tileData.TileBase);
            }
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
        private void SetTiles(Vector3Int[] poses, UTileData[] tileDatas)
        {
            for (int i = 0; i < tileDatas.Length; i++)
            {
                SetTile(poses[i], tileDatas[i]);
            }
        }

        private void SaveTile(Vector3Int pos, UTileData tileData)
        {
            _tileDataDict[pos] = tileData;

        }
        private void SaveTiles(Vector3Int[] poses, TileBase tileToSave)
        {
            for (int i = 0; i < poses.Length; i++)
            {
                if (!_tileDataDict.ContainsKey(poses[i]))
                {
                    _tileDataDict.Add(poses[i], new UTileData(tileToSave));
                }
                else
                {
                    _tileDataDict[poses[i]] = new UTileData(tileToSave);
                }
            }
        }
        private void SaveTiles(Vector3Int[] poses, TileBase[] tileBasesToSave)
        {
            for (int i = 0; i < poses.Length; i++)
            {
                if (!_tileDataDict.ContainsKey(poses[i]))
                {
                    _tileDataDict.Add(poses[i], new UTileData(tileBasesToSave[i]));
                }
                else
                {
                    _tileDataDict[poses[i]] = new UTileData(tileBasesToSave[i]);
                }
            }
        }
        private void SaveTiles(Vector3Int[] poses, UTileData[] tileDatas)
        {
            for (int i = 0; i < tileDatas.Length; i++)
            {
                if (tileDatas[i].Initialized)
                {
                    if (!_tileDataDict.ContainsKey(poses[i]))
                    {
                        _tileDataDict.Add(poses[i], tileDatas[i]);
                    }
                    else
                    {
                        _tileDataDict[poses[i]] = tileDatas[i];
                    }
                }
            }
        }
        private void RemoveTile(Vector3Int pos)
        {
            if (_tileDataDict.ContainsKey(pos))
            {
                _tileDataDict[pos] = new UTileData();
            }
        }
        private void RemoveTiles(Vector3Int[] poses)
        {
            for (int i = 0; i < poses.Length; i++)
            {
                if (_tileDataDict.ContainsKey(poses[i]))
                {
                    RemoveTile(poses[i]);
                }
            }
        }
        private void RemoveAllTiles()
        {
            _tileDataDict.Clear();
        }
    }
}
