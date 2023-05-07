using System.Collections.Generic;
using System.IO;
using Sccg.Builtin.Develop;
using Sccg.Builtin.Formatters;
using Sccg.Core;

namespace Sccg.Builtin.Sources;

/// <summary>
/// Source for VSCode editor theme color.
/// </summary>
public class VSCodeEditorThemeColorSource : SourceColorOnly<VSCodeEditorThemeColorSource.Group.Unit, VSCodeEditorThemeColorSource.Item>
{
    private readonly StdSourceImpl<Group.Unit> _impl = new();

    /// <inheritdoc />
    public override string Name => "VSCodeEditorThemeColor";

    /// <inheritdoc />
    protected override IEnumerable<Item> CollectItems()
    {
        var ids = _impl.Graph.TopologicalOrderList();
        var save = new Dictionary<Group.Unit, Color>();
        foreach (var id in ids)
        {
            var data = _impl.Store.Load(id).data;
            var next = _impl.Graph.GetLink(id);
            if (data is not Group.Unit unit || next is null)
            {
                continue;
            }

            var to = _impl.Store.Load(next.Value).data;
            switch (to)
            {
                case Color color:
                    save[unit] = color;
                    yield return new Item(unit, color);
                    break;
                case Group.Unit link:
                    if (!save.TryGetValue(link, out var linkColor))
                    {
                        throw new InvalidDataException($"Group `{unit.Name}` does not have specific color.");
                    }
                    save[unit] = linkColor;
                    yield return new Item(unit, linkColor);
                    break;
            }
        }
    }

    /// <inheritdoc />
    protected override void Set(Group.Unit group, Color color) => _impl.Set(group, color);

    /// <inheritdoc />
    protected override void Link(Group.Unit from, Group.Unit to) => _impl.Link(from, to);

    /// <summary>
    /// SourceItem for VSCode editor theme color.
    /// </summary>
    public sealed class Item : IVSCodeColorSourceItem
    {
        /// <summary>
        /// Gets the group.
        /// </summary>
        public readonly Group.Unit Group;

        /// <summary>
        /// Gets the color.
        /// </summary>
        public readonly Color Color;

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        public Item(Group.Unit group, Color color)
        {
            Group = group;
            Color = color;
        }

        /// <inheritdoc />
        public VSCodeFormatter.ColorFormattable Extract()
        {
            return new VSCodeFormatter.ColorFormattable
            {
                Name = Group.Name,
                Color = Color,
            };
        }
    }

    /// <summary>
    /// https://github.com/microsoft/vscode-docs/blob/main/api/references/theme-color.md
    /// </summary>
    public static class Group
    {
        public record Unit(string Name);

        /// <summary>
        /// activityBar.activeBackground
        /// </summary>
        public static readonly Unit ActivityBarActiveBackground = new("activityBar.activeBackground");

        /// <summary>
        /// activityBar.activeBorder
        /// </summary>
        public static readonly Unit ActivityBarActiveBorder = new("activityBar.activeBorder");

        /// <summary>
        /// activityBar.activeFocusBorder
        /// </summary>
        public static readonly Unit ActivityBarActiveFocusBorder = new("activityBar.activeFocusBorder");

        /// <summary>
        /// activityBar.background
        /// </summary>
        public static readonly Unit ActivityBarBackground = new("activityBar.background");

        /// <summary>
        /// activityBar.border
        /// </summary>
        public static readonly Unit ActivityBarBorder = new("activityBar.border");

        /// <summary>
        /// activityBar.dropBorder
        /// </summary>
        public static readonly Unit ActivityBarDropBorder = new("activityBar.dropBorder");

        /// <summary>
        /// activityBar.foreground
        /// </summary>
        public static readonly Unit ActivityBarForeground = new("activityBar.foreground");

        /// <summary>
        /// activityBar.inactiveForeground
        /// </summary>
        public static readonly Unit ActivityBarInactiveForeground = new("activityBar.inactiveForeground");

        /// <summary>
        /// activityBarBadge.background
        /// </summary>
        public static readonly Unit ActivityBarBadgeBackground = new("activityBarBadge.background");

        /// <summary>
        /// activityBarBadge.foreground
        /// </summary>
        public static readonly Unit ActivityBarBadgeForeground = new("activityBarBadge.foreground");

        /// <summary>
        /// badge.background
        /// </summary>
        public static readonly Unit BadgeBackground = new("badge.background");

        /// <summary>
        /// badge.foreground
        /// </summary>
        public static readonly Unit BadgeForeground = new("badge.foreground");

        /// <summary>
        /// banner.background
        /// </summary>
        public static readonly Unit BannerBackground = new("banner.background");

        /// <summary>
        /// banner.foreground
        /// </summary>
        public static readonly Unit BannerForeground = new("banner.foreground");

        /// <summary>
        /// banner.iconForeground
        /// </summary>
        public static readonly Unit BannerIconForeground = new("banner.iconForeground");

        /// <summary>
        /// breadcrumb.activeSelectionForeground
        /// </summary>
        public static readonly Unit BreadcrumbActiveSelectionForeground = new("breadcrumb.activeSelectionForeground");

        /// <summary>
        /// breadcrumb.background
        /// </summary>
        public static readonly Unit BreadcrumbBackground = new("breadcrumb.background");

        /// <summary>
        /// breadcrumb.focusForeground
        /// </summary>
        public static readonly Unit BreadcrumbFocusForeground = new("breadcrumb.focusForeground");

        /// <summary>
        /// breadcrumb.foreground
        /// </summary>
        public static readonly Unit BreadcrumbForeground = new("breadcrumb.foreground");

        /// <summary>
        /// breadcrumbPicker.background
        /// </summary>
        public static readonly Unit BreadcrumbPickerBackground = new("breadcrumbPicker.background");

        /// <summary>
        /// button.background
        /// </summary>
        public static readonly Unit ButtonBackground = new("button.background");

        /// <summary>
        /// button.border
        /// </summary>
        public static readonly Unit ButtonBorder = new("button.border");

        /// <summary>
        /// button.foreground
        /// </summary>
        public static readonly Unit ButtonForeground = new("button.foreground");

        /// <summary>
        /// button.hoverBackground
        /// </summary>
        public static readonly Unit ButtonHoverBackground = new("button.hoverBackground");

        /// <summary>
        /// button.secondaryBackground
        /// </summary>
        public static readonly Unit ButtonSecondaryBackground = new("button.secondaryBackground");

        /// <summary>
        /// button.secondaryForeground
        /// </summary>
        public static readonly Unit ButtonSecondaryForeground = new("button.secondaryForeground");

        /// <summary>
        /// button.secondaryHoverBackground
        /// </summary>
        public static readonly Unit ButtonSecondaryHoverBackground = new("button.secondaryHoverBackground");

        /// <summary>
        /// button.separator
        /// </summary>
        public static readonly Unit ButtonSeparator = new("button.separator");

        /// <summary>
        /// charts.blue
        /// </summary>
        public static readonly Unit ChartsBlue = new("charts.blue");

        /// <summary>
        /// charts.foreground
        /// </summary>
        public static readonly Unit ChartsForeground = new("charts.foreground");

        /// <summary>
        /// charts.green
        /// </summary>
        public static readonly Unit ChartsGreen = new("charts.green");

        /// <summary>
        /// charts.lines
        /// </summary>
        public static readonly Unit ChartsLines = new("charts.lines");

        /// <summary>
        /// charts.orange
        /// </summary>
        public static readonly Unit ChartsOrange = new("charts.orange");

        /// <summary>
        /// charts.purple
        /// </summary>
        public static readonly Unit ChartsPurple = new("charts.purple");

        /// <summary>
        /// charts.red
        /// </summary>
        public static readonly Unit ChartsRed = new("charts.red");

        /// <summary>
        /// charts.yellow
        /// </summary>
        public static readonly Unit ChartsYellow = new("charts.yellow");

        /// <summary>
        /// checkbox.background
        /// </summary>
        public static readonly Unit CheckboxBackground = new("checkbox.background");

        /// <summary>
        /// checkbox.border
        /// </summary>
        public static readonly Unit CheckboxBorder = new("checkbox.border");

        /// <summary>
        /// checkbox.foreground
        /// </summary>
        public static readonly Unit CheckboxForeground = new("checkbox.foreground");

        /// <summary>
        /// checkbox.selectBackground
        /// </summary>
        public static readonly Unit CheckboxSelectBackground = new("checkbox.selectBackground");

        /// <summary>
        /// checkbox.selectBorder
        /// </summary>
        public static readonly Unit CheckboxSelectBorder = new("checkbox.selectBorder");

        /// <summary>
        /// commandCenter.activeBackground
        /// </summary>
        public static readonly Unit CommandCenterActiveBackground = new("commandCenter.activeBackground");

        /// <summary>
        /// commandCenter.activeBorder
        /// </summary>
        public static readonly Unit CommandCenterActiveBorder = new("commandCenter.activeBorder");

        /// <summary>
        /// commandCenter.activeForeground
        /// </summary>
        public static readonly Unit CommandCenterActiveForeground = new("commandCenter.activeForeground");

        /// <summary>
        /// commandCenter.background
        /// </summary>
        public static readonly Unit CommandCenterBackground = new("commandCenter.background");

        /// <summary>
        /// commandCenter.border
        /// </summary>
        public static readonly Unit CommandCenterBorder = new("commandCenter.border");

        /// <summary>
        /// commandCenter.foreground
        /// </summary>
        public static readonly Unit CommandCenterForeground = new("commandCenter.foreground");

        /// <summary>
        /// commandCenter.inactiveBorder
        /// </summary>
        public static readonly Unit CommandCenterInactiveBorder = new("commandCenter.inactiveBorder");

        /// <summary>
        /// commandCenter.inactiveForeground
        /// </summary>
        public static readonly Unit CommandCenterInactiveForeground = new("commandCenter.inactiveForeground");

        /// <summary>
        /// contrastActiveBorder
        /// </summary>
        public static readonly Unit ContrastActiveBorder = new("contrastActiveBorder");

        /// <summary>
        /// contrastBorder
        /// </summary>
        public static readonly Unit ContrastBorder = new("contrastBorder");

        /// <summary>
        /// debugConsole.errorForeground
        /// </summary>
        public static readonly Unit DebugConsoleErrorForeground = new("debugConsole.errorForeground");

        /// <summary>
        /// debugConsole.infoForeground
        /// </summary>
        public static readonly Unit DebugConsoleInfoForeground = new("debugConsole.infoForeground");

        /// <summary>
        /// debugConsole.sourceForeground
        /// </summary>
        public static readonly Unit DebugConsoleSourceForeground = new("debugConsole.sourceForeground");

        /// <summary>
        /// debugConsole.warningForeground
        /// </summary>
        public static readonly Unit DebugConsoleWarningForeground = new("debugConsole.warningForeground");

        /// <summary>
        /// debugConsoleInputIcon.foreground
        /// </summary>
        public static readonly Unit DebugConsoleInputIconForeground = new("debugConsoleInputIcon.foreground");

        /// <summary>
        /// debugExceptionWidget.background
        /// </summary>
        public static readonly Unit DebugExceptionWidgetBackground = new("debugExceptionWidget.background");

        /// <summary>
        /// debugExceptionWidget.border
        /// </summary>
        public static readonly Unit DebugExceptionWidgetBorder = new("debugExceptionWidget.border");

        /// <summary>
        /// debugIcon.breakpointCurrentStackframeForeground
        /// </summary>
        public static readonly Unit DebugIconBreakpointCurrentStackframeForeground
            = new("debugIcon.breakpointCurrentStackframeForeground");

        /// <summary>
        /// debugIcon.breakpointDisabledForeground
        /// </summary>
        public static readonly Unit DebugIconBreakpointDisabledForeground
            = new("debugIcon.breakpointDisabledForeground");

        /// <summary>
        /// debugIcon.breakpointForeground
        /// </summary>
        public static readonly Unit DebugIconBreakpointForeground = new("debugIcon.breakpointForeground");

        /// <summary>
        /// debugIcon.breakpointStackframeForeground
        /// </summary>
        public static readonly Unit DebugIconBreakpointStackframeForeground
            = new("debugIcon.breakpointStackframeForeground");

        /// <summary>
        /// debugIcon.breakpointUnverifiedForeground
        /// </summary>
        public static readonly Unit DebugIconBreakpointUnverifiedForeground
            = new("debugIcon.breakpointUnverifiedForeground");

        /// <summary>
        /// debugIcon.continueForeground
        /// </summary>
        public static readonly Unit DebugIconContinueForeground = new("debugIcon.continueForeground");

        /// <summary>
        /// debugIcon.disconnectForeground
        /// </summary>
        public static readonly Unit DebugIconDisconnectForeground = new("debugIcon.disconnectForeground");

        /// <summary>
        /// debugIcon.pauseForeground
        /// </summary>
        public static readonly Unit DebugIconPauseForeground = new("debugIcon.pauseForeground");

        /// <summary>
        /// debugIcon.restartForeground
        /// </summary>
        public static readonly Unit DebugIconRestartForeground = new("debugIcon.restartForeground");

        /// <summary>
        /// debugIcon.startForeground
        /// </summary>
        public static readonly Unit DebugIconStartForeground = new("debugIcon.startForeground");

        /// <summary>
        /// debugIcon.stepBackForeground
        /// </summary>
        public static readonly Unit DebugIconStepBackForeground = new("debugIcon.stepBackForeground");

        /// <summary>
        /// debugIcon.stepIntoForeground
        /// </summary>
        public static readonly Unit DebugIconStepIntoForeground = new("debugIcon.stepIntoForeground");

        /// <summary>
        /// debugIcon.stepOutForeground
        /// </summary>
        public static readonly Unit DebugIconStepOutForeground = new("debugIcon.stepOutForeground");

        /// <summary>
        /// debugIcon.stepOverForeground
        /// </summary>
        public static readonly Unit DebugIconStepOverForeground = new("debugIcon.stepOverForeground");

        /// <summary>
        /// debugIcon.stopForeground
        /// </summary>
        public static readonly Unit DebugIconStopForeground = new("debugIcon.stopForeground");

        /// <summary>
        /// debugTokenExpression.boolean
        /// </summary>
        public static readonly Unit DebugTokenExpressionBoolean = new("debugTokenExpression.boolean");

        /// <summary>
        /// debugTokenExpression.error
        /// </summary>
        public static readonly Unit DebugTokenExpressionError = new("debugTokenExpression.error");

        /// <summary>
        /// debugTokenExpression.name
        /// </summary>
        public static readonly Unit DebugTokenExpressionName = new("debugTokenExpression.name");

        /// <summary>
        /// debugTokenExpression.number
        /// </summary>
        public static readonly Unit DebugTokenExpressionNumber = new("debugTokenExpression.number");

