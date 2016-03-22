using FilePlayer.Model;
using FilePlayer.ViewModels;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace FilePlayer.Views
{
    /// <summary>
    /// Interaction logic for CharGetter.xaml
    /// </summary>
    public partial class CharGetter : UserControl
    {
        private IEventAggregator iEventAggregator;
        public SubscriptionToken filterActionToken;
        public Label[] controls;

        private int selectedControlIndex = 0;

        public CharGetterViewModel CharGetterViewModel { get; set; }
        public CharGetter()
        {
            InitializeComponent();
            Init();

            CharGetterViewModel.PropertyChanged += PropertyChangedHandler;
        }

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrCharSetIndex":
                    SetChars();
                    break;
                case "SelectedControlIndex":
                    SetControlSelected(controls[selectedControlIndex], false);
                    selectedControlIndex = CharGetterViewModel.SelectedControlIndex;
                    SetControlSelected(controls[selectedControlIndex], true);
                    break;
            }
        }

        public void Init()
        {
            iEventAggregator = Event.EventInstance.EventAggregator;

            CharGetterViewModel = new CharGetterViewModel(10, 4, 31, true);
            
            controls = new Label[] {  char00, char01, char02, char03, char04, char05, char06, char07, char08, char09,
                                      char10, char11, char12, char13, char14, char15, char16, char17, char18, char19,
                                      char20, char21, char22, char23, char24, char25, char26, char27, char28, char29,
                                      char30 };


            SetChars();
            for (int i = 0; i < (controls.Length); i++)
            {
                bool isSelected = (i == selectedControlIndex);
                SetControlSelected(controls[i], isSelected);
            }
        }
        

        public void SetChars()
        {
            string[] currCharSet = CharGetterViewModel.GetCurrCharSet();

            this.Dispatcher.Invoke((Action)delegate
            {
                for (int i = 0; i < (controls.Length - 1); i++) //(controls.Length - 1) don't consider spacebar
                {
                    controls[i].Content = currCharSet[i];
                }

                controls[controls.Length - 1].Content = CharGetterViewModel.SpaceText; //Set space
            });
        }


        public void SetControlSelected(Control ctrl, bool isSelected)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                if (isSelected)
                {
                    ctrl.SetResourceReference(Control.StyleProperty, "SelectedCharStyle");
                }
                else
                {
                    ctrl.SetResourceReference(Control.StyleProperty, "UnselectedCharStyle");
                }
            });
        }
        
    }
}
