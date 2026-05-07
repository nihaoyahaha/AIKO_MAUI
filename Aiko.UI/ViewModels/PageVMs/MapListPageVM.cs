using Aiko.IServices.IServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace Aiko.UI.ViewModels.PageVMs
{
    public partial class MapListPageVM : Observablebase<MapListPageVM, IMapListService>
    {
        public MapListPageVM(
            IMapListService service,
            ILogger<MapListPageVM> logger) : base(logger, service)
        {
        }

        [ObservableProperty]
        public partial string? ClassCode { get; set; } = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanGoPrevious))]
        [NotifyPropertyChangedFor(nameof(CanGoNext))]
        [NotifyPropertyChangedFor(nameof(PageInfo))]
        public partial ObservableCollection<string> MapList { get; set; } = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanGoPrevious))]
        [NotifyPropertyChangedFor(nameof(CanGoNext))]
        [NotifyPropertyChangedFor(nameof(PageInfo))]
        public partial int CurrentPosition { get; set; }

        [ObservableProperty]
        public partial string? CurrentImage { get; set; } = string.Empty;

		public bool CanGoPrevious => MapList.Count > 0 && CurrentPosition > 0;

        public bool CanGoNext => MapList.Count > 0 && CurrentPosition < MapList.Count - 1;

        public string PageInfo => MapList.Count == 0
            ? "0 / 0"
            : $"{CurrentPosition + 1} / {MapList.Count}";

        public override async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("ClassCode", out var classCode))
            {
                ClassCode = classCode?.ToString();
                await LoadDataAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            if (string.IsNullOrWhiteSpace(ClassCode))
                return;

            var files = await Service.GetHM12FILEID(ClassCode);

            var tempList = new ObservableCollection<string>();

            foreach (var file in files)
            {
                string name = file.HM12002?.TrimEnd() ?? string.Empty;
                string sourceName = file.HM12003?.TrimEnd() ?? string.Empty;
                string extension = Path.GetExtension(sourceName);

                if (string.IsNullOrEmpty(extension))
                    continue;

                string fileName = $"{name}{extension}";
                string filePath = Path.Combine(Service.AppContext.ConstructionSiteFolder, fileName);

                if (File.Exists(filePath))
                    tempList.Add(filePath);
            }

            MapList = tempList;

            if (MapList.Count > 0)
            {
                CurrentPosition = 0;
                UpdateCurrentImage();
            }
            else
            {
                CurrentPosition = 0;
                CurrentImage = null;
            }
        }

        partial void OnCurrentPositionChanged(int value)
        {
            UpdateCurrentImage();
        }

        private void UpdateCurrentImage()
        {
            if (MapList.Count == 0 || CurrentPosition < 0 || CurrentPosition >= MapList.Count)
            {
                CurrentImage = null;
                return;
            }

            CurrentImage = MapList[CurrentPosition];
        }

        [RelayCommand]
        private void PreviousImage()
        {
            if (!CanGoPrevious)
                return;

            CurrentPosition--;
        }

        [RelayCommand]
        private void NextImage()
        {
            if (!CanGoNext)
                return;

            CurrentPosition++;
        }

        [RelayCommand]
        private async Task GoBack()
        {
            // 返回 MapViewPage 时只触发视口恢复，不触发地图数据刷新。
            await Shell.Current.GoToAsync("..?FromPage=MapListPage");
        }
    }
}
