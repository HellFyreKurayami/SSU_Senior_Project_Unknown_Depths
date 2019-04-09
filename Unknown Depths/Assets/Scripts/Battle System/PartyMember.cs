using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMember : Entity {

    [SerializeField]
    private float growthHealth;
    [SerializeField]
    private float growthMagic;
    [SerializeField]
    private float growthPhsyAtk;
    [SerializeField]
    private float growthPhysDef;
    [SerializeField]
    private float growthMagAtk;
    [SerializeField]
    private float growthMagDef;
    [SerializeField]
    private float growthSpeed;

    public override void Die()
    {
        BattleWindow.Instance.UpdateAction(string.Format("{0} has been killed. What the hell was the Cleric doing?!?", this.EntityName));
        //BattleWindow.Instance.ActiveBattleMembers.Remove(this);
    }

    public override void CheckLevelUp()
    {
        int toNextLevel = Mathf.FloorToInt((level) * (30 * (Mathf.Pow(level + 1, 1.5f))));
        int nextLevel = level;
        while (experience > toNextLevel)
        {
            nextLevel++;
            toNextLevel = Mathf.FloorToInt((nextLevel) * (30 * (Mathf.Pow(nextLevel + 1, 1.5f))));
        }
        if (level < nextLevel)
        {
            //Debug.Log(string.Format("Previous {0} New {1}", level, nextLevel));
            BattleWindow.Instance.UpdateAction(string.Format("{0} has leveled up to level {1}!", EntityName, nextLevel));
            for (int i = level; i < nextLevel; i++)
            {
                //Debug.Log(i);
                List<Skill> temp = Skills.FindAll(x => x.UnlockLevel == i + 1);
                if (temp.Count > 0)
                {
                    foreach (Skill s in temp)
                    {
                        StartCoroutine(LearnSkill(s));
                    }
                }
                LevelUp();
            }
        }
    }

    public override void LevelUp()
    {
        MaxHealth = Mathf.FloorToInt(MaxHealth+((level-1)*growthHealth)*Random.Range(0.95f, 1.05f));
        MaxMagicPoints = Mathf.FloorToInt(MaxMagicPoints + ((level - 1) * growthMagic) * Random.Range(0.95f, 1.05f));
        PhysAttack = Mathf.FloorToInt(PhysAttack + ((level - 1) * growthPhsyAtk) * Random.Range(0.95f, 1.05f));
        PhysDefense = Mathf.FloorToInt(PhysDefense + ((level - 1) * growthPhysDef) * Random.Range(0.95f, 1.05f));
        MagAttack = Mathf.FloorToInt(MagAttack + ((level - 1) * growthMagAtk) * Random.Range(0.95f, 1.05f));
        MagDefense = Mathf.FloorToInt(MagDefense + ((level - 1) * growthMagDef) * Random.Range(0.95f, 1.05f));
        Speed = Mathf.FloorToInt(Speed + ((level - 1) * growthSpeed) * Random.Range(0.95f, 1.05f));
        level++;
    }

    IEnumerator LearnSkill(Skill s)
    {
        yield return new WaitForSeconds(2.0f);
        BattleWindow.Instance.UpdateAction(string.Format("{0} has learned {1}!", EntityName, s.SpellName));
    }
}
