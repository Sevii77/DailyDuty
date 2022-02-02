﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.ConfigurationSystem;
using DailyDuty.Data;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Utility.Signatures;
using ImGuiNET;
#pragma warning disable CS0649
#pragma warning disable CS0169

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal unsafe class EliteHunts : DisplayModule
    {
        private Weekly.EliteHuntSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].EliteHuntSettings;

        protected override GenericSettings GenericSettings => Settings;

        // https://github.com/SheepGoMeh/HuntBuddy/blob/master/Structs/MobHuntStruct.cs
        [Signature("D1 48 8D 0D ?? ?? ?? ?? 48 83 C4 20 5F E9 ?? ?? ?? ??", ScanType = ScanType.StaticAddress)]
        private EliteHuntStruct* HuntData;

        public EliteHunts()
        {
            CategoryString = "Elite Hunts";

            SignatureHelper.Initialise(this);
        }

        protected override void DisplayData()
        {
            ImGui.Text("Only the checked lines will be evaluated for notifications" +
                       "\n( If notifications are enabled )");

            for (int i = 0; i < 5; ++i)
            {
                var data = Settings.EliteHunts[i];

                DrawRow(Settings.EliteHunts[i].Item1, ref Settings.EliteHunts[i].Item2, Settings.EliteHunts[i].Item3);
            }
        }

        protected override void EditModeOptions()
        {
            if (ImGui.BeginTable($"##EditTable{CategoryString}", 3))
            {
                for (int i = 0; i < 5; ++i)
                {
                    var label = Settings.EliteHunts[i].Item1.ToString();

                    ImGui.TableNextColumn();
                    ImGui.Text(label);

                    ImGui.TableNextColumn();
                    if (ImGui.Button($"Mark Complete##{label}{CategoryString}", ImGuiHelpers.ScaledVector2(100, 25)))
                    {
                        Settings.EliteHunts[i].Item3 = true;
                        Service.Configuration.Save();
                    }

                    ImGui.TableNextColumn();
                }

                ImGui.EndTable();
            }


        }

        protected override void NotificationOptions()
        {
            OnLoginReminderCheckbox(Settings);

            OnTerritoryChangeCheckbox(Settings);
        }

        public override void Dispose()
        {
        }

        private void DisplayStatus(string label, HuntStatus status, bool active)
        {
            if (ImGui.BeginTable($"##{label}{CategoryString}", 3 ))
            {
                if (active)
                {
                    ImGui.TableNextColumn();
                    ImGui.Text(label);

                    ImGui.TableNextColumn();
                    DrawConditionalText(status.Killed, "Target Killed", "Target Alive");

                    ImGui.TableNextColumn();
                }
                else
                {
                    ImGui.TableNextColumn();
                    ImGui.Text(label);

                    ImGui.TableNextColumn();
                    DrawConditionalText(false, "", "Elite Mark Available");

                    ImGui.TableNextColumn();
                }

                ImGui.EndTable();
            }
        }

        private static void DrawConditionalText(bool condition, string trueString, string falseString)
        {
            if (condition)
            {
                ImGui.TextColored(new Vector4(0, 255, 0, 0.8f), trueString);
            }
            else
            {
                ImGui.TextColored(new Vector4(185, 0, 0, 0.8f), falseString);
            }
        }

        private void DrawRow(EliteHuntEnum status, ref bool track, bool active)
        {
            ImGui.Checkbox($"##{status.ToString()}{CategoryString}", ref track);

            ImGui.SameLine();

            DisplayStatus($"{status.ToString()}", HuntData->GetStatus(status), active);
        }
    }
}
