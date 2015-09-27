using System.Windows;
using Prism.Mvvm;
using FilePlayer.ViewModels;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Practices.Prism.PubSubEvents;

namespace FilePlayer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window//, IView
    {
        private readonly IEventAggregator _eventAggregator;

        public MainWindowViewModel MainWindowViewModel { get; set; }
        private ModalAdorner modalAdorner;

        public MainWindow()
        {
            InitializeComponent();
            modalAdorner = null;
        }

        void PerformViewAction(object sender, ViewEventArgs e)
        {
            switch (e.action)
            {
                case "MODAL_DIALOG_BACKGROUND_ON":
                    ModalBackground(true);
                    break;
                case "MODAL_DIALOG_BACKGROUND_OFF":
                    ModalBackground(false);
                    break;
            }
        }

        public void ModalBackground(bool isOn)
        {
            var layer = AdornerLayer.GetAdornerLayer(this);

            if (isOn)
            {
                if (modalAdorner == null)
                {
                    modalAdorner = new ModalAdorner(ParentWindow);
                }

                layer.Add(modalAdorner);
            }
            else
            {
                layer.Remove(modalAdorner);
            }
        }
    }

    public class ModalAdorner : Adorner
    {   
        public ModalAdorner(UIElement adornedElement) : base(adornedElement) {}

        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);
            // Some arbitrary drawing implements.
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
            renderBrush.Opacity = 0.2;

            drawingContext.DrawRectangle(renderBrush, null, adornedElementRect);
        }

    }
}
