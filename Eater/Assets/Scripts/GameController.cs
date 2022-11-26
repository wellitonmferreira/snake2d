using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public enum Direction
    {
        LEFT, RIGHT, UP, DOWN
    }

    [SerializeField] private Direction moveDirection;

    [SerializeField] private float delayStep;
    [SerializeField] private float step;

    [SerializeField] private Transform head;

    [SerializeField] private List<Transform> tail;

    [SerializeField] private Transform food;
    [SerializeField] private GameObject tailPrefab;

    private Vector3 lastPos;

    public int col = 35;
    public int row = 19;

    [SerializeField] private Text txtScore;
    [SerializeField] private Text txtHiScore;
    private int score;
    private int hiScore;

    [SerializeField] private GameObject panelGameOver;
    [SerializeField] private GameObject panelTitle;

    void Start()
    {
        StartCoroutine("MoveSnake");
        SetFood();
        hiScore = PlayerPrefs.GetInt("hiScore");
        txtHiScore.text = "Hi-Score: " + hiScore.ToString();
        Time.timeScale = 0;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            moveDirection = Direction.LEFT;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            moveDirection = Direction.RIGHT;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            moveDirection = Direction.UP;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            moveDirection = Direction.DOWN;
        }
    }

    IEnumerator MoveSnake()
    {
        yield return new WaitForSeconds(delayStep);
        Vector3 nextPos = Vector3.zero;
        switch(moveDirection)
        {
            case Direction.LEFT:
                nextPos = Vector3.left;
                break;

            case Direction.RIGHT:
                nextPos = Vector3.right;
                break;

            case Direction.UP:
                nextPos = Vector3.up;
                break;

            case Direction.DOWN:
                nextPos = Vector3.down;
                break;
        }

        nextPos *= step;
        lastPos = head.position;
        head.position += nextPos;

        foreach(Transform t in tail)
        {
            Vector3 temp = t.position;
            t.position = lastPos;
            lastPos = temp;
            t.gameObject.GetComponent<CircleCollider2D>().enabled = true;
        }

        StartCoroutine("MoveSnake");
    }

    public void Eat()
    {
        Vector3 tailPosition = head.position;
        if(tail.Count > 0)
        {
            tailPosition = tail[tail.Count - 1].position;
        }

        GameObject temp = Instantiate(tailPrefab, tailPosition, transform.localRotation);
        tail.Add(temp.transform);
        score += 10;
        txtScore.text = "Score: " + score.ToString();
        SetFood();
    }

    void SetFood()
    {
        int x = Random.Range((col - 1) / 2 * -1, (col - 1) / 2);
        int y = Random.Range((row - 1) / 2 * -1, (row - 1) / 2);

        food.position = new Vector2(x * step, y * step);
    }

    public void GameOver()
    {
        panelGameOver.SetActive(true);
        Time.timeScale = 0;
        if(score > hiScore)
        {
            PlayerPrefs.SetInt("hiScore", score);
            txtHiScore.text = "New Hi-Score: " + score.ToString();
        }
    }

    public void Play()
    {
        head.position = Vector3.zero;
        moveDirection = Direction.LEFT;
        foreach(Transform t in tail)
        {
            Destroy(t.gameObject);
        }
        tail.Clear();
        SetFood();
        score = 0;
        txtScore.text = "Score: 0";
        hiScore = PlayerPrefs.GetInt("hiScore");
        txtHiScore.text = "Hi-Score: " + hiScore.ToString();
        panelGameOver.SetActive(false);
        panelTitle.SetActive(false);
        Time.timeScale = 1;
    }
}
