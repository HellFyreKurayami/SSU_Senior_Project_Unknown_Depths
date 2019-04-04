using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMember : Entity {
    
    public override void Die()
    {
        BattleWindow.Instance.UpdateAction(string.Format("{0} has been killed. What the hell was the Cleric doing?!?", this.EntityName));
        //BattleWindow.Instance.ActiveBattleMembers.Remove(this);
    }

    public override void CheckLevelUp()
    {
        base.CheckLevelUp();
    }
}
