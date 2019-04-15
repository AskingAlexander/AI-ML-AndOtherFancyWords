using Xamarin.Forms;

namespace ProductPlacementDemo.Models
{
    public class ListItem : BindableClass
    {
        private string _itemName;
        public string ItemName
        {
            get => _itemName;
            set => SetProperty(ref _itemName, value);
        }

        private int _itemCount;
        public int ItemCount
        {
            get => _itemCount;
            set => SetProperty(ref _itemCount, value);
        }

        private Color _itemColor = Color.White;
        public Color ItemColor
        {
            get => _itemColor;
            set => SetProperty(ref _itemColor, value);
        }
    }
}
