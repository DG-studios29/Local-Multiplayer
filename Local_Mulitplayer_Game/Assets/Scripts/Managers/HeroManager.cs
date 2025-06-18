using UnityEngine;
using UnityEngine.UI;

public class HeroManager : MonoBehaviour
{
    public Button heroButton;
    public string heroName;

    public Image heroPortrait;

    public void Initialize()
    {
        heroButton.onClick.RemoveAllListeners();
        heroButton.onClick.AddListener(() => SelectHero());
    }

    public void SelectHero()
    {
        if (!string.IsNullOrEmpty(heroName))
        {
            Debug.Log($"🧙‍♂️ Hero '{heroName}' clicked.");
            HeroSelectionUI.Instance.OnHeroSelected(heroName);
        }
    }

}
