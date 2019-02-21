using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMember : Entity {
    
    public override void Die()
    {
        base.Die();
        BattleUIController.BATTLE_UI_CONTROLLER.UpdateAction(string.Format("{0} has been killed. What the hell was the Cleric doing?!?", this.EntityName));
        BattleController.BATTLE_CONTROLLER.ActiveBattleMembers.Remove(this);
    }
}
