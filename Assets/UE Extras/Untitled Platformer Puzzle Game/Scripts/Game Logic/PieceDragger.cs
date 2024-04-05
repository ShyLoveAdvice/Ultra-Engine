using DG.Tweening;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.UntitledNewGame
{
    public class PieceDragger : MonoBehaviour
    {
        private UCharacterPieceDragger _characterPieceDragger;
        public void InitializePieceDragger(UCharacterPieceDragger characterPieceDragger)
        {
            this._characterPieceDragger = characterPieceDragger;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_characterPieceDragger != null)
            {
                if (_characterPieceDragger.DetectingLayers.MMContains(collision.gameObject))
                {
                    if (collision.TryGetComponent<Piece>(out Piece piece))
                    {
                        _characterPieceDragger.RegisterDraggingPiece(piece);
                    }
                    else
                    {
                        _characterPieceDragger.RegisterDraggerCollidingWith(collision);
                    }
                }
            }

        }
    }
}
