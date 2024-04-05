using DG.Tweening;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.UntitledNewGame
{
    public class UCharacterPieceDragger : UCharacterAbility
    {
        public enum ShootDirections { up, down, left, right }

        public PieceDragger PieceDragger;
        public LayerMask DetectingLayers;
        public float ShootDuration;
        public float ShootDistance;
        public MMTweenType ShootTween;

        public float RetrieveOffset = 0.2f;
        public float RetrieveDuration;
        public MMTweenType RetrieveTween;

        public float DraggerResetDuration;
        private Vector3 _draggerInitialPos;

        private IEnumerator _shootCoroutine;
        private IEnumerator _retrieveCoroutine;
        private float _startShootTime;
        private Vector2 _shootDirection;
        private ShootDirections _shootDirectionEnum;
        private float _diagonalDirectionMulti = 0.7f;
        private Vector2 _startShootPosition;
        private Vector2 _endShootPosition;
        private int _draggerHorizontalInput;
        private int _draggerVerticalInput;
        private Collider2D _currentColliding;
        private Piece _currentDraggingPiece;
        private Transform _currentPieceParent;
        private bool _dragFinished;
        private bool _shouldStopDraggingPiece;

        private bool CanShoot
        {
            get
            {
                return _dragFinished;
            }
        }
        protected override void Initialization()
        {
            base.Initialization();

            _dragFinished = true;
            _shouldStopDraggingPiece = false;
            PieceDragger.InitializePieceDragger(this);
            _draggerInitialPos = PieceDragger.transform.localPosition;
        }
        protected override void HandleInput()
        {
            if (AbilityAuthorized)
            {
                _draggerHorizontalInput = Mathf.RoundToInt(_horizontalInput);
                _draggerVerticalInput = Mathf.RoundToInt(_verticalInput);

                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 shootDirection = Vector2.zero;

                    if (_character.IsFacingRight)
                    {
                        shootDirection.x = 1;
                    }
                    else
                    {
                        shootDirection.x = -1;
                    }

                    if (_draggerVerticalInput != 0)
                    {
                        shootDirection.x = 0;
                        shootDirection.y = _draggerVerticalInput;
                    }

                    //if (shootDirection.x != 0 && shootDirection.y != 0)
                    //{
                    //    shootDirection.x *= _diagonalDirectionMulti;
                    //    shootDirection.y *= _diagonalDirectionMulti;
                    //}

                    InitializeShootDragger(shootDirection);
                }

            }
        }
        private void InitializeShootDragger(Vector2 shootDir)
        {
            if(CanShoot)
            {
                _controller.ResetColliderSize();
                _characterHorizontalMovement.ResetHorizontalSpeed();

                _startShootTime = Time.time;
                _shootDirection = shootDir;

                if (_shootDirection.y == 0)
                {
                    if (_shootDirection.x > 0)
                    {
                        _shootDirectionEnum = ShootDirections.right;
                    }
                    if (_shootDirection.x < 0)
                    {
                        _shootDirectionEnum = ShootDirections.left;
                    }
                }
                if (_shootDirection.x == 0)
                {
                    if (_shootDirection.y > 0)
                    {
                        _shootDirectionEnum = ShootDirections.up;
                    }
                    if (_shootDirection.y < 0)
                    {
                        _shootDirectionEnum = ShootDirections.down;
                    }
                }

                _startShootPosition = PieceDragger.transform.position;
                _endShootPosition = _startShootPosition + _shootDirection * ShootDistance;
                _currentColliding = null;
                _currentDraggingPiece = null;
                _currentPieceParent = null;
                _dragFinished = false;
                _shouldStopDraggingPiece = false;

                _movement.ChangeState(CharacterStates.MovementStates.ShootDragger);

                _shootCoroutine = Shoot();
                StartCoroutine(_shootCoroutine);
            }
        }
        private IEnumerator Shoot()
        {
            // if the character is not in a position where it can move freely, we do nothing.
            if (!AbilityAuthorized
                 || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
            {
                yield break;
            }

            while (Time.time - _startShootTime < ShootDuration
                && _movement.CurrentState == CharacterStates.MovementStates.ShootDragger)
            {
                if (_currentDraggingPiece != null)
                {
                    _currentPieceParent = _currentDraggingPiece.gameObject.transform.parent;
                    _currentDraggingPiece.gameObject.transform.SetParent(PieceDragger.transform);
                    Debug.Log(_currentDraggingPiece.gameObject.name + " SetParent To Dragger");
                    break;
                }
                if (_currentColliding != null)
                {
                    break;
                }

                _controller.SetHorizontalForce(0);
                _controller.SetVerticalForce(0);
                _controller.GravityActive(false);

                PieceDragger.transform.position = MMTween.Tween(Time.time, _startShootTime, _startShootTime + ShootDuration, _startShootPosition, _endShootPosition, ShootTween);
                yield return null;
            }

            _retrieveCoroutine = Drag();
            StartCoroutine(_retrieveCoroutine);


        }
        private IEnumerator Drag()
        {
            _startShootTime = Time.time;
            _endShootPosition = _startShootPosition;

            // We Determine The End Position of Current Dragging Piece
            if (_currentDraggingPiece != null)
            {
                float characterBoundingBoxWidth = _controller.BoundsRight.x - _controller.BoundsLeft.x;
                float characterBoundingBoxHeight = _controller.BoundsTop.y - _controller.BoundsBottom.y;

                switch (_shootDirectionEnum)
                {
                    case ShootDirections.up:
                        _endShootPosition.y += characterBoundingBoxHeight / 2 + RetrieveOffset;
                        break;
                    case ShootDirections.down:
                        _endShootPosition.y -= characterBoundingBoxHeight / 2 + RetrieveOffset;
                        break;
                    case ShootDirections.left:
                        _endShootPosition.x -= characterBoundingBoxWidth / 2 + RetrieveOffset;
                        break;
                    case ShootDirections.right:
                        _endShootPosition.x += characterBoundingBoxWidth / 2 + RetrieveOffset;
                        break;
                }
            }

            _startShootPosition = PieceDragger.transform.position;

            while (Time.time - _startShootTime < RetrieveDuration
               && _movement.CurrentState == CharacterStates.MovementStates.ShootDragger)
            {
                if(_shouldStopDraggingPiece)
                {
                    break;
                }

                _controller.SetHorizontalForce(0);
                _controller.SetVerticalForce(0);
                _controller.GravityActive(false);

                PieceDragger.transform.position = MMTween.Tween(Time.time, _startShootTime, _startShootTime + RetrieveDuration, _startShootPosition, _endShootPosition, RetrieveTween);
                yield return null;
            }

            _controller.GravityActive(true);
            _movement.ChangeState(CharacterStates.MovementStates.Idle);

            // We Unparent Current Dragging Piece
            if(_currentDraggingPiece != null)
            {
                if(_currentPieceParent != null)
                {
                    _currentDraggingPiece.transform.SetParent(_currentPieceParent);
                }
                else
                {
                    _currentDraggingPiece.transform.SetParent(null);
                }
            }

            PieceDragger.transform.DOLocalMove(_draggerInitialPos, DraggerResetDuration).OnComplete(DragFinished);
        }
        private void DragFinished() => _dragFinished = true;
        public void RegisterDraggingPiece(Piece collidingPiece)
        {
            _currentDraggingPiece = collidingPiece;
        }
        public void RegisterDraggerCollidingWith(Collider2D colliding)
        {
            _currentColliding = colliding;
        }
        public void RegisterDraggingPieceColliding()
        {
            if(_currentDraggingPiece != null)
            {
                _shouldStopDraggingPiece = true;
            }

            Debug.Log("RegisterDraggingPieceColliding");
        }
    }
}
