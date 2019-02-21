﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity {

    List<Skill> HealingSkills = new List<Skill>();
    

    public void Act()
    {
        int act = Random.Range(0, 2);
        Entity target = BattleController.BATTLE_CONTROLLER.GetRandomPlayer();
        Skill ToCastAttack = null;
        switch (act)
        {
            case 0: // Basic Attack
                BattleController.BATTLE_CONTROLLER.DoAttack(this, target);
                break;
            case 1: // Cast Attack Spell
                ToCastAttack = GetRandomSpell(Skills);
                if(!CastSpell(ToCastAttack, target))
                {
                    BattleController.BATTLE_CONTROLLER.DoAttack(this, target);
                }
                break;
            case 2: // Cast Heal Spell
                Skill ToCastHeal = GetRandomSpell(HealingSkills);
                Entity healTarget = BattleController.BATTLE_CONTROLLER.GetWeakestEnemy();
                if (!CastSpell(ToCastHeal, healTarget))
                {
                    BattleController.BATTLE_CONTROLLER.DoAttack(this, target);
                }
                break;
        }
    }

    Skill GetRandomSpell(List<Skill> s)
    {
        return s[Random.Range(1, s.Count - 1)];
    }


    public override void Die()
    {
        base.Die();
        BattleUIController.BATTLE_UI_CONTROLLER.UpdateAction(string.Format("{0} has been slain!", this.EntityName));
        BattleController.BATTLE_CONTROLLER.ActiveBattleMembers.Remove(this);
    }
}
