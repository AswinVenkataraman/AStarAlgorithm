using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public int columnCount;
    public int rowCount;
    public int totalCell = 50;

    public GameObject starObj;
    public GameObject endObj;

    public List<GameObject> visitedList;


    private void Awake()
    {
        visitedList = new List<GameObject>();

        columnCount = GetComponent<GridLayoutGroup>().constraintCount;
        rowCount = 50 / columnCount;

        int i = totalCell;
        foreach (Transform grid in transform)
        {
            grid.name = i.ToString();
            grid.transform.Find("CellNo").GetComponent<Text>().text = i.ToString();
            i--;

        }

        foreach (Transform grid in transform)
        {
            GridHolder gridHolder = grid.GetComponent<GridHolder>();
            gridHolder.AssignValues();
        }


        foreach (Transform grid in transform)
        {
            GridHolder gridHolder = grid.GetComponent<GridHolder>();
            gridHolder.findNeighbours();
            if (gridHolder.isStartPosition)
                starObj = gridHolder.gameObject;
            else if(gridHolder.isEndPosition)
                endObj = gridHolder.gameObject;
        }

        // Calling this in delay, since in unity gridlayout takes few seconds to return values.
        Invoke("AssignValues", 0.5f);

    }

    private void AssignValues()
    {
        starObj.GetComponent<GridHolder>().gCost = 0;
        starObj.GetComponent<GridHolder>().hCost = Vector2.Distance(starObj.transform.position, endObj.transform.position);
        starObj.GetComponent<GridHolder>().tCost = starObj.GetComponent<GridHolder>().gCost + starObj.GetComponent<GridHolder>().hCost;
        starObj.GetComponent<GridHolder>().calculatedValues = true;
        StartCoroutine(startCalcuatingPath(starObj));
    }

    public IEnumerator startCalcuatingPath(GameObject currPathObj)
    {
        yield return new WaitForSeconds(0.1f);

        GridHolder currGridHolder = currPathObj.GetComponent<GridHolder>();

        currGridHolder.gameObject.GetComponent<Image>().color = Color.blue;

        if (currPathObj == endObj)
        {
            Debug.Log("Found the End Point");
            tracePath(currPathObj);
            StopAllCoroutines();
            yield break;//Found the path
        }

        currGridHolder.calculateCost();

        GameObject obj =  getShortestNode(currGridHolder.tCost,currGridHolder.adjCell);
        if(obj == null)
        {
            currGridHolder.gameObject.GetComponent<Image>().color = Color.blue;
            yield return new WaitForSeconds(0.1f);

            currGridHolder.gameObject.GetComponent<Image>().color = new Color(0.8773585f, 0.73665f, 0.73665f, 1);
            StartCoroutine(startCalcuatingPath(currGridHolder.prevObj));
            yield break;
        }

        visitedList.Add(currPathObj);
        obj.GetComponent<GridHolder>().prevObj = currGridHolder.gameObject;
        StartCoroutine(startCalcuatingPath(obj));
        yield return 0;

    }

    public GameObject getShortestNode(float currTotalCost, List<GameObject> adjList)
    {
        float highestCost = float.MaxValue;
        GameObject foundObj = null;

        foreach(GameObject go in adjList)
        {

            GridHolder currGridHolder = go.GetComponent<GridHolder>();


            if (visitedList.Contains(go) || currGridHolder.tCost > currTotalCost )
                continue;

            if (currGridHolder.isBlocked)
                continue;

            if (currGridHolder.tCost < highestCost)
            {
                foundObj = currGridHolder.gameObject;
                highestCost = currGridHolder.tCost;
            }

        }

        visitedList.Add(foundObj);

        return foundObj;
    }

    public void tracePath(GameObject currPathObj)
    {
        if (currPathObj == null)
            return;

        currPathObj.GetComponent<Image>().color = Color.green;

        if (currPathObj == starObj)
            return;

        GridHolder currGridHolder = currPathObj.GetComponent<GridHolder>();
        tracePath(currGridHolder.prevObj);
    }
}
