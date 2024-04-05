using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;
using UnityEngine.WSA;

namespace Ultra.LevelEditor
{
    public class UPreviewLayer : MonoBehaviour
    {
        protected Tilemap PreviewTileMap { get; private set; }
        private void Awake()
        {
            PreviewTileMap = GetComponent<Tilemap>();
        }
        public void DrawTilesLinePreview(Vector3Int lineStart, Vector3Int lineEnd, TileBase tile)
        {
            DrawTilesPreview(UShapeGetter.GetLine(lineStart, lineEnd), tile);
        }
        public void DrawPreviewTile(Vector3Int pos, TileBase tile)
        {
            PreviewTileMap.SetTile(pos, tile);
        }
        public void DrawTilesPreview(Vector3Int[] poses, TileBase tileBase)
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
        public void DrawTilesPreview(Vector3Int[] poses, TileBase[] tileBases)
        {
            PreviewTileMap.SetTiles(poses, tileBases);
        }
        public void ClearPreviewTiles()
        {
            PreviewTileMap.ClearAllTiles();
        }
    }
}
