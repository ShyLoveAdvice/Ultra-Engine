using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.Utilities;
using System.Linq;

namespace Ultra.LevelEditor
{
    public partial class USelection : SerializedMonoBehaviour
    {
        [Header("Selection Data"), Space(10)]
        /// <summary>
        /// These two dictionaries save selections by using y value as key to access a value describing single or multiple lines in x axis direction
        /// </summary>
        public Dictionary<int, int[][]> ActiveSelectionLineDict;
        public Dictionary<int, int[][]> SelectedSelectionLineDict;
        public Vector3Int[] SelectedTiles { get => _selectedTiles; }
        public Vector3Int[] _selectedTiles;
        private const int LINEXMIN = 0;
        private const int LINEXMAX = 1;
        private int[] _toBeRemovedIndexes;

        int _lineXMin, _lineXMax;
        private void BuildSelectionData(Vector3Int[] selectedCellPoses)
        {
            ClearSelectedSelectionData();
            foreach (Vector3Int currentCellPos in selectedCellPoses)
            {
                int x, y, xMin, xMax;
                x = currentCellPos.x; y = currentCellPos.y; xMin = x; xMax = x;
                if (!SelectedSelectionLineDict.ContainsKey(y))
                {
                    SelectedSelectionLineDict.Add(y, new int[][] { new int[] { currentCellPos.x, currentCellPos.x } });
                }
                else
                {
                    bool foundMin = false;
                    bool foundMax = false;
                    bool minIndexInMiddle = false;
                    bool maxIndexInMiddle = false;
                    int minIndex = -1;
                    int maxIndex = -1;


                    for (int j = 0; j < SelectedSelectionLineDict[y].Length; j++)
                    {
                        if (j == 0)
                        {
                            if (xMax < SelectedSelectionLineDict[y][0][LINEXMIN] || xMin > SelectedSelectionLineDict[y][SelectedSelectionLineDict[y].Length - 1][LINEXMAX])
                            {
                                if (!(xMax == SelectedSelectionLineDict[y][0][LINEXMIN] - 1) && !(xMin == SelectedSelectionLineDict[y][SelectedSelectionLineDict[y].Length - 1][LINEXMAX] + 1))
                                {
                                    break;
                                }
                            }
                        }

                        if (xMin < SelectedSelectionLineDict[y][j][LINEXMIN])
                        {
                            if (!foundMin)
                            {
                                minIndex = j;
                                foundMin = true;
                            }
                        }
                        if ((xMin >= SelectedSelectionLineDict[y][j][LINEXMIN] && xMin <= SelectedSelectionLineDict[y][j][LINEXMAX]) || xMin == SelectedSelectionLineDict[y][j][LINEXMAX] + 1)
                        {
                            if (!foundMin)
                            {
                                minIndex = j;
                                foundMin = true;
                                minIndexInMiddle = true;
                            }
                        }
                        if (xMax > SelectedSelectionLineDict[y][j][LINEXMAX])
                        {
                            if (!foundMax)
                            {
                                maxIndex = j;
                            }
                        }
                        if ((xMax >= SelectedSelectionLineDict[y][j][LINEXMIN] && xMax <= SelectedSelectionLineDict[y][j][LINEXMAX]) || xMax == SelectedSelectionLineDict[y][j][LINEXMIN] - 1)
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
                            xMin = SelectedSelectionLineDict[y][minIndex][LINEXMIN];
                        }
                        if (maxIndexInMiddle)
                        {
                            xMax = SelectedSelectionLineDict[y][maxIndex][LINEXMAX];
                        }

                        _toBeRemovedIndexes = new int[maxIndex - minIndex + 1];
                        for (int index = minIndex; index <= maxIndex; index++)
                        {
                            _toBeRemovedIndexes[index - minIndex] = index;
                        }

                        SelectedSelectionLineDict[y] = RemoveIndexes(SelectedSelectionLineDict[y], _toBeRemovedIndexes);
                    }

                    //add new line to current lines and sort current lines
                    SelectedSelectionLineDict[y] = AddNewLine(SelectedSelectionLineDict[y], xMin, xMax);
                    Sort(SelectedSelectionLineDict[y], LINEXMIN);
                }
            }
        }
        private void BuildActiveSelectionData(Vector3Int[] activeSelectionCellPoses)
        {
            ClearActiveSelectionData();
            foreach (Vector3Int currentCellPos in activeSelectionCellPoses)
            {
                int x, y, xMin, xMax;
                x = currentCellPos.x; y = currentCellPos.y; xMin = x; xMax = x;
                if (!ActiveSelectionLineDict.ContainsKey(y))
                {
                    ActiveSelectionLineDict.Add(y, new int[][] { new int[] { currentCellPos.x, currentCellPos.x } });
                }
                else
                {
                    bool foundMin = false;
                    bool foundMax = false;
                    bool minIndexInMiddle = false;
                    bool maxIndexInMiddle = false;
                    int minIndex = -1;
                    int maxIndex = -1;


                    for (int j = 0; j < ActiveSelectionLineDict[y].Length; j++)
                    {
                        if (j == 0)
                        {
                            if (xMax < ActiveSelectionLineDict[y][0][LINEXMIN] || xMin > ActiveSelectionLineDict[y][ActiveSelectionLineDict[y].Length - 1][LINEXMAX])
                            {
                                if (!(xMax == ActiveSelectionLineDict[y][0][LINEXMIN] - 1) && !(xMin == ActiveSelectionLineDict[y][ActiveSelectionLineDict[y].Length - 1][LINEXMAX] + 1))
                                {
                                    break;
                                }
                            }
                        }

                        if (xMin < ActiveSelectionLineDict[y][j][LINEXMIN])
                        {
                            if (!foundMin)
                            {
                                minIndex = j;
                                foundMin = true;
                            }
                        }
                        if ((xMin >= ActiveSelectionLineDict[y][j][LINEXMIN] && xMin <= ActiveSelectionLineDict[y][j][LINEXMAX]) || xMin == ActiveSelectionLineDict[y][j][LINEXMAX] + 1)
                        {
                            if (!foundMin)
                            {
                                minIndex = j;
                                foundMin = true;
                                minIndexInMiddle = true;
                            }
                        }
                        if (xMax > ActiveSelectionLineDict[y][j][LINEXMAX])
                        {
                            if (!foundMax)
                            {
                                maxIndex = j;
                            }
                        }
                        if ((xMax >= ActiveSelectionLineDict[y][j][LINEXMIN] && xMax <= ActiveSelectionLineDict[y][j][LINEXMAX]) || xMax == ActiveSelectionLineDict[y][j][LINEXMIN] - 1)
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
                            xMin = ActiveSelectionLineDict[y][minIndex][LINEXMIN];
                        }
                        if (maxIndexInMiddle)
                        {
                            xMax = ActiveSelectionLineDict[y][maxIndex][LINEXMAX];
                        }

                        _toBeRemovedIndexes = new int[maxIndex - minIndex + 1];
                        for (int index = minIndex; index <= maxIndex; index++)
                        {
                            _toBeRemovedIndexes[index - minIndex] = index;
                        }

                        ActiveSelectionLineDict[y] = RemoveIndexes(ActiveSelectionLineDict[y], _toBeRemovedIndexes);
                    }

                    //add new line to current lines and sort current lines
                    ActiveSelectionLineDict[y] = AddNewLine(ActiveSelectionLineDict[y], xMin, xMax);
                    Sort(ActiveSelectionLineDict[y], LINEXMIN);
                }
            }
        }
        private void BuildBoxSelectionData(int xMin, int yMin, int xMax, int yMax)
        {
            ClearSelectedSelectionData();
            for (int y = yMin; y <= yMax; y++)
            {
                SelectedSelectionLineDict.Add(y, new int[][] { new int[] { xMin, xMax } });
            }
        }
        private void BuildActiveBoxSelectionData(int xMin, int yMin, int xMax, int yMax)
        {
            ActiveSelectionLineDict.Clear();
            for (int y = yMin; y <= yMax; y++)
            {
                ActiveSelectionLineDict.Add(y, new int[][] { new int[] { xMin, xMax } });
            }
        }
        private void BuildAdditiveBoxSelectionData(int xMin, int yMin, int xMax, int yMax)
        {
            int xMinOriginal = xMin;
            int xMaxOriginal = xMax;

            for (int y = yMin; y <= yMax; y++)
            {
                xMin = xMinOriginal;
                xMax = xMaxOriginal;

                if (SelectedSelectionLineDict.ContainsKey(y))
                {
                    bool foundMin = false;
                    bool foundMax = false;
                    bool minIndexInMiddle = false;
                    bool maxIndexInMiddle = false;
                    int minIndex = -1;
                    int maxIndex = -1;


                    for (int j = 0; j < SelectedSelectionLineDict[y].Length; j++)
                    {
                        if (j == 0)
                        {
                            if (xMax < SelectedSelectionLineDict[y][0][LINEXMIN] || xMin > SelectedSelectionLineDict[y][SelectedSelectionLineDict[y].Length - 1][LINEXMAX])
                            {
                                if (!(xMax == SelectedSelectionLineDict[y][0][LINEXMIN] - 1) && !(xMin == SelectedSelectionLineDict[y][SelectedSelectionLineDict[y].Length - 1][LINEXMAX] + 1))
                                {
                                    break;
                                }
                            }
                        }

                        if (xMin < SelectedSelectionLineDict[y][j][LINEXMIN])
                        {
                            if (!foundMin)
                            {
                                minIndex = j;
                                foundMin = true;
                            }
                        }
                        if ((xMin >= SelectedSelectionLineDict[y][j][LINEXMIN] && xMin <= SelectedSelectionLineDict[y][j][LINEXMAX]) || xMin == SelectedSelectionLineDict[y][j][LINEXMAX] + 1)
                        {
                            if (!foundMin)
                            {
                                minIndex = j;
                                foundMin = true;
                                minIndexInMiddle = true;
                            }
                        }
                        if (xMax > SelectedSelectionLineDict[y][j][LINEXMAX])
                        {
                            if (!foundMax)
                            {
                                maxIndex = j;
                            }
                        }
                        if ((xMax >= SelectedSelectionLineDict[y][j][LINEXMIN] && xMax <= SelectedSelectionLineDict[y][j][LINEXMAX]) || xMax == SelectedSelectionLineDict[y][j][LINEXMIN] - 1)
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
                            xMin = SelectedSelectionLineDict[y][minIndex][LINEXMIN];
                        }
                        if (maxIndexInMiddle)
                        {
                            xMax = SelectedSelectionLineDict[y][maxIndex][LINEXMAX];
                        }

                        _toBeRemovedIndexes = new int[maxIndex - minIndex + 1];
                        for (int index = minIndex; index <= maxIndex; index++)
                        {
                            _toBeRemovedIndexes[index - minIndex] = index;
                        }

                        SelectedSelectionLineDict[y] = RemoveIndexes(SelectedSelectionLineDict[y], _toBeRemovedIndexes);
                    }

                    //add new line to current lines and sort current lines
                    SelectedSelectionLineDict[y] = AddNewLine(SelectedSelectionLineDict[y], xMin, xMax);
                    Sort(SelectedSelectionLineDict[y], LINEXMIN);
                }
                else
                {
                    SelectedSelectionLineDict.Add(y, new int[][] { new int[] { xMin, xMax } });
                }

            }
        }
        public void BuildMovedSelectionData(Vector3Int movedDis)
        {
            Dictionary<int, int[][]> tmpDict = new Dictionary<int, int[][]>();

            foreach (int yi in SelectedSelectionLineDict.Keys)
            {
                for (int i = 0; i < SelectedSelectionLineDict[yi].Length; i++)
                {
                    SelectedSelectionLineDict[yi][i][LINEXMIN] += movedDis.x; 
                    SelectedSelectionLineDict[yi][i][LINEXMAX] += movedDis.x;
                }

                tmpDict.Add(yi + movedDis.y, SelectedSelectionLineDict[yi]);
            }
            
            SelectedSelectionLineDict = tmpDict;
        }

        public void ClearActiveSelectionData()
        {
            ActiveSelectionLineDict.Clear();
        }
        public void ClearSelectedSelectionData()
        {
            SelectedSelectionLineDict.Clear();
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
