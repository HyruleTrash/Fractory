using UnityEngine;

public class Display : MonoBehaviour {
    public TriggerTracker triggerTracker;
    [SerializeField]
    protected Camera displayCamera;
    public GameObject fractal;
    private Material renderMaterial;
    private RenderTexture renderTexture;
    public MeshRenderer screen;
    private bool isRendering = false;
    private float lastInteractTime = 0;

    public void Start() {
        if (fractal != null) {
            fractal.SetActive(false);
        }
    }

    private void Update() {
        PlayerInteract playerInteract = triggerTracker.GetColliderWithClass<PlayerInteract>();
        if (playerInteract != null)
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
            renderTexture = new RenderTexture(256, 256, 24);
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