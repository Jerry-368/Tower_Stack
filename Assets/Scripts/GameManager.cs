using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [Header("Block Setup")]
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform towerParent;
    [SerializeField] private Transform baseBlock;

    [Header("Camera")]
    [SerializeField] private CameraFollow cameraFollow;

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameUIPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;

    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI perfectText;

    [Header("Background")]
    [SerializeField] private Renderer backgroundRenderer;

    [Header("Perfect Settings")]
    [SerializeField] private float perfectThreshold = 0.15f;

    [Header("Particles")]
    [SerializeField] private GameObject perfectParticles;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip placeSound;
    [SerializeField] private AudioClip perfectSound;
    [SerializeField] private AudioClip failSound;

    // 🎨 COLORS
    private Color[] palette = new Color[]
    {
        new Color32(255, 111, 145, 255),
        new Color32(155, 93, 229, 255),
        new Color32(74, 222, 128, 255),
        new Color32(250, 204, 21, 255)
    };

    private BlockMover currentBlock;
    private Transform lastBlock;

    private float blockWidth;
    private float blockHeight;
    private float blockDepth;

    private int score;
    private bool gameStarted;
    private bool gameOver;

    private int lastColorChangeScore;

    void Start()
    {
        ShowMenu();

        if (perfectText != null)
            perfectText.gameObject.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        if (!gameStarted || gameOver) return;

        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            PlaceBlock();
        }
    }

    void ShowMenu()
    {
        Time.timeScale = 1f;

        mainMenuPanel.SetActive(true);
        gameUIPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        gameStarted = false;
        gameOver = false;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;

        mainMenuPanel.SetActive(false);
        gameUIPanel.SetActive(true);
        gameOverPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (scoreText != null)
            scoreText.gameObject.SetActive(true);

        if (cameraFollow != null)
            cameraFollow.ResetCamera();

        ResetGame();
    }

    void ResetGame()
    {
        gameStarted = true;
        gameOver = false;

        score = 0;
        lastColorChangeScore = 0;

        if (scoreText != null)
            scoreText.text = "0";

        // Clear old blocks
        foreach (Transform child in towerParent)
        {
            if (child != baseBlock)
                Destroy(child.gameObject);
        }

        lastBlock = baseBlock;

        Vector3 baseScale = baseBlock.localScale;

        blockWidth = baseScale.x;
        blockHeight = baseScale.y;
        blockDepth = baseScale.z;

        spawnPoint.position = new Vector3(
            spawnPoint.position.x,
            lastBlock.position.y + blockHeight,
            spawnPoint.position.z
        );

        SpawnBlock();
    }

    void SpawnBlock()
    {
        GameObject block = Instantiate(blockPrefab, spawnPoint.position, Quaternion.identity, towerParent);

        currentBlock = block.GetComponent<BlockMover>();

        // Apply color (base stays gold, others random)
        Renderer renderer = block.GetComponent<Renderer>();
        renderer.material = new Material(renderer.material);
        renderer.material.color = palette[Random.Range(0, palette.Length)];

        // Apply correct size
        block.transform.localScale = new Vector3(blockWidth, blockHeight, blockDepth);

        spawnPoint.position += Vector3.up * blockHeight;
    }

    void PlaceBlock()
    {
        if (currentBlock == null) return;

        currentBlock.StopBlock();

        if (audioSource != null)
            audioSource.PlayOneShot(placeSound);

        float offset = currentBlock.transform.position.x - lastBlock.position.x;

        // ✅ PERFECT HIT
        if (Mathf.Abs(offset) <= perfectThreshold)
        {
            PerfectPlacement();
            score += 3;
        }
        else
        {
            CutBlock(offset);

            if (gameOver) return;

            score += 1;
        }

        if (scoreText != null)
            scoreText.text = score.ToString();

        ChangeBackgroundColor();

        lastBlock = currentBlock.transform;

        SpawnBlock();
    }

    void PerfectPlacement()
    {
        Vector3 pos = currentBlock.transform.position;
        pos.x = lastBlock.position.x;
        currentBlock.transform.position = pos;

        if (audioSource != null)
            audioSource.PlayOneShot(perfectSound);

        if (perfectParticles != null)
            Instantiate(perfectParticles, pos, Quaternion.identity);

        StartCoroutine(ShowPerfectText());
    }

    void CutBlock(float offset)
    {
        float overlap = blockWidth - Mathf.Abs(offset);

        if (overlap <= 0)
        {
            GameOver();
            return;
        }

        float cutSize = blockWidth - overlap;

        Vector3 cutPos = currentBlock.transform.position;
        cutPos.x += (offset > 0 ? 1 : -1) * (overlap / 2 + cutSize / 2);

        GameObject cutPiece = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cutPiece.transform.position = cutPos;
        cutPiece.transform.localScale = new Vector3(cutSize, blockHeight, blockDepth);

        cutPiece.AddComponent<Rigidbody>();
        Destroy(cutPiece, 2f);

        // Update width
        blockWidth = overlap;

        currentBlock.transform.localScale = new Vector3(blockWidth, blockHeight, blockDepth);

        Vector3 newPos = currentBlock.transform.position;
        newPos.x -= offset / 2f;
        currentBlock.transform.position = newPos;
    }

    IEnumerator ShowPerfectText()
    {
        if (perfectText == null) yield break;

        perfectText.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.6f);

        perfectText.gameObject.SetActive(false);
    }

    void ChangeBackgroundColor()
    {
        if (score / 10 > lastColorChangeScore)
        {
            lastColorChangeScore = score / 10;

            if (backgroundRenderer != null)
            {
                backgroundRenderer.material.color =
                    Random.ColorHSV(0f, 1f, 0.2f, 0.4f, 0.9f, 1f);
            }
        }
    }

    void GameOver()
    {
        if (gameOver) return;

        gameOver = true;

        if (audioSource != null)
            audioSource.PlayOneShot(failSound);

        if (scoreText != null)
            scoreText.gameObject.SetActive(false);

        gameUIPanel.SetActive(false);
        gameOverPanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = score.ToString();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        gameOverPanel.SetActive(false);
        gameUIPanel.SetActive(true);

        if (scoreText != null)
            scoreText.gameObject.SetActive(true);

        if (cameraFollow != null)
            cameraFollow.ResetCamera();

        ResetGame();
    }

    public void GoToMenu()
    {
        ShowMenu();
    }

    public void PauseGame()
    {
        if (!gameStarted || gameOver) return;

        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}