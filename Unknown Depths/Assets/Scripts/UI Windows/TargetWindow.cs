using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TargetWindow : GenericWindow
{
    [SerializeField]
    private GameObject targetPanel;
    [SerializeField]
    private Button button;
    [SerializeField]
    private Text targetText;

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if(BattleWindow.Instance.playerSelected != null)
            {
                BattleWindow.Instance.OpenSkillWindow(true);
            }
            else
            {
                BattleWindow.Instance.ToggleActionState(true);
                BattleWindow.Instance.SelectButton(false);
                Close();
            }
        }
    }

    public void CreateTargetList(List<Entity> targets)
    {
        if (targetPanel.transform.childCount > 0)
        {
            foreach (Button b in targetPanel.transform.GetComponentsInChildren<Button>())
            {
                Destroy(b.gameObject);
            }
        }
        
        foreach (Entity e in targets)
        {
            Button targetButton = Instantiate<Button>(button, targetPanel.transform);
            targetButton.GetComponentInChildren<Text>().text = e.EntityName;
            targetButton.onClick.AddListener(() => BattleWindow.Instance.SelectCharacter(e));
            EventTrigger trigger = targetButton.gameObject.AddComponent<EventTrigger>();
            var hover = new EventTrigger.Entry();
            var select = new EventTrigger.Entry();
            hover.eventID = EventTriggerType.PointerEnter;
            select.eventID = EventTriggerType.Select;
            hover.callback.AddListener((S) => DisplayInformation(string.Format("{0} | {1} / {2} HP", e.EntityName, e.CurrentHealth, e.MaxHealth)));
            select.callback.AddListener((S) => DisplayInformation(string.Format("{0} | {1} / {2} HP", e.EntityName, e.CurrentHealth, e.MaxHealth)));
            trigger.triggers.Add(hover);
            trigger.triggers.Add(select);
            var nav = targetButton.navigation;
            nav.mode = Navigation.Mode.Automatic;
            targetButton.navigation = nav;
        }

        Button[] temp = targetPanel.transform.GetComponentsInChildren<Button>();
        firstSelected = temp[0].gameObject;
        eventSystem.SetSelectedGameObject(firstSelected);
    }

    public override void Close()
    {
        if (targetPanel.transform.childCount > 0)
        {
            foreach (Button b in targetPanel.transform.GetComponentsInChildren<Button>())
            {
                Destroy(b.gameObject);
            }
        }
        base.Close();
    }

    public void DisplayInformation(string text)
    {
        targetText.text = text;
    }
}
