using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SimulationController : MonoBehaviour
{
    public SquareUIBuilder squareUIBuilder;
    public GameObject squarePrefab;

    private BouncingSquare selectedSquare;
    private List<BouncingSquare> squaresInLevel = new List<BouncingSquare>();
    private bool isStarted = false;
    private bool isPaused = false;

    [SerializeField] private Button startButton = null;
    [SerializeField] private Button pauseButton = null;
    [SerializeField] private Button stopButton = null;

    private void Awake()
    {
        Object[] foundSquares = FindObjectsByType(typeof(BouncingSquare), FindObjectsSortMode.None);
        foreach (Object foundSquare in foundSquares)
        {
            BouncingSquare square = foundSquare as BouncingSquare;
            if (square)
            {
                squaresInLevel.Add(square);
            }
        }

        startButton.onClick.AddListener(HandleSimStart);
        pauseButton.onClick.AddListener(HandleSimPause);
        stopButton.onClick.AddListener(HandleSimStop);

        startButton.interactable = true;
        pauseButton.interactable = false;
        stopButton.interactable = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isStarted)
        {
            HandleSquareSelect();
        }

        if (isStarted)
        {
            CheckForWinner();
        }
    }

    private void HandleSquareSelect()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        BouncingSquare square = hit.transform?.GetComponent<BouncingSquare>();
        if (square)
        {
            selectedSquare?.SetSelected(false);
            square?.SetSelected(true);

            squareUIBuilder?.ClearUI();
            square.CreateUI(squareUIBuilder);

            selectedSquare = square;
        }
    }

    private void CheckForWinner()
    {
        int aliveSquares = 0;
        foreach (BouncingSquare square in squaresInLevel)
        {
            if (square.IsAlive())
                aliveSquares++;
        }

        if (!(aliveSquares > 1))
        {
            Debug.Log(aliveSquares == 1 ? "Someone won :)" : "Draw :(");
            HandleSimStop();
        }
    }

    private void HandleSimStart()
    {
        if (isPaused)
        {
            SetPaused(false);
            return;
        }

        startButton.interactable = false;
        pauseButton.interactable = true;
        stopButton.interactable = true;

        foreach (BouncingSquare square in squaresInLevel)
        {
            square.ApplyModifiers();
            square.SetSelectable(false);
        }

        selectedSquare?.SetSelected(false);
        squareUIBuilder?.ClearUI();

        selectedSquare = null;

        isStarted = true;
    }

    private void HandleSimPause() => SetPaused(true);

    private void HandleSimStop()
    {
        startButton.interactable = true;
        pauseButton.interactable = false;
        stopButton.interactable = false;

        // 1. copy old square properties to new squares
        // 2. delete old squares
        for (int i = squaresInLevel.Count - 1; i >= 0; i--)
        {
            BouncingSquare oldSquare = squaresInLevel[i];
            BouncingSquare newSquare = Instantiate(squarePrefab)?.GetComponent<BouncingSquare>();

            newSquare.name = oldSquare.name;
            newSquare.CopyPropsFrom(oldSquare);

            squaresInLevel.RemoveAt(i);
            squaresInLevel.Add(newSquare);

            Destroy(oldSquare.gameObject);

            newSquare.SetSelectable(true);
        }

        isStarted = false;
        isPaused = false;
        Time.timeScale = 1;

        squareUIBuilder?.AddDefaultUI();
    }

    private void SetPaused(bool state)
    {
        isPaused = state;
        Time.timeScale = (state ? 0 : 1);

        startButton.interactable = state;
        pauseButton.interactable = !state;
    }
}