        /// <summary>
        /// debugTokenExpression.string
        /// </summary>
        public static readonly Unit DebugTokenExpressionString = new("debugTokenExpression.string");

        /// <summary>
        /// debugTokenExpression.value
        /// </summary>
        public static readonly Unit DebugTokenExpressionValue = new("debugTokenExpression.value");

        /// <summary>
        /// debugToolBar.background
        /// </summary>
        public static readonly Unit DebugToolBarBackground = new("debugToolBar.background");

        /// <summary>
        /// debugToolBar.border
        /// </summary>
        public static readonly Unit DebugToolBarBorder = new("debugToolBar.border");

        /// <summary>
        /// debugView.exceptionLabelBackground
        /// </summary>
        public static readonly Unit DebugViewExceptionLabelBackground = new("debugView.exceptionLabelBackground");

        /// <summary>
        /// debugView.exceptionLabelForeground
        /// </summary>
        public static readonly Unit DebugViewExceptionLabelForeground = new("debugView.exceptionLabelForeground");

        /// <summary>
        /// debugView.stateLabelBackground
        /// </summary>
        public static readonly Unit DebugViewStateLabelBackground = new("debugView.stateLabelBackground");

        /// <summary>
        /// debugView.stateLabelForeground
        /// </summary>
        public static readonly Unit DebugViewStateLabelForeground = new("debugView.stateLabelForeground");

        /// <summary>
        /// debugView.valueChangedHighlight
        /// </summary>
        public static readonly Unit DebugViewValueChangedHighlight = new("debugView.valueChangedHighlight");

        /// <summary>
        /// descriptionForeground
        /// </summary>
        public static readonly Unit DescriptionForeground = new("descriptionForeground");

        /// <summary>
        /// diffEditor.border
        /// </summary>
        public static readonly Unit DiffEditorBorder = new("diffEditor.border");

        /// <summary>
        /// diffEditor.diagonalFill
        /// </summary>
        public static readonly Unit DiffEditorDiagonalFill = new("diffEditor.diagonalFill");

        /// <summary>
        /// diffEditor.insertedLineBackground
        /// </summary>
        public static readonly Unit DiffEditorInsertedLineBackground = new("diffEditor.insertedLineBackground");

        /// <summary>
        /// diffEditor.insertedTextBackground
        /// </summary>
        public static readonly Unit DiffEditorInsertedTextBackground = new("diffEditor.insertedTextBackground");

        /// <summary>
        /// diffEditor.insertedTextBorder
        /// </summary>
        public static readonly Unit DiffEditorInsertedTextBorder = new("diffEditor.insertedTextBorder");

        /// <summary>
        /// diffEditor.removedLineBackground
        /// </summary>
        public static readonly Unit DiffEditorRemovedLineBackground = new("diffEditor.removedLineBackground");

        /// <summary>
        /// diffEditor.removedTextBackground
        /// </summary>
        public static readonly Unit DiffEditorRemovedTextBackground = new("diffEditor.removedTextBackground");

        /// <summary>
        /// diffEditor.removedTextBorder
        /// </summary>
        public static readonly Unit DiffEditorRemovedTextBorder = new("diffEditor.removedTextBorder");

        /// <summary>
        /// diffEditorGutter.insertedLineBackground
        /// </summary>
        public static readonly Unit DiffEditorGutterInsertedLineBackground
            = new("diffEditorGutter.insertedLineBackground");

        /// <summary>
        /// diffEditorGutter.removedLineBackground
        /// </summary>
        public static readonly Unit DiffEditorGutterRemovedLineBackground
            = new("diffEditorGutter.removedLineBackground");

        /// <summary>
        /// diffEditorOverview.insertedForeground
        /// </summary>
        public static readonly Unit DiffEditorOverviewInsertedForeground = new("diffEditorOverview.insertedForeground");

        /// <summary>
        /// diffEditorOverview.removedForeground
        /// </summary>
        public static readonly Unit DiffEditorOverviewRemovedForeground = new("diffEditorOverview.removedForeground");

        /// <summary>
        /// disabledForeground
        /// </summary>
        public static readonly Unit DisabledForeground = new("disabledForeground");

        /// <summary>
        /// dropdown.background
        /// </summary>
        public static readonly Unit DropdownBackground = new("dropdown.background");

        /// <summary>
        /// dropdown.border
        /// </summary>
        public static readonly Unit DropdownBorder = new("dropdown.border");

        /// <summary>
        /// dropdown.foreground
        /// </summary>
        public static readonly Unit DropdownForeground = new("dropdown.foreground");

        /// <summary>
        /// dropdown.listBackground
        /// </summary>
        public static readonly Unit DropdownListBackground = new("dropdown.listBackground");

        /// <summary>
        /// editor.background
        /// </summary>
        public static readonly Unit EditorBackground = new("editor.background");

        /// <summary>
        /// editor.findMatchBackground
        /// </summary>
        public static readonly Unit EditorFindMatchBackground = new("editor.findMatchBackground");

        /// <summary>
        /// editor.findMatchBorder
        /// </summary>
        public static readonly Unit EditorFindMatchBorder = new("editor.findMatchBorder");

        /// <summary>
        /// editor.findMatchHighlightBackground
        /// </summary>
        public static readonly Unit EditorFindMatchHighlightBackground = new("editor.findMatchHighlightBackground");

        /// <summary>
        /// editor.findMatchHighlightBorder
        /// </summary>
        public static readonly Unit EditorFindMatchHighlightBorder = new("editor.findMatchHighlightBorder");

        /// <summary>
        /// editor.findRangeHighlightBackground
        /// </summary>
        public static readonly Unit EditorFindRangeHighlightBackground = new("editor.findRangeHighlightBackground");

        /// <summary>
        /// editor.findRangeHighlightBorder
        /// </summary>
        public static readonly Unit EditorFindRangeHighlightBorder = new("editor.findRangeHighlightBorder");

        /// <summary>
        /// editor.focusedStackFrameHighlightBackground
        /// </summary>
        public static readonly Unit EditorFocusedStackFrameHighlightBackground
            = new("editor.focusedStackFrameHighlightBackground");

        /// <summary>
        /// editor.foldBackground
        /// </summary>
        public static readonly Unit EditorFoldBackground = new("editor.foldBackground");

        /// <summary>
        /// editor.foreground
        /// </summary>
        public static readonly Unit EditorForeground = new("editor.foreground");

        /// <summary>
        /// editor.hoverHighlightBackground
        /// </summary>
        public static readonly Unit EditorHoverHighlightBackground = new("editor.hoverHighlightBackground");

        /// <summary>
        /// editor.inactiveSelectionBackground
        /// </summary>
        public static readonly Unit EditorInactiveSelectionBackground = new("editor.inactiveSelectionBackground");

        /// <summary>
        /// editor.inlineValuesBackground
        /// </summary>
        public static readonly Unit EditorInlineValuesBackground = new("editor.inlineValuesBackground");

        /// <summary>
        /// editor.inlineValuesForeground
        /// </summary>
        public static readonly Unit EditorInlineValuesForeground = new("editor.inlineValuesForeground");

        /// <summary>
        /// editor.lineHighlightBackground
        /// </summary>
        public static readonly Unit EditorLineHighlightBackground = new("editor.lineHighlightBackground");

        /// <summary>
        /// editor.lineHighlightBorder
        /// </summary>
        public static readonly Unit EditorLineHighlightBorder = new("editor.lineHighlightBorder");

        /// <summary>
        /// editor.linkedEditingBackground
        /// </summary>
        public static readonly Unit EditorLinkedEditingBackground = new("editor.linkedEditingBackground");

        /// <summary>
        /// editor.rangeHighlightBackground
        /// </summary>
        public static readonly Unit EditorRangeHighlightBackground = new("editor.rangeHighlightBackground");

        /// <summary>
        /// editor.rangeHighlightBorder
        /// </summary>
        public static readonly Unit EditorRangeHighlightBorder = new("editor.rangeHighlightBorder");

        /// <summary>
        /// editor.selectionBackground
        /// </summary>
        public static readonly Unit EditorSelectionBackground = new("editor.selectionBackground");

        /// <summary>
        /// editor.selectionForeground
        /// </summary>
        public static readonly Unit EditorSelectionForeground = new("editor.selectionForeground");

        /// <summary>
        /// editor.selectionHighlightBackground
        /// </summary>
        public static readonly Unit EditorSelectionHighlightBackground = new("editor.selectionHighlightBackground");

        /// <summary>
        /// editor.selectionHighlightBorder
        /// </summary>
        public static readonly Unit EditorSelectionHighlightBorder = new("editor.selectionHighlightBorder");

        /// <summary>
        /// editor.snippetFinalTabstopHighlightBackground
        /// </summary>
        public static readonly Unit EditorSnippetFinalTabstopHighlightBackground
            = new("editor.snippetFinalTabstopHighlightBackground");

        /// <summary>
        /// editor.snippetFinalTabstopHighlightBorder
        /// </summary>
        public static readonly Unit EditorSnippetFinalTabstopHighlightBorder
            = new("editor.snippetFinalTabstopHighlightBorder");

        /// <summary>
        /// editor.snippetTabstopHighlightBackground
        /// </summary>
        public static readonly Unit EditorSnippetTabstopHighlightBackground
            = new("editor.snippetTabstopHighlightBackground");

        /// <summary>
        /// editor.snippetTabstopHighlightBorder
        /// </summary>
        public static readonly Unit EditorSnippetTabstopHighlightBorder = new("editor.snippetTabstopHighlightBorder");

        /// <summary>
        /// editor.stackFrameHighlightBackground
        /// </summary>
        public static readonly Unit EditorStackFrameHighlightBackground = new("editor.stackFrameHighlightBackground");

        /// <summary>
        /// editor.symbolHighlightBackground
        /// </summary>
        public static readonly Unit EditorSymbolHighlightBackground = new("editor.symbolHighlightBackground");

        /// <summary>
        /// editor.symbolHighlightBorder
        /// </summary>
        public static readonly Unit EditorSymbolHighlightBorder = new("editor.symbolHighlightBorder");

        /// <summary>
        /// editor.wordHighlightBackground
        /// </summary>
        public static readonly Unit EditorWordHighlightBackground = new("editor.wordHighlightBackground");

        /// <summary>
        /// editor.wordHighlightBorder
        /// </summary>
        public static readonly Unit EditorWordHighlightBorder = new("editor.wordHighlightBorder");

        /// <summary>
        /// editor.wordHighlightStrongBackground
        /// </summary>
        public static readonly Unit EditorWordHighlightStrongBackground = new("editor.wordHighlightStrongBackground");

        /// <summary>
        /// editor.wordHighlightStrongBorder
        /// </summary>
        public static readonly Unit EditorWordHighlightStrongBorder = new("editor.wordHighlightStrongBorder");

        /// <summary>
        /// editor.wordHighlightTextBackground
        /// </summary>
        public static readonly Unit EditorWordHighlightTextBackground = new("editor.wordHighlightTextBackground");

        /// <summary>
        /// editor.wordHighlightTextBorder
        /// </summary>
        public static readonly Unit EditorWordHighlightTextBorder = new("editor.wordHighlightTextBorder");

        /// <summary>
        /// editorBracketHighlight.foreground1
        /// </summary>
        public static readonly Unit EditorBracketHighlightForeground1 = new("editorBracketHighlight.foreground1");

        /// <summary>
        /// editorBracketHighlight.foreground2
        /// </summary>
        public static readonly Unit EditorBracketHighlightForeground2 = new("editorBracketHighlight.foreground2");

        /// <summary>
        /// editorBracketHighlight.foreground3
        /// </summary>
        public static readonly Unit EditorBracketHighlightForeground3 = new("editorBracketHighlight.foreground3");

        /// <summary>
        /// editorBracketHighlight.foreground4
        /// </summary>
        public static readonly Unit EditorBracketHighlightForeground4 = new("editorBracketHighlight.foreground4");

        /// <summary>
        /// editorBracketHighlight.foreground5
        /// </summary>
        public static readonly Unit EditorBracketHighlightForeground5 = new("editorBracketHighlight.foreground5");

        /// <summary>
        /// editorBracketHighlight.foreground6
        /// </summary>
        public static readonly Unit EditorBracketHighlightForeground6 = new("editorBracketHighlight.foreground6");

        /// <summary>
        /// editorBracketHighlight.unexpectedBracket.foreground
        /// </summary>
        public static readonly Unit EditorBracketHighlightUnexpectedBracketForeground
            = new("editorBracketHighlight.unexpectedBracket.foreground");

        /// <summary>
        /// editorBracketMatch.background
        /// </summary>
        public static readonly Unit EditorBracketMatchBackground = new("editorBracketMatch.background");

        /// <summary>
        /// editorBracketMatch.border
        /// </summary>
        public static readonly Unit EditorBracketMatchBorder = new("editorBracketMatch.border");

        /// <summary>
        /// editorBracketPairGuide.activeBackground1
        /// </summary>
        public static readonly Unit EditorBracketPairGuideActiveBackground1
            = new("editorBracketPairGuide.activeBackground1");

        /// <summary>
        /// editorBracketPairGuide.activeBackground2
        /// </summary>
        public static readonly Unit EditorBracketPairGuideActiveBackground2
            = new("editorBracketPairGuide.activeBackground2");

        /// <summary>
        /// editorBracketPairGuide.activeBackground3
        /// </summary>
        public static readonly Unit EditorBracketPairGuideActiveBackground3
            = new("editorBracketPairGuide.activeBackground3");

        /// <summary>
        /// editorBracketPairGuide.activeBackground4
        /// </summary>
        public static readonly Unit EditorBracketPairGuideActiveBackground4
            = new("editorBracketPairGuide.activeBackground4");

        /// <summary>
        /// editorBracketPairGuide.activeBackground5
        /// </summary>
        public static readonly Unit EditorBracketPairGuideActiveBackground5
            = new("editorBracketPairGuide.activeBackground5");

        /// <summary>
        /// editorBracketPairGuide.activeBackground6
        /// </summary>
        public static readonly Unit EditorBracketPairGuideActiveBackground6
            = new("editorBracketPairGuide.activeBackground6");

        /// <summary>
        /// editorBracketPairGuide.background1
        /// </summary>
        public static readonly Unit EditorBracketPairGuideBackground1 = new("editorBracketPairGuide.background1");

        /// <summary>
        /// editorBracketPairGuide.background2
        /// </summary>
        public static readonly Unit EditorBracketPairGuideBackground2 = new("editorBracketPairGuide.background2");

        /// <summary>
        /// editorBracketPairGuide.background3
        /// </summary>
        public static readonly Unit EditorBracketPairGuideBackground3 = new("editorBracketPairGuide.background3");

