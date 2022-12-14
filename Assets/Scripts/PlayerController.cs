using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float xRange=8f;
    public float zMaxPos=-0.5f;
    public float ZMinPos=-12f;
    public GameObject playerModel;
    private GameManager gameManager;
    public Slider slider;
    int forceCounter = 0;
    public int forceMax = 10;
    private Vector3 playerDirection;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.isGameActive) return;
        PlayerMovement();
        LookAtMousePosition();
        Hit();
    }

    //move player with key
    void PlayerMovement() {
        //Get the value from the keyboard
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 pos = playerModel.transform.position + new Vector3(horizontalInput, 0, verticalInput) * speed * Time.deltaTime;

        //Prevent players from leaving their area
        if (Mathf.Abs(pos[0]) > xRange) pos[0] = pos[0] > 0 ? xRange : -xRange;
        if (pos[2] > zMaxPos) pos[2] = zMaxPos;
        if (pos[2] < ZMinPos) pos[2] = ZMinPos;

        transform.position = pos;
    }

    //Change the direction of the ball and the direction of the player in the direction of the mouse
    void LookAtMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 mousePos =hit.point;
            if (mousePos.z < 0) mousePos.z = 0;
            playerDirection = (mousePos - transform.position).normalized;
            playerDirection[1] = 0;
            playerModel.transform.right = playerDirection;
        }
    }

    //Charge force when holding left mouse
    void Hit()
    {
        if (Input.GetMouseButtonDown(0) && forceCounter == 0)
        {
            StartCoroutine(ChangeForce());
        }
    }

    //Return the direction of the Player's ball
    public Vector3 PlayerDirection()
    {
        Vector3 dir = playerDirection;
        dir[1] = 1;
        return dir;
    }

    //Returns Player's hitting power
    public float PlayerHitForce()
    {
        return forceCounter*0.1f*forceMax;
    }

    //Charge force every 0.05s when hold left mouse button
    //Force increases to max and decreases to 0 overtime
    //Force is held for 1s after releasing the mouse and returns to 0 afterwards
    //Show Force slider when change and hide when releasing after 1s
    IEnumerator ChangeForce()
    {
        slider.gameObject.SetActive(true);
        
        slider.value = forceCounter;
        bool isRaise=true;
        while (Input.GetMouseButton(0))
        {
            yield return new WaitForSeconds(0.05f);
            if (isRaise) forceCounter++;
            else forceCounter--;
            slider.value = forceCounter;
            if (forceCounter == 10 || forceCounter == 0) isRaise = !isRaise;
        }
        yield return new WaitForSeconds(1f);
        forceCounter = 0;
        slider.gameObject.SetActive(false);
    }
}
