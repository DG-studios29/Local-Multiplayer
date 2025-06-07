using UnityEngine;
using UnityEngine.UI;

public class HeroManager : MonoBehaviour
{
    public Button heroButton;
    public string heroName;

    public void Initialize()
    {
        heroButton.onClick.RemoveAllListeners(); 
        heroButton.onClick.AddListener(() => SelectHero());
    }

    public void SelectHero()
    {
        if (!string.IsNullOrEmpty(heroName))
        {
            HeroSelectionUI.Instance.OnHeroSelected(heroName);
        }
        else
        {
            Debug.LogWarning("[HeroManager] Hero name is empty on button.");
        }
    }
}