        /// <summary>
        /// editorBracketPairGuide.background4
        /// </summary>
        public static readonly Unit EditorBracketPairGuideBackground4 = new("editorBracketPairGuide.background4");

        /// <summary>
        /// editorBracketPairGuide.background5
        /// </summary>
        public static readonly Unit EditorBracketPairGuideBackground5 = new("editorBracketPairGuide.background5");

        /// <summary>
        /// editorBracketPairGuide.background6
        /// </summary>
        public static readonly Unit EditorBracketPairGuideBackground6 = new("editorBracketPairGuide.background6");

        /// <summary>
        /// editorCodeLens.foreground
        /// </summary>
        public static readonly Unit EditorCodeLensForeground = new("editorCodeLens.foreground");

        /// <summary>
        /// editorCommentsWidget.rangeActiveBackground
        /// </summary>
        public static readonly Unit EditorCommentsWidgetRangeActiveBackground
            = new("editorCommentsWidget.rangeActiveBackground");

        /// <summary>
        /// editorCommentsWidget.rangeActiveBorder
        /// </summary>
        public static readonly Unit EditorCommentsWidgetRangeActiveBorder
            = new("editorCommentsWidget.rangeActiveBorder");

        /// <summary>
        /// editorCommentsWidget.rangeBackground
        /// </summary>
        public static readonly Unit EditorCommentsWidgetRangeBackground = new("editorCommentsWidget.rangeBackground");

        /// <summary>
        /// editorCommentsWidget.rangeBorder
        /// </summary>
        public static readonly Unit EditorCommentsWidgetRangeBorder = new("editorCommentsWidget.rangeBorder");

        /// <summary>
        /// editorCommentsWidget.resolvedBorder
        /// </summary>
        public static readonly Unit EditorCommentsWidgetResolvedBorder = new("editorCommentsWidget.resolvedBorder");

        /// <summary>
        /// editorCommentsWidget.unresolvedBorder
        /// </summary>
        public static readonly Unit EditorCommentsWidgetUnresolvedBorder = new("editorCommentsWidget.unresolvedBorder");

        /// <summary>
        /// editorCursor.background
        /// </summary>
        public static readonly Unit EditorCursorBackground = new("editorCursor.background");

        /// <summary>
        /// editorCursor.foreground
        /// </summary>
        public static readonly Unit EditorCursorForeground = new("editorCursor.foreground");

        /// <summary>
        /// editorError.background
        /// </summary>
        public static readonly Unit EditorErrorBackground = new("editorError.background");

        /// <summary>
        /// editorError.border
        /// </summary>
        public static readonly Unit EditorErrorBorder = new("editorError.border");

        /// <summary>
        /// editorError.foreground
        /// </summary>
        public static readonly Unit EditorErrorForeground = new("editorError.foreground");

        /// <summary>
        /// editorGhostText.background
        /// </summary>
        public static readonly Unit EditorGhostTextBackground = new("editorGhostText.background");

        /// <summary>
        /// editorGhostText.border
        /// </summary>
        public static readonly Unit EditorGhostTextBorder = new("editorGhostText.border");

        /// <summary>
        /// editorGhostText.foreground
        /// </summary>
        public static readonly Unit EditorGhostTextForeground = new("editorGhostText.foreground");

        /// <summary>
        /// editorGroup.border
        /// </summary>
        public static readonly Unit EditorGroupBorder = new("editorGroup.border");

        /// <summary>
        /// editorGroup.dropBackground
        /// </summary>
        public static readonly Unit EditorGroupDropBackground = new("editorGroup.dropBackground");

        /// <summary>
        /// editorGroup.dropIntoPromptBackground
        /// </summary>
        public static readonly Unit EditorGroupDropIntoPromptBackground = new("editorGroup.dropIntoPromptBackground");

        /// <summary>
        /// editorGroup.dropIntoPromptBorder
        /// </summary>
        public static readonly Unit EditorGroupDropIntoPromptBorder = new("editorGroup.dropIntoPromptBorder");

        /// <summary>
        /// editorGroup.dropIntoPromptForeground
        /// </summary>
        public static readonly Unit EditorGroupDropIntoPromptForeground = new("editorGroup.dropIntoPromptForeground");

        /// <summary>
        /// editorGroup.emptyBackground
        /// </summary>
        public static readonly Unit EditorGroupEmptyBackground = new("editorGroup.emptyBackground");

        /// <summary>
        /// editorGroup.focusedEmptyBorder
        /// </summary>
        public static readonly Unit EditorGroupFocusedEmptyBorder = new("editorGroup.focusedEmptyBorder");

        /// <summary>
        /// editorGroupHeader.border
        /// </summary>
        public static readonly Unit EditorGroupHeaderBorder = new("editorGroupHeader.border");

        /// <summary>
        /// editorGroupHeader.noTabsBackground
        /// </summary>
        public static readonly Unit EditorGroupHeaderNoTabsBackground = new("editorGroupHeader.noTabsBackground");

        /// <summary>
        /// editorGroupHeader.tabsBackground
        /// </summary>
        public static readonly Unit EditorGroupHeaderTabsBackground = new("editorGroupHeader.tabsBackground");

        /// <summary>
        /// editorGroupHeader.tabsBorder
        /// </summary>
        public static readonly Unit EditorGroupHeaderTabsBorder = new("editorGroupHeader.tabsBorder");

        /// <summary>
        /// editorGutter.addedBackground
        /// </summary>
        public static readonly Unit EditorGutterAddedBackground = new("editorGutter.addedBackground");

        /// <summary>
        /// editorGutter.background
        /// </summary>
        public static readonly Unit EditorGutterBackground = new("editorGutter.background");

        /// <summary>
        /// editorGutter.commentGlyphForeground
        /// </summary>
        public static readonly Unit EditorGutterCommentGlyphForeground = new("editorGutter.commentGlyphForeground");

        /// <summary>
        /// editorGutter.commentRangeForeground
        /// </summary>
        public static readonly Unit EditorGutterCommentRangeForeground = new("editorGutter.commentRangeForeground");

        /// <summary>
        /// editorGutter.commentUnresolvedGlyphForeground
        /// </summary>
        public static readonly Unit EditorGutterCommentUnresolvedGlyphForeground
            = new("editorGutter.commentUnresolvedGlyphForeground");

        /// <summary>
        /// editorGutter.deletedBackground
        /// </summary>
        public static readonly Unit EditorGutterDeletedBackground = new("editorGutter.deletedBackground");

        /// <summary>
        /// editorGutter.foldingControlForeground
        /// </summary>
        public static readonly Unit EditorGutterFoldingControlForeground = new("editorGutter.foldingControlForeground");

        /// <summary>
        /// editorGutter.modifiedBackground
        /// </summary>
        public static readonly Unit EditorGutterModifiedBackground = new("editorGutter.modifiedBackground");

        /// <summary>
        /// editorHint.border
        /// </summary>
        public static readonly Unit EditorHintBorder = new("editorHint.border");

        /// <summary>
        /// editorHint.foreground
        /// </summary>
        public static readonly Unit EditorHintForeground = new("editorHint.foreground");

        /// <summary>
        /// editorHoverWidget.background
        /// </summary>
        public static readonly Unit EditorHoverWidgetBackground = new("editorHoverWidget.background");

        /// <summary>
        /// editorHoverWidget.border
        /// </summary>
        public static readonly Unit EditorHoverWidgetBorder = new("editorHoverWidget.border");

        /// <summary>
        /// editorHoverWidget.foreground
        /// </summary>
        public static readonly Unit EditorHoverWidgetForeground = new("editorHoverWidget.foreground");

        /// <summary>
        /// editorHoverWidget.highlightForeground
        /// </summary>
        public static readonly Unit EditorHoverWidgetHighlightForeground = new("editorHoverWidget.highlightForeground");

        /// <summary>
        /// editorHoverWidget.statusBarBackground
        /// </summary>
        public static readonly Unit EditorHoverWidgetStatusBarBackground = new("editorHoverWidget.statusBarBackground");

        /// <summary>
        /// editorIndentGuide.activeBackground
        /// </summary>
        public static readonly Unit EditorIndentGuideActiveBackground = new("editorIndentGuide.activeBackground");

        /// <summary>
        /// editorIndentGuide.background
        /// </summary>
        public static readonly Unit EditorIndentGuideBackground = new("editorIndentGuide.background");

        /// <summary>
        /// editorInfo.background
        /// </summary>
        public static readonly Unit EditorInfoBackground = new("editorInfo.background");

        /// <summary>
        /// editorInfo.border
        /// </summary>
        public static readonly Unit EditorInfoBorder = new("editorInfo.border");

        /// <summary>
        /// editorInfo.foreground
        /// </summary>
        public static readonly Unit EditorInfoForeground = new("editorInfo.foreground");

        /// <summary>
        /// editorInlayHint.background
        /// </summary>
        public static readonly Unit EditorInlayHintBackground = new("editorInlayHint.background");

        /// <summary>
        /// editorInlayHint.foreground
        /// </summary>
        public static readonly Unit EditorInlayHintForeground = new("editorInlayHint.foreground");

        /// <summary>
        /// editorInlayHint.parameterBackground
        /// </summary>
        public static readonly Unit EditorInlayHintParameterBackground = new("editorInlayHint.parameterBackground");

        /// <summary>
        /// editorInlayHint.parameterForeground
        /// </summary>
        public static readonly Unit EditorInlayHintParameterForeground = new("editorInlayHint.parameterForeground");

        /// <summary>
        /// editorInlayHint.typeBackground
        /// </summary>
        public static readonly Unit EditorInlayHintTypeBackground = new("editorInlayHint.typeBackground");

        /// <summary>
        /// editorInlayHint.typeForeground
        /// </summary>
        public static readonly Unit EditorInlayHintTypeForeground = new("editorInlayHint.typeForeground");

        /// <summary>
        /// editorLightBulb.foreground
        /// </summary>
        public static readonly Unit EditorLightBulbForeground = new("editorLightBulb.foreground");

        /// <summary>
        /// editorLightBulbAutoFix.foreground
        /// </summary>
        public static readonly Unit EditorLightBulbAutoFixForeground = new("editorLightBulbAutoFix.foreground");

        /// <summary>
        /// editorLineNumber.activeForeground
        /// </summary>
        public static readonly Unit EditorLineNumberActiveForeground = new("editorLineNumber.activeForeground");

        /// <summary>
        /// editorLineNumber.dimmedForeground
        /// </summary>
        public static readonly Unit EditorLineNumberDimmedForeground = new("editorLineNumber.dimmedForeground");

        /// <summary>
        /// editorLineNumber.foreground
        /// </summary>
        public static readonly Unit EditorLineNumberForeground = new("editorLineNumber.foreground");

        /// <summary>
        /// editorLink.activeForeground
        /// </summary>
        public static readonly Unit EditorLinkActiveForeground = new("editorLink.activeForeground");

        /// <summary>
        /// editorMarkerNavigation.background
        /// </summary>
        public static readonly Unit EditorMarkerNavigationBackground = new("editorMarkerNavigation.background");

        /// <summary>
        /// editorMarkerNavigationError.background
        /// </summary>
        public static readonly Unit EditorMarkerNavigationErrorBackground
            = new("editorMarkerNavigationError.background");

        /// <summary>
        /// editorMarkerNavigationError.headerBackground
        /// </summary>
        public static readonly Unit EditorMarkerNavigationErrorHeaderBackground
            = new("editorMarkerNavigationError.headerBackground");

        /// <summary>
        /// editorMarkerNavigationInfo.background
        /// </summary>
        public static readonly Unit EditorMarkerNavigationInfoBackground = new("editorMarkerNavigationInfo.background");

        /// <summary>
        /// editorMarkerNavigationInfo.headerBackground
        /// </summary>
        public static readonly Unit EditorMarkerNavigationInfoHeaderBackground
            = new("editorMarkerNavigationInfo.headerBackground");

        /// <summary>
        /// editorMarkerNavigationWarning.background
        /// </summary>
        public static readonly Unit EditorMarkerNavigationWarningBackground
            = new("editorMarkerNavigationWarning.background");

        /// <summary>
        /// editorMarkerNavigationWarning.headerBackground
        /// </summary>
        public static readonly Unit EditorMarkerNavigationWarningHeaderBackground
            = new("editorMarkerNavigationWarning.headerBackground");

        /// <summary>
        /// editorOverviewRuler.addedForeground
        /// </summary>
        public static readonly Unit EditorOverviewRulerAddedForeground = new("editorOverviewRuler.addedForeground");

        /// <summary>
        /// editorOverviewRuler.background
        /// </summary>
        public static readonly Unit EditorOverviewRulerBackground = new("editorOverviewRuler.background");

        /// <summary>
        /// editorOverviewRuler.border
        /// </summary>
        public static readonly Unit EditorOverviewRulerBorder = new("editorOverviewRuler.border");

        /// <summary>
        /// editorOverviewRuler.bracketMatchForeground
        /// </summary>
        public static readonly Unit EditorOverviewRulerBracketMatchForeground
            = new("editorOverviewRuler.bracketMatchForeground");

        /// <summary>
        /// editorOverviewRuler.commonContentForeground
        /// </summary>
        public static readonly Unit EditorOverviewRulerCommonContentForeground
            = new("editorOverviewRuler.commonContentForeground");

        /// <summary>
        /// editorOverviewRuler.currentContentForeground
        /// </summary>
        public static readonly Unit EditorOverviewRulerCurrentContentForeground
            = new("editorOverviewRuler.currentContentForeground");

        /// <summary>
        /// editorOverviewRuler.deletedForeground
        /// </summary>
        public static readonly Unit EditorOverviewRulerDeletedForeground = new("editorOverviewRuler.deletedForeground");

        /// <summary>
        /// editorOverviewRuler.errorForeground
        /// </summary>
        public static readonly Unit EditorOverviewRulerErrorForeground = new("editorOverviewRuler.errorForeground");

        /// <summary>
        /// editorOverviewRuler.findMatchForeground
        /// </summary>
        public static readonly Unit EditorOverviewRulerFindMatchForeground
            = new("editorOverviewRuler.findMatchForeground");

        /// <summary>
        /// editorOverviewRuler.incomingContentForeground
        /// </summary>
        public static readonly Unit EditorOverviewRulerIncomingContentForeground
            = new("editorOverviewRuler.incomingContentForeground");

        /// <summary>
        /// editorOverviewRuler.infoForeground
        /// </summary>
        public static readonly Unit EditorOverviewRulerInfoForeground = new("editorOverviewRuler.infoForeground");

        /// <summary>
        /// editorOverviewRuler.modifiedForeground
        /// </summary>
        public static readonly Unit EditorOverviewRulerModifiedForeground
            = new("editorOverviewRuler.modifiedForeground");

