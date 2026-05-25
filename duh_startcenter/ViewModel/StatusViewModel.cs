using System;
using System.Collections.Generic;
using System.Text;

namespace NXStartCenter.ViewModel
{

    public sealed class StatusViewModel : BaseViewModel
    {
        private string _statusText = "Bereit";
        private string _statusColor = "#4ead2b";
        private string _statusState = "ok";

        public string StatusText
        {
            get => _statusText;
            private set => SetField(ref _statusText, value);
        }

        public string StatusColor
        {
            get => _statusColor;
            private set => SetField(ref _statusColor, value);
        }
        public string StatusState
        {
            get => _statusState;
            private set
            {
                SetField(ref _statusState, value);
                switch (_statusState)
                {
                    case "error":
                        {
                            StatusColor = "#cc3527";
                            break;
                        }
                    case "ok":
                        StatusColor = "#4ead2b";
                        break;
                    default:
                        StatusColor = "#C9C9C9";
                        break;
                }
            }
        }

        public void SetBusy(string message)
        {
            StatusState = string.Empty;
            StatusText = message;
        }

        public void SetSuccess(string message)
        {
            StatusState = "ok";
            StatusText = message;
        }

        public void SetError(string message)
        {
            StatusState = "error";
            StatusText = message;
        }
    }
}
