using UnityEngine;

public class BevelStation : FractalStation {
    public float BevelAmount;

    override public void StationTriggered(string tag, TriggerTracker triggerTracker) {
        if (tag == "Player") {
            FractalRenderer renderer = triggerTracker.GetColliderWithClass<FractalRenderer>();
            if (renderer != null) {
                float increment = BevelAmount;
                float bevelLimit = FractalTypeUtil.GetData(renderer.type).BevelLimit;
                if (renderer.bevel + increment > bevelLimit) {
                    renderer.bevel = bevelLimit;
                }else{
                    renderer.bevel += increment;
                }

                // Special interactions may go here
                if (renderer.type == FractalType.Cube && renderer.bevel == bevelLimit) {
                    renderer.type = FractalType.Sphere;
                }
                StationFinished();
            }
        }
    }
}