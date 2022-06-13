using UnityEngine;
[CreateAssetMenu(fileName = "New Category", menuName = "ScriptableObjects/Category", order = 1)]
public class SCategory : SGridItem
{
    [Header("Category-Only Properties")]
    public bool active;
    public SCategory (string title, string id, Sprite icon, bool active)
    {
        this.title = title;
        this.iD = id;
        this.icon = icon;
        this.active = active;
    }
}