        /// <summary>
        /// editorOverviewRuler.rangeHighlightForeground
        /// </summary>
        public static readonly Unit EditorOverviewRulerRangeHighlightForeground
            = new("editorOverviewRuler.rangeHighlightForeground");

        /// <summary>
        /// editorOverviewRuler.selectionHighlightForeground
        /// </summary>
        public static readonly Unit EditorOverviewRulerSelectionHighlightForeground
            = new("editorOverviewRuler.selectionHighlightForeground");

        /// <summary>
        /// editorOverviewRuler.warningForeground
        /// </summary>
        public static readonly Unit EditorOverviewRulerWarningForeground = new("editorOverviewRuler.warningForeground");

        /// <summary>
        /// editorOverviewRuler.wordHighlightForeground
        /// </summary>
        public static readonly Unit EditorOverviewRulerWordHighlightForeground
            = new("editorOverviewRuler.wordHighlightForeground");

        /// <summary>
        /// editorOverviewRuler.wordHighlightStrongForeground
        /// </summary>
        public static readonly Unit EditorOverviewRulerWordHighlightStrongForeground
            = new("editorOverviewRuler.wordHighlightStrongForeground");

        /// <summary>
        /// editorOverviewRuler.wordHighlightTextForeground
        /// </summary>
        public static readonly Unit EditorOverviewRulerWordHighlightTextForeground
            = new("editorOverviewRuler.wordHighlightTextForeground");

        /// <summary>
        /// editorPane.background
        /// </summary>
        public static readonly Unit EditorPaneBackground = new("editorPane.background");

        /// <summary>
        /// editorRuler.foreground
        /// </summary>
        public static readonly Unit EditorRulerForeground = new("editorRuler.foreground");

        /// <summary>
        /// editorStickyScroll.background
        /// </summary>
        public static readonly Unit EditorStickyScrollBackground = new("editorStickyScroll.background");

        /// <summary>
        /// editorStickyScrollHover.background
        /// </summary>
        public static readonly Unit EditorStickyScrollHoverBackground = new("editorStickyScrollHover.background");

        /// <summary>
        /// editorSuggestWidget.background
        /// </summary>
        public static readonly Unit EditorSuggestWidgetBackground = new("editorSuggestWidget.background");

        /// <summary>
        /// editorSuggestWidget.border
        /// </summary>
        public static readonly Unit EditorSuggestWidgetBorder = new("editorSuggestWidget.border");

        /// <summary>
        /// editorSuggestWidget.focusHighlightForeground
        /// </summary>
        public static readonly Unit EditorSuggestWidgetFocusHighlightForeground
            = new("editorSuggestWidget.focusHighlightForeground");

        /// <summary>
        /// editorSuggestWidget.foreground
        /// </summary>
        public static readonly Unit EditorSuggestWidgetForeground = new("editorSuggestWidget.foreground");

        /// <summary>
        /// editorSuggestWidget.highlightForeground
        /// </summary>
        public static readonly Unit EditorSuggestWidgetHighlightForeground
            = new("editorSuggestWidget.highlightForeground");

        /// <summary>
        /// editorSuggestWidget.selectedBackground
        /// </summary>
        public static readonly Unit EditorSuggestWidgetSelectedBackground
            = new("editorSuggestWidget.selectedBackground");

        /// <summary>
        /// editorSuggestWidget.selectedForeground
        /// </summary>
        public static readonly Unit EditorSuggestWidgetSelectedForeground
            = new("editorSuggestWidget.selectedForeground");

        /// <summary>
        /// editorSuggestWidget.selectedIconForeground
        /// </summary>
        public static readonly Unit EditorSuggestWidgetSelectedIconForeground
            = new("editorSuggestWidget.selectedIconForeground");

        /// <summary>
        /// editorSuggestWidgetStatus.foreground
        /// </summary>
        public static readonly Unit EditorSuggestWidgetStatusForeground = new("editorSuggestWidgetStatus.foreground");

        /// <summary>
        /// editorUnicodeHighlight.background
        /// </summary>
        public static readonly Unit EditorUnicodeHighlightBackground = new("editorUnicodeHighlight.background");

        /// <summary>
        /// editorUnicodeHighlight.border
        /// </summary>
        public static readonly Unit EditorUnicodeHighlightBorder = new("editorUnicodeHighlight.border");

        /// <summary>
        /// editorUnnecessaryCode.border
        /// </summary>
        public static readonly Unit EditorUnnecessaryCodeBorder = new("editorUnnecessaryCode.border");

        /// <summary>
        /// editorUnnecessaryCode.opacity
        /// </summary>
        public static readonly Unit EditorUnnecessaryCodeOpacity = new("editorUnnecessaryCode.opacity");

        /// <summary>
        /// editorWarning.background
        /// </summary>
        public static readonly Unit EditorWarningBackground = new("editorWarning.background");

        /// <summary>
        /// editorWarning.border
        /// </summary>
        public static readonly Unit EditorWarningBorder = new("editorWarning.border");

        /// <summary>
        /// editorWarning.foreground
        /// </summary>
        public static readonly Unit EditorWarningForeground = new("editorWarning.foreground");

        /// <summary>
        /// editorWhitespace.foreground
        /// </summary>
        public static readonly Unit EditorWhitespaceForeground = new("editorWhitespace.foreground");

        /// <summary>
        /// editorWidget.background
        /// </summary>
        public static readonly Unit EditorWidgetBackground = new("editorWidget.background");

        /// <summary>
        /// editorWidget.border
        /// </summary>
        public static readonly Unit EditorWidgetBorder = new("editorWidget.border");

        /// <summary>
        /// editorWidget.foreground
        /// </summary>
        public static readonly Unit EditorWidgetForeground = new("editorWidget.foreground");

        /// <summary>
        /// editorWidget.resizeBorder
        /// </summary>
        public static readonly Unit EditorWidgetResizeBorder = new("editorWidget.resizeBorder");

        /// <summary>
        /// errorForeground
        /// </summary>
        public static readonly Unit ErrorForeground = new("errorForeground");

        /// <summary>
        /// extensionBadge.remoteBackground
        /// </summary>
        public static readonly Unit ExtensionBadgeRemoteBackground = new("extensionBadge.remoteBackground");

        /// <summary>
        /// extensionBadge.remoteForeground
        /// </summary>
        public static readonly Unit ExtensionBadgeRemoteForeground = new("extensionBadge.remoteForeground");

        /// <summary>
        /// extensionButton.background
        /// </summary>
        public static readonly Unit ExtensionButtonBackground = new("extensionButton.background");

        /// <summary>
        /// extensionButton.foreground
        /// </summary>
        public static readonly Unit ExtensionButtonForeground = new("extensionButton.foreground");

        /// <summary>
        /// extensionButton.hoverBackground
        /// </summary>
        public static readonly Unit ExtensionButtonHoverBackground = new("extensionButton.hoverBackground");

        /// <summary>
        /// extensionButton.prominentBackground
        /// </summary>
        public static readonly Unit ExtensionButtonProminentBackground = new("extensionButton.prominentBackground");

        /// <summary>
        /// extensionButton.prominentForeground
        /// </summary>
        public static readonly Unit ExtensionButtonProminentForeground = new("extensionButton.prominentForeground");

        /// <summary>
        /// extensionButton.prominentHoverBackground
        /// </summary>
        public static readonly Unit ExtensionButtonProminentHoverBackground
            = new("extensionButton.prominentHoverBackground");

        /// <summary>
        /// extensionButton.separator
        /// </summary>
        public static readonly Unit ExtensionButtonSeparator = new("extensionButton.separator");

        /// <summary>
        /// extensionIcon.preReleaseForeground
        /// </summary>
        public static readonly Unit ExtensionIconPreReleaseForeground = new("extensionIcon.preReleaseForeground");

        /// <summary>
        /// extensionIcon.sponsorForeground
        /// </summary>
        public static readonly Unit ExtensionIconSponsorForeground = new("extensionIcon.sponsorForeground");

        /// <summary>
        /// extensionIcon.starForeground
        /// </summary>
        public static readonly Unit ExtensionIconStarForeground = new("extensionIcon.starForeground");

        /// <summary>
        /// extensionIcon.verifiedForeground
        /// </summary>
        public static readonly Unit ExtensionIconVerifiedForeground = new("extensionIcon.verifiedForeground");

        /// <summary>
        /// focusBorder
        /// </summary>
        public static readonly Unit FocusBorder = new("focusBorder");

        /// <summary>
        /// foreground
        /// </summary>
        public static readonly Unit Foreground = new("foreground");

        /// <summary>
        /// gitDecoration.addedResourceForeground
        /// </summary>
        public static readonly Unit GitDecorationAddedResourceForeground = new("gitDecoration.addedResourceForeground");

        /// <summary>
        /// gitDecoration.conflictingResourceForeground
        /// </summary>
        public static readonly Unit GitDecorationConflictingResourceForeground
            = new("gitDecoration.conflictingResourceForeground");

        /// <summary>
        /// gitDecoration.deletedResourceForeground
        /// </summary>
        public static readonly Unit GitDecorationDeletedResourceForeground
            = new("gitDecoration.deletedResourceForeground");

        /// <summary>
        /// gitDecoration.ignoredResourceForeground
        /// </summary>
        public static readonly Unit GitDecorationIgnoredResourceForeground
            = new("gitDecoration.ignoredResourceForeground");

        /// <summary>
        /// gitDecoration.modifiedResourceForeground
        /// </summary>
        public static readonly Unit GitDecorationModifiedResourceForeground
            = new("gitDecoration.modifiedResourceForeground");

        /// <summary>
        /// gitDecoration.renamedResourceForeground
        /// </summary>
        public static readonly Unit GitDecorationRenamedResourceForeground
            = new("gitDecoration.renamedResourceForeground");

        /// <summary>
        /// gitDecoration.stageDeletedResourceForeground
        /// </summary>
        public static readonly Unit GitDecorationStageDeletedResourceForeground
            = new("gitDecoration.stageDeletedResourceForeground");

        /// <summary>
        /// gitDecoration.stageModifiedResourceForeground
        /// </summary>
        public static readonly Unit GitDecorationStageModifiedResourceForeground
            = new("gitDecoration.stageModifiedResourceForeground");

        /// <summary>
        /// gitDecoration.submoduleResourceForeground
        /// </summary>
        public static readonly Unit GitDecorationSubmoduleResourceForeground
            = new("gitDecoration.submoduleResourceForeground");

        /// <summary>
        /// gitDecoration.untrackedResourceForeground
        /// </summary>
        public static readonly Unit GitDecorationUntrackedResourceForeground
            = new("gitDecoration.untrackedResourceForeground");

        /// <summary>
        /// icon.foreground
        /// </summary>
        public static readonly Unit IconForeground = new("icon.foreground");

        /// <summary>
        /// input.background
        /// </summary>
        public static readonly Unit InputBackground = new("input.background");

        /// <summary>
        /// input.border
        /// </summary>
        public static readonly Unit InputBorder = new("input.border");

        /// <summary>
        /// input.foreground
        /// </summary>
        public static readonly Unit InputForeground = new("input.foreground");

        /// <summary>
        /// input.placeholderForeground
        /// </summary>
        public static readonly Unit InputPlaceholderForeground = new("input.placeholderForeground");

        /// <summary>
        /// inputOption.activeBackground
        /// </summary>
        public static readonly Unit InputOptionActiveBackground = new("inputOption.activeBackground");

        /// <summary>
        /// inputOption.activeBorder
        /// </summary>
        public static readonly Unit InputOptionActiveBorder = new("inputOption.activeBorder");

        /// <summary>
        /// inputOption.activeForeground
        /// </summary>
        public static readonly Unit InputOptionActiveForeground = new("inputOption.activeForeground");

        /// <summary>
        /// inputOption.hoverBackground
        /// </summary>
        public static readonly Unit InputOptionHoverBackground = new("inputOption.hoverBackground");

        /// <summary>
        /// inputValidation.errorBackground
        /// </summary>
        public static readonly Unit InputValidationErrorBackground = new("inputValidation.errorBackground");

        /// <summary>
        /// inputValidation.errorBorder
        /// </summary>
        public static readonly Unit InputValidationErrorBorder = new("inputValidation.errorBorder");

        /// <summary>
        /// inputValidation.errorForeground
        /// </summary>
        public static readonly Unit InputValidationErrorForeground = new("inputValidation.errorForeground");

        /// <summary>
        /// inputValidation.infoBackground
        /// </summary>
        public static readonly Unit InputValidationInfoBackground = new("inputValidation.infoBackground");

        /// <summary>
        /// inputValidation.infoBorder
        /// </summary>
        public static readonly Unit InputValidationInfoBorder = new("inputValidation.infoBorder");

        /// <summary>
        /// inputValidation.infoForeground
        /// </summary>
        public static readonly Unit InputValidationInfoForeground = new("inputValidation.infoForeground");

        /// <summary>
        /// inputValidation.warningBackground
        /// </summary>
        public static readonly Unit InputValidationWarningBackground = new("inputValidation.warningBackground");

        /// <summary>
        /// inputValidation.warningBorder
        /// </summary>
        public static readonly Unit InputValidationWarningBorder = new("inputValidation.warningBorder");

        /// <summary>
        /// inputValidation.warningForeground
        /// </summary>
        public static readonly Unit InputValidationWarningForeground = new("inputValidation.warningForeground");

        /// <summary>
        /// keybindingLabel.background
        /// </summary>
        public static readonly Unit KeybindingLabelBackground = new("keybindingLabel.background");

        /// <summary>
        /// keybindingLabel.border
        /// </summary>
        public static readonly Unit KeybindingLabelBorder = new("keybindingLabel.border");

        /// <summary>
        /// keybindingLabel.bottomBorder
        /// </summary>
        public static readonly Unit KeybindingLabelBottomBorder = new("keybindingLabel.bottomBorder");

        /// <summary>
        /// keybindingLabel.foreground
        /// </summary>
        public static readonly Unit KeybindingLabelForeground = new("keybindingLabel.foreground");

        /// <summary>
        /// keybindingTable.headerBackground
        /// </summary>
        public static readonly Unit KeybindingTableHeaderBackground = new("keybindingTable.headerBackground");

        /// <summary>
        /// keybindingTable.rowsBackground
        /// </summary>
        public static readonly Unit KeybindingTableRowsBackground = new("keybindingTable.rowsBackground");

        /// <summary>
        /// list.activeSelectionBackground
        /// </summary>
        public static readonly Unit ListActiveSelectionBackground = new("list.activeSelectionBackground");

        /// <summary>
        /// list.activeSelectionForeground
        /// </summary>
        public static readonly Unit ListActiveSelectionForeground = new("list.activeSelectionForeground");

