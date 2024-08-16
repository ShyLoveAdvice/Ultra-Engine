using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.LevelEditor
{
    public class UShapeGetter
    {
        private static List<Vector3Int> _tempCellPosList = new List<Vector3Int>();
        public static Vector3Int[] GetLine(Vector3Int lineStart, Vector3Int lineEnd)
        {
            return GetLine(lineStart.x, lineStart.y, lineEnd.x, lineEnd.y);
        }
        public static Vector3Int[] GetLine(int x1, int y1, int x2, int y2)
        {
            int dx = x2 - x1;
            int dy = y2 - y1;

            if (dx == 0 && dy == 0)
            {
                return new Vector3Int[1] { new Vector3Int(x1, y1) };
            }

            if (dx == 0)
            {
                if (dy > 0)
                {
                    Vector3Int[] cells = new Vector3Int[dy + 1];
                    int index = 0;
                    for (int y = y1; y <= y2; y++)
                    {
                        cells[index] = new Vector3Int(x1, y);
                        index++;
                    }
                    return cells;
                }
                else if (dy < 0)
                {
                    Vector3Int[] cells = new Vector3Int[-dy + 1];
                    int index = 0;
                    for (int y = y1; y >= y2; y--)
                    {
                        cells[index] = new Vector3Int(x1, y);
                        index++;
                    }
                    return cells;
                }
            }

            if (dy == 0)
            {
                if (dx > 0)
                {
                    Vector3Int[] cells = new Vector3Int[dx + 1];
                    int index = 0;
                    for (int x = x1; x <= x2; x++)
                    {
                        cells[index] = new Vector3Int(x, y1);
                        index++;
                    }
                    return cells;
                }
                else if (dx < 0)
                {
                    Vector3Int[] cells = new Vector3Int[-dx + 1];
                    int index = 0;
                    for (int x = x1; x >= x2; x--)
                    {
                        cells[index] = new Vector3Int(x, y1);
                        index++;
                    }
                    return cells;
                }
            }

            float m = Mathf.Abs((float)dy / (float)dx);

            if (Mathf.Abs(m) == 1)
            {
                if (dx > 0 && dy > 0)
                {
                    Vector3Int[] cells = new Vector3Int[dx + 1];
                    for (int i = 0; i <= dx; i++)
                    {
                        cells[i] = new Vector3Int(x1 + i, y1 + i);
                    }
                    return cells;
                }
                else if (dx > 0 && dy < 0)
                {
                    Vector3Int[] cells = new Vector3Int[dx + 1];
                    for (int i = 0; i <= dx; i++)
                    {
                        cells[i] = new Vector3Int(x1 + i, y1 - i);
                    }
                    return cells;
                }
                else if (dx < 0 && dy < 0)
                {
                    Vector3Int[] cells = new Vector3Int[-dx + 1];
                    for (int i = 0; i <= -dx; i++)
                    {
                        cells[i] = new Vector3Int(x1 - i, y1 - i);
                    }
                    return cells;
                }
                else if (dx < 0 && dy > 0)
                {
                    Vector3Int[] cells = new Vector3Int[-dx + 1];
                    for (int i = 0; i <= -dx; i++)
                    {
                        cells[i] = new Vector3Int(x1 - i, y1 + i);
                    }
                    return cells;
                }
            }

            return BRESLine(x1, y1, x2, y2, dx, dy, m);
        }
        public static Vector3Int[] GetBox(Vector3Int boxStartCell, Vector3Int boxEndCell)
        {
            int xMin, xMax, yMin, yMax, boxCellSize, index;
            xMin = boxStartCell.x; yMin = boxStartCell.y;
            xMax = boxEndCell.x; yMax = boxEndCell.y;

            if(xMin > xMax)
            {
                xMin = xMax; xMax = boxStartCell.x;
            }
            if(yMin > yMax)
            {
                yMin = yMax; yMax = boxStartCell.y;
            }

            boxCellSize = (xMax - xMin + 1) * (yMax - yMin + 1);
            Vector3Int[] result = new Vector3Int[boxCellSize];
            index = 0;
            for (int y = yMin; y <= yMax; y++)
            {
                for (int x = xMin; x <= xMax; x++)
                {
                    result[index] = new Vector3Int(x, y);
                    index++;
                }
            }
            return result;
            
        }
        public static Vector3Int[] GetCircleByDiameter(Vector3Int circleEdge1, Vector3Int circleEdge2)
        {
            Vector2Int circleCenter = new Vector2Int(Mathf.RoundToInt((circleEdge1.x + circleEdge2.x) / 2), Mathf.RoundToInt((circleEdge1.y + circleEdge2.y) / 2));
            return BRESCircle(circleCenter.x, circleCenter.y, circleEdge1.x, circleEdge1.y);
        }
        public static Vector3Int[] GetCircleByCenter(Vector3Int circleCenter, Vector3Int circleEdge)
        {
            return BRESCircle(circleCenter.x, circleCenter.y, circleEdge.x, circleEdge.y);
        }
        public static Vector3Int[] GetCircle(Vector3Int circleCenter, int radius)
        {
            return BRESCircle(circleCenter.x, circleCenter.y, radius);
        }
        public static Vector3Int[] GetEllipse(Vector3Int ellipseCenter, int a, int b)
        {
            return MidPointEllipse(ellipseCenter, a, b);
        }
        public static Vector3Int[] GetEllipseByDiagonalLine(Vector3Int dPoint1, Vector3Int dPoint2)
        {
            Vector3Int ellipseCenter = (dPoint1 + dPoint2) / 2;
            int a = Mathf.FloorToInt((Mathf.Abs((dPoint1.x - dPoint2.x) / 2f)));
            int b = Mathf.FloorToInt(Mathf.Abs((dPoint1.y - dPoint2.y) / 2f));
            Debug.Log($"ellipseCenter: {ellipseCenter}");
            Debug.Log($"a: {a}");
            Debug.Log($"b: {b}");
            if( a == 0 || b == 0 )
            {
                return Rect(dPoint1.x, dPoint2.x, dPoint1.y, dPoint2.y);
            }
            return GetEllipse(ellipseCenter, a, b);

        }
        private static Vector3Int[] BRESLine(int x1, int y1, int x2, int y2, int dx, int dy, float m)
        {
            int normalXi = 0;
            int normalYi = 0;

            int normalXEnd = 0;

            int normalDx = 0;
            int normalDy = 0;

            int normalXOutputMult = 1;
            int normalYOutputMult = 1;

            bool outPutSwitchXY = false;


            if (dx > 0 && dy > 0)
            {
                if (m > 0 && m < 1)
                {
                    normalXi = x1;
                    normalYi = y1;

                    normalXEnd = x2;

                    normalDx = dx;
                    normalDy = dy;

                    normalXOutputMult = 1;
                    normalYOutputMult = 1;
                }
                else if (m > 1)
                {
                    normalXi = y1;
                    normalYi = x1;

                    normalXEnd = y2;

                    normalDx = dy;
                    normalDy = dx;

                    normalXOutputMult = 1;
                    normalYOutputMult = 1;

                    outPutSwitchXY = true;
                }
            }
            else if (dx > 0 && dy < 0)
            {
                if (m > 0 && m < 1)
                {
                    normalXi = x1;
                    normalYi = -y1;

                    normalXEnd = x2;

                    normalDx = dx;
                    normalDy = -dy;

                    normalXOutputMult = 1;
                    normalYOutputMult = -1;
                }
                else if (m > 1)
                {
                    normalXi = -y1;
                    normalYi = x1;

                    normalXEnd = -y2;

                    normalDx = -dy;
                    normalDy = dx;

                    normalXOutputMult = 1;
                    normalYOutputMult = -1;

                    outPutSwitchXY = true;
                }
            }
            else if (dx < 0 && dy < 0)
            {
                if (m > 0 && m < 1)
                {
                    normalXi = -x1;
                    normalYi = -y1;

                    normalXEnd = -x2;

                    normalDx = -dx;
                    normalDy = -dy;

                    normalXOutputMult = -1;
                    normalYOutputMult = -1;
                }
                else if (m > 1)
                {
                    normalXi = -y1;
                    normalYi = -x1;

                    normalXEnd = -y2;

                    normalDx = -dy;
                    normalDy = -dx;

                    normalXOutputMult = -1;
                    normalYOutputMult = -1;

                    outPutSwitchXY = true;
                }
            }
            else if (dx < 0 && dy > 0)
            {
                if (m > 0 && m < 1)
                {
                    normalXi = -x1;
                    normalYi = y1;

                    normalXEnd = -x2;

                    normalDx = -dx;
                    normalDy = dy;

                    normalXOutputMult = -1;
                    normalYOutputMult = 1;
                }
                else if (m > 1)
                {
                    normalXi = y1;
                    normalYi = -x1;

                    normalXEnd = y2;

                    normalDx = dy;
                    normalDy = -dx;

                    normalXOutputMult = -1;
                    normalYOutputMult = 1;

                    outPutSwitchXY = true;
                }
            }

            float p = 2 * normalDy - normalDx;

            Vector3Int[] cells = new Vector3Int[normalXEnd - normalXi + 1];

            int index = 0;
            while (normalXi <= normalXEnd)
            {

                if (!outPutSwitchXY)
                {
                    cells[index] = new Vector3Int(normalXOutputMult * normalXi, normalYOutputMult * normalYi);
                }
                else
                {
                    cells[index] = new Vector3Int(normalXOutputMult * normalYi, normalYOutputMult * normalXi);
                }

                index++;
                normalXi++;
                if (p < 0)
                {
                    p = p + 2 * normalDy;
                }
                else
                {
                    p = p + 2 * normalDy - 2 * normalDx;
                    normalYi += 1;
                }
            }

            return cells;
        }
        private static Vector3Int[] BRESCircle(int xc, int yc, int x, int y)
        {
            //int r = Mathf.RoundToInt(Mathf.Sqrt((x - xc) * (x - xc) + (y - yc) * (y - yc)));
            float r = Mathf.Sqrt((x - xc) * (x - xc) + (y - yc) * (y - yc));
            return BRESCircle(xc, yc, r);
        }
        private static Vector3Int[] BRESCircle(int xc, int yc, float r)
        {
            int x, y;
            float d;
            x = 0; y = Mathf.RoundToInt(r); d = 3 - 2 * r;

            _tempCellPosList.Clear();

            while (x < y || x == y)
            {
                _tempCellPosList.Add(new Vector3Int(x + xc, y + yc));
                _tempCellPosList.Add(new Vector3Int(-x + xc, y + yc));
                _tempCellPosList.Add(new Vector3Int(-x + xc, -y + yc));
                _tempCellPosList.Add(new Vector3Int(x + xc, -y + yc));
                _tempCellPosList.Add(new Vector3Int(y + xc, x + yc));
                _tempCellPosList.Add(new Vector3Int(-y + xc, x + yc));
                _tempCellPosList.Add(new Vector3Int(-y + xc, -x + yc));
                _tempCellPosList.Add(new Vector3Int(y + xc, -x + yc));

                x++;
                if (d < 0 || d == 0)
                {
                    d = d + 4 * x + 6;
                }
                else
                {
                    y = y - 1;
                    d = d + 4 * (x - y) + 10;
                }
            }

            return _tempCellPosList.ToArray();
        }
        private static Vector3Int[] BRESCircle(int xc, int yc, int r)
        {
            int x, y, d;
            x = 0; y = r; d = 3 - 2 * r;

            _tempCellPosList.Clear();

            while (x < y || x == y)
            {
                _tempCellPosList.Add(new Vector3Int(x + xc, y + yc));
                _tempCellPosList.Add(new Vector3Int(-x + xc, y + yc));
                _tempCellPosList.Add(new Vector3Int(-x + xc, -y + yc));
                _tempCellPosList.Add(new Vector3Int(x + xc, -y + yc));
                _tempCellPosList.Add(new Vector3Int(y + xc, x + yc));
                _tempCellPosList.Add(new Vector3Int(-y + xc, x + yc));
                _tempCellPosList.Add(new Vector3Int(-y + xc, -x + yc));
                _tempCellPosList.Add(new Vector3Int(y + xc, -x + yc));

                x++;
                if (d < 0 || d == 0)
                {
                    d = d + 4 * x + 6;
                }
                else
                {
                    y = y - 1;
                    d = d + 4 * (x - y) + 10;
                }
            }

            return _tempCellPosList.ToArray();
        }
        private static Vector3Int[] MidPointEllipse(Vector3Int center, int a, int b)
        {
            List<Vector3Int> cellPoses = new List<Vector3Int>();
            int xpos = 0, ypos = b;
            int a2 = a * a, b2 = b * b;
            float d = b2 + a2 * (0.25f - b);
            while (a2 * ypos > b2 * xpos)
            {
                GetAllDimensionsPoses(xpos, ypos, center, ref cellPoses);
                if (d < 0)
                {
                    d = d + b2 * ((xpos << 1) + 3);
                    xpos += 1;
                }
                else
                {
                    d = d + b2 * ((xpos << 1) + 3) + a2 * (-(ypos << 1) + 2);
                    xpos += 1;
                    ypos -= 1;
                }
            }
            d = Mathf.RoundToInt(b2 * (xpos + 0.5f) * (xpos + 0.5f) + a2 * (ypos - 1) * (ypos - 1) - a2 * b2);
            while (ypos >= 0)
            {
                GetAllDimensionsPoses(xpos, ypos, center, ref cellPoses);
                if (d < 0)
                {
                    d = d + b2 * ((xpos << 1) + 2) + a2 * (-(ypos << 1) + 3);
                    xpos += 1;
                    ypos -= 1;
                }
                else
                {
                    d = d + a2 * (-(ypos << 1) + 3);
                    ypos -= 1;
                }
            }

            return cellPoses.ToArray();
        }
        private static void GetAllDimensionsPoses(int xpos, int ypos, Vector3Int center, ref List<Vector3Int> cellPoses)
        {
            cellPoses.Add(new Vector3Int(xpos + center.x, ypos + center.y));
            cellPoses.Add(new Vector3Int(xpos + center.x, -ypos + center.y));
            cellPoses.Add(new Vector3Int(-xpos + center.x, -ypos + center.y));
            cellPoses.Add(new Vector3Int(-xpos + center.x, ypos + center.y));
        }
        private static Vector3Int[] Rect(int x1, int x2, int y1, int y2)
        {
            int maxX = x2 >= x1 ? x2 : x1;
            int maxY = y2 >= y1 ? y2 : y1;
            int minX = maxX == x2 ? x1 : x2;
            int minY = maxY == y2 ? y1 : y2;
            int index = 0;

            Vector3Int[] cellPoses = new Vector3Int[(maxX - minX + 1) * (maxY - minY + 1)];
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    cellPoses[index] = new Vector3Int(x, y);
                    index++;
                }
            }
            return cellPoses;
        }
    }
}
