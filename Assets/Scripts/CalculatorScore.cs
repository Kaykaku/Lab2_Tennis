using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculatorScore : MonoBehaviour
{

    private GameManager gameManager;
    private BallBehaviour ballBehaviour;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        ballBehaviour = GetComponent<BallBehaviour>();
    }

    public bool CheckPoint(Vector3 pos , bool isPlayerHit , int groundedCount)
    {
        if (gameManager.hasWall)
        {
            if (groundedCount > 1 )
            {
                AddPlayerPoint(false);
                return true;
            }else if (IsTouchWallBack(pos))
            {
                AddPlayerPoint(false);
                return true;
            }
            return false;
        }
        //Add points when the opponent hits the ball off the field with 0 hits to the ground
        if (groundedCount == 0 && !IsInZone(pos))
        {
            AddPlayerPoint(!isPlayerHit);
            return true;
        }

        //Add points when someone hits the ball and the ball hits the ground 1 time in the opponent's area and out of their area
        if (groundedCount == 1 && !IsInZone(pos))
        {
            if (IsPlayerZone(pos) == !isPlayerHit)
            {
                AddPlayerPoint(isPlayerHit);
            }
            else
            {
                AddPlayerPoint(!isPlayerHit);
            }
            return true;
        }

        //Add points when the opponent hits the ball and the ball hits the ground at least 1 time in their area
        if (groundedCount == 1 && IsInZone(pos))
        {
            if (IsPlayerZone(pos) == isPlayerHit)
            {
                AddPlayerPoint(!isPlayerHit);
                return true;
            }
        }

        //Add points when the opponent let the ball hit the ground more than 1 time in their area
        if (groundedCount > 1 && IsInZone(pos))
        {
            if (!IsPlayerZone(pos) == isPlayerHit)
            {
                AddPlayerPoint(isPlayerHit);
                return true;
            }
        }
        
        return false;
    }

    //Check if the location is in the player area
    bool IsPlayerZone(Vector3 pos)
    {
        return pos.z < 0;
    }

    //Check if the position is in the competition  area
    bool IsInZone(Vector3 pos)
    {
        return pos.z <= ballBehaviour.zRange 
            && pos.z >= - ballBehaviour.zRange
            && pos.x <= ballBehaviour.xRange
            && pos.x >= - ballBehaviour.xRange;
    }

    //Check if the position is in the competition  area
    bool IsTouchWallBack(Vector3 pos)
    {
        return pos.z < -15f;
    }

    //Add points for players or bots
    //Destroy the ball
    void AddPlayerPoint(bool isPlayer)
    {
        if (!gameManager.isGameActive) return;
        if (isPlayer) gameManager.AddPlayerScore(1);
        else gameManager.AddBotScore(1);
        Destroy(gameObject,1f);
    }

    //Generate new ball when destroy old ball
    private void OnDestroy()
    {
        gameManager.SpawnBall();
    }
}