        /// <summary>
        /// list.activeSelectionIconForeground
        /// </summary>
        public static readonly Unit ListActiveSelectionIconForeground = new("list.activeSelectionIconForeground");

        /// <summary>
        /// list.deemphasizedForeground
        /// </summary>
        public static readonly Unit ListDeemphasizedForeground = new("list.deemphasizedForeground");

        /// <summary>
        /// list.dropBackground
        /// </summary>
        public static readonly Unit ListDropBackground = new("list.dropBackground");

        /// <summary>
        /// list.errorForeground
        /// </summary>
        public static readonly Unit ListErrorForeground = new("list.errorForeground");

        /// <summary>
        /// list.filterMatchBackground
        /// </summary>
        public static readonly Unit ListFilterMatchBackground = new("list.filterMatchBackground");

        /// <summary>
        /// list.filterMatchBorder
        /// </summary>
        public static readonly Unit ListFilterMatchBorder = new("list.filterMatchBorder");

        /// <summary>
        /// list.focusAndSelectionOutline
        /// </summary>
        public static readonly Unit ListFocusAndSelectionOutline = new("list.focusAndSelectionOutline");

        /// <summary>
        /// list.focusBackground
        /// </summary>
        public static readonly Unit ListFocusBackground = new("list.focusBackground");

        /// <summary>
        /// list.focusForeground
        /// </summary>
        public static readonly Unit ListFocusForeground = new("list.focusForeground");

        /// <summary>
        /// list.focusHighlightForeground
        /// </summary>
        public static readonly Unit ListFocusHighlightForeground = new("list.focusHighlightForeground");

        /// <summary>
        /// list.focusOutline
        /// </summary>
        public static readonly Unit ListFocusOutline = new("list.focusOutline");

        /// <summary>
        /// list.highlightForeground
        /// </summary>
        public static readonly Unit ListHighlightForeground = new("list.highlightForeground");

        /// <summary>
        /// list.hoverBackground
        /// </summary>
        public static readonly Unit ListHoverBackground = new("list.hoverBackground");

        /// <summary>
        /// list.hoverForeground
        /// </summary>
        public static readonly Unit ListHoverForeground = new("list.hoverForeground");

        /// <summary>
        /// list.inactiveFocusBackground
        /// </summary>
        public static readonly Unit ListInactiveFocusBackground = new("list.inactiveFocusBackground");

        /// <summary>
        /// list.inactiveFocusOutline
        /// </summary>
        public static readonly Unit ListInactiveFocusOutline = new("list.inactiveFocusOutline");

        /// <summary>
        /// list.inactiveSelectionBackground
        /// </summary>
        public static readonly Unit ListInactiveSelectionBackground = new("list.inactiveSelectionBackground");

        /// <summary>
        /// list.inactiveSelectionForeground
        /// </summary>
        public static readonly Unit ListInactiveSelectionForeground = new("list.inactiveSelectionForeground");

        /// <summary>
        /// list.inactiveSelectionIconForeground
        /// </summary>
        public static readonly Unit ListInactiveSelectionIconForeground = new("list.inactiveSelectionIconForeground");

        /// <summary>
        /// list.invalidItemForeground
        /// </summary>
        public static readonly Unit ListInvalidItemForeground = new("list.invalidItemForeground");

        /// <summary>
        /// list.warningForeground
        /// </summary>
        public static readonly Unit ListWarningForeground = new("list.warningForeground");

        /// <summary>
        /// listFilterWidget.background
        /// </summary>
        public static readonly Unit ListFilterWidgetBackground = new("listFilterWidget.background");

        /// <summary>
        /// listFilterWidget.noMatchesOutline
        /// </summary>
        public static readonly Unit ListFilterWidgetNoMatchesOutline = new("listFilterWidget.noMatchesOutline");

        /// <summary>
        /// listFilterWidget.outline
        /// </summary>
        public static readonly Unit ListFilterWidgetOutline = new("listFilterWidget.outline");

        /// <summary>
        /// listFilterWidget.shadow
        /// </summary>
        public static readonly Unit ListFilterWidgetShadow = new("listFilterWidget.shadow");

        /// <summary>
        /// menu.background
        /// </summary>
        public static readonly Unit MenuBackground = new("menu.background");

        /// <summary>
        /// menu.border
        /// </summary>
        public static readonly Unit MenuBorder = new("menu.border");

        /// <summary>
        /// menu.foreground
        /// </summary>
        public static readonly Unit MenuForeground = new("menu.foreground");

        /// <summary>
        /// menu.selectionBackground
        /// </summary>
        public static readonly Unit MenuSelectionBackground = new("menu.selectionBackground");

        /// <summary>
        /// menu.selectionBorder
        /// </summary>
        public static readonly Unit MenuSelectionBorder = new("menu.selectionBorder");

        /// <summary>
        /// menu.selectionForeground
        /// </summary>
        public static readonly Unit MenuSelectionForeground = new("menu.selectionForeground");

        /// <summary>
        /// menu.separatorBackground
        /// </summary>
        public static readonly Unit MenuSeparatorBackground = new("menu.separatorBackground");

        /// <summary>
        /// menubar.selectionBackground
        /// </summary>
        public static readonly Unit MenubarSelectionBackground = new("menubar.selectionBackground");

        /// <summary>
        /// menubar.selectionBorder
        /// </summary>
        public static readonly Unit MenubarSelectionBorder = new("menubar.selectionBorder");

        /// <summary>
        /// menubar.selectionForeground
        /// </summary>
        public static readonly Unit MenubarSelectionForeground = new("menubar.selectionForeground");

        /// <summary>
        /// merge.border
        /// </summary>
        public static readonly Unit MergeBorder = new("merge.border");

        /// <summary>
        /// merge.commonContentBackground
        /// </summary>
        public static readonly Unit MergeCommonContentBackground = new("merge.commonContentBackground");

        /// <summary>
        /// merge.commonHeaderBackground
        /// </summary>
        public static readonly Unit MergeCommonHeaderBackground = new("merge.commonHeaderBackground");

        /// <summary>
        /// merge.currentContentBackground
        /// </summary>
        public static readonly Unit MergeCurrentContentBackground = new("merge.currentContentBackground");

        /// <summary>
        /// merge.currentHeaderBackground
        /// </summary>
        public static readonly Unit MergeCurrentHeaderBackground = new("merge.currentHeaderBackground");

        /// <summary>
        /// merge.incomingContentBackground
        /// </summary>
        public static readonly Unit MergeIncomingContentBackground = new("merge.incomingContentBackground");

        /// <summary>
        /// merge.incomingHeaderBackground
        /// </summary>
        public static readonly Unit MergeIncomingHeaderBackground = new("merge.incomingHeaderBackground");

        /// <summary>
        /// mergeEditor.change.background
        /// </summary>
        public static readonly Unit MergeEditorChangeBackground = new("mergeEditor.change.background");

        /// <summary>
        /// mergeEditor.change.word.background
        /// </summary>
        public static readonly Unit MergeEditorChangeWordBackground = new("mergeEditor.change.word.background");

        /// <summary>
        /// mergeEditor.changeBase.background
        /// </summary>
        public static readonly Unit MergeEditorChangeBaseBackground = new("mergeEditor.changeBase.background");

        /// <summary>
        /// mergeEditor.changeBase.word.background
        /// </summary>
        public static readonly Unit MergeEditorChangeBaseWordBackground = new("mergeEditor.changeBase.word.background");

        /// <summary>
        /// mergeEditor.conflict.handled.minimapOverViewRuler
        /// </summary>
        public static readonly Unit MergeEditorConflictHandledMinimapOverViewRuler
            = new("mergeEditor.conflict.handled.minimapOverViewRuler");

        /// <summary>
        /// mergeEditor.conflict.handledFocused.border
        /// </summary>
        public static readonly Unit MergeEditorConflictHandledFocusedBorder
            = new("mergeEditor.conflict.handledFocused.border");

        /// <summary>
        /// mergeEditor.conflict.handledUnfocused.border
        /// </summary>
        public static readonly Unit MergeEditorConflictHandledUnfocusedBorder
            = new("mergeEditor.conflict.handledUnfocused.border");

        /// <summary>
        /// mergeEditor.conflict.input1.background
        /// </summary>
        public static readonly Unit MergeEditorConflictInput1Background = new("mergeEditor.conflict.input1.background");

        /// <summary>
        /// mergeEditor.conflict.input2.background
        /// </summary>
        public static readonly Unit MergeEditorConflictInput2Background = new("mergeEditor.conflict.input2.background");

        /// <summary>
        /// mergeEditor.conflict.unhandled.minimapOverViewRuler
        /// </summary>
        public static readonly Unit MergeEditorConflictUnhandledMinimapOverViewRuler
            = new("mergeEditor.conflict.unhandled.minimapOverViewRuler");

        /// <summary>
        /// mergeEditor.conflict.unhandledFocused.border
        /// </summary>
        public static readonly Unit MergeEditorConflictUnhandledFocusedBorder
            = new("mergeEditor.conflict.unhandledFocused.border");

        /// <summary>
        /// mergeEditor.conflict.unhandledUnfocused.border
        /// </summary>
        public static readonly Unit MergeEditorConflictUnhandledUnfocusedBorder
            = new("mergeEditor.conflict.unhandledUnfocused.border");

        /// <summary>
        /// mergeEditor.conflictingLines.background
        /// </summary>
        public static readonly Unit MergeEditorConflictingLinesBackground
            = new("mergeEditor.conflictingLines.background");

        /// <summary>
        /// minimap.background
        /// </summary>
        public static readonly Unit MinimapBackground = new("minimap.background");

        /// <summary>
        /// minimap.errorHighlight
        /// </summary>
        public static readonly Unit MinimapErrorHighlight = new("minimap.errorHighlight");

        /// <summary>
        /// minimap.findMatchHighlight
        /// </summary>
        public static readonly Unit MinimapFindMatchHighlight = new("minimap.findMatchHighlight");

        /// <summary>
        /// minimap.foregroundOpacity
        /// </summary>
        public static readonly Unit MinimapForegroundOpacity = new("minimap.foregroundOpacity");

        /// <summary>
        /// minimap.selectionHighlight
        /// </summary>
        public static readonly Unit MinimapSelectionHighlight = new("minimap.selectionHighlight");

        /// <summary>
        /// minimap.selectionOccurrenceHighlight
        /// </summary>
        public static readonly Unit MinimapSelectionOccurrenceHighlight = new("minimap.selectionOccurrenceHighlight");

        /// <summary>
        /// minimap.warningHighlight
        /// </summary>
        public static readonly Unit MinimapWarningHighlight = new("minimap.warningHighlight");

        /// <summary>
        /// minimapGutter.addedBackground
        /// </summary>
        public static readonly Unit MinimapGutterAddedBackground = new("minimapGutter.addedBackground");

        /// <summary>
        /// minimapGutter.deletedBackground
        /// </summary>
        public static readonly Unit MinimapGutterDeletedBackground = new("minimapGutter.deletedBackground");

        /// <summary>
        /// minimapGutter.modifiedBackground
        /// </summary>
        public static readonly Unit MinimapGutterModifiedBackground = new("minimapGutter.modifiedBackground");

        /// <summary>
        /// minimapSlider.activeBackground
        /// </summary>
        public static readonly Unit MinimapSliderActiveBackground = new("minimapSlider.activeBackground");

        /// <summary>
        /// minimapSlider.background
        /// </summary>
        public static readonly Unit MinimapSliderBackground = new("minimapSlider.background");

        /// <summary>
        /// minimapSlider.hoverBackground
        /// </summary>
        public static readonly Unit MinimapSliderHoverBackground = new("minimapSlider.hoverBackground");

        /// <summary>
        /// notebook.cellBorderColor
        /// </summary>
        public static readonly Unit NotebookCellBorderColor = new("notebook.cellBorderColor");

        /// <summary>
        /// notebook.cellEditorBackground
        /// </summary>
        public static readonly Unit NotebookCellEditorBackground = new("notebook.cellEditorBackground");

        /// <summary>
        /// notebook.cellHoverBackground
        /// </summary>
        public static readonly Unit NotebookCellHoverBackground = new("notebook.cellHoverBackground");

        /// <summary>
        /// notebook.cellInsertionIndicator
        /// </summary>
        public static readonly Unit NotebookCellInsertionIndicator = new("notebook.cellInsertionIndicator");

        /// <summary>
        /// notebook.cellStatusBarItemHoverBackground
        /// </summary>
        public static readonly Unit NotebookCellStatusBarItemHoverBackground
            = new("notebook.cellStatusBarItemHoverBackground");

        /// <summary>
        /// notebook.cellToolbarSeparator
        /// </summary>
        public static readonly Unit NotebookCellToolbarSeparator = new("notebook.cellToolbarSeparator");

        /// <summary>
        /// notebook.editorBackground
        /// </summary>
        public static readonly Unit NotebookEditorBackground = new("notebook.editorBackground");

        /// <summary>
        /// notebook.focusedCellBackground
        /// </summary>
        public static readonly Unit NotebookFocusedCellBackground = new("notebook.focusedCellBackground");

        /// <summary>
        /// notebook.focusedCellBorder
        /// </summary>
        public static readonly Unit NotebookFocusedCellBorder = new("notebook.focusedCellBorder");

        /// <summary>
        /// notebook.focusedEditorBorder
        /// </summary>
        public static readonly Unit NotebookFocusedEditorBorder = new("notebook.focusedEditorBorder");

        /// <summary>
        /// notebook.inactiveFocusedCellBorder
        /// </summary>
        public static readonly Unit NotebookInactiveFocusedCellBorder = new("notebook.inactiveFocusedCellBorder");

        /// <summary>
        /// notebook.inactiveSelectedCellBorder
        /// </summary>
        public static readonly Unit NotebookInactiveSelectedCellBorder = new("notebook.inactiveSelectedCellBorder");

        /// <summary>
        /// notebook.outputContainerBackgroundColor
        /// </summary>
        public static readonly Unit NotebookOutputContainerBackgroundColor
            = new("notebook.outputContainerBackgroundColor");

        /// <summary>
        /// notebook.outputContainerBorderColor
        /// </summary>
        public static readonly Unit NotebookOutputContainerBorderColor = new("notebook.outputContainerBorderColor");

        /// <summary>
        /// notebook.selectedCellBackground
        /// </summary>
        public static readonly Unit NotebookSelectedCellBackground = new("notebook.selectedCellBackground");

        /// <summary>
        /// notebook.selectedCellBorder
        /// </summary>
        public static readonly Unit NotebookSelectedCellBorder = new("notebook.selectedCellBorder");

        /// <summary>
        /// notebook.symbolHighlightBackground
        /// </summary>
        public static readonly Unit NotebookSymbolHighlightBackground = new("notebook.symbolHighlightBackground");

