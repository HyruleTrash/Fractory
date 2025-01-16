using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerEpressHandler : MonoBehaviour {
    [HideInInspector]
    public List<PlayerEpress> playerEpressComponents = new List<PlayerEpress>();

    class PlayerEpressWeight {
        public PlayerEpress playerEpress;
        public float distance;
        public float lookAt;
    }

    private void Start() {
        foreach (Component component in gameObject.GetComponents<Component>())
        {
            if (component is PlayerEpress)
            {
                playerEpressComponents.Add((PlayerEpress)component);
            }
        }
    }

    private void Update() {
        Gamepad gamepad = Gamepad.current;
        if (gamepad != null){
            if (gamepad.aButton.wasPressedThisFrame)
            {
                OnPress();
            }
        }else{
            if (Input.GetKeyDown(KeyCode.E))
            {
                OnPress();
            }
        }
    }

    private void OnPress() {
        List<PlayerEpressWeight> playerEpressWeights = new List<PlayerEpressWeight>(playerEpressComponents.Count);
        foreach (PlayerEpress playerEpress in playerEpressComponents)
        {
            playerEpressWeights.Add(new PlayerEpressWeight() {
                playerEpress = playerEpress,
                distance = playerEpress.GetDistance(),
                lookAt = -playerEpress.GetLookAt()
            });
            
            PlayerEpressWeight playerEpressWeight = playerEpressWeights[playerEpressWeights.Count - 1];
        }

        List<PlayerEpressWeight> orderedPlayerEpressWeights = playerEpressWeights.OrderBy(a => a.lookAt).ThenBy(a => a.distance).ToList();
        orderedPlayerEpressWeights[0].playerEpress.OnPress();
    }
}