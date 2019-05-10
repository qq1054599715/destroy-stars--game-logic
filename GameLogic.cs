using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameLogic
{
    public struct Cell
    {
        public int x, y, value, fromX, fromY;
        public bool hasChek;

        public Cell(int x, int y, int value)
        {
            this.x = x;
            this.y = y;
            this.value = value;
            this.fromX = -1;
            this.fromY = -1;
            this.hasChek = false;
        }
    }

    private int width, height;

    public List<Cell> cells;

    public GameLogic(int width, int height, int typeCount)
    {
        cells = new List<Cell>();
        this.width = width;
        this.height = height;
        for (int i = 0; i < this.height; i++)
        {
            for (int j = 0; j < this.width; j++)
            {
                Cell cell = new Cell(j, i, -1);
                cells.Add(cell);
            }
        }

        int countPerType = (this.width * this.height) / typeCount;
        List<int> types = new List<int>();
        for (int i = 0; i < typeCount; i++)
        {
            for (int j = 0; j < countPerType; j++)
            {
                types.Add(i);
            }
        }

        if (types.Count < cells.Count)
        {
            for (int i = types.Count; i < cells.Count; i++)
            {
                types.Add(UnityEngine.Random.Range(0, typeCount));
            }
        }

        for (int i = 0; i < cells.Count; i++)
        {
            var cell = cells[i];
            int rid = UnityEngine.Random.Range(0, types.Count);
            int rvalue = types[rid];
            cell.value = rvalue;
            cells[i] = cell;
            types.RemoveAt(rid);
        }
    }

    /// <summary>
    /// 清除列的空格
    /// </summary>
    /// <param name="col"></param>
    /// <param name="position"></param>
    private void RemoveSpace(List<Cell> col, int position)
    {
        if (col.Count > position)
        {
            Cell cell = col[position];
            if (cell.value == -1)
            {
                col.RemoveAt(position);
                RemoveSpace(col, position);
            }
            else
            {
                RemoveSpace(col, position + 1);
            }
        }
    }

    private void DealSpaceCell(List<Cell> col, ref bool hasInSpace, ref bool allSpace)
    {
        int baseCount = col.Count;
        allSpace = true;

        for (int i = 0; i < col.Count; i++)
        {
            var currentCol = col[i];
            if (currentCol.value != -1)
            {
                allSpace = false;
            }
        }

        if (allSpace != true)
        {
            RemoveSpace(col, 0);
            if (col.Count > 0 && col.Count != baseCount)
            {
                hasInSpace = true;
            }
        }
        else
        {
            hasInSpace = false;
        }
    }

    private List<Cell> neighbors;

    private void SearchNeighbors(Cell cell)
    {
        int x = cell.x;
        int y = cell.y;
        int value = cell.value;
        if (x < width - 1)
        {
            Cell frontCell = cells[y * width + x + 1];
            if (frontCell.value == value && frontCell.hasChek == false)
            {
                frontCell.hasChek = true;
                cells[y * width + x + 1] = frontCell;
                neighbors.Add(frontCell);
                SearchNeighbors(frontCell);
            }
        }

        if (x > 0)
        {
            Cell backCell = cells[y * width + x - 1];

            if (backCell.value == value && backCell.hasChek == false)
            {
                backCell.hasChek = true;
                cells[y * width + x - 1] = backCell;
                neighbors.Add(backCell);
                SearchNeighbors(backCell);
            }
        }

        if (y < height - 1)
        {
            Cell upCell = cells[(y + 1) * width + x];
            if (upCell.value == value && upCell.hasChek == false)
            {
                upCell.hasChek = true;
                cells[(y + 1) * width + x] = upCell;
                neighbors.Add(upCell);
                SearchNeighbors(upCell);
            }
        }

        if (y > 0)
        {
            Cell downCell = cells[(y - 1) * width + x];
            if (downCell.value == value && downCell.hasChek == false)
            {
                downCell.hasChek = true;
                cells[(y - 1) * width + x] = downCell;
                neighbors.Add(downCell);
                SearchNeighbors(downCell);
            }
        }
    }

    /// <summary>
    /// 首先处理removeList
    /// 然后处理movingList
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="removeList"></param>
    /// <param name="movingList"></param>
    /// <returns></returns>
    public bool TryRemoveCell(Cell cell, ref List<Cell> removeList, ref List<List<Cell>> movingList)
    {
        bool effective = false;
        if (cell.value == -1)
        {
            effective = false;
            return effective;
        }

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Cell currentCell = cells[j + i * width];
                currentCell.hasChek = false;
            }
        }

        neighbors = new List<Cell>();
        SearchNeighbors(cell);
        if (neighbors.Count > 0)
        {
            effective = true;
            removeList = neighbors;
            for (int i = 0; i < removeList.Count; i++)
            {
                Cell currentCell = removeList[i];
                int x = currentCell.x;
                int y = currentCell.y;
                //int value = currentCell.value;
                Cell baseCell = cells[x + y * width];
                baseCell.value = -1;
                cells[x + y * width] = baseCell;
            }

            movingList = new List<List<Cell>>();
            int moveCount = 0;
            for (int i = 0; i < width - moveCount; i++)
            {
                List<Cell> singleMovingList = new List<Cell>();
                //检查当前列，是否有内置空格,有内置空格就去除内置空格
                //是否是空列，是空列就让后面的列依次填充
                List<Cell> checkList = new List<Cell>();
                for (int j = 0; j < height; j++)
                {
                    checkList.Add(cells[width * j + i]);
                }

                bool hasInSpace = false;
                bool allSpace = false;

//                string s = "A: ";
//                for (int j = 0; j < checkList.Count; j++)
//                {
//                    s += checkList[j].value.ToString() + " ";
//                }
//
//                Debug.Log(s);


                DealSpaceCell(checkList, ref hasInSpace, ref allSpace);

//                s = "B: ";
//                for (int j = 0; j < checkList.Count; j++)
//                {
//                    s += checkList[j].value.ToString() + " ";
//                }
//
//                Debug.Log(s + hasInSpace.ToString());

                if (allSpace == true)
                {
                    //全是空格
                    for (int k = i + 1; k < width; k++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            Cell currentCell = cells[width * j + k];
                            currentCell.fromX = currentCell.x;
                            currentCell.fromY = currentCell.y;
                            currentCell.x = k - 1;
                            currentCell.y = j;
                            cells[width * j + k - 1] = currentCell;
                            if (currentCell.value != -1)
                            {
                                singleMovingList.Add(currentCell);
                            }
                        }
                    }

                    //最后一列全置空
                    for (int j = 0; j < height; j++)
                    {
                        int position = (width - 1)+ width * j;
                        Cell currentCell = cells[position];
                        currentCell.value = -1;
                        cells[position] = currentCell;
                    }

                    if (singleMovingList.Count > 0)
                    {
                        movingList.Add(singleMovingList);
                    }

                    i--;
                    moveCount++;
                }
                else
                {
                    if (hasInSpace == true)
                    {
                        //有内置空格
                        for (int j = 0; j < height; j++)
                        {
                            if (checkList.Count > 0)
                            {
                                Cell currentCell = checkList[0];
                                currentCell.fromX = currentCell.x;
                                currentCell.fromY = currentCell.y;
                                currentCell.x = currentCell.x;
                                currentCell.y = j;
                                cells[width * j + currentCell.x] = currentCell;
                                checkList.RemoveAt(0);
                                singleMovingList.Add(currentCell);
                            }
                            else
                            {
                                Cell currentCell = cells[width * j + i];
                                currentCell.value = -1;
                                cells[width * j + i] = currentCell;
                                //这里不做位移
                            }
                        }

                        movingList.Add(singleMovingList);
                    }
                }
            }
        }

        return effective;
    }
}
