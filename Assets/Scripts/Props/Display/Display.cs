using UnityEngine;

public class Display : MonoBehaviour {
    public GameObject player;
    public InteractPopUp interactPopUp;
    public Camera displayCamera;
    public GameObject fractal;
    private Material renderMaterial;
    private RenderTexture renderTexture;
    public MeshRenderer screen;
    public bool isRendering = false;
    private float lastInteractTime = 0;

    public void Start() {
        if (fractal != null) {
            fractal.SetActive(false);
        }
        
        player = GameObject.FindWithTag("Player");
        PlayerInteract playerInteract = player.GetComponent<PlayerInteract>();
        if (playerInteract != null)
        {
            interactPopUp.distanceThreshold = playerInteract.interactDistance;
        }
    }

    private void Update() {
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
            renderMaterial = new Material(Shader.Find("Unlit/Texture"));
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
        AfterInteract();
    }

    virtual protected void AfterInteract(){
        // Override this method to add custom behavior after the interact event.
    }
    
    public void TurnOff(){
        isRendering = false;
        if (fractal != null)
        {
            fractal.SetActive(false);
        }
        screen.GetComponent<DisplayScreenGrowShrink>().Shrink();
        AfterTurnOff();
    }

    virtual protected void AfterTurnOff(){
        // Override this method to add custom behavior after the interact event.
    }
}