        /// <summary>
        /// notebookEditorOverviewRuler.runningCellForeground
        /// </summary>
        public static readonly Unit NotebookEditorOverviewRulerRunningCellForeground
            = new("notebookEditorOverviewRuler.runningCellForeground");

        /// <summary>
        /// notebookScrollbarSlider.activeBackground
        /// </summary>
        public static readonly Unit NotebookScrollbarSliderActiveBackground
            = new("notebookScrollbarSlider.activeBackground");

        /// <summary>
        /// notebookScrollbarSlider.background
        /// </summary>
        public static readonly Unit NotebookScrollbarSliderBackground = new("notebookScrollbarSlider.background");

        /// <summary>
        /// notebookScrollbarSlider.hoverBackground
        /// </summary>
        public static readonly Unit NotebookScrollbarSliderHoverBackground
            = new("notebookScrollbarSlider.hoverBackground");

        /// <summary>
        /// notebookStatusErrorIcon.foreground
        /// </summary>
        public static readonly Unit NotebookStatusErrorIconForeground = new("notebookStatusErrorIcon.foreground");

        /// <summary>
        /// notebookStatusRunningIcon.foreground
        /// </summary>
        public static readonly Unit NotebookStatusRunningIconForeground = new("notebookStatusRunningIcon.foreground");

        /// <summary>
        /// notebookStatusSuccessIcon.foreground
        /// </summary>
        public static readonly Unit NotebookStatusSuccessIconForeground = new("notebookStatusSuccessIcon.foreground");

        /// <summary>
        /// notificationCenter.border
        /// </summary>
        public static readonly Unit NotificationCenterBorder = new("notificationCenter.border");

        /// <summary>
        /// notificationCenterHeader.background
        /// </summary>
        public static readonly Unit NotificationCenterHeaderBackground = new("notificationCenterHeader.background");

        /// <summary>
        /// notificationCenterHeader.foreground
        /// </summary>
        public static readonly Unit NotificationCenterHeaderForeground = new("notificationCenterHeader.foreground");

        /// <summary>
        /// notificationLink.foreground
        /// </summary>
        public static readonly Unit NotificationLinkForeground = new("notificationLink.foreground");

        /// <summary>
        /// notificationToast.border
        /// </summary>
        public static readonly Unit NotificationToastBorder = new("notificationToast.border");

        /// <summary>
        /// notifications.background
        /// </summary>
        public static readonly Unit NotificationsBackground = new("notifications.background");

        /// <summary>
        /// notifications.border
        /// </summary>
        public static readonly Unit NotificationsBorder = new("notifications.border");

        /// <summary>
        /// notifications.foreground
        /// </summary>
        public static readonly Unit NotificationsForeground = new("notifications.foreground");

        /// <summary>
        /// notificationsErrorIcon.foreground
        /// </summary>
        public static readonly Unit NotificationsErrorIconForeground = new("notificationsErrorIcon.foreground");

        /// <summary>
        /// notificationsInfoIcon.foreground
        /// </summary>
        public static readonly Unit NotificationsInfoIconForeground = new("notificationsInfoIcon.foreground");

        /// <summary>
        /// notificationsWarningIcon.foreground
        /// </summary>
        public static readonly Unit NotificationsWarningIconForeground = new("notificationsWarningIcon.foreground");

        /// <summary>
        /// panel.background
        /// </summary>
        public static readonly Unit PanelBackground = new("panel.background");

        /// <summary>
        /// panel.border
        /// </summary>
        public static readonly Unit PanelBorder = new("panel.border");

        /// <summary>
        /// panel.dropBorder
        /// </summary>
        public static readonly Unit PanelDropBorder = new("panel.dropBorder");

        /// <summary>
        /// panelInput.border
        /// </summary>
        public static readonly Unit PanelInputBorder = new("panelInput.border");

        /// <summary>
        /// panelSection.border
        /// </summary>
        public static readonly Unit PanelSectionBorder = new("panelSection.border");

        /// <summary>
        /// panelSection.dropBackground
        /// </summary>
        public static readonly Unit PanelSectionDropBackground = new("panelSection.dropBackground");

        /// <summary>
        /// panelSectionHeader.background
        /// </summary>
        public static readonly Unit PanelSectionHeaderBackground = new("panelSectionHeader.background");

        /// <summary>
        /// panelSectionHeader.border
        /// </summary>
        public static readonly Unit PanelSectionHeaderBorder = new("panelSectionHeader.border");

        /// <summary>
        /// panelSectionHeader.foreground
        /// </summary>
        public static readonly Unit PanelSectionHeaderForeground = new("panelSectionHeader.foreground");

        /// <summary>
        /// panelTitle.activeBorder
        /// </summary>
        public static readonly Unit PanelTitleActiveBorder = new("panelTitle.activeBorder");

        /// <summary>
        /// panelTitle.activeForeground
        /// </summary>
        public static readonly Unit PanelTitleActiveForeground = new("panelTitle.activeForeground");

        /// <summary>
        /// panelTitle.inactiveForeground
        /// </summary>
        public static readonly Unit PanelTitleInactiveForeground = new("panelTitle.inactiveForeground");

        /// <summary>
        /// peekView.border
        /// </summary>
        public static readonly Unit PeekViewBorder = new("peekView.border");

        /// <summary>
        /// peekViewEditor.background
        /// </summary>
        public static readonly Unit PeekViewEditorBackground = new("peekViewEditor.background");

        /// <summary>
        /// peekViewEditor.matchHighlightBackground
        /// </summary>
        public static readonly Unit PeekViewEditorMatchHighlightBackground
            = new("peekViewEditor.matchHighlightBackground");

        /// <summary>
        /// peekViewEditor.matchHighlightBorder
        /// </summary>
        public static readonly Unit PeekViewEditorMatchHighlightBorder = new("peekViewEditor.matchHighlightBorder");

        /// <summary>
        /// peekViewEditorGutter.background
        /// </summary>
        public static readonly Unit PeekViewEditorGutterBackground = new("peekViewEditorGutter.background");

        /// <summary>
        /// peekViewResult.background
        /// </summary>
        public static readonly Unit PeekViewResultBackground = new("peekViewResult.background");

        /// <summary>
        /// peekViewResult.fileForeground
        /// </summary>
        public static readonly Unit PeekViewResultFileForeground = new("peekViewResult.fileForeground");

        /// <summary>
        /// peekViewResult.lineForeground
        /// </summary>
        public static readonly Unit PeekViewResultLineForeground = new("peekViewResult.lineForeground");

        /// <summary>
        /// peekViewResult.matchHighlightBackground
        /// </summary>
        public static readonly Unit PeekViewResultMatchHighlightBackground
            = new("peekViewResult.matchHighlightBackground");

        /// <summary>
        /// peekViewResult.selectionBackground
        /// </summary>
        public static readonly Unit PeekViewResultSelectionBackground = new("peekViewResult.selectionBackground");

        /// <summary>
        /// peekViewResult.selectionForeground
        /// </summary>
        public static readonly Unit PeekViewResultSelectionForeground = new("peekViewResult.selectionForeground");

        /// <summary>
        /// peekViewTitle.background
        /// </summary>
        public static readonly Unit PeekViewTitleBackground = new("peekViewTitle.background");

        /// <summary>
        /// peekViewTitleDescription.foreground
        /// </summary>
        public static readonly Unit PeekViewTitleDescriptionForeground = new("peekViewTitleDescription.foreground");

        /// <summary>
        /// peekViewTitleLabel.foreground
        /// </summary>
        public static readonly Unit PeekViewTitleLabelForeground = new("peekViewTitleLabel.foreground");

        /// <summary>
        /// pickerGroup.border
        /// </summary>
        public static readonly Unit PickerGroupBorder = new("pickerGroup.border");

        /// <summary>
        /// pickerGroup.foreground
        /// </summary>
        public static readonly Unit PickerGroupForeground = new("pickerGroup.foreground");

        /// <summary>
        /// ports.iconRunningProcessForeground
        /// </summary>
        public static readonly Unit PortsIconRunningProcessForeground = new("ports.iconRunningProcessForeground");

        /// <summary>
        /// problemsErrorIcon.foreground
        /// </summary>
        public static readonly Unit ProblemsErrorIconForeground = new("problemsErrorIcon.foreground");

        /// <summary>
        /// problemsInfoIcon.foreground
        /// </summary>
        public static readonly Unit ProblemsInfoIconForeground = new("problemsInfoIcon.foreground");

        /// <summary>
        /// problemsWarningIcon.foreground
        /// </summary>
        public static readonly Unit ProblemsWarningIconForeground = new("problemsWarningIcon.foreground");

        /// <summary>
        /// profileBadge.background
        /// </summary>
        public static readonly Unit ProfileBadgeBackground = new("profileBadge.background");

        /// <summary>
        /// profileBadge.foreground
        /// </summary>
        public static readonly Unit ProfileBadgeForeground = new("profileBadge.foreground");

        /// <summary>
        /// progressBar.background
        /// </summary>
        public static readonly Unit ProgressBarBackground = new("progressBar.background");

        /// <summary>
        /// quickInput.background
        /// </summary>
        public static readonly Unit QuickInputBackground = new("quickInput.background");

        /// <summary>
        /// quickInput.foreground
        /// </summary>
        public static readonly Unit QuickInputForeground = new("quickInput.foreground");

        /// <summary>
        /// quickInputList.focusBackground
        /// </summary>
        public static readonly Unit QuickInputListFocusBackground = new("quickInputList.focusBackground");

        /// <summary>
        /// quickInputList.focusForeground
        /// </summary>
        public static readonly Unit QuickInputListFocusForeground = new("quickInputList.focusForeground");

        /// <summary>
        /// quickInputList.focusIconForeground
        /// </summary>
        public static readonly Unit QuickInputListFocusIconForeground = new("quickInputList.focusIconForeground");

        /// <summary>
        /// quickInputTitle.background
        /// </summary>
        public static readonly Unit QuickInputTitleBackground = new("quickInputTitle.background");

        /// <summary>
        /// sash.hoverBorder
        /// </summary>
        public static readonly Unit SashHoverBorder = new("sash.hoverBorder");

        /// <summary>
        /// scm.providerBorder
        /// </summary>
        public static readonly Unit ScmProviderBorder = new("scm.providerBorder");

        /// <summary>
        /// scrollbar.shadow
        /// </summary>
        public static readonly Unit ScrollbarShadow = new("scrollbar.shadow");

        /// <summary>
        /// scrollbarSlider.activeBackground
        /// </summary>
        public static readonly Unit ScrollbarSliderActiveBackground = new("scrollbarSlider.activeBackground");

        /// <summary>
        /// scrollbarSlider.background
        /// </summary>
        public static readonly Unit ScrollbarSliderBackground = new("scrollbarSlider.background");

        /// <summary>
        /// scrollbarSlider.hoverBackground
        /// </summary>
        public static readonly Unit ScrollbarSliderHoverBackground = new("scrollbarSlider.hoverBackground");

        /// <summary>
        /// search.resultsInfoForeground
        /// </summary>
        public static readonly Unit SearchResultsInfoForeground = new("search.resultsInfoForeground");

        /// <summary>
        /// searchEditor.findMatchBackground
        /// </summary>
        public static readonly Unit SearchEditorFindMatchBackground = new("searchEditor.findMatchBackground");

        /// <summary>
        /// searchEditor.findMatchBorder
        /// </summary>
        public static readonly Unit SearchEditorFindMatchBorder = new("searchEditor.findMatchBorder");

        /// <summary>
        /// searchEditor.textInputBorder
        /// </summary>
        public static readonly Unit SearchEditorTextInputBorder = new("searchEditor.textInputBorder");

        /// <summary>
        /// selection.background
        /// </summary>
        public static readonly Unit SelectionBackground = new("selection.background");

        /// <summary>
        /// settings.checkboxBackground
        /// </summary>
        public static readonly Unit SettingsCheckboxBackground = new("settings.checkboxBackground");

        /// <summary>
        /// settings.checkboxBorder
        /// </summary>
        public static readonly Unit SettingsCheckboxBorder = new("settings.checkboxBorder");

        /// <summary>
        /// settings.checkboxForeground
        /// </summary>
        public static readonly Unit SettingsCheckboxForeground = new("settings.checkboxForeground");

        /// <summary>
        /// settings.dropdownBackground
        /// </summary>
        public static readonly Unit SettingsDropdownBackground = new("settings.dropdownBackground");

        /// <summary>
        /// settings.dropdownBorder
        /// </summary>
        public static readonly Unit SettingsDropdownBorder = new("settings.dropdownBorder");

        /// <summary>
        /// settings.dropdownForeground
        /// </summary>
        public static readonly Unit SettingsDropdownForeground = new("settings.dropdownForeground");

        /// <summary>
        /// settings.dropdownListBorder
        /// </summary>
        public static readonly Unit SettingsDropdownListBorder = new("settings.dropdownListBorder");

        /// <summary>
        /// settings.focusedRowBackground
        /// </summary>
        public static readonly Unit SettingsFocusedRowBackground = new("settings.focusedRowBackground");

        /// <summary>
        /// settings.focusedRowBorder
        /// </summary>
        public static readonly Unit SettingsFocusedRowBorder = new("settings.focusedRowBorder");

        /// <summary>
        /// settings.headerBorder
        /// </summary>
        public static readonly Unit SettingsHeaderBorder = new("settings.headerBorder");

        /// <summary>
        /// settings.headerForeground
        /// </summary>
        public static readonly Unit SettingsHeaderForeground = new("settings.headerForeground");

        /// <summary>
        /// settings.modifiedItemIndicator
        /// </summary>
        public static readonly Unit SettingsModifiedItemIndicator = new("settings.modifiedItemIndicator");

        /// <summary>
        /// settings.numberInputBackground
        /// </summary>
        public static readonly Unit SettingsNumberInputBackground = new("settings.numberInputBackground");

        /// <summary>
        /// settings.numberInputBorder
        /// </summary>
        public static readonly Unit SettingsNumberInputBorder = new("settings.numberInputBorder");

        /// <summary>
        /// settings.numberInputForeground
        /// </summary>
        public static readonly Unit SettingsNumberInputForeground = new("settings.numberInputForeground");

        /// <summary>
        /// settings.rowHoverBackground
        /// </summary>
        public static readonly Unit SettingsRowHoverBackground = new("settings.rowHoverBackground");

        /// <summary>
        /// settings.sashBorder
        /// </summary>
        public static readonly Unit SettingsSashBorder = new("settings.sashBorder");

        /// <summary>
        /// settings.settingsHeaderHoverForeground
        /// </summary>
        public static readonly Unit SettingsSettingsHeaderHoverForeground
            = new("settings.settingsHeaderHoverForeground");

