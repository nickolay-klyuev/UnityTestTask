using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> blocks;
    [SerializeField] private GameObject ball;
    [SerializeField] private GameObject aim;
    [SerializeField] private GameObject restartBtn;
    [SerializeField] private float ballPowerMod;

    private Rigidbody ballRb;
    private Vector3 ballInitPos;
    public static bool isBallLaunched = false;

    public static bool gameOver = false;
    public static bool rewindTime = false;

    // Start is called before the first frame update
    void Start()
    {
        ballRb = ball.GetComponent<Rigidbody>();
        ballRb.useGravity = false;
        ballInitPos = ball.transform.position;

        aim.SetActive(false);
        restartBtn.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver && !rewindTime)
        {
            restartBtn.SetActive(true);
        }

        if (Input.touchCount == 1 && !gameOver && !isBallLaunched)
        {
            aim.SetActive(true);

            Touch touch = Input.GetTouch(0);

            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Mathf.Abs(Camera.main.transform.position.z)));

            aim.transform.position = Camera.main.ScreenToViewportPoint(touch.position) * new Vector2(Screen.width, Screen.height);

            if (touch.phase == TouchPhase.Ended)
            {
                aim.SetActive(false);
                ballRb.useGravity = true;
                ballRb.AddForce((touchWorldPos - ball.transform.position) * ballPowerMod, ForceMode.Impulse);
                isBallLaunched = true;

                StartCoroutine(CheckIfGameOver());
            }
        }
    }

    void FixedUpdate()
    {
        // check if all blocks are rewinded
        if (rewindTime)
        {
            bool isRewindOver = true;
            foreach (GameObject block in blocks)
            {
                if (!block.GetComponent<TimeCapsule>().GetIsRewindOver())
                {
                    isRewindOver = false;
                    break;
                }
            }

            rewindTime = !isRewindOver;
            gameOver = !isRewindOver;
        }
    }

    IEnumerator CheckIfGameOver()
    {
        yield return new WaitForSeconds(1f);

        while (!gameOver)
        {
            List<Vector3> blocksPosition = new List<Vector3>();

            foreach (GameObject block in blocks)
            {
                blocksPosition.Add(block.transform.position);
            }

            yield return new WaitForSeconds(.5f);

            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].transform.position != blocksPosition[i])
                {
                    break;
                }
                gameOver = true;
            }
        }
    }

    public void RestartGame()
    {
        rewindTime = true;

        ballRb.useGravity = false;
        ballRb.velocity = Vector3.zero;
        ball.transform.position = ballInitPos;
        isBallLaunched = false;

        restartBtn.SetActive(false);
    }
}
