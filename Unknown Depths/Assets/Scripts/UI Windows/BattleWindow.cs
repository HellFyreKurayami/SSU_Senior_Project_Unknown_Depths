using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleWindow : GenericWindow {

    public static BattleWindow Instance { get; set; }

    [System.Serializable]
    public struct BattleSpawnGroups
    {
        [SerializeField] public BattleSpawnPoint[] SpawnPoints;
    }

    [SerializeField]
    private List<BattleSpawnGroups> SpawnGroups;
    //[SerializeField]
    //private GameObject spellPanel;
    [SerializeField]
    private Button[] actionButtons;
    /*[SerializeField]
    private Button button;*/
    [SerializeField]
    public Text[] entityInfo;
    [SerializeField]
    private Text action;

    public List<Entity> ActiveBattleMembers = new List<Entity>();
    public int currentTurn = 0;
    public Skill playerSelected;
    public bool playerAttacking;

    public delegate void BattleOver(bool playerWin);
    public BattleOver battleOverCall;

    private SpellWindow spellWindow;
    private TargetWindow targetWindow;

    //Used only if the player wins the battle
    private int EXP = 0;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        //spellPanel.SetActive(false);
    }

    public void StartBattle(List<Entity> c, List<MapMaker.EnemySpawns> e, int floor)
    {
        foreach (Entity pm in c)
        {
            if (!System.IO.File.Exists(Application.persistentDataPath + "/PlayerInfo/" + pm.EntityName + ".dat"))
            {
                pm.SaveData();
                //Debug.Log(string.Format("{0} data file has been created", pm.EntityName));
            }
            //Debug.Log(Application.persistentDataPath);
            pm.LoadData();
        }
        int activeParty = c.Count > 4 ? 4 : c.Count;

        c.Sort((x1, x2) => x1.position.CompareTo(x2.position));

        for (int i = 0; i < activeParty; i++)
        {
            ActiveBattleMembers.Add(SpawnGroups[0].SpawnPoints[i].spawn(c[i]));
            //ActiveBattleMembers.Add(SpawnPoints[i + 4].spawn(c[i]));
        }

        int inBattle = Random.Range(1, 3);
        for (int i = 0; i < inBattle; i++)
        {
            Entity spawn = getEntityToSpawn(e[floor - 1].enemies);
            ActiveBattleMembers.Add(SpawnGroups[inBattle].SpawnPoints[i].spawn(spawn));
            //ActiveBattleMembers.Add(SpawnPoints[i].spawn(spawn));
        }

        currentTurn = 0;
        ActiveBattleMembers.Sort((x1, x2) => x1.Speed.CompareTo(x2.Speed));
        ActiveBattleMembers.Reverse();
        UpdateAction(string.Format("Battle has started!"));
        UpdateCharUI();
        //spellPanel.SetActive(false);
        Invoke("NextAct", 3);
    }

    private Entity getEntityToSpawn(List<Entity> enemies)
    {
        return enemies[Random.Range(0, enemies.Count)];
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
        List<Entity> c = ActiveBattleMembers.FindAll(x => x.Chara.Equals(CharaType.PLAYER) && x.alive);
        List<Entity> e = ActiveBattleMembers.FindAll(x => x.Chara.Equals(CharaType.ENEMY));
        if (c.Count == 0 || e.Count == 0)
        {
            //NextTurn();
            //Debug.Log(string.Format("Current Turn: {0} with {1} as the turn entity", currentTurn, GetCurrentCharacter().EntityName));
            //Destroy(Instance);
            //Debug.Log("Battle Has Ended");
            if (c.Count == 0)
            {
                for (int i = 0; i < ActiveBattleMembers.Count; i++)
                {
                    Destroy(ActiveBattleMembers[i].gameObject);
                }
                ActiveBattleMembers.Clear();
                UpdateAction("And they were never heard from again...");
                battleOverCall(false);
            }
            else if (e.Count == 0)
            {
                StartCoroutine(PlayerWin());
            }
        }
        else
        {
            if (GetCurrentCharacter().Chara.Equals(CharaType.PLAYER))
            {
                firstSelected = actionButtons[0].gameObject;
                eventSystem.SetSelectedGameObject(firstSelected);
                if (GetCurrentCharacter().alive)
                {
                    ToggleActionState(true);
                    //BuildSpellList(GetCurrentCharacter().Skills);
                    UpdateAction(string.Format("What Will {0} Do?", GetCurrentCharacter().EntityName));
                }
                else
                {
                    NextTurn();
                    NextAct();
                }
            }
            else
            {
                //Debug.Log("Should be an enemy turn: " + currentTurn);
                ToggleActionState(false);
                StartCoroutine(PerformAct());
            }
        }        
    }

    // Player Controllers

    public void SelectCharacter(Entity e)
    {
        if (playerAttacking)
        {
            DoAttack(GetCurrentCharacter(), e);
            targetWindow.Close();
            ToggleActionState(false);
            playerAttacking = false;
            if(GetCurrentCharacter().CurrentMagicPoints != GetCurrentCharacter().MaxMagicPoints){
                if(GetCurrentCharacter().CurrentMagicPoints + 3 > GetCurrentCharacter().MaxMagicPoints)
                {
                    GetCurrentCharacter().CurrentMagicPoints += 3;
                }
                else
                {
                    GetCurrentCharacter().CurrentMagicPoints = GetCurrentCharacter().MaxMagicPoints;
                }
            }
            NextTurn();
            Invoke("NextAct", 3);
            //NextAct();
        }
        else if (playerSelected != null)
        {
            if (GetCurrentCharacter().CastSpell(playerSelected, e))
            {
                targetWindow.Close();
                UpdateCharUI();
                ToggleActionState(false);
                Invoke("NextAct", 3);
                NextTurn();
                playerSelected = null;
                //NextAct();
            }
            else
            {
                UpdateAction("Not Enough Mana to Cast " + playerSelected.SpellName);
                //Debug.LogWarning("ERROR: Not Enough Mana to Cast " + playerSelected.SpellName);
                StartCoroutine(ResetAction(string.Format("What Will {0} Do?", GetCurrentCharacter().EntityName)));
            }
        }
        new WaitForSeconds(2);
    }

    public void DoAttack(Entity caster, Entity target)
    {
        caster.CastSpell(caster.Skills[0], target);
    }

    // Enemy Controllers

    public Entity GetRandomPlayer()
    {
        List<Entity> c = ActiveBattleMembers.FindAll(x => x.Chara.Equals(CharaType.PLAYER) && x.alive);
        //Debug.Log(string.Format("This List has {0} elements", c.Count));
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

    IEnumerator PerformAct()
    {
        //yield return new WaitForSeconds(3.0f);
        if (ActiveBattleMembers[currentTurn].alive &&
            ActiveBattleMembers[currentTurn].Chara.Equals(CharaType.ENEMY))
        {
            GetCurrentCharacter().GetComponent<Enemy>().Act();
            //GetCurrentCharacter().Act();
        }
        UpdateCharUI();
        yield return new WaitForSeconds(3.0f);
        NextTurn();
        NextAct();
    }

    //UI UPDATES

    public void UpdateCharUI()
    {
        List<Entity> c = ActiveBattleMembers.FindAll(x => x.Chara.Equals(CharaType.PLAYER));
        for (int i = 0; i < c.Count; i++)
        {
            Entity p = c[i];
            entityInfo[i].text = string.Format("{0} - HP: {1}/{2} | MP: {3}/{4}", p.EntityName, p.CurrentHealth, p.MaxHealth, p.CurrentMagicPoints, p.MaxMagicPoints);
        }
    }

    public void UpdateAction(string s)
    {
        action.text = s;
    }

    IEnumerator ResetAction(string s)
    {
        yield return new WaitForSeconds(1.5f);
        action.text = s;
    }

    /*public void ToggleSpellPanel(bool state)
    {
        spellPanel.SetActive(state);
        if (state == true)
        {
            BuildSpellList(GetCurrentCharacter().Skills);
        }
    }*/

    public void OpenSkillWindow(bool fromTarget)
    {
        if (fromTarget)
        {
            targetWindow.Close();
        }
        ToggleActionState(false);
        spellWindow = GenericWindow.manager.Open((int)Windows.SpellWindow - 1, false) as SpellWindow;
        spellWindow.BuildSpellList(GetCurrentCharacter().Skills);
    }

    public void OpenTargetWindow()
    {
        targetWindow = GenericWindow.manager.Open((int)Windows.TargetWindow - 1, false) as TargetWindow;
        targetWindow.CreateTargetList(ActiveBattleMembers.FindAll(x => x.Chara == CharaType.ENEMY));
    }



    public void ToggleActionState(bool state)
    {
        //ToggleSpellPanel(state);
        foreach (Button b in actionButtons)
        {
            b.interactable = state;
        }
    }

    /*public void BuildSpellList(List<Skill> spells)
    {
        //Remove visable spells in spell panel
        if (spellPanel.transform.childCount > 0)
        {
            foreach (Button b in spellPanel.transform.GetComponentsInChildren<Button>())
            {
                Destroy(b.gameObject);
            }
        }

        foreach (Skill s in spells)
        {
            if (!s.SpellName.Equals("Basic Attack"))
            {
                Button spellButton = Instantiate<Button>(button, spellPanel.transform);
                spellButton.GetComponentInChildren<Text>().text = s.SpellName;
                spellButton.onClick.AddListener(() => SelectSpell(s));
            }
        }
    }*/

    //End of Battle

    public void GiveEXP(int exp)
    {
        EXP += exp;
    }

    IEnumerator PlayerWin()
    {
        UpdateAction("You have won the battle!"); // Initial Statement
        yield return new WaitForSeconds(1.0f);
        if (ActiveBattleMembers[0].alive)
        {
            UpdateAction(string.Format("{0} has gained {1} EXP", ActiveBattleMembers[0].EntityName, EXP)); // Position 0 EXP Gain
            ActiveBattleMembers[0].experience += EXP;
            yield return new WaitForSeconds(1.0f);
        }
        ActiveBattleMembers[0].SaveData();
        if (ActiveBattleMembers[1].alive)
        {
            UpdateAction(string.Format("{0} has gained {1} EXP", ActiveBattleMembers[1].EntityName, EXP)); // Position 1 EXP Gain
            ActiveBattleMembers[1].experience += EXP;
            yield return new WaitForSeconds(1.0f);
        }
        ActiveBattleMembers[1].SaveData();
        if (ActiveBattleMembers[2].alive)
        {
            UpdateAction(string.Format("{0} has gained {1} EXP", ActiveBattleMembers[2].EntityName, EXP)); // Position 2 EXP Gain
            ActiveBattleMembers[2].experience += EXP;
            yield return new WaitForSeconds(1.0f);
        }
        ActiveBattleMembers[2].SaveData();
        if (ActiveBattleMembers[3].alive)
        {
            UpdateAction(string.Format("{0} has gained {1} EXP", ActiveBattleMembers[3].EntityName, EXP)); // Position 3 EXP Gain
            ActiveBattleMembers[3].experience += EXP;
            yield return new WaitForSeconds(1.0f);
        }
        ActiveBattleMembers[3].SaveData();
        /*if (ActiveBattleMembers[4].alive)
        {
            ActiveBattleMembers[4].experience += (int)(EXP*0.6);
            ActiveBattleMembers[4].SaveData();
            yield return new WaitForSeconds(1.0f);
        }
        if (ActiveBattleMembers[5].alive)
        {
            ActiveBattleMembers[5].experience += (int)(EXP * 0.6);
            ActiveBattleMembers[5].SaveData();
            yield return new WaitForSeconds(1.0f);
        }*/
        //Item / Gold drops
        yield return new WaitForSeconds(1.0f);
        for (int i = 0; i < ActiveBattleMembers.Count; i++)
        {
            Destroy(ActiveBattleMembers[i].gameObject);
        }
        EXP = 0;
        ActiveBattleMembers.Clear();
        battleOverCall(true);

    }

    // Boolean Controllers

    public void SelectAttack()
    {
        playerSelected = null;
        //ToggleSpellPanel(false);
        playerAttacking = true;
        OpenTargetWindow();
    }

    public void SelectSpell(Skill spell)
    {
        playerSelected = spell;
        playerAttacking = false;
        spellWindow.Close();
        OpenTargetWindow();
    }
}
