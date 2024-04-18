using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.Utilities;
using System.Linq;
using MoreMountains.Tools;
using UnityEngine.Tilemaps;

namespace Ultra.LevelEditor
{
    public partial class USelection : SerializedMonoBehaviour
    {
        [Header("Selection Data"), Space(10)]
        /// <summary>
        /// These two dictionaries save selected by using y value as key to access a value describing single or multiple lines in x axis direction
        /// </summary>
        [HideInInspector] public Dictionary<int, int[][]> ActiveLineDict;
        [HideInInspector] public Dictionary<int, int[][]> SelectedLineDict;
        public UTileData[] SelectedTileDatas { get => _selectedTileDatas; }
        public Vector3Int[] SelectedCellPoses { get => _selectedCellPoses; }
        [SerializeField] private UTileData[] _selectedTileDatas = new UTileData[0];
        /*[SerializeField]*/ private Vector3Int[] _selectedCellPoses = new Vector3Int[0];
        public Dictionary<Vector3Int,  UTileData> _selectedTileDatasDict = new Dictionary<Vector3Int, UTileData>();

        private Vector3Int _moveOrigin;
        private Vector3Int[] _originalSelectedCellPoses;
        private UTileData[] _originalSelectedTileDatas;
        private const int LINEXMIN = 0;
        private const int LINEXMAX = 1;
        private int[] _toBeRemovedIndexes;
        int _lineXMin, _lineXMax;
        private void BuildSelectedData(Vector3Int[] selectedCellPoses)
        {
            ClearSelectedData();
            BuildLineData(selectedCellPoses, SelectedLineDict);
        }
        private void BuildBoxSelectedData(int xMin, int yMin, int xMax, int yMax)
        {
            ClearSelectedData();
            for (int y = yMin; y <= yMax; y++)
            {
                SelectedLineDict.Add(y, new int[][] { new int[] { xMin, xMax } });
            }
        }
        private void BuildBoxAdditiveSelectedData(int xMin, int yMin, int xMax, int yMax)
        {
            int xMinOriginal = xMin;
            int xMaxOriginal = xMax;

            for (int y = yMin; y <= yMax; y++)
            {
                xMin = xMinOriginal;
                xMax = xMaxOriginal;

                if (SelectedLineDict.ContainsKey(y))
                {
                    bool foundMin = false;
                    bool foundMax = false;
                    bool minIndexInMiddle = false;
                    bool maxIndexInMiddle = false;
                    int minIndex = -1;
                    int maxIndex = -1;


                    for (int j = 0; j < SelectedLineDict[y].Length; j++)
                    {
                        if (j == 0)
                        {
                            if (xMax < SelectedLineDict[y][0][LINEXMIN] || xMin > SelectedLineDict[y][SelectedLineDict[y].Length - 1][LINEXMAX])
                            {
                                if (!(xMax == SelectedLineDict[y][0][LINEXMIN] - 1) && !(xMin == SelectedLineDict[y][SelectedLineDict[y].Length - 1][LINEXMAX] + 1))
                                {
                                    break;
                                }
                            }
                        }

                        if (xMin < SelectedLineDict[y][j][LINEXMIN])
                        {
                            if (!foundMin)
                            {
                                minIndex = j;
                                foundMin = true;
                            }
                        }
                        if ((xMin >= SelectedLineDict[y][j][LINEXMIN] && xMin <= SelectedLineDict[y][j][LINEXMAX]) || xMin == SelectedLineDict[y][j][LINEXMAX] + 1)
                        {
                            if (!foundMin)
                            {
                                minIndex = j;
                                foundMin = true;
                                minIndexInMiddle = true;
                            }
                        }
                        if (xMax > SelectedLineDict[y][j][LINEXMAX])
                        {
                            if (!foundMax)
                            {
                                maxIndex = j;
                            }
                        }
                        if ((xMax >= SelectedLineDict[y][j][LINEXMIN] && xMax <= SelectedLineDict[y][j][LINEXMAX]) || xMax == SelectedLineDict[y][j][LINEXMIN] - 1)
                        {
                            if (!foundMax)
                            {
                                maxIndex = j;
                                foundMax = true;
                                maxIndexInMiddle = true;
                            }
                        }
                    }

                    //remove overlapped lines
                    if (minIndex != -1 && maxIndex != -1)
                    {
                        if (minIndexInMiddle)
                        {
                            xMin = SelectedLineDict[y][minIndex][LINEXMIN];
                        }
                        if (maxIndexInMiddle)
                        {
                            xMax = SelectedLineDict[y][maxIndex][LINEXMAX];
                        }

                        _toBeRemovedIndexes = new int[maxIndex - minIndex + 1];
                        for (int index = minIndex; index <= maxIndex; index++)
                        {
                            _toBeRemovedIndexes[index - minIndex] = index;
                        }

                        SelectedLineDict[y] = RemoveIndexes(SelectedLineDict[y], _toBeRemovedIndexes);
                    }

                    //add new line to current lines and sort current lines
                    SelectedLineDict[y] = AddNewLine(SelectedLineDict[y], xMin, xMax);
                    Sort(SelectedLineDict[y], LINEXMIN);
                }
                else
                {
                    SelectedLineDict.Add(y, new int[][] { new int[] { xMin, xMax } });
                }

            }
        }
        private void BuildActiveData(Vector3Int[] activeCellPoses)
        {
            ClearActiveData();
            BuildLineData(activeCellPoses, ActiveLineDict);
        }
        private void BuildBoxActiveData(int xMin, int yMin, int xMax, int yMax)
        {
            ActiveLineDict.Clear();
            for (int y = yMin; y <= yMax; y++)
            {
                ActiveLineDict.Add(y, new int[][] { new int[] { xMin, xMax } });
            }
        }
        private void BuildSelectedTileData(Vector3Int[] selectedCellPoses)
        {
            _selectedTileDatas = LevelEditor.CurrentLayer.GetTileDatas(selectedCellPoses);
            LevelEditor.CurrentLayer.EraseTiles(selectedCellPoses);

            _originalSelectedTileDatas = new UTileData[_selectedTileDatas.Length];
            for (int i = 0; i < _selectedTileDatas.Length; i++)
            {
                _originalSelectedTileDatas[i] = _selectedTileDatas[i];
            }

            for (int i = 0; i < _selectedTileDatas.Length; i++)
            {
                if (!_selectedTileDatasDict.ContainsKey(_selectedTileDatas[i].Pos))
                {
                    _selectedTileDatasDict.Add(_selectedTileDatas[i].Pos, _selectedTileDatas[i]);
                }
                else
                {
                    _selectedTileDatasDict[_selectedTileDatas[i].Pos] = _selectedTileDatas[i];
                }
            }
        }
        private void BuildSelectedTileData(UTileData[] selectedTileDatas)
        {
            _selectedTileDatas = selectedTileDatas;

            _originalSelectedTileDatas = new UTileData[_selectedTileDatas.Length];
            for (int i = 0; i < _selectedTileDatas.Length; i++)
            {
                _originalSelectedTileDatas[i] = _selectedTileDatas[i];
            }

            for (int i = 0; i < _selectedTileDatas.Length; i++)
            {
                if (!_selectedTileDatasDict.ContainsKey(_selectedTileDatas[i].Pos))
                {
                    _selectedTileDatasDict.Add(_selectedTileDatas[i].Pos, _selectedTileDatas[i]);
                }
                else
                {
                    _selectedTileDatasDict[_selectedTileDatas[i].Pos] = _selectedTileDatas[i];
                }
            }
        }
        private void BuildSelectedTileDataAdditive(Vector3Int[] selectedCellPosesAdditive)
        {
            UTileData[] selectedTileDatasAdditive = LevelEditor.CurrentLayer.GetTileDatas(selectedCellPosesAdditive);
            LevelEditor.CurrentLayer.EraseTiles(selectedCellPosesAdditive);

            _selectedTileDatas = _selectedTileDatas.Concat(selectedTileDatasAdditive).ToArray();

            _originalSelectedTileDatas = new UTileData[_selectedTileDatas.Length];
            for (int i = 0; i < _selectedTileDatas.Length; i++)
            {
                _originalSelectedTileDatas[i] = _selectedTileDatas[i];
            }

            for (int i = 0; i < _selectedTileDatas.Length; i++)
            {
                if (!_selectedTileDatasDict.ContainsKey(_selectedTileDatas[i].Pos))
                {
                    _selectedTileDatasDict.Add(_selectedTileDatas[i].Pos, _selectedTileDatas[i]);
                }
                else
                {
                    _selectedTileDatasDict[_selectedTileDatas[i].Pos] = _selectedTileDatas[i];
                }
            }
        }
        private void BuildSelectedTileData(int xMin, int yMin, int xMax, int yMax)
        {
            List<Vector3Int> selectedCellList = new List<Vector3Int>();
            for (int y = yMin; y <= yMax; y++)
            {
                for (int x = xMin; x <= xMax; x++)
                {
                    selectedCellList.Add(new Vector3Int(x, y));
                }
            }
            Vector3Int[] selectedCellPoses = selectedCellList.ToArray();
            _originalSelectedCellPoses = selectedCellPoses;

            _selectedTileDatas = LevelEditor.CurrentLayer.GetTileDatas(selectedCellPoses);
            _originalSelectedTileDatas = new UTileData[_selectedTileDatas.Length];
            for (int i = 0; i < _selectedTileDatas.Length; i++)
            {
                _originalSelectedTileDatas[i] = _selectedTileDatas[i];
            }
            LevelEditor.CurrentLayer.EraseTiles(selectedCellPoses);

            for (int i = 0; i < _selectedTileDatas.Length; i++)
            {
                if (!_selectedTileDatasDict.ContainsKey(_selectedTileDatas[i].Pos))
                {
                    _selectedTileDatasDict.Add(_selectedTileDatas[i].Pos, _selectedTileDatas[i]);
                }
                else
                {
                    _selectedTileDatasDict[_selectedTileDatas[i].Pos] = _selectedTileDatas[i];
                }
            }
        }

