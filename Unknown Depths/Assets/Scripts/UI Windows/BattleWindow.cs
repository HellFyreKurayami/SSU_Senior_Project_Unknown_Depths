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
    [SerializeField]
    private GameObject spellPanel;
    [SerializeField]
    private Button[] actionButtons;
    [SerializeField]
    private Button button;
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
        spellPanel.SetActive(false);
    }

    public void StartBattle(List<Entity> c, List<MapMaker.EnemySpawns> e, int floor)
    {

        for (int i = 0; i < c.Count; i++)
        {
            ActiveBattleMembers.Add(SpawnGroups[0].SpawnPoints[i].spawn(c[i]));
            //ActiveBattleMembers.Add(SpawnPoints[i + 4].spawn(c[i]));
        }

        int inBattle = Random.Range(1, 5);
        for (int i = 0; i < inBattle; i++)
        {
            Enemy spawn = getEntityToSpawn(e[floor - 1].enemies);
            ActiveBattleMembers.Add(SpawnGroups[inBattle].SpawnPoints[i].spawn(spawn));
            //ActiveBattleMembers.Add(SpawnPoints[i].spawn(spawn));
        }

        ActiveBattleMembers.Sort((x1, x2) => x1.Speed.CompareTo(x2.Speed));
        ActiveBattleMembers.Reverse();
        Debug.Log(string.Format("Current Turn: {0} with {1} as the turn entity", currentTurn, GetCurrentCharacter().EntityName));
        UpdateAction(string.Format("What Will {0} Do?", GetCurrentCharacter().EntityName));
        UpdateCharUI();
        spellPanel.SetActive(false);
    }

    private Enemy getEntityToSpawn(List<Enemy> enemies)
    {
        return enemies[Random.Range(0, enemies.Count)];
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //This may be useless, will change later
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hitInfo = Physics2D.Raycast(r.origin, r.direction);
            if (hitInfo.collider != null && hitInfo.collider.CompareTag("Entity"))
            {
                Debug.Log("You Clicked an Entity");
                SelectCharacter(hitInfo.collider.GetComponent<Entity>());
            }
        }
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
                UpdateAction("And they were never heard from again...");
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
            ToggleActionState(true);
            BuildSpellList(GetCurrentCharacter().Skills);
            UpdateAction(string.Format("What Will {0} Do?", GetCurrentCharacter().EntityName));
        }
        else
        {
            ToggleActionState(false);
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
            entityInfo[i].text = string.Format("{0} - HP: {1}/{2} | MP: {3}/{4}", p.EntityName, p.CurrentHealth, p.MaxHealth, p.CurrentMagicPoints, p.MaxMagicPoints);
        }
    }

    //UI UPDATES

    public void UpdateAction(string s)
    {
        action.text = s;
    }

    public void ToggleSpellPanel(bool state)
    {
        spellPanel.SetActive(state);
        if (state == true)
        {
            BuildSpellList(GetCurrentCharacter().Skills);
        }
    }

    public void ToggleActionState(bool state)
    {
        ToggleSpellPanel(state);
        foreach (Button b in actionButtons)
        {
            b.interactable = state;
        }
    }

    public void BuildSpellList(List<Skill> spells)
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
    }

    public void SelectAttack()
    {
        playerSelected = null;
        playerAttacking = true;
    }

    private void SelectSpell(Skill spell)
    {
        playerSelected = spell;
        playerAttacking = false;
    }
}
