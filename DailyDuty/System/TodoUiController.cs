﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Atk;

namespace DailyDuty.System;

public unsafe class TodoUiController : IDisposable
{
    private static AtkUnitBase* AddonNamePlate => (AtkUnitBase*) Service.GameGui.GetAddonByName("NamePlate");

    private readonly ResNode rootNode;

    private const uint ContainerNodeId = 1000;
    private const uint BackgroundImageBaseId = 5000;
    private const uint ExtrasBaseId = 6000;

    private readonly Dictionary<ModuleType, TodoUiCategoryController> categories = new();
    private readonly ImageNode backgroundImageNode;
    private readonly TextNode previewModeTextNode;

    public TodoUiController()
    {
        rootNode = new ResNode(new ResNodeOptions
        {
            Id = ContainerNodeId,
            Position = new Vector2(2560 / 2.0f, 200),
            Size = new Vector2(200.0f, 200.0f),
        });
        
        Node.LinkNodeAtStart(rootNode.GetResourceNode(), AddonNamePlate);
        
        backgroundImageNode = new ImageNode(new ImageNodeOptions
        {
            Id = BackgroundImageBaseId,
            Color = new Vector4(1.0f, 1.0f, 1.0f, 0.25f),
        });
        backgroundImageNode.GetResourceNode()->SetScale(1.1f, 1.1f);
        rootNode.AddResourceNode(backgroundImageNode, AddonNamePlate);

        previewModeTextNode = new TextNode(new TextNodeOptions
        {
            Id = ExtrasBaseId,
            Alignment = AlignmentType.Center,
            Flags = TextFlags.Edge,
            Type = NodeType.Text,
            BackgroundColor = KnownColor.Black.AsVector4(),
            EdgeColor = KnownColor.Black.AsVector4(),
            TextColor = KnownColor.OrangeRed.AsVector4(),
            FontSize = 16
        });
        previewModeTextNode.SetText("Preview Mode is Enabled");
        rootNode.AddResourceNode(previewModeTextNode, AddonNamePlate);

        foreach (var category in Enum.GetValues<ModuleType>())
        {
            var newCategory = new TodoUiCategoryController(rootNode, category);
            categories.Add(category, newCategory);
        }
    }
    
    public void Dispose()
    {
        Node.UnlinkNodeAtStart(rootNode.GetResourceNode(), AddonNamePlate);
        rootNode.Dispose();
        backgroundImageNode.Dispose();
        previewModeTextNode.Dispose();

        foreach (var category in categories)
        {
            category.Value.Dispose();
        }
        
        categories.Clear();
    }

    public void Update(TodoConfig config)
    {
        foreach (var category in categories)
        {
            category.Value.UpdatePositions(config);
        }

        UpdatePositions(config);
    }
    
    private void UpdatePositions(TodoConfig config)
    {
        rootNode.GetResourceNode()->SetPositionFloat(config.Position.X, config.Position.Y);
        
        var cumulativeSize = 0;
        var padding = config.CategorySpacing;

        ushort largestWidth = 0;
        var anyVisible = false;
        
        foreach (var category in categories)
        {
            var resNode = category.Value.GetCategoryContainer().GetResourceNode();

            if (resNode->Width > largestWidth) largestWidth = resNode->Width;
            
            if (resNode->IsVisible)
            {
                anyVisible = true;
                var xPos = config.RightAlign ? rootNode.GetResourceNode()->Width - resNode->Width : 0.0f;
                
                resNode->SetPositionFloat(xPos, cumulativeSize);
                cumulativeSize += resNode->GetHeight() + padding;
            }
        }

        var finalHeight = (ushort) (cumulativeSize - config.CategorySpacing);
        
        rootNode.GetResourceNode()->SetHeight(finalHeight);
        rootNode.GetResourceNode()->SetWidth(largestWidth);
        
        backgroundImageNode.GetResourceNode()->ToggleVisibility(config.BackgroundImage && anyVisible);
        backgroundImageNode.GetResourceNode()->SetPositionFloat(-largestWidth * 0.05f, -finalHeight * 0.05f);
        backgroundImageNode.GetResourceNode()->SetHeight(finalHeight);
        backgroundImageNode.GetResourceNode()->SetWidth(largestWidth);
        
        previewModeTextNode.SetVisible(config.PreviewMode);
        previewModeTextNode.GetResourceNode()->SetWidth(largestWidth);
        previewModeTextNode.GetResourceNode()->SetPositionFloat(0.0f, -24.0f);
    }
    
    public void UpdateCategoryStyle(ModuleType type, ImageNodeOptions options)
    {
        options.Id = BackgroundImageBaseId;
        backgroundImageNode.UpdateOptions(options);

        backgroundImageNode.GetResourceNode()->Color.A = (byte) (options.Color.W * 255);
        backgroundImageNode.GetResourceNode()->AddRed = (byte) (options.Color.X * 255);
        backgroundImageNode.GetResourceNode()->AddGreen = (byte) (options.Color.Y * 255);
        backgroundImageNode.GetResourceNode()->AddBlue = (byte) (options.Color.Z * 255);
    }

    public void UpdateModule(ModuleType type, ModuleName module, string label, bool visible) => categories[type].UpdateModule(module, label, visible);
    public void UpdateModuleStyle(ModuleType type, ModuleName module, TextNodeOptions options) => categories[type].UpdateModuleStyle(module, options);
    public void UpdateCategoryHeader(ModuleType type, string label, bool show) => categories[type].UpdateCategoryHeader(label, show);
    public void UpdateHeaderStyle(ModuleType type, TextNodeOptions options) => categories[type].UpdateHeaderStyle(options);
    public void UpdateCategory(ModuleType type, bool enabled) => categories[type].SetVisible(enabled);
    public void Show(bool visible) => rootNode.SetVisibility(visible);
    public void Hide() => rootNode.SetVisibility(false);
}