        private void BuildLineData(Vector3Int[] selectionCellPoses, Dictionary<int, int[][]> selectionLineDict)
        {
            foreach (Vector3Int currentCellPos in selectionCellPoses)
            {
                int x, y, xMin, xMax;
                x = currentCellPos.x; y = currentCellPos.y; xMin = x; xMax = x;
                if (!selectionLineDict.ContainsKey(y))
                {
                    selectionLineDict.Add(y, new int[][] { new int[] { currentCellPos.x, currentCellPos.x } });
                }
                else
                {
                    bool foundMin = false;
                    bool foundMax = false;
                    bool minIndexInMiddle = false;
                    bool maxIndexInMiddle = false;
                    int minIndex = -1;
                    int maxIndex = -1;


                    for (int j = 0; j < selectionLineDict[y].Length; j++)
                    {
                        if (j == 0)
                        {
                            if (xMax < selectionLineDict[y][0][LINEXMIN] || xMin > selectionLineDict[y][selectionLineDict[y].Length - 1][LINEXMAX])
                            {
                                if (!(xMax == selectionLineDict[y][0][LINEXMIN] - 1) && !(xMin == selectionLineDict[y][selectionLineDict[y].Length - 1][LINEXMAX] + 1))
                                {
                                    break;
                                }
                            }
                        }

                        if (xMin < selectionLineDict[y][j][LINEXMIN])
                        {
                            if (!foundMin)
                            {
                                minIndex = j;
                                foundMin = true;
                            }
                        }
                        if ((xMin >= selectionLineDict[y][j][LINEXMIN] && xMin <= selectionLineDict[y][j][LINEXMAX]) || xMin == selectionLineDict[y][j][LINEXMAX] + 1)
                        {
                            if (!foundMin)
                            {
                                minIndex = j;
                                foundMin = true;
                                minIndexInMiddle = true;
                            }
                        }
                        if (xMax > selectionLineDict[y][j][LINEXMAX])
                        {
                            if (!foundMax)
                            {
                                maxIndex = j;
                            }
                        }
                        if ((xMax >= selectionLineDict[y][j][LINEXMIN] && xMax <= selectionLineDict[y][j][LINEXMAX]) || xMax == selectionLineDict[y][j][LINEXMIN] - 1)
                        {
                            if (!foundMax)
                            {
                                maxIndex = j;
                                foundMax = true;
                                maxIndexInMiddle = true;
                            }
                        }
                    }

                    //remove overlapped lines
                    if (minIndex != -1 && maxIndex != -1)
                    {
                        if (minIndexInMiddle)
                        {
                            xMin = selectionLineDict[y][minIndex][LINEXMIN];
                        }
                        if (maxIndexInMiddle)
                        {
                            xMax = selectionLineDict[y][maxIndex][LINEXMAX];
                        }

                        _toBeRemovedIndexes = new int[maxIndex - minIndex + 1];
                        for (int index = minIndex; index <= maxIndex; index++)
                        {
                            _toBeRemovedIndexes[index - minIndex] = index;
                        }

                        selectionLineDict[y] = RemoveIndexes(selectionLineDict[y], _toBeRemovedIndexes);
                    }

                    //add new line to current lines and sort current lines
                    selectionLineDict[y] = AddNewLine(selectionLineDict[y], xMin, xMax);
                    Sort(selectionLineDict[y], LINEXMIN);
                }
            }
        }

