using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
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
                //if (ItemImageExists(value))
                //{
                    itemImage = value;
                //}
                //else
                //{
                //    itemImage = null;
                //}

                OnPropertyChanged("ItemImage");
            }
        }

        public bool ItemImageExists(string imageURL)
        {
            if (imageURL.Equals(""))
            {
                return false;
            }

            var request = (HttpWebRequest)WebRequest.Create(imageURL);
            request.Method = "HEAD";

            try
            {
                using (var response = request.GetResponse())
                {
                    bool isImageValid = response.ContentType.ToLower(CultureInfo.InvariantCulture).StartsWith("image/", StringComparison.OrdinalIgnoreCase);
                    return isImageValid;
                }

            }
            catch (WebException ex)
            {
                return false;
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
