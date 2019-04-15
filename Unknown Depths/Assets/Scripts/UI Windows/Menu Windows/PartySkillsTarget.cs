using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PartySkillsTarget : GenericWindow
{
    [SerializeField]
    private GameObject targetPanel;
    [SerializeField]
    private Button button;
    [SerializeField]
    private Text targetText;

    [System.NonSerialized]
    public Skill selectedSpell = null;
    [System.NonSerialized]
    public Entity selectedEntity = null;
    [System.NonSerialized]
    public List<Entity> entities = null;
    [System.NonSerialized]
    public bool inFocus = false;

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            selectedSpell = null;
            selectedEntity = null;
            inFocus = false;
            Close();
        }
    }

    public override void Open()
    {
        if (targetPanel.transform.childCount > 0)
        {
            foreach (Button b in targetPanel.transform.GetComponentsInChildren<Button>())
            {
                Destroy(b.gameObject);
            }
        }
        inFocus = true;
        base.Open();
    }

    public void createButtons()
    {
        Debug.Log(entities.Count);
        foreach (Entity player in entities)
        {
            //Debug.Log(player.EntityName);
            Button targetButton = Instantiate<Button>(button, targetPanel.transform);
            targetButton.GetComponentInChildren<Text>().text = player.EntityName;
            targetButton.onClick.AddListener(() => CastFromTarget(selectedEntity, player));
            EventTrigger trigger = targetButton.gameObject.AddComponent<EventTrigger>();
            var hover = new EventTrigger.Entry();
            var select = new EventTrigger.Entry();
            hover.eventID = EventTriggerType.PointerEnter;
            select.eventID = EventTriggerType.Select;
            hover.callback.AddListener((S) => DisplayInformation(string.Format("{0} | {1} / {2} HP", player.EntityName, player.CurrentHealth, player.MaxHealth)));
            select.callback.AddListener((S) => DisplayInformation(string.Format("{0} | {1} / {2} HP", player.EntityName, player.CurrentHealth, player.MaxHealth)));
            trigger.triggers.Add(hover);
            trigger.triggers.Add(select);

            var nav = targetButton.navigation;
            nav.mode = Navigation.Mode.Automatic;
            targetButton.navigation = nav;
        }
        Button[] btemp = targetPanel.transform.GetComponentsInChildren<Button>();
        firstSelected = btemp[0].gameObject;
        eventSystem.SetSelectedGameObject(firstSelected);
    }

    private void DisplayInformation(string text)
    {
        targetText.text = text;
    }

    private void CastFromTarget(Entity caster, Entity target)
    {
        caster = MapMaker.Instance.Players.Find(x => x.EntityName.Equals(caster.EntityName));
        target = MapMaker.Instance.Players.Find(x => x.EntityName.Equals(target.EntityName));
        if (caster.CastSpell(selectedSpell, target, true) && target.CurrentHealth < target.MaxHealth)
        {
            caster.SaveData();
            target.SaveData();
        }
        target.LoadData();
        caster.LoadData();
        DisplayInformation(string.Format("{0} | {1} / {2}", target.EntityName, target.CurrentHealth, target.MaxHealth));
    }
}
