using System.Linq;
using UnityEngine;

public class GameStateLayer : MonoBehaviour {
    [SerializeField]
    public GameState[] activeLayer; // The Game state this component and its children should be considered active
    [HideInInspector]
    public string id;
    private GameStateManager gameStateManager;

    
    private void Start() {
        gameStateManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
        if (RequiredComponentsFound()){
            id = gameStateManager.AddLayer(this);
        }
    }

    protected bool RequiredComponentsFound()
    {
        gameStateManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
        if (gameStateManager){
            return true;
        }else{
            return false;
        }
    }

    public void UpdateActiveStatus(GameState state){
        gameObject.SetActive(FindState(state));
    }

    public bool FindState(GameState state){
        for (int i = 0; i < activeLayer.Count(); i++)
        {
            if (activeLayer[i] == state){
                return true;
            }
        }
        return false;
    }
}