﻿using OfficeOpenXml;
using System;
using System.IO;

namespace TimeTracker
{
    public class MainWindowViewModel : BaseViewModel
    {
        public DelegateCommand? StartCommand { get; set; }
        public DelegateCommand? StopCommand { get; set; }
        private DateTime _today;
        private DateTime _stopDate;
        private bool _isStartButtonEnabled = true;
        private bool _isStopButtonEnabled;
        private bool _displayStartDate;
        private bool _displayStopDate;
        private TimeSpan _duration;

        public MainWindowViewModel()
        {
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            StartCommand = new DelegateCommand(
                (o) =>
                {
                    StartDate = DateTime.Now;
                    DisplayStartDate = true;
                    IsStartButtonEnabled = false;
                    IsStopButtonEnabled = true;
                    OnStart();
                });
            StopCommand = new DelegateCommand(
                (o) =>
                {
                    StopDate = DateTime.Now;
                    DisplayStopDate = true;
                    IsStartButtonEnabled = true;
                    IsStopButtonEnabled = false;
                    CalculateDuration();
                    OnStop();
                });
        }

        #region Methods
        private FileInfo CreatePathAndDirectory()
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var separator = Path.DirectorySeparatorChar;
            var directory = Directory.CreateDirectory(filePath + separator + "TimeTracker");
            var date = DateTime.Now;
            var filePathAndName = directory.FullName + separator + date.ToString("MMMM_") + "WorkingHours.xlsx";

            return new FileInfo(filePathAndName);
        }

        private void OnStart()
        {
            var fileInfo = CreatePathAndDirectory();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet sheet;
                if (package.Workbook.Worksheets[DateTime.Now.ToString("MMMM yyyy")] != null)
                {
                    sheet = package.Workbook.Worksheets[DateTime.Now.ToString("MMMM yyyy")];
                    var lastRow = sheet.Dimension.End.Row;
                    sheet.DeleteRow(lastRow);
                    sheet.Cells[lastRow, 1].Style.Numberformat.Format = "dd-MM-yyyy HH:mm:ss";
                    sheet.Cells[$"A{lastRow}"].Value = StartDate;
                    sheet.Cells.AutoFitColumns();
                }
                else
                {
                    sheet = package.Workbook.Worksheets.Add(DateTime.Now.ToString("MMMM yyyy"));
                    sheet.Cells[2, 1].Style.Numberformat.Format = "dd-MM-yyyy HH:mm:ss";
                    sheet.Cells[$"A1"].Value = "Start Date & Time";
                    sheet.Cells[$"B1"].Value = "End Date & Time";
                    sheet.Cells[$"C1"].Value = "Shift Time";
                    sheet.Cells[$"A2"].Value = StartDate;
                    sheet.Cells.AutoFitColumns();
                }
                package.Save();
            }
        }
        private void OnStop()
        {
            var fileInfo = CreatePathAndDirectory();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet sheet;
                if (package.Workbook.Worksheets[DateTime.Now.ToString("MMMM yyyy")] != null)
                {
                    sheet = package.Workbook.Worksheets[DateTime.Now.ToString("MMMM yyyy")];
                    var lastRow = sheet.Dimension.End.Row;
                    sheet.Cells[lastRow, 2].Style.Numberformat.Format = "dd-MM-yyyy HH:mm:ss";
                    sheet.Cells[lastRow, 3].Style.Numberformat.Format = "HH:mm:ss";
                    sheet.Cells[$"B{lastRow}"].Value = StopDate;
                    sheet.Cells[$"C{lastRow}"].Value = Duration;
                    sheet.Cells.AutoFitColumns();
                }
                else
                {
                    return;
                }

                var firstRow = sheet.Dimension.Start.Row;
                var totalHoursRow = sheet.Dimension.End.Row;
                totalHoursRow++;
                sheet.Cells[$"B{totalHoursRow}"].Value = "Total Hours";
                var sumCell = totalHoursRow - 1;
                sheet.Cells[$"C{totalHoursRow}"].Formula = $"=SUM(C{firstRow}:C{sumCell})";
                sheet.Cells[$"C{totalHoursRow}"].Style.Numberformat.Format = "HH:mm:ss";
                sheet.Cells[$"C{totalHoursRow}"].Style.Font.Bold = true;
                package.Save();
            }
        }

        private void CalculateDuration()
        {
            if (StartDate < StopDate)
            {
                Duration = StopDate - StartDate;
            }
            else
            {
                Duration = StartDate - StopDate;
            }
        }

        #endregion

        #region Properties
        public DateTime StartDate
        {
            get => _today;
            set
            {
                _today = value;
                RaisePropertyChanged(nameof(StartDate));
            }
        }
        public DateTime StopDate
        {
            get => _stopDate;
            set
            {
                _stopDate = value;
                RaisePropertyChanged(nameof(StopDate));
            }
        }
        public bool IsStartButtonEnabled
        {
            get { return _isStartButtonEnabled; }
            set
            {
                _isStartButtonEnabled = value;
                RaisePropertyChanged(nameof(IsStartButtonEnabled));
            }
        }
        public bool IsStopButtonEnabled
        {
            get { return _isStopButtonEnabled; }
            set
            {
                _isStopButtonEnabled = value;
                RaisePropertyChanged(nameof(IsStopButtonEnabled));
            }
        }
        public bool DisplayStartDate
        {
            get => _displayStartDate;
            set
            {
                _displayStartDate = value;
                RaisePropertyChanged(nameof(DisplayStartDate));
            }
        }
        public bool DisplayStopDate
        {
            get => _displayStopDate;
            set
            {
                _displayStopDate = value;
                RaisePropertyChanged(nameof(DisplayStopDate));
            }
        }
        public TimeSpan Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                RaisePropertyChanged(nameof(Duration));
            }
        }
        public bool OnClose { get; set; }
        #endregion
    }
}