using UnityEngine;

public class DiceSceneInstaller : SceneInstallerBase
{
    public OutcomeSet outcomeSet;
    public void Update()
    {
        if(Input.GetMouseButtonDown(1)){
            ResponseData responseData = new ResponseData{type = ResponseType.SkillCheck, skillType = EnumSkillDice.knowledge, outcomes = outcomeSet};
            GameContext.SceneEvents.Publish(new SkillCheckRequestedEvent{response = responseData});
            Debug.Log("Published SkillCheckRequestedEvent");
        }
    }
    public override void RegisterServices()
    {
        // Services.Register(new SkillCheckManager());
    }
}
