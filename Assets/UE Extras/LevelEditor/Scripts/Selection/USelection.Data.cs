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
        public struct USelectData
        {
            public UTileData TileData;
            public USelectData(UTileData tileData)
            {
                TileData = tileData;
            }
        }

        [Header("Selection Data"), Space(10)]
        public Dictionary<Vector3Int, USelectData> _selectedDataDict;
        public Dictionary<Vector3Int, USelectData> _originalSelectedDataDict;

        /// <summary>
        /// These two dictionaries save selected by using y value as key to access a value describing single or multiple lines in x axis direction
        /// </summary>
        [HideInInspector] public Dictionary<int, int[][]> ActiveLineDict;
        [HideInInspector] public Dictionary<int, int[][]> SelectedLineDict;

        private Vector3Int _moveOrigin;
        private const int LINEXMIN = 0;
        private const int LINEXMAX = 1;
        private int[] _toBeRemovedIndexes;
        int _lineXMin, _lineXMax;
        private void SetSelectedData(Vector3Int[] selectedCellPoses)
        {
            for (int i = 0; i < selectedCellPoses.Length; i++)
            {
                if (!_selectedDataDict.ContainsKey(selectedCellPoses[i]))
                {
                    UTileData tileData = LevelEditor.CurrentLayer.GetTileData(selectedCellPoses[i]);
                    _selectedDataDict.Add(selectedCellPoses[i], new USelectData(tileData));
                }
            }

            SetOriginalSelectedDataClear();
            SetSelectedLineDataClear(_selectedDataDict.Keys.ToArray());
        }
        private void SetSelectedData(Vector3Int[] selectedCellPoses, USelectData[] selectDatas)
        {
            for (int i = 0; i < selectedCellPoses.Length; i++)
            {
                if (!_selectedDataDict.ContainsKey(selectedCellPoses[i]))
                {
                    _selectedDataDict.Add(selectedCellPoses[i], selectDatas[i]);
                }
            }

            SetOriginalSelectedDataClear();
            SetSelectedLineDataClear(_selectedDataDict.Keys.ToArray());
        }
        private void SetSelectedData(Vector3Int[] selectedCellPoses, UTileData[] tileDatas)
        {
            for (int i = 0; i < selectedCellPoses.Length; i++)
            {
                if (!_selectedDataDict.ContainsKey(selectedCellPoses[i]))
                {
                    _selectedDataDict.Add(selectedCellPoses[i], new USelectData(tileDatas[i]));
                }
            }

            SetOriginalSelectedDataClear();
            SetSelectedLineDataClear(_selectedDataDict.Keys.ToArray());
        }
        private void SetSelectedDataClear(Vector3Int[] selectedCellPoses, UTileData[] tileDatas)
        {
            _selectedDataDict.Clear();
            _originalSelectedDataDict.Clear();
            for (int i = 0; i < selectedCellPoses.Length; i++)
            {;
                _selectedDataDict.Add(selectedCellPoses[i], new USelectData(tileDatas[i]));
                _originalSelectedDataDict.Add(selectedCellPoses[i], new USelectData(tileDatas[i]));
            }

            SetSelectedLineDataClear(_selectedDataDict.Keys.ToArray());
        }
        private void SetSelectedDataClear(Vector3Int[] selectedCellPoses)
        {
            _selectedDataDict.Clear();
            _originalSelectedDataDict.Clear();
            for (int i = 0; i < selectedCellPoses.Length; i++)
            {
                UTileData tileData = LevelEditor.CurrentLayer.GetTileData(selectedCellPoses[i]);
                _selectedDataDict.Add(selectedCellPoses[i], new USelectData(tileData));
                _originalSelectedDataDict.Add(selectedCellPoses[i], new USelectData(tileData));
            }

            SetSelectedLineDataClear(_selectedDataDict.Keys.ToArray());
        }

        private void SetSelectedLineDataClear(Vector3Int[] selectedCellPoses)
        {
            SelectedLineDict.Clear();
            SetLineData(selectedCellPoses, SelectedLineDict);
        }
        private void SetActiveLineDataClear(Vector3Int[] activeCellPoses)
        {
            ClearActiveData();
            SetLineData(activeCellPoses, ActiveLineDict);
        }

        private void SetOriginalSelectedDataClear()
        {
            _originalSelectedDataDict.Clear();
            foreach (var item in _selectedDataDict)
            {
                _originalSelectedDataDict.Add(item.Key, item.Value);
            }
        }

        private void ClearActiveData()
        {
            ActiveLineDict.Clear();
        }
        private void ClearSelectedData()
        {
            _moveOrigin = Vector3Int.zero;

            _originalSelectedDataDict.Clear();
            _selectedDataDict.Clear();
            SelectedLineDict.Clear();
        }

        private void SetLineData(Vector3Int[] selectionCellPoses, Dictionary<int, int[][]> selectionLineDict)
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
