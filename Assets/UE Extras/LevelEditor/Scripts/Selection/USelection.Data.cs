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
        /// <summary>
        /// These two dictionaries save selections by using y value as key to access a value describing single or multiple lines in x axis direction
        /// </summary>
        public Dictionary<int, int[][]> ActiveLineDict;
        public Dictionary<int, int[][]> SelectedLineDict;
        public Vector3Int[] SelectedTiles { get => _selectedTiles; }
        public Vector3Int[] _selectedTiles;
        private const int LINEXMIN = 0;
        private const int LINEXMAX = 1;
        private int[] _toBeRemovedIndexes;

        int _lineXMin, _lineXMax;
        private void BuildBoxSelectionData(int xMin, int yMin, int xMax, int yMax)
        {
            ClearSelectionData();
            for (int y = yMin; y <= yMax; y++)
            {
                SelectedLineDict.Add(y, new int[][] { new int[] { xMin, xMax } });
            }
        }
        private void BuildBoxSelectionActiveData(int xMin, int yMin, int xMax, int yMax)
        {
            ActiveLineDict.Clear();
            for (int y = yMin; y <= yMax; y++)
            {
                ActiveLineDict.Add(y, new int[][] { new int[] { xMin, xMax } });
            }
        }
        private void BuildBoxSelectionDataAdditive(int xMin, int yMin, int xMax, int yMax)
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
        private void BuildSelectionData(Vector3Int[] selectedCellPoses)
        {
            ClearSelectionData();
            foreach (Vector3Int currentCellPos in selectedCellPoses)
            {
                int x, y, xMin, xMax;
                x = currentCellPos.x; y = currentCellPos.y; xMin = x; xMax = x;
                if(!SelectedLineDict.ContainsKey(y))
                {
                    SelectedLineDict.Add(y, new int[][] { new int[] { currentCellPos.x, currentCellPos.x } });
                }
                else
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
            }
        }
        public void BuildMovedSelectionData(Vector3Int movedDis)
        {
            Dictionary<int, int[][]> tmpDict = new Dictionary<int, int[][]>();

            foreach (int yi in SelectedLineDict.Keys)
            {
                for (int i = 0; i < SelectedLineDict[yi].Length; i++)
                {
                    SelectedLineDict[yi][i][LINEXMIN] += movedDis.x; 
                    SelectedLineDict[yi][i][LINEXMAX] += movedDis.x;
                }

                tmpDict.Add(yi + movedDis.y, SelectedLineDict[yi]);
            }
            
            SelectedLineDict = tmpDict;
        }

        public void ClearActiveData()
        {
            ActiveLineDict.Clear();
        }
        public void ClearSelectionData()
        {
            SelectedLineDict.Clear();
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

            for (int i = 0; i < newData.Length; i++)
            {
                for (int j = 0; j < newData[i].Length; j++)
                {
                    Debug.Log(newData[i][j]);
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
