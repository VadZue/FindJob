﻿using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace FindJob;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    WindowSwitcher _sw;
    Database _db;
    public MainWindow(Database db, WindowSwitcher sw)
    {
        _sw = sw;
        _db = db;
        InitializeComponent();
    }

    private void VacanciesRequestBtn_Click(object sender, RoutedEventArgs e)
    {
        _sw.Open<VacanciesRequestWindow>(this);
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await UpdateVacancies();
    }

    private async Task UpdateVacancies()
    {
        var vacancies = await _db.Vacansies.ToListAsync();
        VacanciesGrid.ItemsSource = vacancies;
        VacanciTb.Text = vacancies.Count.ToString();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        await UpdateVacancies();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        _sw.Open<AnaliticsWindow>(this);
    }

}