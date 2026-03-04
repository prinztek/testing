using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffChoiceManager : MonoBehaviour
{
    public static BuffChoiceManager Instance;
    [SerializeField] private GameObject choicePanel; // UI panel prefab
    [SerializeField] private Transform choiceContainer; // parent of buttons
    [SerializeField] private GameObject buffButtonPrefab; // a button template
    [SerializeField] private Button confirmBtn; // a button template
    private List<BuffOption> allBuffs = new List<BuffOption>();
    private System.Random random = new System.Random();
    private System.Action<Buff> onBuffChosen;
    private BuffOption currentlySelectedBuff;

    [Header("Sound Clip References")]
    [SerializeField] private AudioClip buffAcquireSoundClip;

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
            new BuffOption("Haste", "Move faster and attack quicker with a small damage boost for 15s",
                Resources.Load<Sprite>("Icons/Haste"), () => new HasteBuff(15f, 1.5f, 1.25f, 1.2f)),

            new BuffOption("Fire Infuse", "Attacks deal fire damage for 15s",
                Resources.Load<Sprite>("Icons/Fire Infuse"), () => new FireInfuseBuff(15f, 8)),

            new BuffOption("Power Surge", "Massive damage boost for 15s",
                Resources.Load<Sprite>("Icons/Power Surge"), () => new PowerSurgeBuff(15f, 5)),

            new BuffOption("Shield Bloom", "Gain a shield that deflects attack for 15s",
                Resources.Load<Sprite>("Icons/Shield Bloom"), () => new ShieldBloomBuff(15f, 99)),

            new BuffOption("Precision Strike", "Critical hits guaranteed 1 times within 15s",
                Resources.Load<Sprite>("Icons/Precision Strike"), () => new PrecisionStrikeBuff(15f, 50, 1)),

            // new BuffOption("Cold Infuse", "Attacks slows enemy movement for 5s",
            //     Resources.Load<Sprite>("Icons/Cold Infuse"), () => new ColdInfuseBuff(8f, 3)),

            //  new BuffOption("Bomb", "Attacks places a bomb on enemy that would later explode",
            //     Resources.Load<Sprite>("Icons/Cold Infuse"), () => new BombBuff(8f, 3, 3, 25)),
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
        UIManager.Instance.ShowModal(choicePanel);

        currentlySelectedBuff = null; // reset

        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);

        foreach (var opt in options)
        {
            var btnObj = Instantiate(buffButtonPrefab, choiceContainer);
            var buffButton = btnObj.GetComponent<BuffChoiceButton>();
            // passing the actual buff instance to the button and what to do when selected
            buffButton.Setup(opt, optionSelected =>
            {
                currentlySelectedBuff = optionSelected;
                HighlightSelected(btnObj); // highlight the selected buff
            });
        }

        confirmBtn.onClick.RemoveAllListeners();
        confirmBtn.onClick.AddListener(ConfirmChosenBuff);
    }

    private void ConfirmChosenBuff()
    {
        if (currentlySelectedBuff == null)
            return; // or disable button until selected

        UIManager.Instance.ClosePanel(choicePanel);

        onBuffChosen?.Invoke(currentlySelectedBuff.CreateBuff());

        SoundFXManager.Instance.playOneShotSoundFXClilp(buffAcquireSoundClip, transform, 0.5f);
    }

    private GameObject lastHighlighted;

    private void HighlightSelected(GameObject btn)
    {
        if (lastHighlighted != null)
            lastHighlighted.GetComponent<Image>().color = Color.white;

        lastHighlighted = btn;
        lastHighlighted.GetComponent<Image>().color = new Color(0.8f, 0.8f, 1f);
    }


}
