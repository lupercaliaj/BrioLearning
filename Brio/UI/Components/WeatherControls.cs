﻿using Brio.Game.World;
using Dalamud.Interface;
using ImGuiNET;
using System.Linq;

namespace Brio.UI.Components;
public static class WeatherControls
{
    private static string _searchTerm = string.Empty;

    public static void Draw()
    {
        if(ImGui.CollapsingHeader("Weather", ImGuiTreeNodeFlags.DefaultOpen))
        {
            var isLocked = WeatherService.Instance.WeatherOverrideEnabled;
            var isLockedPrevious = isLocked;
            ImGui.Checkbox("Lock Weather", ref isLocked);
            if(isLocked != isLockedPrevious)
                WeatherService.Instance.WeatherOverrideEnabled = isLocked;

            if(!isLocked) ImGui.BeginDisabled();
            var currentWeather = (int) WeatherService.Instance.CurrentWeather;
            var previousWeather = currentWeather;
            ImGui.SetNextItemWidth(150f);
            if(ImGui.InputInt("Weather", ref currentWeather, 0, 0))
            {
               // Nada
            }
            ImGui.SameLine();
            ImGui.PushFont(UiBuilder.IconFont);
            if(ImGui.Button(FontAwesomeIcon.Search.ToIconString()))
            {
                ImGui.OpenPopup("###global_weather_list");
            }
            ImGui.PopFont();


            ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
            if(ImGui.BeginListBox("###territory_weather_list", new System.Numerics.Vector2(0, 80)))
            {
                foreach(var weather in WeatherService.Instance.TerritoryWeatherTable)
                {
                    bool isSelected = weather.RowId == currentWeather;
                    if(ImGui.Selectable($"{weather.Name} ({weather.RowId})###territory_weather_{weather.RowId}", isSelected))
                    {
                        currentWeather = (byte) weather.RowId;
                    }
                }
                ImGui.EndListBox();
            }
            ImGui.PopItemWidth();

            if(ImGui.BeginPopup("###global_weather_list"))
            {
                ImGui.InputText("###global_weather_search", ref _searchTerm, 64);

                if(ImGui.BeginListBox("###global_weather_listbox"))
                {
                    var list = WeatherService.Instance.WeatherTable.Where(x => x.Name.RawString.Contains(_searchTerm, System.StringComparison.CurrentCultureIgnoreCase)).ToList();
                    foreach(var weather in list)
                    {
                        if(ImGui.Selectable($"{weather.Name} ({weather.RowId})###global_weather_{weather.RowId}", false))
                        {
                            currentWeather = (byte)weather.RowId;
                        }
                    }
                    ImGui.EndListBox();
                }

                ImGui.EndPopup();
            }

            if(isLocked && currentWeather != previousWeather)
            {
                WeatherService.Instance.CurrentWeather = (byte)currentWeather;
            }

            if(!isLocked) ImGui.EndDisabled();
        }
    }
}