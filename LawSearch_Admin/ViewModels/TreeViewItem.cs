using LawSearch_Core.Models;

namespace LawSearch_Admin.ViewModels
{
    public class TreeViewItem
    {
        public string? Text { get; set; }
        public string? Type { get; set; }
        public int? ID { get; set; }
        public Chapter? Chapter { get; set; } = null;
        public Section? Section { get; set; } = null;
        public Artical? Artical { get; set; } = null;
        public List<TreeViewItem>? Children { get; set; }
        public bool IsCollapsed { get; set; }
        public bool IsSelected { get; set; }

        public bool HasChildren => Children != null && Children.Any();

        public TreeViewItem(string? text, string type, int iD, bool isCollapsed = true, bool isSelected = false)
        {
            Text = text;
            Type = type;
            ID = iD;
            IsCollapsed = isCollapsed;
            IsSelected = isSelected;
        }

        public TreeViewItem(string? text, List<TreeViewItem>? children = null, bool isCollapsed = true, bool isSelected = false)
        {
            Text = text;
            Children = children;
            IsCollapsed = isCollapsed;
            IsSelected = isSelected;
        }
    }
}
