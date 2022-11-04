using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    public GameObject ball;
    public float followSpeed;
    public float xRange=8f;
    public float zMaxPos = 12f;
    public float ZMinPos = 0.5f;
    public float maxForce = 12;
    public float minForce = 7;
    public bool isFreeMovement = false;

    // Update is called once per frame
    void Update()
    {
        if (ball == null) return;
        FollowBall();
    }

    //Bot will move to the point where the ball hits the ground when the ball passes the Bot's area
    //Bot will return to the middle of the field when the ball is in the player area
    //Bot only moves within Bot's limited area
    void FollowBall()
    {
        Vector3 wantedPosition;
        if (ball.transform.position.z > 0)
        {
            Vector3 ballHitGround = ball.GetComponent<BallBehaviour>().GetPosHitGround();
            wantedPosition = new Vector3(ballHitGround.x, transform.position.y, ballHitGround.z);
        }
        else
        {
            wantedPosition = new Vector3(0, 2f, 12f);
        }
        
        if (wantedPosition[2] > zMaxPos) wantedPosition[2] = zMaxPos;
        else if (wantedPosition[2] < ZMinPos) wantedPosition[2] = ZMinPos;
        if (wantedPosition[0] > xRange) wantedPosition[0] = xRange;
        else if (wantedPosition[0] < -xRange) wantedPosition[0] = -xRange;
        transform.position = Vector3.MoveTowards(transform.position, wantedPosition, Time.deltaTime * followSpeed);
    }
    //Return the direction of the Bot's ball
    public Vector3 BotDirection()
    {
        Vector3 dir = (new Vector3(Random.Range(-xRange,xRange),1f,-12f) -transform.position).normalized;
        dir[1] = 1;
        return dir;
    }

    //Returns BOT's hitting power
    public float BotHitForce()
    {
        return Random.Range(minForce,maxForce);
    }

    //Set the parameters of the BOT according to the difficulty
    public void SetBotDifficulty(int difficulty)
    {
        xRange += xRange*difficulty * 0.1f;
        minForce -= minForce * difficulty * 0.1f;
        maxForce += maxForce * difficulty * 0.1f;
        followSpeed -= followSpeed * difficulty * 0.2f;
    }
}
