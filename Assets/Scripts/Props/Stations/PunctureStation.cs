using UnityEngine;

public class PunctureStation : FractalStation {
    override public void StationTriggered(string tag, TriggerTracker triggerTracker) {
        if (tag == "Player" || tag == null) {
            FractalRenderer renderer = triggerTracker.GetColliderWithClass<FractalRenderer>();
            if (renderer != null) {
                int complexityLimit = FractalTypeUtil.GetData(renderer.type).complexityLimit;
                if (renderer.complexity + 1 > complexityLimit) {
                    renderer.complexity = complexityLimit;
                }else{
                    renderer.complexity++;
                }
                StationFinished();
            }
        }
    }
}