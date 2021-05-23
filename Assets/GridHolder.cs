using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridHolder : MonoBehaviour
{
    public int cellNo;
    public List<GameObject> adjCell;

    GridManager currGridMgr;

    public int currCellRow;
    public int currCellColumn;

    public bool isStartPosition;
    public bool isEndPosition;
    public bool isBlocked;
    public bool didChecked;

    // gCost For Each Cell is 10, if diagnoal it is 14
    // hCost is distance formula

    //Cost Related
    public float gCost;
    public float hCost;
    public float tCost;

    public GameObject prevObj;
    public bool calculatedValues = false;

    public void AssignValues()
    {
        adjCell = new List<GameObject>();
        cellNo = int.Parse(name);
        currGridMgr = transform.parent.GetComponent<GridManager>();

        currCellRow = cellNo / currGridMgr.columnCount;
        if (cellNo % currGridMgr.columnCount == 0)
            currCellRow -= 1;

        currCellColumn = cellNo % currGridMgr.columnCount;
        if (cellNo % currGridMgr.columnCount == 0)
            currCellColumn = currGridMgr.columnCount;

        if (isBlocked)
            GetComponent<Image>().color = Color.black;

        if (isStartPosition)
            GetComponent<Image>().color = Color.magenta;

        if (isEndPosition)
            GetComponent<Image>().color = Color.red;
    }

    public void findNeighbours()
    {
        for (int i = -1; i <= 1; i++) // Rows
        {
            for (int j = -1; j <= 1; j++) // Column
            {
                if (i == 0 && j == 0)
                    continue;

                int targetCell = cellNo - (i * 5) + j;


                if (targetCell > currGridMgr.totalCell || targetCell <=0)
                    continue;

                GridHolder targetGridHolder = transform.parent.transform.Find(targetCell.ToString()).GetComponent<GridHolder>();
                int targetCellRow = targetGridHolder.currCellRow;
                int targetCellColumn = targetGridHolder.currCellColumn;

                if (Mathf.Abs(currCellRow - targetCellRow) >= 2 || Mathf.Abs(currCellColumn - targetCellColumn) >= 2)
                    continue;

                Transform obj = transform.parent.Find(targetCell.ToString());

                if (obj != null)
                    adjCell.Add(obj.gameObject);

            }
        }
    }

    public void calculateCost()
    {
        foreach(GameObject go in adjCell)
        {
            GridHolder currGridHolder = go.GetComponent<GridHolder>();

            if (currGridHolder.isBlocked)
                continue;

            currGridHolder.calculatedValues = true;

            currGridHolder.hCost =  Vector2.Distance(currGridHolder.gameObject.transform.position, currGridMgr.endObj.transform.position);

            if (Mathf.Abs(currGridHolder.currCellRow - currCellRow) == 1 && Mathf.Abs(currGridHolder.currCellColumn - currCellColumn) == 1)
                currGridHolder.gCost = gCost+ 14;
            else
                currGridHolder.gCost = gCost+ 10;

            currGridHolder.tCost = currGridHolder.hCost + currGridHolder.gCost;

        }
    }

}
