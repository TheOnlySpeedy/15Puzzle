using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlidePuzzle : MonoBehaviour
{
    public int width = 4;
    public int height = 4;
    public int shuffleCount = 250;
    public int maxShuffleFramePause = 30;

    float blockSize = 2f;
    bool blockInput = false;
    bool isShuffeling = false;
    int shuffleCounter;
    int shuffleFramePauseCounter = 0;
    int minShuffleFramePause = 5;
    int shuffleFramePause;
    bool newGame = true;
    bool gameFinished = false;
    int moveCount;
    float timer;

    public int MoveCount { 
        get {
            return moveCount;
        }
        private set {
            moveCount = value;
            movesText.text = "Moves: " + moveCount;
        }
    }

    SlideBlock[] blocks;

    public SlideBlock slideBlockPrefab;
    public Text movesText;
    public Text timerText;
    public Text victoryText;

    void Start() {
        // GenerateBlocks();
        StartShuffleBlocks();
    }

    void Update()
    {
        if (isShuffeling) {
            if (shuffleFramePauseCounter <= 0) {
                shuffleFramePauseCounter = shuffleFramePause;
                DoShuffle();
                shuffleFramePause = Mathf.Max(minShuffleFramePause, maxShuffleFramePause * (shuffleCount - shuffleCounter) / shuffleCount);
            } else {
                shuffleFramePauseCounter--;
            }
        } else if (!blockInput) {
            if (Input.GetKeyUp(KeyCode.Space)) {
                GenerateBlocks();
            }
            if (Input.GetKeyUp(KeyCode.R)) {
                StartShuffleBlocks();
            }

            if (!newGame && !gameFinished) {
                UpdateTimer();
            }

            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W)) {
                Move(Vector3.forward);
            } else if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S)) {
                Move(Vector3.back);
            } else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A)) { 
                Move(Vector3.left);
            } else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D)) {
                Move(Vector3.right);
            }
        }
    }

    void ClearBlocks() {
        if (blocks != null && blocks.Length > 0) {
            foreach (SlideBlock block in blocks)
            {
                if (block != null) {
                    Destroy(block.gameObject);
                }
            }
        }
    }

    void GenerateBlocks() {
        ResetGame();
        ClearBlocks();
        blocks = new SlideBlock[(width*height)-1];
        float startX = -((width-1) * blockSize)/2;
        float startY = ((height-1) * blockSize)/2;
        int startNumber = 1;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (x + 1 < width || y + 1 < height) {
                    SlideBlock slideBlock = Instantiate(slideBlockPrefab, new Vector3(
                        (startX + x * blockSize),
                        0.25f,
                        (startY - y * blockSize)
                    ), Quaternion.identity) as SlideBlock;
                    Text number = slideBlock.GetComponentInChildren<Text>();
                    number.text = startNumber.ToString();
                    blocks[startNumber-1] = slideBlock;
                    startNumber++;
                }
            }
        }
    }

    void StartShuffleBlocks() {
        blockInput = true;
        GenerateBlocks();
        shuffleCounter = 0;
        isShuffeling = true;
        shuffleFramePause = maxShuffleFramePause;
    }

    void DoShuffle() {
        bool doneShuffle = false;
        Vector3[] directions = new Vector3[4] {Vector3.forward, Vector3.back, Vector3.left, Vector3.right};
        do {
            int rand = Random.Range(0, directions.Length);
            Vector3 dir = directions[rand];
            bool didMove = Move(dir);
            if (didMove) {
                doneShuffle = true;
                shuffleCounter++;
            } else {
                Vector3[] newDirections = new Vector3[directions.Length - 1];
                int counter = 0;
                for (int i = 0; i < directions.Length; i++)
                {
                    if (i != rand) {
                        newDirections[counter] = directions[i];
                        counter++;
                    }
                }
                directions = newDirections;
            }
        } while(!doneShuffle);

        if (shuffleCounter >= shuffleCount) {
            isShuffeling = false;
            blockInput = false;
            MoveCount = 0;
        }
    }

    bool Move(Vector3 dir) {
        foreach (SlideBlock block in blocks) {
            if (block.CheckCanMove(dir)) {
                MoveBlock(block, dir);
                return true;
            }
        }
        return false;
    }

    void MoveBlock(SlideBlock block, Vector3 dir) {
        block.transform.position += dir * blockSize;
        if (!isShuffeling) {   
            if (newGame) {
                newGame = false;
            }
            MoveCount++;
            CheckFinished();
        }
    }

    void UpdateTimer() {
        timer += Time.deltaTime;
        int minutes = (int) Mathf.Floor(timer / 60);
        int seconds = (int) Mathf.Floor(timer % 60);
        int fraction = (int) (timer * 1000) % 1000;

        if (minutes > 0) {
            timerText.text = string.Format ("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction);
        } else {
            timerText.text = string.Format ("{0:00}:{1:000}", seconds, fraction);
        }
    }

    void CheckFinished() {
        foreach (SlideBlock block in blocks)
        {
            if (!block.IsCorrect()) {
                return;
            }
        }
        FinishGame();
    }

    void FinishGame() {
        ClearBlocks();
        victoryText.enabled = true;
        gameFinished = true;
    }

    void ResetGame() {
        victoryText.enabled = false;
        newGame = true;
        gameFinished = false;
        MoveCount = 0;
        timer = 0;
        timerText.text = "Timer: 0:000";
    }

}
