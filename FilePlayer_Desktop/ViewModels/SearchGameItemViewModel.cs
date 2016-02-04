using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilePlayer.ViewModels
{
    class SearchGameItemViewModel : ViewModelBase
    {
        private IEventAggregator iEventAggregator;
        public string itemName;
        public string itemImage;

        public string ItemName
        {
            get
            {
                return itemName;
            }
            set
            {
                itemName = value;
                OnPropertyChanged("ItemName");
            }
        }
        public string ItemImage
        {
            get
            {
                return itemImage;
            }
            set
            {
                itemImage = value;
                OnPropertyChanged("ItemImage");
            }
        }


        public SearchGameItemViewModel(IEventAggregator iEventAggregator, string _itemName, string _itemImage)
        {
            ItemName = _itemName;
            ItemImage = _itemImage;
            this.iEventAggregator = iEventAggregator;
        }
    }
}
