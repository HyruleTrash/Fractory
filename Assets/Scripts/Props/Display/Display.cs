using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Display : MonoBehaviour {
    public GameObject player;
    public InteractPopUp interactPopUp;
    public Camera displayCamera;
    public GameObject fractal;
    private Material renderMaterial;
    private RenderTexture renderTexture;
    public MeshRenderer screen;
    [SerializeField]
    public Shader screenShader;
    public bool isRendering = false;
    private GameStateManager gameStateManager;
    public GameObject[] hintObjects;
    
    [SerializeField]
    public List<DisplayUI> userInterfaces = new List<DisplayUI>();
    [SerializeField]
    private GameObject userInterfacePrefab;
    private float lastInteractTime = 0;

    private void Start() {
        if (fractal != null) {
            fractal.SetActive(false);
        }
        
        gameStateManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
        player = GameObject.FindWithTag("Player");
        PlayerInteract playerInteract = player.GetComponent<PlayerInteract>();
        if (playerInteract != null)
        {
            interactPopUp.distanceThreshold = playerInteract.interactDistance;
        }

        
        GameObject canvas = GameObject.Find("Canvas");
        GameObject userInterface = Instantiate(userInterfacePrefab, canvas.transform);

        for (int i = 0; i < userInterface.transform.childCount; i++)
        {
            GameObject element = userInterface.transform.GetChild(i).gameObject;
            DisplayUI displayUI = element.GetComponent<DisplayUI>();
            if (displayUI != null)
            {
                userInterfaces.Add(displayUI);
                displayUI.gameObject.SetActive(false);
                displayUI.displayScreen = screen.gameObject;
            }
        }

        AfterStart();
    }

    virtual protected void AfterStart(){
        // Override this method to add custom behavior after the interact event.
    }

    private void Update() {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
        PlayerInteract playerInteract = player.GetComponent<PlayerInteract>();
        if (playerInteract != null && 
            MathUtil.GetDistanceXZ(transform.position, player.transform.position) <= playerInteract.interactDistance &&
            playerInteract.nearestInteractable == null)
        {
            playerInteract.onInteract = OnInteract;
            playerInteract.nearestInteractable = gameObject;
        }
        if (!isRendering && screen.enabled == true)
        {
            TurnOff();
        }
        if (screen.enabled == true && Input.GetKeyDown(KeyCode.Escape))
        {
            TurnOff();
            playerInteract.nearestInteractable = null;
            playerInteract.onInteract = null;
        }
    }

    public void OnInteract(){
        if (isRendering)
        {
            if (Time.time - lastInteractTime < 0.5f)
            {
                return;
            }
            TurnOff();
            return;
        }
        if (gameStateManager == null)
        {
            gameStateManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
        }
        gameStateManager.SetCurrentState(GameState.menu);
        lastInteractTime = Time.time;
        isRendering = true;

        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(512, 512, 24);
            renderTexture.Create();
            displayCamera.targetTexture = renderTexture;
        }
        if (renderMaterial == null)
        {
            renderMaterial = new Material(screenShader);
            renderMaterial.mainTexture = renderTexture;
        }else{
            renderMaterial.mainTexture = renderTexture;
        }
        if (screen != null)
        {
            screen.material = renderMaterial;
            screen.enabled = true;
            screen.GetComponent<DisplayScreenGrowShrink>().Grow();
            displayCamera.Render();
        }
        if (fractal != null)
        {
            fractal.SetActive(true);
        }
        removeHintObjects();
        AfterInteract();
    }

    protected void SetUIText(string text){
        foreach (DisplayUI userInterface in userInterfaces)
        {
            TextMeshProUGUI uiText = userInterface.GetComponent<TextMeshProUGUI>();
            if (uiText == null)
                uiText = HierarchyUtil.GetFirstChildWithComponent(userInterface.transform, typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            if (uiText != null)
            {
                uiText.text = text;
                return;
            }
        }
    }

    public void removeHintObjects(){
        foreach (GameObject hintObject in hintObjects)
        {
            hintObject.SetActive(false);
        }
    }

    virtual protected void AfterInteract(){
        if (fractal != null)
        {
            SetUIText("Iterations: " + fractal.GetComponent<FractalRenderer>().complexity + "\n" + "Rounding: " + fractal.GetComponent<FractalRenderer>().bevel);
        }
        foreach (DisplayUI userInterface in userInterfaces)
        {
            userInterface.SetPosition();
            userInterface.gameObject.SetActive(true);
        }
    }
    
    private void ReturnGameState(){
        gameStateManager.SetCurrentState(GameState.playing);
    }

    public void TurnOff(){
        isRendering = false;
        if (fractal != null)
        {
            fractal.SetActive(false);
        }
        screen.GetComponent<DisplayScreenGrowShrink>().Shrink();
        Invoke("ReturnGameState", 1f);
        AfterTurnOff();
    }

    virtual protected void AfterTurnOff(){
        foreach (DisplayUI userInterface in userInterfaces)
            userInterface.gameObject.SetActive(false);
    }
}