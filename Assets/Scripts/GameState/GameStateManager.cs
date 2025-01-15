using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : GlobalMonoBehaviour
{
    [SerializeField]
    private GameState currentState = GameState.playing;
    [SerializeField]
    private bool pauzed = false;

    private List<GameStateLayer> gameStateLayers = new List<GameStateLayer>();
    private int idCount = 0;

    public void SetCurrentState(GameState newState){
        currentState = newState;
        
        if (newState == GameState.pauzed){
            Pauze();
        }
        UpdateLayers();
    }

    public GameState GetCurrentState(){
        return currentState;
    }

    public void ExitGame(){
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void Update() {
        if (Input.GetKeyDown("escape") && currentState != GameState.menu)
        {
            SetCurrentState(GameState.pauzed);
        }
    }

    void UnPauze(){
        currentState = GameState.playing;
        UpdateLayers();
    }

    public void Pauze(){
        pauzed = !pauzed;
        GameObject.Find("Player").GetComponent<PlayerMotion>().enabled = !pauzed;
        GameObject.Find("Player").GetComponent<PlayerTurning>().enabled = !pauzed;
        GameObject.Find("Player").GetComponent<PlayerEpressHandler>().enabled = !pauzed;
        if (pauzed){
            Time.timeScale = 0;
        }else{
            Time.timeScale = 1;
            UnPauze();
        }
    }

    public string AddLayer(GameStateLayer layer){
        gameStateLayers.Add(layer);
        idCount++;
        layer.UpdateActiveStatus(currentState);
        return idCount.ToBinaryString();
    }

    public void RemoveLayer(string id){
        for (int i = 0; i < gameStateLayers.Count; i++)
        {
            if (gameStateLayers[i].id == id){
                gameStateLayers.RemoveAt(i);
            }
        }
    }

    public void UpdateLayers(){
        for (int i = 0; i < gameStateLayers.Count; i++)
        {
            if (gameStateLayers[i] != null){
                gameStateLayers[i].UpdateActiveStatus(currentState);
            }else{
                gameStateLayers.RemoveAt(i);
            }
        }
    }
}
