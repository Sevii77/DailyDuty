﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Windows.Settings.SettingsHeaders;

internal class CountdownTimersConfiguration : ICollapsibleHeader
{
    private CountdownTimerSettings Settings => Service.Configuration.TimerSettings;

    public void Dispose()
    {
            
    }

    public string HeaderText { get; } = "Countdown Timers Configuration";
    void ICollapsibleHeader.DrawContents()
    {
        ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

        if (ImGui.BeginTable("CountdownTimersTable", 2))
        {
            ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 200f);
            ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 50f);

            DrawDailyResetCountdownOptions();

            DrawWeeklyResetCountdownOptions();

            DrawFashionReportCountdownOptions();


            ImGui.EndTable();
        }

        DrawCountdownWidthSlider();

        ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
    }

    private void DrawCountdownWidthSlider()
    {
        ImGui.Text("Bar Size");

        ImGui.SameLine();

        ImGui.PushItemWidth(175);
        ImGui.SliderInt("", ref Settings.TimerWidth, 170, 600);
        ImGui.PopItemWidth();
    }

    private void DrawFashionReportCountdownOptions()
    {
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        Draw.NotificationField("Fashion Report Reset", HeaderText, ref Settings.FashionReportCountdownEnabled,
            "Show/Hide Fashion Report Reset Timer.");

        ImGui.TableNextColumn();
        ImGui.ColorEdit4("##Fashion Report Reset Bar Color", ref Settings.FashionReportCountdownColor,
            ImGuiColorEditFlags.NoInputs);
    }

    private void DrawWeeklyResetCountdownOptions()
    {
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        Draw.NotificationField("Weekly Reset", HeaderText, ref Settings.WeeklyCountdownEnabled,
            "Show/Hide Weekly Reset Timer.");

        ImGui.TableNextColumn();
        ImGui.ColorEdit4("##Weekly Reset Bar Color", ref Settings.WeeklyCountdownColor, ImGuiColorEditFlags.NoInputs);
    }

    private void DrawDailyResetCountdownOptions()
    {
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        Draw.NotificationField("Daily Reset", HeaderText, ref Settings.DailyCountdownEnabled,
            "Show/Hide Daily Reset Timer.");

        ImGui.TableNextColumn();
        ImGui.ColorEdit4("##Daily Reset Bar Color", ref Settings.DailyCountdownColor, ImGuiColorEditFlags.NoInputs);
    }
}