        private void ClearActiveData()
        {
            ActiveLineDict.Clear();
        }
        private void ClearSelectedData()
        {
            SelectedLineDict.Clear();
        }

        private void ClearSelectedTileData()
        {
            _selectedTileDatas = new UTileData[0];
            _originalSelectedTileDatas = new UTileData[0];
        }
        
        private int[][] AddNewLine(int[][] lines, int xMin, int xMax)
        {
            int[][] newLines = lines;
            Array.Resize(ref newLines, lines.Length + 1);
            newLines[newLines.Length - 1] = new int[] { xMin, xMax };
            return newLines;
        }
        private int[][] RemoveIndexes(int[][] data, int[] toBeRemoveIndexes)
        {
            int[][] newData = new int[data.Length - toBeRemoveIndexes.Length][];
            int currentIndex = 0;

            for (int index = 0; index < data.Length; index++)
            {
                if (!IsIndexInArray(index, toBeRemoveIndexes))
                {
                    newData[currentIndex] = data[index];
                    currentIndex++;
                }
            }

            return newData;
        }
        private bool IsIndexInArray(int index, int[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (index == data[i])
                {
                    return true;
                }
            }
            return false;
        }
        private void Sort<T>(T[][] data, int col)
        {
            Comparer<T> comparer = Comparer<T>.Default;
            Array.Sort<T[]>(data, (x, y) => comparer.Compare(x[col], y[col]));
        }
    }
}
