using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Ultra.LevelEditor
{
    public class UPreviewLayer : MonoBehaviour
    {
        protected Tilemap PreviewTileMap { get; private set; }
        private void Awake()
        {
            PreviewTileMap = GetComponent<Tilemap>();
        }
        public void DrawPreviewTilesLine(Vector3Int lineStart, Vector3Int lineEnd, TileBase tile)
        {
            DrawPreviewTiles(UShapeGetter.GetLine(lineStart, lineEnd), tile);
        }
        public void DrawPreviewTile(Vector3Int pos, TileBase tile)
        {
            PreviewTileMap.SetTile(pos, tile);
        }
        public void DrawPreviewTiles(Vector3Int[] poses, TileBase tileBase)
        {
            if (poses != null && poses.Length > 0)
            {
                TileBase[] tileBases = new TileBase[poses.Length];
                for (int i = 0; i < tileBases.Length; i++)
                {
                    tileBases[i] = tileBase;
                }
                PreviewTileMap.SetTiles(poses, tileBases);
            }
        }
        public void DrawPreviewTiles(Vector3Int[] poses, TileBase[] tileBases)
        {
            PreviewTileMap.SetTiles(poses, tileBases);
        }
        public void DrawPreviewTiles(UTileData[] tileDatas)
        {
            for (int i = 0; i < tileDatas.Length; i++)
            {
                PreviewTileMap.SetTile(tileDatas[i].Pos, tileDatas[i].TileBase);
            }
        }

        public void ClearPreviewTiles()
        {
            PreviewTileMap.ClearAllTiles();
        }
    }
}
