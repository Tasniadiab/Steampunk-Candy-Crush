using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class circle : MonoBehaviour
{
    [Header("Board Variables")]
    public int column; 
    public int row; 
    public int previousColumn; 
    public int previousRow; 
    public int targetX;
    public int targetY;
    public bool isMatched = false; 

    private Board board; 
    private GameObject otherCircles;
    private Vector2 firstTouchPosition; 
    private Vector2 finalTouchPosition; 
    private Vector2 tempCirclePos;
    public float swipeAngle;

    // for score counter
    int count;
    public Text countText;
    public Text winText;
    public int numPickups;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>(); 
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        column = targetX;
        row = targetY; 
        previousRow = row; 
        previousColumn = column;

        winText.text = "";
        count = 0;
        SetCount();
    }

    // Update is called once per frame
    void Update()
    { 
        FindMatches(); 
        if(isMatched){
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .2f);
        }
        targetX = column; 
        targetY = row;

        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempCirclePos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempCirclePos, .4f);
        }else
        {
            tempCirclePos = new Vector2(targetX, transform.position.y);
            transform.position = tempCirclePos;
            board.allCircles[column, row] = this.gameObject; 
        }
        if(Mathf.Abs(targetY - transform.position.y) > .1)
        {   tempCirclePos = new Vector2(transform.position.x, targetY );
            transform.position = Vector2.Lerp(transform.position, tempCirclePos, .4f);
        }else
        {
            tempCirclePos = new Vector2(transform.position.x, targetY );
            transform.position = tempCirclePos;
            board.allCircles[column, row] = this.gameObject;
        }
    }
    
    public IEnumerator CheckMoveCo(){
        yield return new WaitForSeconds(.5f); 
        if(otherCircles != null){
            if(!isMatched && !otherCircles.GetComponent<circle>().isMatched){
                otherCircles.GetComponent<circle>().row = row;
                otherCircles.GetComponent<circle>().column = column;
                row = previousRow; 
                column = previousColumn; 
            }else{
                board.DestroyMatches();
                
                // for score counter
                count += 1;
                SetCount();
            }
            otherCircles = null; 
        }
    }

    // for score counter
    void SetCount()
    {
        countText.text = "Score: " + count.ToString();
        if(count >= numPickups)
        {
            winText.text = "You win.";
        }
    }
        
    private void OnMouseDown ()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
        //Debug.Log(firstTouchPosition);
    }

    private void OnMouseUp ()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
        CalculateAngle();
        //Debug.Log(finalTouchPosition);
        MovePieces();
    }

    void CalculateAngle (){
        float xDir = finalTouchPosition.x - firstTouchPosition.x;
        float yDir = finalTouchPosition.y - firstTouchPosition.y;
        swipeAngle = Mathf.Atan2(yDir, xDir) * (180 / Mathf.PI);
        
    }
    void MovePieces (){
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width-1){
            //Right Swipe
            otherCircles = board.allCircles[column + 1, row]; 
            otherCircles.GetComponent<circle>().column -=1; 
            column += 1;
        } else if(swipeAngle > 45 && swipeAngle <= 135 && row < board.height-1){
            //Up Swipe
            otherCircles = board.allCircles[column, row + 1]; 
            otherCircles.GetComponent<circle>().row -=1; 
            row += 1;
        } else if((swipeAngle > 135 || swipeAngle <= -135) && column > 0) {
            //LeftSwipe
            otherCircles = board.allCircles[column - 1, row]; 
            otherCircles.GetComponent<circle>().column +=1; 
            column -= 1;
        } else if(swipeAngle < -45 && swipeAngle >= -135 && row > 0){
            //Down Swipe
            otherCircles = board.allCircles[column, row - 1]; 
            otherCircles.GetComponent<circle>().row +=1; 
            row -= 1;
        }
        StartCoroutine(CheckMoveCo());
    }
    void FindMatches(){
        if (column > 0 && column < board.width -1){
            GameObject leftCircle1 = board.allCircles[column -1, row];
            GameObject rightCircle1 = board.allCircles[column +1, row];
            if(leftCircle1 != null && rightCircle1 != null)
            {
            if (leftCircle1.tag == this.gameObject.tag && rightCircle1.tag == this.gameObject.tag){
                leftCircle1.GetComponent<circle>().isMatched = true;
                rightCircle1.GetComponent<circle>().isMatched = true;
                isMatched = true; 
                }
            }
        } 
        if (row > 0 && column < board.height -1)
        {
            GameObject upCircle1 = board.allCircles[column, row + 1];
            GameObject downCircle1 = board.allCircles[column , row - 1];
            if(upCircle1 != null && downCircle1 != null)
            {

            if (upCircle1.tag == this.gameObject.tag && downCircle1.tag == this.gameObject.tag){
                upCircle1.GetComponent<circle>().isMatched = true;
                downCircle1.GetComponent<circle>().isMatched = true;
                isMatched = true; 
            
            }
            }
        }
        
    }
}