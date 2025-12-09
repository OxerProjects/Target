using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Graphics;

namespace Target.Models
{
    public class CalendarDay : ObservableObject
    {
        public DateTime Date { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty(ref _isSelected, value))
                {
                    OnPropertyChanged(nameof(BackgroundColor));
                    OnPropertyChanged(nameof(TextColor));
                }
            }
        }

        private bool _isToday;
        public bool IsToday
        {
            get => _isToday;
            set
            {
                SetProperty(ref _isToday, value);
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }

        private bool _isCurrentMonth;
        public bool IsCurrentMonth
        {
            get => _isCurrentMonth;
            set
            {
                SetProperty(ref _isCurrentMonth, value);
                OnPropertyChanged(nameof(TextColor));
            }
        }

        private bool _hasEvent;
        public bool HasEvent
        {
            get => _hasEvent;
            set
            {
                SetProperty(ref _hasEvent, value);
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }

        public Color BackgroundColor
        {
            get
            {
                var app = Application.Current;
                if (app != null)
                {
                    if (IsSelected) return app.Resources.TryGetValue("Tertiary", out var secondary) && secondary is Color c2 ? c2 : Colors.Gray;
                    if (IsToday) return app.Resources.TryGetValue("PrimaryDark", out var primary) && primary is Color c1 ? c1 : Colors.Gray;
                    if (!IsCurrentMonth) return Colors.Transparent;
                }
                return app.Resources.TryGetValue("Primary", out var primary2) && primary2 is Color c6 ? c6: Colors.Gray;
            }
        }
       
        public Color TextColor
        {
            get
            {
                if (IsToday) return Colors.White;
                if (!IsCurrentMonth) return Colors.Gray;
                return IsSelected ? Colors.White : Colors.Gray;
            }
        }
    }
}
