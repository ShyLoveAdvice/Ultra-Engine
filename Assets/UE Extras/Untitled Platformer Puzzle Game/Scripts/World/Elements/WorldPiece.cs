using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ultra.UntitledNewGame
{
    public class WorldPiece : Piece
    {
        public List<BoxCollider2D> ColliderSegments = new List<BoxCollider2D>();
        public override PieceType Type => PieceType.World;
        public bool Diabled = false;
        private void Awake()
        {
            var array = transform.GetComponentsInChildren<BoxCollider2D>();
            ColliderSegments = array.ToList();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(!Diabled)
            {
                if (UWorldManager.Instance.BlockWorldPieceLayers.MMContains(collision.gameObject))
                {
                    if (collision.gameObject.TryGetComponent<WorldPiece>(out WorldPiece worldPiece))
                    {
                        if(!worldPiece.Diabled)
                        {
                            UWorldManager.Instance.CharacterPieceDragger.RegisterDraggingPieceColliding();

                            for (int i = 0; i < worldPiece.ColliderSegments.Count; i++)
                            {
                                worldPiece.ColliderSegments[i].transform.SetParent(transform);
                            }

                            worldPiece.ColliderSegments.Clear();
                            worldPiece.Diabled = true;
                        }
                        
                    }
                }
            }
            
        }
    }
}
