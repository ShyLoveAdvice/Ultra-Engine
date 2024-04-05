using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace Ultra.UntitledNewGame
{
    public class UWorldManager: MMSerializedSingleton<UWorldManager>
    {
        public LayerMask BlockWorldPieceLayers;
        public UCharacterPieceDragger CharacterPieceDragger
        {
            get
            {
                if(_characterPieceDragger == null)
                {
                    _characterPieceDragger = FindObjectOfType<UCharacterPieceDragger>();
                }
                return _characterPieceDragger;
            }
        }
        private UCharacterPieceDragger _characterPieceDragger;

        public GameObject WorldPiecePrefab;
        public GameObject VoidPiecePrefab;

        public GameObject WorldPieceParent => GameObject.Find("WorldPieceParent");

        public MMSerializableDictionary<Vector2Int, Piece> PosPiecesDict = new MMSerializableDictionary<Vector2Int, Piece>();

        protected readonly Vector2 _pieceOffset = new Vector2(0.5f, 0.5f);
        protected Piece _currentPiece;
        protected Vector2Int _posInt;
        public void DrawWorldPiece(Vector2 pos)
        {
            bool canDraw = true;
            _posInt = pos.MMVector2Int();
            if (PosPiecesDict.TryGetValue(_posInt, out _currentPiece))
            {
                if(_currentPiece != null)
                {
                    canDraw = false;
                    if (_currentPiece.Type != PieceType.World)
                    {
                        DestroyImmediate(_currentPiece.gameObject);
                        canDraw = true;
                    }
                }
                
            }
            if(canDraw)
            {
                CreatePiece(WorldPiecePrefab, pos, "New World Piece Creation");
            }
        }
        public void DrawVoidPiece(Vector2 pos)
        {
            bool canDraw = true;
            _posInt = pos.MMVector2Int();
            if (PosPiecesDict.TryGetValue(_posInt, out _currentPiece))
            {
                if (_currentPiece != null)
                {
                    canDraw = false;
                    if (_currentPiece.Type != PieceType.Void)
                    {
                        DestroyImmediate(_currentPiece.gameObject);
                        canDraw = true;
                    }
                }

            }
            if (canDraw)
            {
                CreatePiece(VoidPiecePrefab, pos, "New Void Piece Creation");
            }
        }
        protected virtual void CreatePiece(GameObject piece, Vector2 pos, string _undoText)
        {
            GameObject newGO = Instantiate(piece, pos + _pieceOffset, Quaternion.identity);
            newGO.transform.parent = WorldPieceParent.transform;
            //Undo.RegisterCreatedObjectUndo(newGO, "New World Piece Creation");
            //Undo.SetTransformParent(newGO.transform, WorldPieceParent.transform, "Set To Parent");

            _currentPiece = newGO.GetComponent<Piece>();
            _currentPiece.Data.worldPos = _posInt;

            if(PosPiecesDict.ContainsKey(_posInt))
            {
                PosPiecesDict[_posInt] = _currentPiece;
            }
            else
            {
                PosPiecesDict.Add(_posInt, _currentPiece);
            }
        }
        public virtual void ClearPieces()
        {
            foreach (var piece in PosPiecesDict)
            {
                if(piece.Value != null)
                {
                    Undo.DestroyObjectImmediate(piece.Value.gameObject);
                }
            }
        }
    }
}