        /// <summary>
        /// settings.textInputBackground
        /// </summary>
        public static readonly Unit SettingsTextInputBackground = new("settings.textInputBackground");

        /// <summary>
        /// settings.textInputBorder
        /// </summary>
        public static readonly Unit SettingsTextInputBorder = new("settings.textInputBorder");

        /// <summary>
        /// settings.textInputForeground
        /// </summary>
        public static readonly Unit SettingsTextInputForeground = new("settings.textInputForeground");

        /// <summary>
        /// sideBar.background
        /// </summary>
        public static readonly Unit SideBarBackground = new("sideBar.background");

        /// <summary>
        /// sideBar.border
        /// </summary>
        public static readonly Unit SideBarBorder = new("sideBar.border");

        /// <summary>
        /// sideBar.dropBackground
        /// </summary>
        public static readonly Unit SideBarDropBackground = new("sideBar.dropBackground");

        /// <summary>
        /// sideBar.foreground
        /// </summary>
        public static readonly Unit SideBarForeground = new("sideBar.foreground");

        /// <summary>
        /// sideBarSectionHeader.background
        /// </summary>
        public static readonly Unit SideBarSectionHeaderBackground = new("sideBarSectionHeader.background");

        /// <summary>
        /// sideBarSectionHeader.border
        /// </summary>
        public static readonly Unit SideBarSectionHeaderBorder = new("sideBarSectionHeader.border");

        /// <summary>
        /// sideBarSectionHeader.foreground
        /// </summary>
        public static readonly Unit SideBarSectionHeaderForeground = new("sideBarSectionHeader.foreground");

        /// <summary>
        /// sideBarTitle.foreground
        /// </summary>
        public static readonly Unit SideBarTitleForeground = new("sideBarTitle.foreground");

        /// <summary>
        /// sideBySideEditor.horizontalBorder
        /// </summary>
        public static readonly Unit SideBySideEditorHorizontalBorder = new("sideBySideEditor.horizontalBorder");

        /// <summary>
        /// sideBySideEditor.verticalBorder
        /// </summary>
        public static readonly Unit SideBySideEditorVerticalBorder = new("sideBySideEditor.verticalBorder");

        /// <summary>
        /// statusBar.background
        /// </summary>
        public static readonly Unit StatusBarBackground = new("statusBar.background");

        /// <summary>
        /// statusBar.border
        /// </summary>
        public static readonly Unit StatusBarBorder = new("statusBar.border");

        /// <summary>
        /// statusBar.debuggingBackground
        /// </summary>
        public static readonly Unit StatusBarDebuggingBackground = new("statusBar.debuggingBackground");

        /// <summary>
        /// statusBar.debuggingBorder
        /// </summary>
        public static readonly Unit StatusBarDebuggingBorder = new("statusBar.debuggingBorder");

        /// <summary>
        /// statusBar.debuggingForeground
        /// </summary>
        public static readonly Unit StatusBarDebuggingForeground = new("statusBar.debuggingForeground");

        /// <summary>
        /// statusBar.focusBorder
        /// </summary>
        public static readonly Unit StatusBarFocusBorder = new("statusBar.focusBorder");

        /// <summary>
        /// statusBar.foreground
        /// </summary>
        public static readonly Unit StatusBarForeground = new("statusBar.foreground");

        /// <summary>
        /// statusBar.noFolderBackground
        /// </summary>
        public static readonly Unit StatusBarNoFolderBackground = new("statusBar.noFolderBackground");

        /// <summary>
        /// statusBar.noFolderBorder
        /// </summary>
        public static readonly Unit StatusBarNoFolderBorder = new("statusBar.noFolderBorder");

        /// <summary>
        /// statusBar.noFolderForeground
        /// </summary>
        public static readonly Unit StatusBarNoFolderForeground = new("statusBar.noFolderForeground");

        /// <summary>
        /// statusBarItem.activeBackground
        /// </summary>
        public static readonly Unit StatusBarItemActiveBackground = new("statusBarItem.activeBackground");

        /// <summary>
        /// statusBarItem.compactHoverBackground
        /// </summary>
        public static readonly Unit StatusBarItemCompactHoverBackground = new("statusBarItem.compactHoverBackground");

        /// <summary>
        /// statusBarItem.errorBackground
        /// </summary>
        public static readonly Unit StatusBarItemErrorBackground = new("statusBarItem.errorBackground");

        /// <summary>
        /// statusBarItem.errorForeground
        /// </summary>
        public static readonly Unit StatusBarItemErrorForeground = new("statusBarItem.errorForeground");

        /// <summary>
        /// statusBarItem.focusBorder
        /// </summary>
        public static readonly Unit StatusBarItemFocusBorder = new("statusBarItem.focusBorder");

        /// <summary>
        /// statusBarItem.hoverBackground
        /// </summary>
        public static readonly Unit StatusBarItemHoverBackground = new("statusBarItem.hoverBackground");

        /// <summary>
        /// statusBarItem.prominentBackground
        /// </summary>
        public static readonly Unit StatusBarItemProminentBackground = new("statusBarItem.prominentBackground");

        /// <summary>
        /// statusBarItem.prominentForeground
        /// </summary>
        public static readonly Unit StatusBarItemProminentForeground = new("statusBarItem.prominentForeground");

        /// <summary>
        /// statusBarItem.prominentHoverBackground
        /// </summary>
        public static readonly Unit StatusBarItemProminentHoverBackground
            = new("statusBarItem.prominentHoverBackground");

        /// <summary>
        /// statusBarItem.remoteBackground
        /// </summary>
        public static readonly Unit StatusBarItemRemoteBackground = new("statusBarItem.remoteBackground");

        /// <summary>
        /// statusBarItem.remoteForeground
        /// </summary>
        public static readonly Unit StatusBarItemRemoteForeground = new("statusBarItem.remoteForeground");

        /// <summary>
        /// statusBarItem.warningBackground
        /// </summary>
        public static readonly Unit StatusBarItemWarningBackground = new("statusBarItem.warningBackground");

        /// <summary>
        /// statusBarItem.warningForeground
        /// </summary>
        public static readonly Unit StatusBarItemWarningForeground = new("statusBarItem.warningForeground");

        /// <summary>
        /// symbolIcon.arrayForeground
        /// </summary>
        public static readonly Unit SymbolIconArrayForeground = new("symbolIcon.arrayForeground");

        /// <summary>
        /// symbolIcon.booleanForeground
        /// </summary>
        public static readonly Unit SymbolIconBooleanForeground = new("symbolIcon.booleanForeground");

        /// <summary>
        /// symbolIcon.classForeground
        /// </summary>
        public static readonly Unit SymbolIconClassForeground = new("symbolIcon.classForeground");

        /// <summary>
        /// symbolIcon.colorForeground
        /// </summary>
        public static readonly Unit SymbolIconColorForeground = new("symbolIcon.colorForeground");

        /// <summary>
        /// symbolIcon.constantForeground
        /// </summary>
        public static readonly Unit SymbolIconConstantForeground = new("symbolIcon.constantForeground");

        /// <summary>
        /// symbolIcon.constructorForeground
        /// </summary>
        public static readonly Unit SymbolIconConstructorForeground = new("symbolIcon.constructorForeground");

        /// <summary>
        /// symbolIcon.enumeratorForeground
        /// </summary>
        public static readonly Unit SymbolIconEnumeratorForeground = new("symbolIcon.enumeratorForeground");

        /// <summary>
        /// symbolIcon.enumeratorMemberForeground
        /// </summary>
        public static readonly Unit SymbolIconEnumeratorMemberForeground = new("symbolIcon.enumeratorMemberForeground");

        /// <summary>
        /// symbolIcon.eventForeground
        /// </summary>
        public static readonly Unit SymbolIconEventForeground = new("symbolIcon.eventForeground");

        /// <summary>
        /// symbolIcon.fieldForeground
        /// </summary>
        public static readonly Unit SymbolIconFieldForeground = new("symbolIcon.fieldForeground");

        /// <summary>
        /// symbolIcon.fileForeground
        /// </summary>
        public static readonly Unit SymbolIconFileForeground = new("symbolIcon.fileForeground");

        /// <summary>
        /// symbolIcon.folderForeground
        /// </summary>
        public static readonly Unit SymbolIconFolderForeground = new("symbolIcon.folderForeground");

        /// <summary>
        /// symbolIcon.functionForeground
        /// </summary>
        public static readonly Unit SymbolIconFunctionForeground = new("symbolIcon.functionForeground");

        /// <summary>
        /// symbolIcon.interfaceForeground
        /// </summary>
        public static readonly Unit SymbolIconInterfaceForeground = new("symbolIcon.interfaceForeground");

        /// <summary>
        /// symbolIcon.keyForeground
        /// </summary>
        public static readonly Unit SymbolIconKeyForeground = new("symbolIcon.keyForeground");

        /// <summary>
        /// symbolIcon.keywordForeground
        /// </summary>
        public static readonly Unit SymbolIconKeywordForeground = new("symbolIcon.keywordForeground");

        /// <summary>
        /// symbolIcon.methodForeground
        /// </summary>
        public static readonly Unit SymbolIconMethodForeground = new("symbolIcon.methodForeground");

        /// <summary>
        /// symbolIcon.moduleForeground
        /// </summary>
        public static readonly Unit SymbolIconModuleForeground = new("symbolIcon.moduleForeground");

        /// <summary>
        /// symbolIcon.namespaceForeground
        /// </summary>
        public static readonly Unit SymbolIconNamespaceForeground = new("symbolIcon.namespaceForeground");

        /// <summary>
        /// symbolIcon.nullForeground
        /// </summary>
        public static readonly Unit SymbolIconNullForeground = new("symbolIcon.nullForeground");

        /// <summary>
        /// symbolIcon.numberForeground
        /// </summary>
        public static readonly Unit SymbolIconNumberForeground = new("symbolIcon.numberForeground");

        /// <summary>
        /// symbolIcon.objectForeground
        /// </summary>
        public static readonly Unit SymbolIconObjectForeground = new("symbolIcon.objectForeground");

        /// <summary>
        /// symbolIcon.operatorForeground
        /// </summary>
        public static readonly Unit SymbolIconOperatorForeground = new("symbolIcon.operatorForeground");

        /// <summary>
        /// symbolIcon.packageForeground
        /// </summary>
        public static readonly Unit SymbolIconPackageForeground = new("symbolIcon.packageForeground");

        /// <summary>
        /// symbolIcon.propertyForeground
        /// </summary>
        public static readonly Unit SymbolIconPropertyForeground = new("symbolIcon.propertyForeground");

        /// <summary>
        /// symbolIcon.referenceForeground
        /// </summary>
        public static readonly Unit SymbolIconReferenceForeground = new("symbolIcon.referenceForeground");

        /// <summary>
        /// symbolIcon.snippetForeground
        /// </summary>
        public static readonly Unit SymbolIconSnippetForeground = new("symbolIcon.snippetForeground");

        /// <summary>
        /// symbolIcon.stringForeground
        /// </summary>
        public static readonly Unit SymbolIconStringForeground = new("symbolIcon.stringForeground");

        /// <summary>
        /// symbolIcon.structForeground
        /// </summary>
        public static readonly Unit SymbolIconStructForeground = new("symbolIcon.structForeground");

        /// <summary>
        /// symbolIcon.textForeground
        /// </summary>
        public static readonly Unit SymbolIconTextForeground = new("symbolIcon.textForeground");

        /// <summary>
        /// symbolIcon.typeParameterForeground
        /// </summary>
        public static readonly Unit SymbolIconTypeParameterForeground = new("symbolIcon.typeParameterForeground");

        /// <summary>
        /// symbolIcon.unitForeground
        /// </summary>
        public static readonly Unit SymbolIconUnitForeground = new("symbolIcon.unitForeground");

        /// <summary>
        /// symbolIcon.variableForeground
        /// </summary>
        public static readonly Unit SymbolIconVariableForeground = new("symbolIcon.variableForeground");

        /// <summary>
        /// tab.activeBackground
        /// </summary>
        public static readonly Unit TabActiveBackground = new("tab.activeBackground");

        /// <summary>
        /// tab.activeBorderTop
        /// </summary>
        public static readonly Unit TabActiveBorderTop = new("tab.activeBorderTop");

        /// <summary>
        /// tab.activeBorder
        /// </summary>
        public static readonly Unit TabActiveBorder = new("tab.activeBorder");

        /// <summary>
        /// tab.activeForeground
        /// </summary>
        public static readonly Unit TabActiveForeground = new("tab.activeForeground");

        /// <summary>
        /// tab.activeModifiedBorder
        /// </summary>
        public static readonly Unit TabActiveModifiedBorder = new("tab.activeModifiedBorder");

        /// <summary>
        /// tab.border
        /// </summary>
        public static readonly Unit TabBorder = new("tab.border");

        /// <summary>
        /// tab.hoverBackground
        /// </summary>
        public static readonly Unit TabHoverBackground = new("tab.hoverBackground");

        /// <summary>
        /// tab.hoverBorder
        /// </summary>
        public static readonly Unit TabHoverBorder = new("tab.hoverBorder");

        /// <summary>
        /// tab.hoverForeground
        /// </summary>
        public static readonly Unit TabHoverForeground = new("tab.hoverForeground");

        /// <summary>
        /// tab.inactiveBackground
        /// </summary>
        public static readonly Unit TabInactiveBackground = new("tab.inactiveBackground");

        /// <summary>
        /// tab.inactiveForeground
        /// </summary>
        public static readonly Unit TabInactiveForeground = new("tab.inactiveForeground");

        /// <summary>
        /// tab.inactiveModifiedBorder
        /// </summary>
        public static readonly Unit TabInactiveModifiedBorder = new("tab.inactiveModifiedBorder");

        /// <summary>
        /// tab.lastPinnedBorder
        /// </summary>
        public static readonly Unit TabLastPinnedBorder = new("tab.lastPinnedBorder");

        /// <summary>
        /// tab.unfocusedActiveBackground
        /// </summary>
        public static readonly Unit TabUnfocusedActiveBackground = new("tab.unfocusedActiveBackground");

        /// <summary>
        /// tab.unfocusedActiveBorderTop
        /// </summary>
        public static readonly Unit TabUnfocusedActiveBorderTop = new("tab.unfocusedActiveBorderTop");

        /// <summary>
        /// tab.unfocusedActiveBorder
        /// </summary>
        public static readonly Unit TabUnfocusedActiveBorder = new("tab.unfocusedActiveBorder");

        /// <summary>
        /// tab.unfocusedActiveForeground
        /// </summary>
        public static readonly Unit TabUnfocusedActiveForeground = new("tab.unfocusedActiveForeground");

