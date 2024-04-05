using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.UntitledNewGame
{
    public enum PieceType
    {
        None,
        World,
        Void
    }
    [Serializable]
    public struct PieceData
    {
        public Vector2Int worldPos;
    }
    public class Piece : MonoBehaviour
    {
        public PieceData Data;
        public CompositeCollider2D Collider;
        public virtual PieceType Type => PieceType.None;
        private void Awake()
        {
            Collider = GetComponent<CompositeCollider2D>();
            //Debug.Log(Collider.bounds);
        }
    }
}
