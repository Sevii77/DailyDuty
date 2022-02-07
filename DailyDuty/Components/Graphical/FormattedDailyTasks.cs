﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;
using DailyDuty.System;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Components.Graphical;

internal class FormattedDailyTasks : ITaskCategoryDisplay
{
    private readonly List<ICompletable> tasks;
    List<ICompletable> ITaskCategoryDisplay.Tasks => tasks;

    public FormattedDailyTasks(List<ICompletable> tasks)
    {
        this.tasks = tasks;
    }

    public string HeaderText { get; } = "Daily Tasks";
}