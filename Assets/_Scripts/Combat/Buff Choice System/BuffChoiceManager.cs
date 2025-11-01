using System.Collections.Generic;
using UnityEngine;

public class BuffChoiceManager : MonoBehaviour
{
    public static BuffChoiceManager Instance;

    [SerializeField] private GameObject choicePanel; // UI panel prefab
    [SerializeField] private Transform choiceContainer; // parent of buttons
    [SerializeField] private GameObject buffButtonPrefab; // a button template
    private List<BuffOption> allBuffs = new List<BuffOption>();
    private System.Random random = new System.Random();
    private System.Action<Buff> onBuffChosen;

    private void Awake()
    {
        Instance = this;
        choicePanel.SetActive(false);
        InitializeBuffPool();

    }

    private void InitializeBuffPool()
    {
        allBuffs = new List<BuffOption>()
        {
            new BuffOption("Haste", "Move faster and attack quicker for 8s",
                Resources.Load<Sprite>("Icons/HasteBuff"), () => new HasteBuff(8f, 3)),

            new BuffOption("Fire Infuse", "Attacks deal fire damage for 8s",
                Resources.Load<Sprite>("Icons/FireInfuseBuff"), () => new FireInfuseBuff(8f, 3)),

            new BuffOption("Power Surge", "Massive damage boost for 8s",
                Resources.Load<Sprite>("Icons/PowerSurgeBuff"), () => new PowerSurgeBuff(8f, 5)),

            new BuffOption("Shield Bloom", "Gain a shield for 20 hits",
                Resources.Load<Sprite>("Icons/ShieldBloomBuff"), () => new ShieldBloomBuff(8f, 20)),

            new BuffOption("Precision Strike", "Critical hits guaranteed 3 times",
                Resources.Load<Sprite>("Icons/PrecisionStrikeBuff"), () => new PrecisionStrikeBuff(8f, 50, 3)),
        };
    }

    public List<BuffOption> GetRandomBuffChoices(int count)
    {
        List<BuffOption> pool = new List<BuffOption>(allBuffs);
        List<BuffOption> chosen = new List<BuffOption>();

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int index = random.Next(pool.Count);
            chosen.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return chosen;
    }

    // takes a list of BuffOptions and a callback for when one is chosen
    public void ShowChoices(List<BuffOption> options, System.Action<Buff> onChosen)
    {
        onBuffChosen = onChosen;
        choicePanel.SetActive(true);
        UIManager.Instance.ShowModal(choicePanel);

        // Clear old buttons
        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);

        // Create new buttons
        foreach (var opt in options)
        {
            var btnObj = Instantiate(buffButtonPrefab, choiceContainer);
            var ui = btnObj.GetComponent<BuffChoiceButton>();
            ui.Setup(opt, () =>
            {
                UIManager.Instance.ClosePanel(choicePanel);
                onBuffChosen?.Invoke(opt.CreateBuff());
            });
        }
    }
}
