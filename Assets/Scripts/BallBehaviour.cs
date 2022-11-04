using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallBehaviour : MonoBehaviour
{
    public float speed=1f;
    public float force=1f;
    public float time;
    public float radius=0.25f;
    public float bounceForce=1f;
    public float xRange=9f;
    public float zRange=12f;
    public bool isCollision =false;
    public int groundCollision =0;
    public bool isCheck =false;
    public bool isPlayerHit =false;
    private GameManager gameManager;
    private PlayerController playerController;
    private BotController botController;
    private CalculatorScore calculatorScore;
    public Vector3 direction;
    private Vector3 startPos;
    private Vector3 currentPos;
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        direction = new Vector3(0, 2f, transform.position.z > 0 ? -4.5f:4.5f) ;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        botController = GameObject.Find("Bot").GetComponent<BotController>();
        calculatorScore = GetComponent<CalculatorScore>();
        botController.ball = gameObject;
        speed -= gameManager.difficulty * 0.2f * speed;
    }
    // Update is called once per frame
    void Update()
    {
        ProcessPosNotOverlap();
        CheckZone();
        if (isCollision)
        {
            ProcessBounce();
            return;
        }
        DetectCollision();
    }

    //Calculate ball trajectory position over time
    public Vector3 CalculatorPositionByTime(float t)
    {
        return startPos + direction * t * force + 0.5f * Physics.gravity * t * t;
    }
    void DetectCollision()
    {
        Vector3 nextPos = CalculatorPositionByTime((time + Time.deltaTime) * speed);
        float distance = Vector3.Distance(currentPos, nextPos) + radius;
        Ray ray = new Ray(transform.position, transform.TransformDirection((nextPos - transform.position)));
        Debug.DrawRay(transform.position, transform.TransformDirection((nextPos - transform.position)));
        isCollision = Physics.Raycast(ray, out hit, distance);
    }

    //Handling the ball with a collision
    void ProcessBounce()
    {
        startPos = transform.position;

        //If the ball collides with the Player, the direction of the Ball is returned and the force is based on the Player operation
        if (hit.transform.CompareTag("Player"))
        {
            direction = playerController.PlayerDirection();
            force = playerController.PlayerHitForce();
            groundCollision = 0;
            isPlayerHit = true;
        }
        //If the ball collides with the ground, the ball will reflect to the ground with bouncing force
        else if (hit.transform.CompareTag("Ground"))
        {
            groundCollision ++;
            direction = Vector3.Reflect(direction, Vector3.zero) * bounceForce;
        }
        // If the ball collides with the BOT, the direction of the ball will be returned and the force is based on the BOT's AI
        else if (hit.transform.name.Equals("Bot"))
        {
            direction = botController.BotDirection();
            force = botController.BotHitForce();
            groundCollision = 0;
            isPlayerHit = false;
        }
        //If the ball collides with the other, the ball will reflect to the ground with bouncing force
        else
        {
            direction = Vector3.Reflect(direction, hit.normal) * bounceForce;
        }
        time = 0;
        isCollision = false;
    }
    //Set ball position over time
    //Handle the ball that doesn't overlap the ground
    void ProcessPosNotOverlap()
    {
        currentPos = CalculatorPositionByTime(time * speed);
        time += Time.deltaTime;
        if (transform.position.Equals(currentPos)) return;
        if (currentPos.y < radius) currentPos[1] = radius;
        transform.position = currentPos;
    }

    //Calculate the point when the ball leaves the field
    //Calculate the point when the ball hits the ground
    void CheckZone()
    {
        if (isCheck) return;
        if( transform.position.z > zRange
            || transform.position.z < -zRange
            || transform.position.x > xRange
            || transform.position.x < -xRange || isCollision)
        {
            isCheck = calculatorScore.CheckPoint(transform.position , isPlayerHit , groundCollision);
        }
    }
    //Calculate where the ball hits the ground
    public Vector3 GetPosHitGround()
    {
        float t = 0;
        while (true)
        {
            Vector3 pos = CalculatorPositionByTime(t);
            t += Time.deltaTime;
            if (pos.y <= 0) return pos;
        }
    }
}