        /// <summary>
        /// tab.unfocusedActiveModifiedBorder
        /// </summary>
        public static readonly Unit TabUnfocusedActiveModifiedBorder = new("tab.unfocusedActiveModifiedBorder");

        /// <summary>
        /// tab.unfocusedHoverBackground
        /// </summary>
        public static readonly Unit TabUnfocusedHoverBackground = new("tab.unfocusedHoverBackground");

        /// <summary>
        /// tab.unfocusedHoverBorder
        /// </summary>
        public static readonly Unit TabUnfocusedHoverBorder = new("tab.unfocusedHoverBorder");

        /// <summary>
        /// tab.unfocusedHoverForeground
        /// </summary>
        public static readonly Unit TabUnfocusedHoverForeground = new("tab.unfocusedHoverForeground");

        /// <summary>
        /// tab.unfocusedInactiveBackground
        /// </summary>
        public static readonly Unit TabUnfocusedInactiveBackground = new("tab.unfocusedInactiveBackground");

        /// <summary>
        /// tab.unfocusedInactiveForeground
        /// </summary>
        public static readonly Unit TabUnfocusedInactiveForeground = new("tab.unfocusedInactiveForeground");

        /// <summary>
        /// tab.unfocusedInactiveModifiedBorder
        /// </summary>
        public static readonly Unit TabUnfocusedInactiveModifiedBorder = new("tab.unfocusedInactiveModifiedBorder");

        /// <summary>
        /// terminal.ansiBlack
        /// </summary>
        public static readonly Unit TerminalAnsiBlack = new("terminal.ansiBlack");

        /// <summary>
        /// terminal.ansiBlue
        /// </summary>
        public static readonly Unit TerminalAnsiBlue = new("terminal.ansiBlue");

        /// <summary>
        /// terminal.ansiBrightBlack
        /// </summary>
        public static readonly Unit TerminalAnsiBrightBlack = new("terminal.ansiBrightBlack");

        /// <summary>
        /// terminal.ansiBrightBlue
        /// </summary>
        public static readonly Unit TerminalAnsiBrightBlue = new("terminal.ansiBrightBlue");

        /// <summary>
        /// terminal.ansiBrightCyan
        /// </summary>
        public static readonly Unit TerminalAnsiBrightCyan = new("terminal.ansiBrightCyan");

        /// <summary>
        /// terminal.ansiBrightGreen
        /// </summary>
        public static readonly Unit TerminalAnsiBrightGreen = new("terminal.ansiBrightGreen");

        /// <summary>
        /// terminal.ansiBrightMagenta
        /// </summary>
        public static readonly Unit TerminalAnsiBrightMagenta = new("terminal.ansiBrightMagenta");

        /// <summary>
        /// terminal.ansiBrightRed
        /// </summary>
        public static readonly Unit TerminalAnsiBrightRed = new("terminal.ansiBrightRed");

        /// <summary>
        /// terminal.ansiBrightWhite
        /// </summary>
        public static readonly Unit TerminalAnsiBrightWhite = new("terminal.ansiBrightWhite");

        /// <summary>
        /// terminal.ansiBrightYellow
        /// </summary>
        public static readonly Unit TerminalAnsiBrightYellow = new("terminal.ansiBrightYellow");

        /// <summary>
        /// terminal.ansiCyan
        /// </summary>
        public static readonly Unit TerminalAnsiCyan = new("terminal.ansiCyan");

        /// <summary>
        /// terminal.ansiGreen
        /// </summary>
        public static readonly Unit TerminalAnsiGreen = new("terminal.ansiGreen");

        /// <summary>
        /// terminal.ansiMagenta
        /// </summary>
        public static readonly Unit TerminalAnsiMagenta = new("terminal.ansiMagenta");

        /// <summary>
        /// terminal.ansiRed
        /// </summary>
        public static readonly Unit TerminalAnsiRed = new("terminal.ansiRed");

        /// <summary>
        /// terminal.ansiWhite
        /// </summary>
        public static readonly Unit TerminalAnsiWhite = new("terminal.ansiWhite");

        /// <summary>
        /// terminal.ansiYellow
        /// </summary>
        public static readonly Unit TerminalAnsiYellow = new("terminal.ansiYellow");

        /// <summary>
        /// terminal.background
        /// </summary>
        public static readonly Unit TerminalBackground = new("terminal.background");

        /// <summary>
        /// terminal.border
        /// </summary>
        public static readonly Unit TerminalBorder = new("terminal.border");

        /// <summary>
        /// terminal.dropBackground
        /// </summary>
        public static readonly Unit TerminalDropBackground = new("terminal.dropBackground");

        /// <summary>
        /// terminal.findMatchBackground
        /// </summary>
        public static readonly Unit TerminalFindMatchBackground = new("terminal.findMatchBackground");

        /// <summary>
        /// terminal.findMatchBorder
        /// </summary>
        public static readonly Unit TerminalFindMatchBorder = new("terminal.findMatchBorder");

        /// <summary>
        /// terminal.findMatchHighlightBackground
        /// </summary>
        public static readonly Unit TerminalFindMatchHighlightBackground = new("terminal.findMatchHighlightBackground");

        /// <summary>
        /// terminal.findMatchHighlightBorder
        /// </summary>
        public static readonly Unit TerminalFindMatchHighlightBorder = new("terminal.findMatchHighlightBorder");

        /// <summary>
        /// terminal.foreground
        /// </summary>
        public static readonly Unit TerminalForeground = new("terminal.foreground");

        /// <summary>
        /// terminal.hoverHighlightBackground
        /// </summary>
        public static readonly Unit TerminalHoverHighlightBackground = new("terminal.hoverHighlightBackground");

        /// <summary>
        /// terminal.inactiveSelectionBackground
        /// </summary>
        public static readonly Unit TerminalInactiveSelectionBackground = new("terminal.inactiveSelectionBackground");

        /// <summary>
        /// terminal.selectionBackground
        /// </summary>
        public static readonly Unit TerminalSelectionBackground = new("terminal.selectionBackground");

        /// <summary>
        /// terminal.selectionForeground
        /// </summary>
        public static readonly Unit TerminalSelectionForeground = new("terminal.selectionForeground");

        /// <summary>
        /// terminal.tab.activeBorder
        /// </summary>
        public static readonly Unit TerminalTabActiveBorder = new("terminal.tab.activeBorder");

        /// <summary>
        /// terminalCommandDecoration.defaultBackground
        /// </summary>
        public static readonly Unit TerminalCommandDecorationDefaultBackground
            = new("terminalCommandDecoration.defaultBackground");

        /// <summary>
        /// terminalCommandDecoration.errorBackground
        /// </summary>
        public static readonly Unit TerminalCommandDecorationErrorBackground
            = new("terminalCommandDecoration.errorBackground");

        /// <summary>
        /// terminalCommandDecoration.successBackground
        /// </summary>
        public static readonly Unit TerminalCommandDecorationSuccessBackground
            = new("terminalCommandDecoration.successBackground");

        /// <summary>
        /// terminalCursor.background
        /// </summary>
        public static readonly Unit TerminalCursorBackground = new("terminalCursor.background");

        /// <summary>
        /// terminalCursor.foreground
        /// </summary>
        public static readonly Unit TerminalCursorForeground = new("terminalCursor.foreground");

        /// <summary>
        /// terminalOverviewRuler.cursorForeground
        /// </summary>
        public static readonly Unit TerminalOverviewRulerCursorForeground
            = new("terminalOverviewRuler.cursorForeground");

        /// <summary>
        /// terminalOverviewRuler.findMatchForeground
        /// </summary>
        public static readonly Unit TerminalOverviewRulerFindMatchForeground
            = new("terminalOverviewRuler.findMatchForeground");

        /// <summary>
        /// testing.iconErrored
        /// </summary>
        public static readonly Unit TestingIconErrored = new("testing.iconErrored");

        /// <summary>
        /// testing.iconFailed
        /// </summary>
        public static readonly Unit TestingIconFailed = new("testing.iconFailed");

        /// <summary>
        /// testing.iconPassed
        /// </summary>
        public static readonly Unit TestingIconPassed = new("testing.iconPassed");

        /// <summary>
        /// testing.iconQueued
        /// </summary>
        public static readonly Unit TestingIconQueued = new("testing.iconQueued");

        /// <summary>
        /// testing.iconSkipped
        /// </summary>
        public static readonly Unit TestingIconSkipped = new("testing.iconSkipped");

        /// <summary>
        /// testing.iconUnset
        /// </summary>
        public static readonly Unit TestingIconUnset = new("testing.iconUnset");

        /// <summary>
        /// testing.message.error.decorationForeground
        /// </summary>
        public static readonly Unit TestingMessageErrorDecorationForeground
            = new("testing.message.error.decorationForeground");

        /// <summary>
        /// testing.message.error.lineBackground
        /// </summary>
        public static readonly Unit TestingMessageErrorLineBackground = new("testing.message.error.lineBackground");

        /// <summary>
        /// testing.message.info.decorationForeground
        /// </summary>
        public static readonly Unit TestingMessageInfoDecorationForeground
            = new("testing.message.info.decorationForeground");

        /// <summary>
        /// testing.message.info.lineBackground
        /// </summary>
        public static readonly Unit TestingMessageInfoLineBackground = new("testing.message.info.lineBackground");

        /// <summary>
        /// testing.peekBorder
        /// </summary>
        public static readonly Unit TestingPeekBorder = new("testing.peekBorder");

        /// <summary>
        /// testing.peekHeaderBackground
        /// </summary>
        public static readonly Unit TestingPeekHeaderBackground = new("testing.peekHeaderBackground");

        /// <summary>
        /// testing.runAction
        /// </summary>
        public static readonly Unit TestingRunAction = new("testing.runAction");

        /// <summary>
        /// textBlockQuote.background
        /// </summary>
        public static readonly Unit TextBlockQuoteBackground = new("textBlockQuote.background");

        /// <summary>
        /// textBlockQuote.border
        /// </summary>
        public static readonly Unit TextBlockQuoteBorder = new("textBlockQuote.border");

        /// <summary>
        /// textCodeBlock.background
        /// </summary>
        public static readonly Unit TextCodeBlockBackground = new("textCodeBlock.background");

        /// <summary>
        /// textLink.activeForeground
        /// </summary>
        public static readonly Unit TextLinkActiveForeground = new("textLink.activeForeground");

        /// <summary>
        /// textLink.foreground
        /// </summary>
        public static readonly Unit TextLinkForeground = new("textLink.foreground");

        /// <summary>
        /// textPreformat.foreground
        /// </summary>
        public static readonly Unit TextPreformatForeground = new("textPreformat.foreground");

        /// <summary>
        /// textSeparator.foreground
        /// </summary>
        public static readonly Unit TextSeparatorForeground = new("textSeparator.foreground");

        /// <summary>
        /// titleBar.activeBackground
        /// </summary>
        public static readonly Unit TitleBarActiveBackground = new("titleBar.activeBackground");

        /// <summary>
        /// titleBar.activeForeground
        /// </summary>
        public static readonly Unit TitleBarActiveForeground = new("titleBar.activeForeground");

        /// <summary>
        /// titleBar.border
        /// </summary>
        public static readonly Unit TitleBarBorder = new("titleBar.border");

        /// <summary>
        /// titleBar.inactiveBackground
        /// </summary>
        public static readonly Unit TitleBarInactiveBackground = new("titleBar.inactiveBackground");

        /// <summary>
        /// titleBar.inactiveForeground
        /// </summary>
        public static readonly Unit TitleBarInactiveForeground = new("titleBar.inactiveForeground");

        /// <summary>
        /// toolbar.activeBackground
        /// </summary>
        public static readonly Unit ToolbarActiveBackground = new("toolbar.activeBackground");

        /// <summary>
        /// toolbar.hoverBackground
        /// </summary>
        public static readonly Unit ToolbarHoverBackground = new("toolbar.hoverBackground");

        /// <summary>
        /// toolbar.hoverOutline
        /// </summary>
        public static readonly Unit ToolbarHoverOutline = new("toolbar.hoverOutline");

        /// <summary>
        /// tree.inactiveIndentGuidesStroke
        /// </summary>
        public static readonly Unit TreeInactiveIndentGuidesStroke = new("tree.inactiveIndentGuidesStroke");

        /// <summary>
        /// tree.indentGuidesStroke
        /// </summary>
        public static readonly Unit TreeIndentGuidesStroke = new("tree.indentGuidesStroke");

        /// <summary>
        /// tree.tableColumnsBorder
        /// </summary>
        public static readonly Unit TreeTableColumnsBorder = new("tree.tableColumnsBorder");

        /// <summary>
        /// tree.tableOddRowsBackground
        /// </summary>
        public static readonly Unit TreeTableOddRowsBackground = new("tree.tableOddRowsBackground");

        /// <summary>
        /// walkThrough.embeddedEditorBackground
        /// </summary>
        public static readonly Unit WalkThroughEmbeddedEditorBackground = new("walkThrough.embeddedEditorBackground");

        /// <summary>
        /// walkthrough.stepTitle.foreground
        /// </summary>
        public static readonly Unit WalkthroughStepTitleForeground = new("walkthrough.stepTitle.foreground");

        /// <summary>
        /// welcomePage.background
        /// </summary>
        public static readonly Unit WelcomePageBackground = new("welcomePage.background");

        /// <summary>
        /// welcomePage.progress.background
        /// </summary>
        public static readonly Unit WelcomePageProgressBackground = new("welcomePage.progress.background");

        /// <summary>
        /// welcomePage.progress.foreground
        /// </summary>
        public static readonly Unit WelcomePageProgressForeground = new("welcomePage.progress.foreground");

        /// <summary>
        /// welcomePage.tileBackground
        /// </summary>
        public static readonly Unit WelcomePageTileBackground = new("welcomePage.tileBackground");

        /// <summary>
        /// welcomePage.tileBorder
        /// </summary>
        public static readonly Unit WelcomePageTileBorder = new("welcomePage.tileBorder");

        /// <summary>
        /// welcomePage.tileHoverBackground
        /// </summary>
        public static readonly Unit WelcomePageTileHoverBackground = new("welcomePage.tileHoverBackground");

        /// <summary>
        /// widget.border
        /// </summary>
        public static readonly Unit WidgetBorder = new("widget.border");

        /// <summary>
        /// widget.shadow
        /// </summary>
        public static readonly Unit WidgetShadow = new("widget.shadow");

        /// <summary>
        /// window.activeBorder
        /// </summary>
        public static readonly Unit WindowActiveBorder = new("window.activeBorder");

        /// <summary>
        /// window.inactiveBorder
        /// </summary>
        public static readonly Unit WindowInactiveBorder = new("window.inactiveBorder");
    }
}