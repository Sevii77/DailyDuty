﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Windows.Settings;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Interfaces;

internal interface IConfigurable : ICollapsibleHeader
{
    public GenericSettings GenericSettings { get; }

    void NotificationOptions();

    void EditModeOptions();

    void DisplayData();

    void ICollapsibleHeader.DrawContents()
    {
        ImGui.Checkbox($"Enabled##{HeaderText}", ref GenericSettings.Enabled);
        ImGui.Spacing();

        if (GenericSettings.Enabled)
        {
            ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

            DisplayData();

            ImGui.Spacing();

            if (ConfigurationTabItem.EditModeEnabled)
            {
                ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

                EditModeOptions();
                ImGui.Spacing();

                ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
            }

            NotificationOptions();
            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
        }

        ImGui.Spacing();
    }
}