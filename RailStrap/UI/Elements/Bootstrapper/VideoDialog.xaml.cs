using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Shell;

using RailStrap.UI.ViewModels.Bootstrapper;

namespace RailStrap.UI.Elements.Bootstrapper
{
    /// <summary>
    /// Interaction logic for VideoDialog.xaml
    /// </summary>
    public partial class VideoDialog : IBootstrapperDialog
    {
        private readonly BootstrapperDialogViewModel _viewModel;

        public RailStrap.Bootstrapper? Bootstrapper { get; set; }

        private bool _isClosing;

        #region UI Elements
        public string Message
        {
            get => _viewModel.Message;
            set
            {
                _viewModel.Message = value;
                _viewModel.OnPropertyChanged(nameof(_viewModel.Message));
            }
        }

        public ProgressBarStyle ProgressStyle
        {
            get => _viewModel.ProgressIndeterminate ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous;
            set
            {
                _viewModel.ProgressIndeterminate = (value == ProgressBarStyle.Marquee);
                _viewModel.OnPropertyChanged(nameof(_viewModel.ProgressIndeterminate));
            }
        }

        public int ProgressMaximum
        {
            get => _viewModel.ProgressMaximum;
            set
            {
                _viewModel.ProgressMaximum = value;
                _viewModel.OnPropertyChanged(nameof(_viewModel.ProgressMaximum));
            }
        }

        public int ProgressValue
        {
            get => _viewModel.ProgressValue;
            set
            {
                _viewModel.ProgressValue = value;
                _viewModel.OnPropertyChanged(nameof(_viewModel.ProgressValue));
            }
        }

        public TaskbarItemProgressState TaskbarProgressState
        {
            get => _viewModel.TaskbarProgressState;
            set
            {
                _viewModel.TaskbarProgressState = value;
                _viewModel.OnPropertyChanged(nameof(_viewModel.TaskbarProgressState));
            }
        }

        public double TaskbarProgressValue
        {
            get => _viewModel.TaskbarProgressValue;
            set
            {
                _viewModel.TaskbarProgressValue = value;
                _viewModel.OnPropertyChanged(nameof(_viewModel.TaskbarProgressValue));
            }
        }

        public bool CancelEnabled
        {
            get => _viewModel.CancelEnabled;
            set
            {
                _viewModel.CancelEnabled = value;

                _viewModel.OnPropertyChanged(nameof(_viewModel.CancelButtonVisibility));
                _viewModel.OnPropertyChanged(nameof(_viewModel.CancelEnabled));
            }
        }
        #endregion

        public VideoDialog()
        {
            InitializeComponent();

            _viewModel = new BootstrapperDialogViewModel(this);
            DataContext = _viewModel;

            Loaded += VideoDialog_Loaded;
        }

        private async void VideoDialog_Loaded(object sender, RoutedEventArgs e)
        {
            string videoPath = Path.Combine(Paths.Temp, "LoadingVideo.mp4");

            if (!File.Exists(videoPath))
            {
                Directory.CreateDirectory(Paths.Temp);
                await File.WriteAllBytesAsync(videoPath, await Resource.Get("Loading.mp4"));
            }

            LoadingVideo.Source = new Uri(videoPath);
            LoadingVideo.Play();
        }

        private void LoadingVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            LoadingVideo.Position = TimeSpan.Zero;
            LoadingVideo.Play();
        }

        private void UiWindow_Closing(object sender, CancelEventArgs e)
        {
            LoadingVideo.Stop();
            LoadingVideo.Close();

            if (!_isClosing)
                Bootstrapper?.Cancel();
        }

        #region IBootstrapperDialog Methods
        public void ShowBootstrapper() => this.ShowDialog();

        public void CloseBootstrapper()
        {
            _isClosing = true;
            Dispatcher.BeginInvoke(this.Close);
        }

        public void ShowSuccess(string message, Action? callback) => Base.BaseFunctions.ShowSuccess(message, callback);
        #endregion
    }
}
