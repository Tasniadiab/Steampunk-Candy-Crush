using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour 
{
    public int width; 
    public int height; 
    public GameObject tilePrefab;
    public GameObject [] circle;
    private BackgroundTile [,] allTiles;
    public GameObject [,] allCircles; 
    

    // Start is called before the first frame update
    void Start ()
    {
        allTiles = new BackgroundTile[width, height];  
        allCircles = new GameObject[width, height]; 
        SetUp();
        
    }
    
    private void SetUp(){
        for (int i = 0; i < width; i ++)
        {
            for (int j = 0; j < height; j ++)
            {
                Vector2 tempPosition = new Vector2(i , j);
                GameObject backgroundTile = Instantiate (tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform; 
                backgroundTile.name = "bg("+ i +"," + j + ")";
                int circleToUse = Random.Range (0, circle.Length); 
                int maxIterations = 0;
                while(MatchesAt(i,j,circle[circleToUse]) && maxIterations < 100){
                    circleToUse = Random.Range(0, circle.Length);
                    maxIterations++; 
                    Debug.Log(maxIterations);
                }
              

                GameObject circles = Instantiate (circle[circleToUse], tempPosition, Quaternion.identity) as GameObject; 
                circles.transform.parent = this.transform; 
                circles.name = "circ("+ i +"," + j + ")";  
                allCircles [i,j] = circles;
            }
        }
    }
    private bool MatchesAt(int column, int row, GameObject piece){
        if(column> 1 && row > 1){
            if(allCircles[column - 1, row].tag == piece.tag && allCircles[column -2, row].tag == piece.tag){
                return true; 
            }
            if(allCircles[column, row-1].tag == piece.tag && allCircles[column, row-2].tag == piece.tag)
            {
                return true; 
            }
        }else if(column <= 1 || row <= 1){
            if(row > 1){
                if(allCircles[column, row -1].tag == piece.tag && allCircles[column, row-2].tag == piece.tag){
                    return true; 
                }
            }
             if(column > 1)
             {
                if(allCircles[column-1, row].tag == piece.tag && allCircles[column-2, row].tag == piece.tag){
                    return true; 
                }
            }

        }


        return false; 
    }
    private void DestroyMatchesAt(int column, int row){
        if(allCircles[column, row].GetComponent<circle>().isMatched){
            Destroy(allCircles[column,row]);
            allCircles[column, row] = null; 
        }
    }
    

    public void DestroyMatches(){
        for (int i = 0; i < width; i ++){
            for (int j = 0; j < height; j++){
                if (allCircles[i, j] != null){
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }
    private IEnumerator DecreaseRowCo(){
        int nullCount = 0; 
        for (int i= 0; i< width; i ++){
            for (int j = 0; j < height; j ++){
                if(allCircles[i,j] == null){
                    nullCount++;
                }else if(nullCount > 0){
                    allCircles[i, j].GetComponent<circle>().row -= nullCount;
                    allCircles[i, j] = null;
                }
            }
            nullCount = 0; 
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard(){
        for(int i = 0; i < width; i ++){
            for (int j = 0; j < height; j ++){
                if(allCircles[i, j] == null){
                    Vector2 tempPosition = new Vector2(i, j);
                    int circleToUse = Random.Range(0, circle.Length);
                    GameObject piece = Instantiate(circle[circleToUse], tempPosition, Quaternion.identity);
                    allCircles[i, j] = piece;
                }
            }
        }
    }

    private bool MatchesOnBoard(){
        for (int i = 0; i < width; i ++){
            for (int j = 0; j < height; j ++){
                if(allCircles[i, j]!= null){
                    if(allCircles[i, j].GetComponent<circle>().isMatched){
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo(){
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while(MatchesOnBoard()){
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
    }

}

