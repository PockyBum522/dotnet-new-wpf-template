﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using Unique.Id.To.Be.Replaced.Core;
using Unique.Id.To.Be.Replaced.Core.Logic.WindowsHelpers;
using Unique.Id.To.Be.Replaced.UI.Interfaces;
using Unique.Id.To.Be.Replaced.UI.WindowResources.SettingsWindow;

namespace Unique.Id.To.Be.Replaced.UI.TrayIcon;

/// <summary>
/// ViewModel for Tray Icon
/// </summary>
public partial class TrayIconViewModel : ObservableObject, ITrayIconViewModel
{
    private readonly ILogger _logger;
    //private readonly ISettingsApplicationLocal _settingsAppLocal;
    
    private readonly SettingsWindow _settingsWindow;
    
    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected logger to use</param>
    /// <param name="settingsViewModel">Injected settings window viewmodel to use</param>
    // /// <param name="settingsAppLocal">Injected application local settings to use</param>
    public TrayIconViewModel(ILogger logger, SettingsViewModel settingsViewModel) //, ISettingsApplicationLocal settingsAppLocal)
    {
        _logger = logger;
        
        _logger.Information("Initializing Tray Icon View");
        
        //_settingsAppLocal = settingsAppLocal;

        _settingsWindow = new()
        {
            DataContext = settingsViewModel
        };

        _settingsWindow.Hide();
        
        _logger.Information("Hid settings window, tray icon init finished");
    }
    
    /// <summary>
    /// Command to exit the application completely
    /// </summary>
    [RelayCommand]
    private void ExitApplication() => Environment.Exit(0);
    
    /// <summary>
    /// Command to show the settings window
    /// </summary>
    [RelayCommand]
    private void OpenSettingsWindow() => _settingsWindow.Show();
    
    // Example of some commonly used things that might be needed for a menu entry
    // private void EnableDevice()
    // {
    //     var deviceCategoryGuid = new Guid(_settingsAppLocal.DeviceClassGuid);
    //
    //     var instancePath = _settingsAppLocal.DeviceInstancePath;
    //
    //     DeviceHelper.SetDeviceEnabled(deviceCategoryGuid, instancePath, true);
    //     
    //     _logger.Information("Enabled device at path: {DevicePath}",
    //         _settingsAppLocal.DeviceInstancePath);
    // }
    
    /// <summary>
    /// Command to open the log file in VSCode
    /// </summary>
    [RelayCommand]
    private void OpenLogfileInVscode()
    {
        // This is just to parent the messageboxes
        var temporaryWindow = new Window()
        {
            Visibility = Visibility.Hidden,
            // Just hiding the window is not sufficient, as it still temporarily pops up the first time.
            // Therefore, make it transparent.
            AllowsTransparency = true,
            Background = System.Windows.Media.Brushes.Transparent,
            WindowStyle = WindowStyle.None,
            ShowInTaskbar = false
        };

        temporaryWindow.Show();
        
        var vscodeProcess = new Process();

        var newestFileName = FolderHelpers.GetNewestFileNameIn(ApplicationPaths.LogAppBasePath);
        
        const string vsCodePath = @"C:\Program Files\Microsoft VS Code\Code.exe";

        _logger.Information("Looking for VSCode at path: {VscodePath}", vsCodePath);
        
        if (!File.Exists(vsCodePath))
        {
            MessageBox.Show(temporaryWindow, 
                $"Please install Visual Studio Code to {vsCodePath} before trying to open log");
            
            return;
        }

        vscodeProcess.StartInfo.FileName = vsCodePath;
        vscodeProcess.StartInfo.Arguments = "\"" + Path.Combine(ApplicationPaths.LogAppBasePath, newestFileName) + "\"";
        
        vscodeProcess.Start();
    }
}