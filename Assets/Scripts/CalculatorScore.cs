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
        if (groundedCount == 0 && !IsInZone(pos))
        {
            AddPlayerPoint(!isPlayerHit);
            return true;
        }

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

        if (groundedCount == 1 && IsInZone(pos))
        {
            if (IsPlayerZone(pos) == isPlayerHit)
            {
                AddPlayerPoint(!isPlayerHit);
                return true;
            }
        }

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

    bool IsPlayerZone(Vector3 pos)
    {
        return pos.z < 0;
    }

    bool IsInZone(Vector3 pos)
    {
        return pos.z <= ballBehaviour.zRange 
            && pos.z >= - ballBehaviour.zRange
            && pos.x <= ballBehaviour.xRange
            && pos.x >= - ballBehaviour.xRange;
    }

    void AddPlayerPoint(bool isPlayer)
    {
        if (isPlayer) gameManager.AddPlayerScore(1);
        else gameManager.AddBotScore(1);
        Destroy(gameObject,1f);
    }

    private void OnDestroy()
    {
        gameManager.SpawnBall();
    }
}
