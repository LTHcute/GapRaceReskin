using System.Collections;
using UniPay;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public UIManager uIManager;

	public ScoreManager scoreManager;

	[Header("Game settings")]
	[Space(5f)]
	public GameObject player;
	private Sprite[] spriteTable;

	public float followSpeed = 4f;

	public float yDelta = 0.5f;

	public float minPlayerSize = 0.3f;

	public float maxPlayerSize = 1f;

	public float playerGrowingSpeed = 0.05f;

	public float playerShrinkingSpeed = 0.002f;

	[Space(5f)]
	public GameObject leftSide;

	[Space(5f)]
	public GameObject rightSide;

	[Space(5f)]
	public float sidesOpeningSpeed = 0.05f;

	[Space(5f)]
	public float sidesClosingSpeed = 0.002f;

	[Space(5f)]
	public Color[] colorTable;

	[Space(5f)]
	public GameObject obstaclesPrefab;

	[Space(5f)]
	[Range(0.15f, 0.6f)]
	public float delayBetweenObstacles = 0.4f;

	public float minObstacleSpeed = 3f;

	public float maxObstacleSpeed = 8f;

	public float minAplitude = 0.5f;

	public float maxAmplitude = 2f;

	public float minLeftRightSpeed = 1f;

	public float maxLeftRightSpeed = 7f;

	[Space(5f)]
	public bool spawning;

	private float sidesSpeed;

	private Vector3 screenSize;

	private Vector3 destination;

	private Vector2 playerTargetSize;

	private Vector2 leftSideTargetPosition;

	private Vector2 rightSideTargetPosition;

	private GameObject obstacle;

	private float sideXStartPos;

	private float sideXClosePos;

	private float playerSizeChange;

	public static GameManager Instance
	{
		get;
		set;
	}

	public GameObject notication;
   // public Button store;
    public GameObject panelStore;

    private void Awake()
	{
	
        Debug.Log("GameAwwaek");
        Object.DontDestroyOnLoad(this);
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
        Debug.Log("Gamestartt");
        screenSize = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
		Application.targetFrameRate = 60;
		SetSides();
		//ShowPlayer();
	}

	private void Update()
	{
		if(uIManager.gameState == GameState.PLAYING)
		{
			ShowPlayer();
		}
        if (uIManager.gameState == GameState.MENU)
        {
            HidePlayer();
        }
        if (uIManager.gameState != GameState.PLAYING || uIManager.IsButton())
		{
			return;
		}
		if (Input.GetMouseButton(0))
		{
			destination = UnityEngine.Input.mousePosition;
			destination = Camera.main.ScreenToWorldPoint(new Vector3(destination.x, destination.y, 16.7f));
			player.transform.position = Vector3.Lerp(player.transform.position, new Vector3(destination.x, destination.y + yDelta, 16.7f), followSpeed * Time.deltaTime);
			if (!spawning)
			{
				spawning = true;
				CloseSides();
				PlayerGrow();
				AudioManager.Instance.PlayMusic(AudioManager.Instance.gameMusic);
				ScoreManager.Instance.StartCounting();
				StartCoroutine(SpawnObstacle(delayBetweenObstacles));
			}
		}
		if (leftSide.transform.position.x + sideXStartPos < 0.01f)
		{
			CloseSides();
		}
		if (player.transform.localScale.x - minPlayerSize < 0.01f)
		{
			PlayerGrow();
		}
		leftSide.transform.position = Vector2.Lerp(leftSide.transform.position, leftSideTargetPosition, sidesSpeed);
		rightSide.transform.position = Vector2.Lerp(rightSide.transform.position, rightSideTargetPosition, sidesSpeed);
		player.transform.localScale = Vector2.Lerp(player.transform.localScale, playerTargetSize, playerSizeChange);
	}

	private IEnumerator SpawnObstacle(float delay)
	{
		while (spawning)
		{
			obstacle = UnityEngine.Object.Instantiate(obstaclesPrefab);
			obstacle.transform.position = new Vector3(UnityEngine.Random.Range(leftSide.transform.position.x + leftSide.transform.localScale.x / 2f + 0.75f, rightSide.transform.position.x - rightSide.transform.localScale.x / 2f - 0.75f), screenSize.y + 1f);
            Sprite[] obstacleSprites = Resources.LoadAll<Sprite>("ObstacleSprite");
            Sprite[] playerSprites = Resources.LoadAll<Sprite>("PlayerSprite");

            spriteTable = new Sprite[obstacleSprites.Length + playerSprites.Length];

            obstacleSprites.CopyTo(spriteTable, 0);
            playerSprites.CopyTo(spriteTable, obstacleSprites.Length);
			
            Debug.Log("Tổng số sprite: " + spriteTable.Length);
			int spritePos = Random.Range(0, spriteTable.Length);

			obstacle.GetComponent<SpriteRenderer>().sprite = spriteTable[spritePos];
			if(spritePos >= spriteTable.Length/2)
			{
				obstacle.tag = "Player";
			}	
		//	obstacle.GetComponent<SpriteRenderer>().color = colorTable[Random.Range(0, colorTable.Length)];
			obstacle.GetComponent<Obstacle>().InitOBstacle(UnityEngine.Random.Range(minObstacleSpeed, maxObstacleSpeed), UnityEngine.Random.Range(minAplitude, maxAmplitude), UnityEngine.Random.Range(minLeftRightSpeed, maxLeftRightSpeed));
			yield return new WaitForSeconds(delayBetweenObstacles);
		}
	}

	public void ShowPlayer()
	{
		player.SetActive(true);
		player.GetComponent<SpriteRenderer>().enabled = true;
		

        player.transform.localScale = new Vector2(minPlayerSize, minPlayerSize);
		sideXStartPos = screenSize.x + leftSide.transform.localScale.x / 2f;
		sideXClosePos = leftSide.transform.localScale.x / 2f;
	//player.transform.position = new Vector2(0f, -2.5f);
	}
    public void HidePlayer()
    {
        player.SetActive(false);
        player.GetComponent<SpriteRenderer>().enabled = false;
        //	player.GetComponent<SpriteRenderer>().color = colorTable[Random.Range(0, colorTable.Length)];
        //player.transform.localScale = new Vector2(minPlayerSize, minPlayerSize);
        //sideXStartPos = screenSize.x + leftSide.transform.localScale.x / 2f;
        //sideXClosePos = leftSide.transform.localScale.x / 2f;
        //	player.transform.position = new Vector2(0f, -2.5f);
    }

    public void RestartGame()
	{
		StopAllCoroutines();
		if (uIManager.gameState == GameState.PAUSED)
		{
			Time.timeScale = 1f;
		}
		ClearScene();
		ShowPlayer();
		scoreManager.ResetCurrentScore();
		spawning = true;
		uIManager.ShowGameplay();
		AudioManager.Instance.PlayMusic(AudioManager.Instance.gameMusic);
		OpenSides();
		ScoreManager.Instance.StartCounting();
		StartCoroutine(SpawnObstacle(delayBetweenObstacles));
	}

	void ShowNotications()
	{
        notication.SetActive(true);
        CancelInvoke();
        Invoke("HideNotications", 1f);

    }
    void HideNotications()
    {
        notication.SetActive(false);
		

    }

    public void ContinueGame()
	{
		int reviveCount = DBManager.GetCurrency("revive");
		if(reviveCount == 0)
		{
			ShowNotications();

            return;
		}	
        ScoreManager.Instance.ReviveConsum();
        if (uIManager.gameState == GameState.PAUSED)
        {
            Time.timeScale = 1f;
        }
		ShowPlayer();
        spawning = true;
        uIManager.ShowGameplay();
        StartCoroutine(SpawnObstacle(delayBetweenObstacles));

    }	
	public void SetSides()
	{
		leftSide.transform.localScale = new Vector2(screenSize.x, 2f * screenSize.y);
		rightSide.transform.localScale = new Vector2(screenSize.x, 2f * screenSize.y);
		leftSide.transform.position = new Vector3(0f - screenSize.x - leftSide.GetComponent<SpriteRenderer>().bounds.size.x / 2f, 0f, 0f);
		rightSide.transform.position = new Vector3(screenSize.x + rightSide.GetComponent<SpriteRenderer>().bounds.size.x / 2f, 0f, 0f);
	}

	public void OpenSides()
	{
		sidesSpeed = sidesOpeningSpeed;
		leftSideTargetPosition = new Vector2(0f - sideXStartPos, leftSide.transform.position.y);
		rightSideTargetPosition = new Vector2(sideXStartPos, rightSide.transform.position.y);
		//player.GetComponent<SpriteRenderer>().color = colorTable[Random.Range(0, colorTable.Length)];
		PlayerShrink();
	}

	public void CloseSides()
	{
		sidesSpeed = sidesClosingSpeed;
		leftSideTargetPosition = new Vector2(0f - sideXClosePos, leftSide.transform.position.y);
		rightSideTargetPosition = new Vector2(sideXClosePos, rightSide.transform.position.y);
	}

	public void PlayerGrow()
	{
		playerTargetSize = new Vector2(maxPlayerSize, maxPlayerSize);
		playerSizeChange = playerGrowingSpeed;
	}

	public void PlayerShrink()
	{
		playerTargetSize = new Vector2(minPlayerSize, minPlayerSize);
		playerSizeChange = playerShrinkingSpeed;
	}

	public void ClearScene()
	{
		spawning = false;
		StopAllCoroutines();
		GameObject[] array = GameObject.FindGameObjectsWithTag("Obstacle");
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.Destroy(array[i]);
		}
	}

	public void GameOver()
	{
		if (uIManager.gameState == GameState.PLAYING)
		{
			ScoreManager.Instance.StopCounting();
			AudioManager.Instance.PlayEffects(AudioManager.Instance.gameOver);
			AudioManager.Instance.PlayMusic(AudioManager.Instance.menuMusic);
			uIManager.ShowGameOver();
			player.gameObject.GetComponent<SpriteRenderer>().enabled = false;
			scoreManager.UpdateScoreGameover();
		}
	}
    public void Store()
    {
        panelStore.SetActive(true);
        //store.onClick.RemoveAllListeners();
        //store.onClick.AddListener(OpenStore);
    }

    void OpenStore()
    {
        
    }
}
