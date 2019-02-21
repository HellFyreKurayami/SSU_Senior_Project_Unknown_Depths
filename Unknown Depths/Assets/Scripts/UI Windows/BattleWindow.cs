using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWindow : GenericWindow {
    [SerializeField]
    private BattleSpawnPoint[] SpawnPoints;

    public List<Entity> ActiveBattleMembers = new List<Entity>();
    public int currentTurn = 0;
    public Skill playerSelected;
    public bool playerAttacking;

    public delegate void BattleOver(bool playerWin);
    public BattleOver battleOverCall;

    [SerializeField]
    private BattleUIController UI;

    public void StartBattle(List<Entity> c, List<Entity> e)
    {
        for (int i = 0; i < c.Count; i++)
        {
            ActiveBattleMembers.Add(SpawnPoints[i + 4].spawn(c[i]));
        }
        for (int i = 0; i < e.Count; i++)
        {
            ActiveBattleMembers.Add(SpawnPoints[i].spawn(e[i]));
        }

        ActiveBattleMembers.Sort((x1, x2) => x1.Speed.CompareTo(x2.Speed));
        ActiveBattleMembers.Reverse();
        Debug.Log(string.Format("Current Turn: {0} with {1} as the turn entity", currentTurn, GetCurrentCharacter().EntityName));
        UI.UpdateAction(string.Format("What Will {0} Do?", GetCurrentCharacter().EntityName));
        UpdateCharUI();
    }

    private void NextTurn()
    {
        if (currentTurn + 1 > ActiveBattleMembers.Count - 1)
        {
            currentTurn = 0;
        }
        else
        {
            currentTurn += 1;
        }
    }

    private void NextAct()
    {
        List<Entity> c = ActiveBattleMembers.FindAll(x => x.Chara.Equals(CharaType.PLAYER));
        List<Entity> e = ActiveBattleMembers.FindAll(x => x.Chara.Equals(CharaType.ENEMY));
        if (c.Count > 0 && e.Count > 0)
        {
            NextTurn();
            Debug.Log(string.Format("Current Turn: {0} with {1} as the turn entity", currentTurn, GetCurrentCharacter().EntityName));
        }
        else
        {
            if (c.Count == 0)
            {
                Debug.Log("Battle Has Ended");
                UI.UpdateAction("And they were never heard from again...");
                battleOverCall(false);
            }
            else if (e.Count == 0)
            {
                Debug.Log("Battle Has Ended");
                battleOverCall(true);
            }
        }

        if (GetCurrentCharacter().Chara.Equals(CharaType.PLAYER))
        {
            UI.ToggleActionState(true);
            UI.BuildSpellList(GetCurrentCharacter().Skills);
            UI.UpdateAction(string.Format("What Will {0} Do?", GetCurrentCharacter().EntityName));
        }
        else
        {
            UI.ToggleActionState(false);
            StartCoroutine(PerformAct());
        }
    }

    IEnumerator PerformAct()
    {
        //yield return new WaitForSeconds(3.0f);
        if (ActiveBattleMembers[currentTurn].CurrentHealth > 0)
        {
            GetCurrentCharacter().GetComponent<Enemy>().Act();
        }
        UpdateCharUI();
        yield return new WaitForSeconds(3.0f);
        NextAct();
    }

    public void SelectCharacter(Entity e)
    {
        if (playerAttacking)
        {
            DoAttack(GetCurrentCharacter(), e);
            playerAttacking = false;
            Invoke("NextAct", 3);
            //NextAct();
        }
        else if (playerSelected != null)
        {
            if (GetCurrentCharacter().CastSpell(playerSelected, e))
            {
                UpdateCharUI();
                Invoke("NextAct", 3);
                //NextAct();
            }
            else
            {
                Debug.LogWarning("ERROR: Not Enough Mana to Cast " + playerSelected.SpellName);
            }
        }
        new WaitForSeconds(2);
    }

    public void DoAttack(Entity caster, Entity target)
    {
        caster.CastSpell(caster.Skills[0], target);
    }

    public Entity GetRandomPlayer()
    {
        List<Entity> c = ActiveBattleMembers.FindAll(x => x.Chara.Equals(CharaType.PLAYER));
        return c[Random.Range(0, c.Count - 1)];
    }

    public Entity GetWeakestEnemy()
    {

        List<Entity> e = ActiveBattleMembers.FindAll(x => x.Chara.Equals(CharaType.ENEMY));
        Entity weakest = e[0];
        foreach (Entity x in e)
        {
            if (x.CurrentHealth < weakest.CurrentHealth)
            {
                weakest = x;
            }
        }
        return weakest;
    }

    public Entity GetCurrentCharacter()
    {
        return ActiveBattleMembers[currentTurn];
    }

    public void UpdateCharUI()
    {
        List<Entity> c = ActiveBattleMembers.FindAll(x => x.Chara.Equals(CharaType.PLAYER));
        for (int i = 0; i < c.Count; i++)
        {
            Entity p = c[i];
            UI.entityInfo[i].text = string.Format("{0} - HP: {1}/{2} | MP: {3}/{4}", p.EntityName, p.CurrentHealth, p.MaxHealth, p.CurrentMagicPoints, p.MaxMagicPoints);
        }
    }
}
