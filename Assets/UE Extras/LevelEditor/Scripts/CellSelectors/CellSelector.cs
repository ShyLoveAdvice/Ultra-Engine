using MoreMountains.CorgiEngine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Ultra.LevelEditor
{
    public enum BoxSelectorStates
    {
        New,
        Additive
    }
    /// <summary>
    /// Used to describe the direction (diagnally) the of points of the selected area and be passed to draw functions
    /// </summary>
    public enum SelectionPointDirections
    {
        TopLeft, TopRight, BottomLeft, BottomRight,
        ConcaveTopLeft, ConcaveTopRight, ConcaveBottomLeft, ConcaveBottomRight
    }
    public class CellSelector : MonoBehaviour
    {
        public Vector2Int[] SelectedCells { get => selectedCells; set => selectedCells = value; }
        protected Vector2Int[] _drawnSelectedCells;

        public Tilemap testTilemap;
        public Tile testTile;
        public GameObject ActiveBox;
        public GameObject SelectedBox;

        protected Vector2Int[] selectedCells;
        protected Vector2Int[] selectingCells;

        private List<Vector2Int> selectedPoints;
        private List<Vector3> selectedVertexesWithEnum;
        private List<Vector4> selectedDirectionalVertexes;
        public List<Vector4> selectedDirectionalVertexesNonDestroy;
        private List<Vector3> toBeDrawnVertexes;

        protected Vector3Int CurrentCell { get; private set; }
        protected Vector2Int _startVertex;
        protected bool _startTurn;

        protected BoxSelectorStates _boxSelectorState;
        protected Bounds _activeBoxBounds;
        protected Bounds _selectedBoxBounds;
        protected Vector3 CurrentBoxTopLeft
        {
            get
            {
                return new Vector3(_activeBoxMinX, _activeBoxMaxY);
            }
        }
        protected Vector3 CurrentBoxTopRight
        {
            get
            {
                return new Vector3(_activeBoxMaxX, _activeBoxMaxY);
            }
        }
        protected Vector3 CurrentBoxBottomLeft
        {
            get
            {
                return new Vector3(_activeBoxMinX, _activeBoxMinY);
            }
        }
        protected Vector3 CurrentBoxBottomRight
        {
            get
            {
                return new Vector3(_activeBoxMaxX, _activeBoxMinY);
            }
        }
        protected Vector3 _currentBoxSize
        {
            get
            {
                return new Vector3(_activeBoxMaxX - _activeBoxMinX, _activeBoxMaxY - _activeBoxMinY);
            }
        }
        protected Vector3Int _activeBoxStartPos;
        protected Vector3 _mouseWorldPos;

        protected LineRenderer ActiveBoxLR;
        protected List<LineRenderer> SelectedLRs;

        int _activeBoxMaxX;
        int _activeBoxMaxY;
        int _activeBoxMinX;
        int _activeBoxMinY;

        Vector2Int _activeBoxSize;

        private void Awake()
        {
            ActiveBoxLR = Instantiate(ActiveBox, transform).GetComponent<LineRenderer>();
            ActiveBoxLR.enabled = false;

            selectedCells = new Vector2Int[0];
            selectingCells = new Vector2Int[0];

            selectedPoints = new List<Vector2Int>();
            selectedVertexesWithEnum = new List<Vector3>();
            selectedDirectionalVertexes = new List<Vector4>();
            toBeDrawnVertexes = new List<Vector3>();

            _drawnSelectedCells = new Vector2Int[0];
        }
        private void Update()
        {
            _mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector3 screenMin = Camera.main.ScreenToWorldPoint(Vector2.zero);
            Vector3 screenMax = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

            _mouseWorldPos.x = Mathf.Clamp(_mouseWorldPos.x, screenMin.x, screenMax.x);
            _mouseWorldPos.y = Mathf.Clamp(_mouseWorldPos.y, screenMin.y, screenMax.y);
            _mouseWorldPos.z = 0f;

            CurrentCell = new Vector3Int(Mathf.FloorToInt(_mouseWorldPos.x), Mathf.FloorToInt(_mouseWorldPos.y));

            BoxSelector();


        }
        protected virtual void BoxSelector()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _boxSelectorState = BoxSelectorStates.Additive;
            }
            else
            {
                _boxSelectorState = BoxSelectorStates.New;
            }

            if (Input.GetMouseButtonDown(0))
            {
                _activeBoxStartPos = CurrentCell;
                ActiveBoxLR.enabled = true;

                if (_boxSelectorState == BoxSelectorStates.New)
                {
                    if (SelectedLRs != null)
                    {
                        SelectedLRs.ForEach(lr => lr.enabled = false);
                    }
                }
                else
                {
                    //Donothing
                }
            }

            if (Input.GetMouseButton(0))
            {
                _activeBoxMaxX = CurrentCell.x >= _activeBoxStartPos.x ? CurrentCell.x : _activeBoxStartPos.x;
                _activeBoxMaxY = CurrentCell.y >= _activeBoxStartPos.y ? CurrentCell.y : _activeBoxStartPos.y;
                _activeBoxMinX = _activeBoxMaxX == CurrentCell.x ? _activeBoxStartPos.x : CurrentCell.x;
                _activeBoxMinY = _activeBoxMaxY == CurrentCell.y ? _activeBoxStartPos.y : CurrentCell.y;

                _activeBoxMaxX += 1;
                _activeBoxMaxY += 1;

                if (_activeBoxBounds == null || _activeBoxBounds.max.x != _activeBoxMaxX || _activeBoxBounds.max.y != _activeBoxMaxY || _activeBoxBounds.min.x != _activeBoxMinX || _activeBoxBounds.min.y != _activeBoxMinY)
                {
                    Vector3 center = new Vector3((_activeBoxMinX + _activeBoxMaxX) / 2, (_activeBoxMinY + _activeBoxMaxY) / 2);
                    Vector2 size = new Vector3((_activeBoxMaxX - _activeBoxMinX), (_activeBoxMaxY - _activeBoxMinY));
                    _activeBoxSize = size.MMVector2Int();
                    _activeBoxBounds = new Bounds(center, size);

                    SetBoxPositions(ActiveBoxLR);
                    SetBoxTiling(ActiveBoxLR);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                ActiveBoxLR.enabled = false;

                SetSelectingCells(_activeBoxMinX, _activeBoxMinY, _activeBoxSize);

                GetButtonUp();

                SetSelectedCells();

                
            }

            if (_drawnSelectedCells != SelectedCells)
            {
                DetermineSelectedPointDir();

                DrawSelection();
            }
        }
        private void GetButtonUp()
        {
            switch (_boxSelectorState)
            {
                case BoxSelectorStates.New:
                    NewSelectionGetMouseButtonUp(); break;
                case BoxSelectorStates.Additive:
                    AdditiveSelectionGetMouseButtonUp(); break;
            }
        }
        protected virtual void NewSelectionGetMouseButtonUp()
        {
            Vector3 center = new Vector3((_activeBoxMinX + _activeBoxMaxX) / 2, (_activeBoxMinY + _activeBoxMaxY) / 2);
            Vector3 size = new Vector3((_activeBoxMaxX - _activeBoxMinX), (_activeBoxMaxY - _activeBoxMinY));
            _selectedBoxBounds = new Bounds(center, size);
        }
        protected virtual void AdditiveSelectionGetMouseButtonUp()
        {

        }
        private void SetBoxPositions(LineRenderer boxLR)
        {
            boxLR.positionCount = 5;
            boxLR.SetPosition(0, CurrentBoxTopLeft);
            boxLR.SetPosition(1, CurrentBoxTopRight);
            boxLR.SetPosition(2, CurrentBoxBottomRight);
            boxLR.SetPosition(3, CurrentBoxBottomLeft);
            boxLR.SetPosition(4, CurrentBoxTopLeft);
        }
        private void SetBoxTiling(LineRenderer boxLR)
        {
            float tilingAmount = (_currentBoxSize.x + _currentBoxSize.y) * 2;
            SetTiling(boxLR, tilingAmount);
        }
        private void SetTiling(LineRenderer lr, float tilingAmount)
        {
            lr.material.SetFloat("_TilingAmount", tilingAmount);
        }
        private void SetSelectingCells(int minX, int minY, Vector2Int size)
        {
            SetCells(minX, minY, size, ref selectingCells);
        }
        private void SetSelectedCells()
        {
            Vector2Int[] selection = new Vector2Int[0];
            switch (_boxSelectorState)
            {
                case BoxSelectorStates.New:
                    selection = selectingCells;
                    break;
                case BoxSelectorStates.Additive:
                    if (selectedCells != null)
                    {
                        selection = selectedCells.Concat(selectingCells).ToArray();
                        selection = selection.Distinct().ToArray();
                    }
                    else
                    {
                        selection = selectingCells;
                    }
                    break;
            }
            //SelectCommand selectCommand = new SelectCommand(this, selection);
            //LevelEditorCommandInvoker.ExecuteCommand(selectCommand);
        }
        private void SetCells(int minX, int minY, Vector2Int size, ref Vector2Int[] cells)
        {
            cells = new Vector2Int[size.x * size.y];

            int index = 0;
            int maxX = minX + size.x;
            int maxY = minY + size.y;

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    cells[index] = new Vector2Int(x, y);
                    index++;
                }
            }
        }
        private void DrawSelection()
        {
            toBeDrawnVertexes = new List<Vector3>();

            if (SelectedLRs != null)
            {
                SelectedLRs.ForEach(lr => Destroy(lr.gameObject));
            }
            SelectedLRs = new List<LineRenderer>();

            int length = 0;
            int index = 0;
            int numOfTries = 0;
            bool start = true;

            selectedDirectionalVertexesNonDestroy = selectedDirectionalVertexes.ToArray().ToList();

            while (selectedDirectionalVertexes.Count != 0 && numOfTries <= 1000)
            {
                numOfTries++;

                Vector2 currentPos = new Vector2(selectedDirectionalVertexes[index].x, selectedDirectionalVertexes[index].y);
                Vector2 currentDir = new Vector2(selectedDirectionalVertexes[index].z, selectedDirectionalVertexes[index].w);

                toBeDrawnVertexes.Add(currentPos);

                if (start)
                {
                    start = false;
                    _startTurn = true;
                    _startVertex = currentPos.MMVector2Int();
                }
                else
                {
                    selectedDirectionalVertexes.RemoveAt(index);
                }

                //first we check if it's possible to continue to make a closed shape
                bool canFormLine = false;
                foreach (var vertex in selectedDirectionalVertexes)
                {
                    if (canFormLine)
                    {
                        break;
                    }

                    if (currentDir.x != 0)
                    {
                        if (vertex.y == currentPos.y)
                        {
                            if (currentDir.x < 0 && vertex.x < currentPos.x)
                            {
                                canFormLine = true;
                            }
                            if (currentDir.x > 0 && vertex.x > currentPos.x)
                            {
                                canFormLine = true;
                            }
                        }
                    }
                    if (currentDir.y != 0)
                    {
                        if (vertex.x == currentPos.x)
                        {
                            if (currentDir.y < 0 && vertex.y < currentPos.y)
                            {
                                canFormLine = true;
                            }
                            if (currentDir.y > 0 && vertex.y > currentPos.y)
                            {
                                canFormLine = true;
                            }
                        }
                    }
                }
                if(currentPos == _startVertex && _startTurn == false)
                {
                    canFormLine = false;
                }

                //Move in the direction of current vertex until meet next
                if (canFormLine)
                {
                    bool foundNext = false;
                    int step = 0;
                    while (foundNext == false && step < 800)
                    {
                        step++;
                        Vector2 nextPos = currentPos + currentDir * step;
                        Vector4 nextPoint = selectedDirectionalVertexes.Find(v => v.x == nextPos.x && v.y == nextPos.y);
                        if (nextPoint.z != 0 || nextPoint.w != 0)
                        {
                            foundNext = true;
                        }

                        if (foundNext)
                        {
                            index = selectedDirectionalVertexes.IndexOf(nextPoint);
                            length += step;
                        }
                    }
                }
                //if we can't make a line, we create a new LineRenderer and start the process all over again
                else
                {
                    index = 0;
                    start = true;

                    LineRenderer currentLR = Instantiate(SelectedBox, transform).GetComponent<LineRenderer>();
                    SelectedLRs.Add(currentLR);
                    currentLR.positionCount = toBeDrawnVertexes.Count;
                    currentLR.SetPositions(toBeDrawnVertexes.ToArray());
                    currentLR.enabled = true;

                    SetTiling(currentLR, length);
                    length = 0;

                    toBeDrawnVertexes.Clear();
                }

                _startTurn = false;
            }
            _drawnSelectedCells = SelectedCells;
        }
        private void DetermineSelectedPointDir()
        {
            //Convert cells to points
            selectedPoints = new List<Vector2Int>();
            selectedVertexesWithEnum = new List<Vector3>();

            foreach (var cell in selectedCells)
            {
                Vector2Int TopCell = new Vector2Int(cell.x, cell.y + 1);
                Vector2Int BottomCell = new Vector2Int(cell.x, cell.y - 1);
                Vector2Int LeftCell = new Vector2Int(cell.x - 1, cell.y);
                Vector2Int RightCell = new Vector2Int(cell.x + 1, cell.y);
                Vector2Int TopLeftCell = new Vector2Int(cell.x - 1, cell.y + 1);
                Vector2Int TopRightCell = new Vector2Int(cell.x + 1, cell.y + 1);
                Vector2Int BottomLeftCell = new Vector2Int(cell.x - 1, cell.y - 1);
                Vector2Int BottomRightCell = new Vector2Int(cell.x + 1, cell.y - 1);

                bool hasTopCell = selectedCells.Contains(TopCell);
                bool hasBottomCell = selectedCells.Contains(BottomCell);
                bool hasLeftCell = selectedCells.Contains(LeftCell);
                bool hasRightCell = selectedCells.Contains(RightCell);
                bool hasTopLeftCell = selectedCells.Contains(TopLeftCell);
                bool hasTopRightCell = selectedCells.Contains(TopRightCell);
                bool hasBottomLeftCell = selectedCells.Contains(BottomLeftCell);
                bool hasBottomRightCell = selectedCells.Contains(BottomRightCell);

                Vector2Int currentCellTopLeftCorner = new Vector2Int(cell.x, cell.y + 1);
                Vector2Int currentCellTopRightCorner = new Vector2Int(cell.x + 1, cell.y + 1);
                Vector2Int currentCellBottomLeftCorner = new Vector2Int(cell.x, cell.y);
                Vector2Int currentCellBottomRightCorner = new Vector2Int(cell.x + 1, cell.y);

                selectedPoints.Add(currentCellTopLeftCorner);
                selectedPoints.Add(currentCellTopRightCorner);
                selectedPoints.Add(currentCellBottomLeftCorner);
                selectedPoints.Add(currentCellBottomRightCorner);

                //Protrude
                if (!hasTopCell && !hasLeftCell && !hasTopLeftCell)
                {
                    selectedVertexesWithEnum.Add(new Vector3(currentCellTopLeftCorner.x, currentCellTopLeftCorner.y, (int)SelectionPointDirections.TopLeft));
                }
                if (!hasTopCell && !hasRightCell && !hasTopRightCell)
                {
                    selectedVertexesWithEnum.Add(new Vector3(currentCellTopRightCorner.x, currentCellTopRightCorner.y, (int)SelectionPointDirections.TopRight));
                }
                if (!hasBottomCell && !hasLeftCell && !hasBottomLeftCell)
                {
                    selectedVertexesWithEnum.Add(new Vector3(currentCellBottomLeftCorner.x, currentCellBottomLeftCorner.y, (int)SelectionPointDirections.BottomLeft));
                }
                if (!hasBottomCell && !hasRightCell && !hasBottomRightCell)
                {
                    selectedVertexesWithEnum.Add(new Vector3(currentCellBottomRightCorner.x, currentCellBottomRightCorner.y, (int)SelectionPointDirections.BottomRight));
                }

                //Concave
                if (hasTopCell && hasLeftCell && !hasTopLeftCell)
                {
                    selectedVertexesWithEnum.Add(new Vector3(currentCellTopLeftCorner.x, currentCellTopLeftCorner.y, (int)SelectionPointDirections.ConcaveTopLeft));
                }
                if (hasTopCell && hasRightCell && !hasTopRightCell)
                {
                    selectedVertexesWithEnum.Add(new Vector3(currentCellTopRightCorner.x, currentCellTopRightCorner.y, (int)SelectionPointDirections.ConcaveTopRight));
                }
                if (hasBottomCell && hasLeftCell && !hasBottomLeftCell)
                {
                    selectedVertexesWithEnum.Add(new Vector3(currentCellBottomLeftCorner.x, currentCellBottomLeftCorner.y, (int)SelectionPointDirections.ConcaveBottomLeft));
                }
                if (hasBottomCell && hasRightCell && !hasBottomRightCell)
                {
                    selectedVertexesWithEnum.Add(new Vector3(currentCellBottomRightCorner.x, currentCellBottomRightCorner.y, (int)SelectionPointDirections.ConcaveBottomRight));
                }

                //Vertex with two or more directions
                {
                    if (!hasTopCell && !hasRightCell && hasTopRightCell)
                    {
                        selectedVertexesWithEnum.Add(new Vector3(currentCellTopRightCorner.x, currentCellTopRightCorner.y, (int)SelectionPointDirections.ConcaveTopLeft));
                    }
                    if (!hasTopCell && !hasLeftCell && hasTopLeftCell)
                    {
                        selectedVertexesWithEnum.Add(new Vector3(currentCellTopLeftCorner.x, currentCellTopLeftCorner.y, (int)SelectionPointDirections.ConcaveTopRight));
                    }
                    if (!hasBottomCell && !hasRightCell && hasBottomRightCell)
                    {
                        selectedVertexesWithEnum.Add(new Vector3(currentCellBottomRightCorner.x, currentCellBottomRightCorner.y, (int)SelectionPointDirections.ConcaveBottomLeft));
                    }
                    if (!hasBottomCell && !hasLeftCell && hasBottomLeftCell)
                    {
                        selectedVertexesWithEnum.Add(new Vector3(currentCellBottomLeftCorner.x, currentCellBottomLeftCorner.y, (int)SelectionPointDirections.ConcaveBottomRight));
                    }
                }
            }
            selectedPoints = selectedPoints.Distinct().ToList();
            selectedVertexesWithEnum = selectedVertexesWithEnum.Distinct().ToList();

            selectedDirectionalVertexes = new List<Vector4>();

            //Determine the directions of the points
            foreach (var vertex in selectedVertexesWithEnum)
            {
                SelectionPointDirections pointDir = (SelectionPointDirections)vertex.z;
                Vector2 dir = pointDir switch
                {
                    SelectionPointDirections.TopLeft => Vector2.right,
                    SelectionPointDirections.TopRight => Vector2.down,
                    SelectionPointDirections.BottomLeft => Vector2.up,
                    SelectionPointDirections.BottomRight => Vector2.left,
                    SelectionPointDirections.ConcaveTopLeft => Vector2.up,
                    SelectionPointDirections.ConcaveTopRight => Vector2.right,
                    SelectionPointDirections.ConcaveBottomLeft => Vector2.left,
                    SelectionPointDirections.ConcaveBottomRight => Vector2.down,
                    _ => Vector2.zero
                };
                selectedDirectionalVertexes.Add(new Vector4(vertex.x, vertex.y, dir.x, dir.y));
            }

        }
    }
}

