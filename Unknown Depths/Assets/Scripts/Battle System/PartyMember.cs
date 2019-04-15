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
                    StartCoroutine(LearnSkill(temp));
                }
                LevelUp();
            }
        }
    }

    public override void LevelUp()
    {
        MaxHealth = Mathf.FloorToInt(MaxHealth + (growthHealth) * Random.Range(0.95f, 1.05f));
        MaxMagicPoints = Mathf.FloorToInt(MaxMagicPoints + (growthMagic) * Random.Range(0.95f, 1.05f));
        PhysAttack = Mathf.FloorToInt(PhysAttack + (growthPhsyAtk) * Random.Range(0.95f, 1.05f));
        PhysDefense = Mathf.FloorToInt(PhysDefense + (growthPhysDef) * Random.Range(0.95f, 1.05f));
        MagAttack = Mathf.FloorToInt(MagAttack + (growthMagAtk) * Random.Range(0.95f, 1.05f));
        MagDefense = Mathf.FloorToInt(MagDefense + (growthMagDef) * Random.Range(0.95f, 1.05f));
        Speed = Mathf.FloorToInt(Speed + (growthSpeed) * Random.Range(0.95f, 1.05f));
        level++;
    }

    IEnumerator LearnSkill(List<Skill> learned)
    {

        foreach(Skill s in learned)
        {
            yield return new WaitForSeconds(1.0f);
            BattleWindow.Instance.UpdateAction(string.Format("{0} has learned {1}!", EntityName, s.SpellName));
        }
    }
}
