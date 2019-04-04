using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity {

    public List<Skill> HealingSkills = new List<Skill>();
    public int droppedEXP;

    public void Act()
    {
        int act = Random.Range(0, 2);
        Entity target = BattleWindow.Instance.GetRandomPlayer();
        Skill ToCastAttack = null;
        switch (act)
        {
            case 0: // Basic Attack
                BattleWindow.Instance.DoAttack(this, target);
                break;
            case 1: // Cast Attack Spell
                ToCastAttack = GetRandomSpell(Skills);
                if(!CastSpell(ToCastAttack, target))
                {
                    BattleWindow.Instance.DoAttack(this, target);
                }
                break;
            case 2: // Cast Heal Spell
                Skill ToCastHeal = GetRandomSpell(HealingSkills);
                Entity healTarget = BattleWindow.Instance.GetWeakestEnemy();
                if (!CastSpell(ToCastHeal, healTarget))
                {
                    BattleWindow.Instance.DoAttack(this, target);
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
        BattleWindow.Instance.GiveEXP(droppedEXP);
        BattleWindow.Instance.UpdateAction(string.Format("{0} has been slain!", this.EntityName));
        BattleWindow.Instance.ActiveBattleMembers.Remove(this);
        Destroy(this.gameObject);
        base.Die();
    }
}
