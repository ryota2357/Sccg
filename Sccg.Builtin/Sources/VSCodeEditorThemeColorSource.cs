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
            return new VSCodeFormatter.ColorFormattable {
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
        /// The contrast colors are typically only set for high contrast themes.
        /// If set, they add an additional border around items across the UI to increase the contrast.
        /// </summary>
        public static class Contrast
        {
            /// <summary>
            /// contrastActiveBorder: An extra border around active elements to separate them from others for greater contrast.
            /// </summary>
            public static readonly Unit ActiveBorder = new("contrastActiveBorder");

            /// <summary>
            /// contrastBorder: An extra border around elements to separate them from others for greater contrast.
            /// </summary>
            public static readonly Unit Border = new("contrastBorder");
        }

        /// <summary>
        ///
        /// </summary>
        public static class Base
        {
            /// <summary>
            /// focusBorder: Overall border color for focused elements. This color is only used if not overridden by a component.
            /// </summary>
            public static readonly Unit FocusBorder = new("focusBorder");

            /// <summary>
            /// foreground: Overall foreground color. This color is only used if not overridden by a component.
            /// </summary>
            public static readonly Unit Foreground = new("foreground");

            /// <summary>
            /// disabledForeground: Overall foreground for disabled elements. This color is only used if not overridden by a component.
            /// </summary>
            public static readonly Unit DisabledForeground = new("disabledForeground");

            /// <summary>
            /// widget.border: Border color of widgets such as Find/Replace inside the editor.
            /// </summary>
            public static readonly Unit WidgetBorder = new("widget.border");

            /// <summary>
            /// widget.shadow: Shadow color of widgets such as Find/Replace inside the editor.
            /// </summary>
            public static readonly Unit WidgetShadow = new("widget.shadow");

            /// <summary>
            /// selection.background: Background color of text selections in the workbench (for input fields or text areas, does not apply to selections within the editor and the terminal).
            /// </summary>
            public static readonly Unit SelectionBackground = new("selection.background");

            /// <summary>
            /// descriptionForeground: Foreground color for description text providing additional information, for example for a label.
            /// </summary>
            public static readonly Unit DescriptionForeground = new("descriptionForeground");

            /// <summary>
            /// errorForeground: Overall foreground color for error messages (this color is only used if not overridden by a component).
            /// </summary>
            public static readonly Unit ErrorForeground = new("errorForeground");

            /// <summary>
            /// icon.foreground: The default color for icons in the workbench.
            /// </summary>
            public static readonly Unit IconForeground = new("icon.foreground");

            /// <summary>
            /// sash.hoverBorder: The hover border color for draggable sashes.
            /// </summary>
            public static readonly Unit SashHoverBorder = new("sash.hoverBorder");
        }

        /// <summary>
        /// The theme colors for VS Code window border.
        /// </summary>
        public static class Window
        {
            /// <summary>
            /// window.activeBorder: Border color for the active (focused) window.
            /// </summary>
            public static readonly Unit ActiveBorder = new("window.activeBorder");

            /// <summary>
            /// window.inactiveBorder: Border color for the inactive (unfocused) windows.
            /// </summary>
            public static readonly Unit InactiveBorder = new("window.inactiveBorder");
        }

        /// <summary>
        /// Colors inside a text document, such as the welcome page.
        /// </summary>
        public static class Text
        {
            /// <summary>
            /// textBlockQuote.background: Background color for block quotes in text.
            /// </summary>
            public static readonly Unit BlockQuoteBackground = new("textBlockQuote.background");

            /// <summary>
            /// textBlockQuote.border: Border color for block quotes in text.
            /// </summary>
            public static readonly Unit BlockQuoteBorder = new("textBlockQuote.border");

            /// <summary>
            /// textCodeBlock.background: Background color for code blocks in text.
            /// </summary>
            public static readonly Unit CodeBlockBackground = new("textCodeBlock.background");

            /// <summary>
            /// textLink.activeForeground: Foreground color for links in text when clicked on and on mouse hover.
            /// </summary>
            public static readonly Unit LinkActiveForeground = new("textLink.activeForeground");

            /// <summary>
            /// textLink.foreground: Foreground color for links in text.
            /// </summary>
            public static readonly Unit LinkForeground = new("textLink.foreground");

            /// <summary>
            /// textPreformat.foreground: Foreground color for preformatted text segments.
            /// </summary>
            public static readonly Unit PreformatForeground = new("textPreformat.foreground");

            /// <summary>
            /// textSeparator.foreground: Color for text separators.
            /// </summary>
            public static readonly Unit SeparatorForeground = new("textSeparator.foreground");
        }

        /// <summary>
        /// A set of colors to control the interactions with actions across the workbench.
        /// </summary>
        public static class Action
        {
            /// <summary>
            /// toolbar.hoverBackground: Toolbar background when hovering over actions using the mouse
            /// </summary>
            public static readonly Unit ToolbarHoverBackground = new("toolbar.hoverBackground");

            /// <summary>
            /// toolbar.hoverOutline: Toolbar outline when hovering over actions using the mouse
            /// </summary>
            public static readonly Unit ToolbarHoverOutline = new("toolbar.hoverOutline");

            /// <summary>
            /// toolbar.activeBackground: Toolbar background when holding the mouse over actions
            /// </summary>
            public static readonly Unit ToolbarActiveBackground = new("toolbar.activeBackground");
        }

        /// <summary>
        /// A set of colors for button widgets such as Open Folder button in the Explorer of a new window.
        /// </summary>
        public static class Button
        {
            /// <summary>
            /// button.background: Button background color.
            /// </summary>
            public static readonly Unit Background = new("button.background");

            /// <summary>
            /// button.foreground: Button foreground color.
            /// </summary>
            public static readonly Unit Foreground = new("button.foreground");

            /// <summary>
            /// button.border: Button border color.
            /// </summary>
            public static readonly Unit Border = new("button.border");

            /// <summary>
            /// button.separator: Button separator color.
            /// </summary>
            public static readonly Unit Separator = new("button.separator");

            /// <summary>
            /// button.hoverBackground: Button background color when hovering.
            /// </summary>
            public static readonly Unit HoverBackground = new("button.hoverBackground");

            /// <summary>
            /// button.secondaryForeground: Secondary button foreground color.
            /// </summary>
            public static readonly Unit SecondaryForeground = new("button.secondaryForeground");

            /// <summary>
            /// button.secondaryBackground: Secondary button background color.
            /// </summary>
            public static readonly Unit SecondaryBackground = new("button.secondaryBackground");

            /// <summary>
            /// button.secondaryHoverBackground: Secondary button background color when hovering.
            /// </summary>
            public static readonly Unit SecondaryHoverBackground = new("button.secondaryHoverBackground");

            /// <summary>
            /// checkbox.background: Background color of checkbox widget.
            /// </summary>
            public static readonly Unit CheckboxBackground = new("checkbox.background");

            /// <summary>
            /// checkbox.foreground: Foreground color of checkbox widget.
            /// </summary>
            public static readonly Unit CheckboxForeground = new("checkbox.foreground");

            /// <summary>
            /// checkbox.border: Border color of checkbox widget.
            /// </summary>
            public static readonly Unit CheckboxBorder = new("checkbox.border");

            /// <summary>
            /// checkbox.selectBackground: Background color of checkbox widget when the element it's in is selected.
            /// </summary>
            public static readonly Unit CheckboxSelectBackground = new("checkbox.selectBackground");

            /// <summary>
            /// checkbox.selectBorder: Border color of checkbox widget when the element it's in is selected.
            /// </summary>
            public static readonly Unit CheckboxSelectBorder = new("checkbox.selectBorder");
        }

        /// <summary>
        /// A set of colors for all Dropdown widgets such as in the Integrated Terminal or the Output panel.
        /// Note that the Dropdown control is not used on macOS currently.
        /// </summary>
        public static class Dropdown
        {
            /// <summary>
            /// dropdown.background: Dropdown background.
            /// </summary>
            public static readonly Unit Background = new("dropdown.background");

            /// <summary>
            /// dropdown.listBackground: Dropdown list background.
            /// </summary>
            public static readonly Unit ListBackground = new("dropdown.listBackground");

            /// <summary>
            /// dropdown.border: Dropdown border.
            /// </summary>
            public static readonly Unit Border = new("dropdown.border");

            /// <summary>
            /// dropdown.foreground: Dropdown foreground.
            /// </summary>
            public static readonly Unit Foreground = new("dropdown.foreground");
        }

        /// <summary>
        /// Colors for input controls such as in the Search view or the Find/Replace dialog.
        /// </summary>
        public static class Input
        {
            /// <summary>
            /// input.background: Input box background.
            /// </summary>
            public static readonly Unit Background = new("input.background");

            /// <summary>
            /// input.border: Input box border.
            /// </summary>
            public static readonly Unit Border = new("input.border");

            /// <summary>
            /// input.foreground: Input box foreground.
            /// </summary>
            public static readonly Unit Foreground = new("input.foreground");

            /// <summary>
            /// input.placeholderForeground: Input box foreground color for placeholder text.
            /// </summary>
            public static readonly Unit PlaceholderForeground = new("input.placeholderForeground");

            /// <summary>
            /// inputOption.activeBackground: Background color of activated options in input fields.
            /// </summary>
            public static readonly Unit OptionActiveBackground = new("inputOption.activeBackground");

            /// <summary>
            /// inputOption.activeBorder: Border color of activated options in input fields.
            /// </summary>
            public static readonly Unit OptionActiveBorder = new("inputOption.activeBorder");

            /// <summary>
            /// inputOption.activeForeground: Foreground color of activated options in input fields.
            /// </summary>
            public static readonly Unit OptionActiveForeground = new("inputOption.activeForeground");

            /// <summary>
            /// inputOption.hoverBackground: Background color of activated options in input fields.
            /// </summary>
            public static readonly Unit OptionHoverBackground = new("inputOption.hoverBackground");

            /// <summary>
            /// inputValidation.errorBackground: Input validation background color for error severity.
            /// </summary>
            public static readonly Unit ValidationErrorBackground = new("inputValidation.errorBackground");

            /// <summary>
            /// inputValidation.errorForeground: Input validation foreground color for error severity.
            /// </summary>
            public static readonly Unit ValidationErrorForeground = new("inputValidation.errorForeground");

            /// <summary>
            /// inputValidation.errorBorder: Input validation border color for error severity.
            /// </summary>
            public static readonly Unit ValidationErrorBorder = new("inputValidation.errorBorder");

            /// <summary>
            /// inputValidation.infoBackground: Input validation background color for information severity.
            /// </summary>
            public static readonly Unit ValidationInfoBackground = new("inputValidation.infoBackground");

            /// <summary>
            /// inputValidation.infoForeground: Input validation foreground color for information severity.
            /// </summary>
            public static readonly Unit ValidationInfoForeground = new("inputValidation.infoForeground");

            /// <summary>
            /// inputValidation.infoBorder: Input validation border color for information severity.
            /// </summary>
            public static readonly Unit ValidationInfoBorder = new("inputValidation.infoBorder");

            /// <summary>
            /// inputValidation.warningBackground: Input validation background color for information warning.
            /// </summary>
            public static readonly Unit ValidationWarningBackground = new("inputValidation.warningBackground");

            /// <summary>
            /// inputValidation.warningForeground: Input validation foreground color for warning severity.
            /// </summary>
            public static readonly Unit ValidationWarningForeground = new("inputValidation.warningForeground");

            /// <summary>
            /// inputValidation.warningBorder: Input validation border color for warning severity.
            /// </summary>
            public static readonly Unit ValidationWarningBorder = new("inputValidation.warningBorder");
        }

        /// <summary>
        ///
        /// </summary>
        public static class Scrollbar
        {

            /// <summary>
            /// scrollbar.shadow: Scrollbar slider shadow to indicate that the view is scrolled.
            /// </summary>
            public static readonly Unit Shadow = new("scrollbar.shadow");

            /// <summary>
            /// scrollbarSlider.activeBackground: Scrollbar slider background color when clicked on.
            /// </summary>
            public static readonly Unit SliderActiveBackground = new("scrollbarSlider.activeBackground");

            /// <summary>
            /// scrollbarSlider.background: Scrollbar slider background color.
            /// </summary>
            public static readonly Unit SliderBackground = new("scrollbarSlider.background");

            /// <summary>
            /// scrollbarSlider.hoverBackground: Scrollbar slider background color when hovering.
            /// </summary>
            public static readonly Unit SliderHoverBackground = new("scrollbarSlider.hoverBackground");
        }

        /// <summary>
        /// Badges are small information labels, for example, search results count.
        /// </summary>
        public static class Badge
        {
            /// <summary>
            /// badge.foreground: Badge foreground color.
            /// </summary>
            public static readonly Unit Foreground = new("badge.foreground");

            /// <summary>
            /// badge.background: Badge background color.
            /// </summary>
            public static readonly Unit Background = new("badge.background");

        }

        /// <summary>
        ///
        /// </summary>
        public static class ProgressBar
        {
            /// <summary>
            /// progressBar.background: Background color of the progress bar shown for long running operations.
            /// </summary>
            public static readonly Unit Background = new("progressBar.background");
        }

        /// <summary>
        /// Colors for list and trees like the File Explorer. An active list/tree has keyboard focus, an inactive does not.
        /// </summary>
        public static class List
        {
            /// <summary>
            /// list.activeSelectionBackground: List/Tree background color for the selected item when the list/tree is active.
            /// </summary>
            public static readonly Unit ActiveSelectionBackground = new("list.activeSelectionBackground");

            /// <summary>
            /// list.activeSelectionForeground: List/Tree foreground color for the selected item when the list/tree is active.
            /// </summary>
            public static readonly Unit ActiveSelectionForeground = new("list.activeSelectionForeground");

            /// <summary>
            /// list.activeSelectionIconForeground: List/Tree icon foreground color for the selected item when the list/tree is active. An active list/tree has keyboard focus, an inactive does not.
            /// </summary>
            public static readonly Unit ActiveSelectionIconForeground = new("list.activeSelectionIconForeground");

            /// <summary>
            /// list.dropBackground: List/Tree drag and drop background when moving items around using the mouse.
            /// </summary>
            public static readonly Unit DropBackground = new("list.dropBackground");

            /// <summary>
            /// list.focusBackground: List/Tree background color for the focused item when the list/tree is active.
            /// </summary>
            public static readonly Unit FocusBackground = new("list.focusBackground");

            /// <summary>
            /// list.focusForeground: List/Tree foreground color for the focused item when the list/tree is active. An active list/tree has keyboard focus, an inactive does not.
            /// </summary>
            public static readonly Unit FocusForeground = new("list.focusForeground");

            /// <summary>
            /// list.focusHighlightForeground: List/Tree foreground color of the match highlights on actively focused items when searching inside the list/tree.
            /// </summary>
            public static readonly Unit FocusHighlightForeground = new("list.focusHighlightForeground");

            /// <summary>
            /// list.focusOutline: List/Tree outline color for the focused item when the list/tree is active. An active list/tree has keyboard focus, an inactive does not.
            /// </summary>
            public static readonly Unit FocusOutline = new("list.focusOutline");

            /// <summary>
            /// list.focusAndSelectionOutline: List/Tree outline color for the focused item when the list/tree is active and selected. An active list/tree has keyboard focus, an inactive does not.
            /// </summary>
            public static readonly Unit FocusAndSelectionOutline = new("list.focusAndSelectionOutline");

            /// <summary>
            /// list.highlightForeground: List/Tree foreground color of the match highlights when searching inside the list/tree.
            /// </summary>
            public static readonly Unit HighlightForeground = new("list.highlightForeground");

            /// <summary>
            /// list.hoverBackground: List/Tree background when hovering over items using the mouse.
            /// </summary>
            public static readonly Unit HoverBackground = new("list.hoverBackground");

            /// <summary>
            /// list.hoverForeground: List/Tree foreground when hovering over items using the mouse.
            /// </summary>
            public static readonly Unit HoverForeground = new("list.hoverForeground");

            /// <summary>
            /// list.inactiveSelectionBackground: List/Tree background color for the selected item when the list/tree is inactive.
            /// </summary>
            public static readonly Unit InactiveSelectionBackground = new("list.inactiveSelectionBackground");

            /// <summary>
            /// list.inactiveSelectionForeground: List/Tree foreground color for the selected item when the list/tree is inactive. An active list/tree has keyboard focus, an inactive does not.
            /// </summary>
            public static readonly Unit InactiveSelectionForeground = new("list.inactiveSelectionForeground");

            /// <summary>
            /// list.inactiveSelectionIconForeground: List/Tree icon foreground color for the selected item when the list/tree is inactive. An active list/tree has keyboard focus, an inactive does not.
            /// </summary>
            public static readonly Unit InactiveSelectionIconForeground = new("list.inactiveSelectionIconForeground");

            /// <summary>
            /// list.inactiveFocusBackground: List background color for the focused item when the list is inactive. An active list has keyboard focus, an inactive does not. Currently only supported in lists.
            /// </summary>
            public static readonly Unit InactiveFocusBackground = new("list.inactiveFocusBackground");

            /// <summary>
            /// list.inactiveFocusOutline: List/Tree outline color for the focused item when the list/tree is inactive. An active list/tree has keyboard focus, an inactive does not.
            /// </summary>
            public static readonly Unit InactiveFocusOutline = new("list.inactiveFocusOutline");

            /// <summary>
            /// list.invalidItemForeground: List/Tree foreground color for invalid items, for example an unresolved root in explorer.
            /// </summary>
            public static readonly Unit InvalidItemForeground = new("list.invalidItemForeground");

            /// <summary>
            /// list.errorForeground: Foreground color of list items containing errors.
            /// </summary>
            public static readonly Unit ErrorForeground = new("list.errorForeground");

            /// <summary>
            /// list.warningForeground: Foreground color of list items containing warnings.
            /// </summary>
            public static readonly Unit WarningForeground = new("list.warningForeground");

            /// <summary>
            /// listFilterWidget.background: List/Tree Filter background color of typed text when searching inside the list/tree.
            /// </summary>
            public static readonly Unit FilterWidgetBackground = new("listFilterWidget.background");

            /// <summary>
            /// listFilterWidget.outline: List/Tree Filter Widget's outline color of typed text when searching inside the list/tree.
            /// </summary>
            public static readonly Unit FilterWidgetOutline = new("listFilterWidget.outline");

            /// <summary>
            /// listFilterWidget.noMatchesOutline: List/Tree Filter Widget's outline color when no match is found of typed text when searching inside the list/tree.
            /// </summary>
            public static readonly Unit FilterWidgetNoMatchesOutline = new("listFilterWidget.noMatchesOutline");

            /// <summary>
            /// listFilterWidget.shadow: Shadow color of the type filter widget in lists and tree
            /// </summary>
            public static readonly Unit FilterWidgetShadow = new("listFilterWidget.shadow");

            /// <summary>
            /// list.filterMatchBackground: Background color of the filtered matches in lists and trees.
            /// </summary>
            public static readonly Unit FilterMatchBackground = new("list.filterMatchBackground");

            /// <summary>
            /// list.filterMatchBorder: Border color of the filtered matches in lists and trees.
            /// </summary>
            public static readonly Unit FilterMatchBorder = new("list.filterMatchBorder");

            /// <summary>
            /// list.deemphasizedForeground: List/Tree foreground color for items that are deemphasized.
            /// </summary>
            public static readonly Unit DeemphasizedForeground = new("list.deemphasizedForeground");

            /// <summary>
            /// tree.indentGuidesStroke: Tree Widget's stroke color for indent guides.
            /// </summary>
            public static readonly Unit TreeIndentGuidesStroke = new("tree.indentGuidesStroke");

            /// <summary>
            /// tree.inactiveIndentGuidesStroke: Tree stroke color for the indentation guides that are not active.
            /// </summary>
            public static readonly Unit TreeInactiveIndentGuidesStroke = new("tree.inactiveIndentGuidesStroke");

            /// <summary>
            /// tree.tableColumnsBorder: Tree stroke color for the indentation guides.
            /// </summary>
            public static readonly Unit TreeTableColumnsBorder = new("tree.tableColumnsBorder");

            /// <summary>
            /// tree.tableOddRowsBackground: Background color for odd table rows.
            /// </summary>
            public static readonly Unit TreeTableOddRowsBackground = new("tree.tableOddRowsBackground");
        }

        /// <summary>
        /// The Activity Bar is displayed either on the far left or right of the workbench and allows fast switching between views of the Side Bar.
        /// </summary>
        public static class ActivityBar
        {
            /// <summary>
            /// activityBar.background: Activity Bar background color.
            /// </summary>
            public static readonly Unit Background = new("activityBar.background");

            /// <summary>
            /// activityBar.dropBorder: Drag and drop feedback color for the activity bar items. The activity bar is showing on the far left or right and allows to switch between views of the side bar.
            /// </summary>
            public static readonly Unit DropBorder = new("activityBar.dropBorder");

            /// <summary>
            /// activityBar.foreground: Activity Bar foreground color (for example used for the icons).
            /// </summary>
            public static readonly Unit Foreground = new("activityBar.foreground");

            /// <summary>
            /// activityBar.inactiveForeground: Activity Bar item foreground color when it is inactive.
            /// </summary>
            public static readonly Unit InactiveForeground = new("activityBar.inactiveForeground");

            /// <summary>
            /// activityBar.border: Activity Bar border color with the Side Bar.
            /// </summary>
            public static readonly Unit Border = new("activityBar.border");

            /// <summary>
            /// activityBarBadge.background: Activity notification badge background color.
            /// </summary>
            public static readonly Unit BadgeBackground = new("activityBarBadge.background");

            /// <summary>
            /// activityBarBadge.foreground: Activity notification badge foreground color.
            /// </summary>
            public static readonly Unit BadgeForeground = new("activityBarBadge.foreground");

            /// <summary>
            /// activityBar.activeBorder: Activity Bar active indicator border color.
            /// </summary>
            public static readonly Unit ActiveBorder = new("activityBar.activeBorder");

            /// <summary>
            /// activityBar.activeBackground: Activity Bar optional background color for the active element.
            /// </summary>
            public static readonly Unit ActiveBackground = new("activityBar.activeBackground");

            /// <summary>
            /// activityBar.activeFocusBorder: Activity bar focus border color for the active item.
            /// </summary>
            public static readonly Unit ActiveFocusBorder = new("activityBar.activeFocusBorder");
        }

        /// <summary>
        ///
        /// </summary>
        public static class Profiles
        {
            /// <summary>
            /// profileBadge.background: Profile badge background color. The profile badge shows on top of the settings gear icon in the activity bar.
            /// </summary>
            public static readonly Unit Background = new("profileBadge.background");

            /// <summary>
            /// profileBadge.foreground: Profile badge foreground color. The profile badge shows on top of the settings gear icon in the activity bar.
            /// </summary>
            public static readonly Unit Foreground = new("profileBadge.foreground");
        }

        /// <summary>
        /// The Side Bar contains views like the Explorer and Search.
        /// </summary>
        public static class SideBar
        {
            /// <summary>
            /// sideBar.background: Side Bar background color.
            /// </summary>
            public static readonly Unit Background = new("sideBar.background");

            /// <summary>
            /// sideBar.foreground: Side Bar foreground color. The Side Bar is the container for views like Explorer and Search.
            /// </summary>
            public static readonly Unit Foreground = new("sideBar.foreground");

            /// <summary>
            /// sideBar.border: Side Bar border color on the side separating the editor.
            /// </summary>
            public static readonly Unit Border = new("sideBar.border");

            /// <summary>
            /// sideBar.dropBackground: Drag and drop feedback color for the side bar sections. The color should have transparency so that the side bar sections can still shine through.
            /// </summary>
            public static readonly Unit DropBackground = new("sideBar.dropBackground");

            /// <summary>
            /// sideBarTitle.foreground: Side Bar title foreground color.
            /// </summary>
            public static readonly Unit BarTitleForeground = new("sideBarTitle.foreground");

            /// <summary>
            /// sideBarSectionHeader.background: Side Bar section header background color.
            /// </summary>
            public static readonly Unit SectionHeaderBackground = new("sideBarSectionHeader.background");

            /// <summary>
            /// sideBarSectionHeader.foreground: Side Bar section header foreground color.
            /// </summary>
            public static readonly Unit SectionHeaderForeground = new("sideBarSectionHeader.foreground");

            /// <summary>
            /// sideBarSectionHeader.border: Side bar section header border color.
            /// </summary>
            public static readonly Unit SectionHeaderBorder = new("sideBarSectionHeader.border");
        }

        /// <summary>
        /// The Minimap shows a minified version of the current file.
        /// </summary>
        public static class Minimap
        {
            /// <summary>
            /// minimap.findMatchHighlight: Highlight color for matches from search within files.
            /// </summary>
            public static readonly Unit FindMatchHighlight = new("minimap.findMatchHighlight");

            /// <summary>
            /// minimap.selectionHighlight: Highlight color for the editor selection.
            /// </summary>
            public static readonly Unit SelectionHighlight = new("minimap.selectionHighlight");

            /// <summary>
            /// minimap.errorHighlight: Highlight color for errors within the editor.
            /// </summary>
            public static readonly Unit ErrorHighlight = new("minimap.errorHighlight");

            /// <summary>
            /// minimap.warningHighlight: Highlight color for warnings within the editor.
            /// </summary>
            public static readonly Unit WarningHighlight = new("minimap.warningHighlight");

            /// <summary>
            /// minimap.background: Minimap background color.
            /// </summary>
            public static readonly Unit Background = new("minimap.background");

            /// <summary>
            /// minimap.selectionOccurrenceHighlight: Minimap marker color for repeating editor selections.
            /// </summary>
            public static readonly Unit SelectionOccurrenceHighlight = new("minimap.selectionOccurrenceHighlight");

            /// <summary>
            /// minimap.foregroundOpacity: Opacity of foreground elements rendered in the minimap. For example, "#000000c0" will render the elements with 75% opacity.
            /// </summary>
            public static readonly Unit ForegroundOpacity = new("minimap.foregroundOpacity");

            /// <summary>
            /// minimapSlider.background: Minimap slider background color.
            /// </summary>
            public static readonly Unit SliderBackground = new("minimapSlider.background");

            /// <summary>
            /// minimapSlider.hoverBackground: Minimap slider background color when hovering.
            /// </summary>
            public static readonly Unit SliderHoverBackground = new("minimapSlider.hoverBackground");

            /// <summary>
            /// minimapSlider.activeBackground: Minimap slider background color when clicked on.
            /// </summary>
            public static readonly Unit SliderActiveBackground = new("minimapSlider.activeBackground");

            /// <summary>
            /// minimapGutter.addedBackground: Minimap gutter color for added content.
            /// </summary>
            public static readonly Unit GutterAddedBackground = new("minimapGutter.addedBackground");

            /// <summary>
            /// minimapGutter.modifiedBackground: Minimap gutter color for modified content.
            /// </summary>
            public static readonly Unit GutterModifiedBackground = new("minimapGutter.modifiedBackground");

            /// <summary>
            /// minimapGutter.deletedBackground: Minimap gutter color for deleted content.
            /// </summary>
            public static readonly Unit GutterDeletedBackground = new("minimapGutter.deletedBackground");
        }

        /// <summary>
        /// Editor Groups are the containers of editors. There can be many editor groups.
        /// </summary>
        public static class EditorGroup
        {
            /// <summary>
            /// editorGroup.border: Color to separate multiple editor groups from each other.
            /// </summary>
            public static readonly Unit Border = new("editorGroup.border");

            /// <summary>
            /// editorGroup.dropBackground: Background color when dragging editors around.
            /// </summary>
            public static readonly Unit DropBackground = new("editorGroup.dropBackground");

            /// <summary>
            /// editorGroupHeader.noTabsBackground: Background color of the editor group title header when Tabs are disabled (set `"workbench.editor.showTabs": false`).
            /// </summary>
            public static readonly Unit HeaderNoTabsBackground = new("editorGroupHeader.noTabsBackground");

            /// <summary>
            /// editorGroupHeader.tabsBackground: Background color of the Tabs container.
            /// </summary>
            public static readonly Unit HeaderTabsBackground = new("editorGroupHeader.tabsBackground");

            /// <summary>
            /// editorGroupHeader.tabsBorder: Border color below the editor tabs control when tabs are enabled.
            /// </summary>
            public static readonly Unit HeaderTabsBorder = new("editorGroupHeader.tabsBorder");

            /// <summary>
            /// editorGroupHeader.border: Border color between editor group header and editor (below breadcrumbs if enabled).
            /// </summary>
            public static readonly Unit HeaderBorder = new("editorGroupHeader.border");

            /// <summary>
            /// editorGroup.emptyBackground: Background color of an empty editor group.
            /// </summary>
            public static readonly Unit EmptyBackground = new("editorGroup.emptyBackground");

            /// <summary>
            /// editorGroup.focusedEmptyBorder: Border color of an empty editor group that is focused.
            /// </summary>
            public static readonly Unit FocusedEmptyBorder = new("editorGroup.focusedEmptyBorder");

            /// <summary>
            /// editorGroup.dropIntoPromptForeground: Foreground color of text shown over editors when dragging files. This text informs the user that they can hold shift to drop into the editor.
            /// </summary>
            public static readonly Unit DropIntoPromptForeground = new("editorGroup.dropIntoPromptForeground");

            /// <summary>
            /// editorGroup.dropIntoPromptBackground: Background color of text shown over editors when dragging files. This text informs the user that they can hold shift to drop into the editor.
            /// </summary>
            public static readonly Unit DropIntoPromptBackground = new("editorGroup.dropIntoPromptBackground");

            /// <summary>
            /// editorGroup.dropIntoPromptBorder: Border color of text shown over editors when dragging files. This text informs the user that they can hold shift to drop into the editor.
            /// </summary>
            public static readonly Unit DropIntoPromptBorder = new("editorGroup.dropIntoPromptBorder");
        }

        /// <summary>
        /// A Tab is the container of an editor. Multiple Tabs can be opened in one editor group.
        /// </summary>
        public static class Tab
        {
            /// <summary>
            /// tab.activeBackground: Active Tab background color in an active group.
            /// </summary>
            public static readonly Unit ActiveBackground = new("tab.activeBackground");

            /// <summary>
            /// tab.unfocusedActiveBackground: Active Tab background color in an inactive editor group.
            /// </summary>
            public static readonly Unit UnfocusedActiveBackground = new("tab.unfocusedActiveBackground");

            /// <summary>
            /// tab.activeForeground: Active Tab foreground color in an active group.
            /// </summary>
            public static readonly Unit ActiveForeground = new("tab.activeForeground");

            /// <summary>
            /// tab.border: Border to separate Tabs from each other.
            /// </summary>
            public static readonly Unit Border = new("tab.border");

            /// <summary>
            /// tab.activeBorder: Bottom border for the active tab.
            /// </summary>
            public static readonly Unit ActiveBorder = new("tab.activeBorder");

            /// <summary>
            /// tab.unfocusedActiveBorder: Bottom border for the active tab in an inactive editor group.
            /// </summary>
            public static readonly Unit UnfocusedActiveBorder = new("tab.unfocusedActiveBorder");

            /// <summary>
            /// tab.activeBorderTop: Top border for the active tab.
            /// </summary>
            public static readonly Unit ActiveBorderTop = new("tab.activeBorderTop");

            /// <summary>
            /// tab.unfocusedActiveBorderTop: Top border for the active tab in an inactive editor group
            /// </summary>
            public static readonly Unit UnfocusedActiveBorderTop = new("tab.unfocusedActiveBorderTop");

            /// <summary>
            /// tab.lastPinnedBorder: Border on the right of the last pinned editor to separate from unpinned editors.
            /// </summary>
            public static readonly Unit LastPinnedBorder = new("tab.lastPinnedBorder");

            /// <summary>
            /// tab.inactiveBackground: Inactive Tab background color.
            /// </summary>
            public static readonly Unit InactiveBackground = new("tab.inactiveBackground");

            /// <summary>
            /// tab.unfocusedInactiveBackground: Inactive Tab background color in an unfocused group
            /// </summary>
            public static readonly Unit UnfocusedInactiveBackground = new("tab.unfocusedInactiveBackground");

            /// <summary>
            /// tab.inactiveForeground: Inactive Tab foreground color in an active group.
            /// </summary>
            public static readonly Unit InactiveForeground = new("tab.inactiveForeground");

            /// <summary>
            /// tab.unfocusedActiveForeground: Active tab foreground color in an inactive editor group.
            /// </summary>
            public static readonly Unit UnfocusedActiveForeground = new("tab.unfocusedActiveForeground");

            /// <summary>
            /// tab.unfocusedInactiveForeground: Inactive tab foreground color in an inactive editor group.
            /// </summary>
            public static readonly Unit UnfocusedInactiveForeground = new("tab.unfocusedInactiveForeground");

            /// <summary>
            /// tab.hoverBackground: Tab background color when hovering
            /// </summary>
            public static readonly Unit HoverBackground = new("tab.hoverBackground");

            /// <summary>
            /// tab.unfocusedHoverBackground: Tab background color in an unfocused group when hovering
            /// </summary>
            public static readonly Unit UnfocusedHoverBackground = new("tab.unfocusedHoverBackground");

            /// <summary>
            /// tab.hoverForeground: Tab foreground color when hovering
            /// </summary>
            public static readonly Unit HoverForeground = new("tab.hoverForeground");

            /// <summary>
            /// tab.unfocusedHoverForeground: Tab foreground color in an unfocused group when hovering
            /// </summary>
            public static readonly Unit UnfocusedHoverForeground = new("tab.unfocusedHoverForeground");

            /// <summary>
            /// tab.hoverBorder: Border to highlight tabs when hovering
            /// </summary>
            public static readonly Unit HoverBorder = new("tab.hoverBorder");

            /// <summary>
            /// tab.unfocusedHoverBorder: Border to highlight tabs in an unfocused group when hovering
            /// </summary>
            public static readonly Unit UnfocusedHoverBorder = new("tab.unfocusedHoverBorder");

            /// <summary>
            /// tab.activeModifiedBorder: Border on the top of modified (dirty) active tabs in an active group.
            /// </summary>
            public static readonly Unit ActiveModifiedBorder = new("tab.activeModifiedBorder");

            /// <summary>
            /// tab.inactiveModifiedBorder: Border on the top of modified (dirty) inactive tabs in an active group.
            /// </summary>
            public static readonly Unit InactiveModifiedBorder = new("tab.inactiveModifiedBorder");

            /// <summary>
            /// tab.unfocusedActiveModifiedBorder: Border on the top of modified (dirty) active tabs in an unfocused group.
            /// </summary>
            public static readonly Unit UnfocusedActiveModifiedBorder = new("tab.unfocusedActiveModifiedBorder");

            /// <summary>
            /// tab.unfocusedInactiveModifiedBorder: Border on the top of modified (dirty) inactive tabs in an unfocused group.
            /// </summary>
            public static readonly Unit UnfocusedInactiveModifiedBorder = new("tab.unfocusedInactiveModifiedBorder");

            /// <summary>
            /// editorPane.background: Background color of the editor pane visible on the left and right side of the centered editor layout.
            /// </summary>
            public static readonly Unit EditorPaneBackground = new("editorPane.background");

            /// <summary>
            /// sideBySideEditor.horizontalBorder: Color to separate two editors from each other when shown side by side in an editor group from top to bottom.
            /// </summary>
            public static readonly Unit SideBySideEditorHorizontalBorder = new("sideBySideEditor.horizontalBorder");

            /// <summary>
            /// sideBySideEditor.verticalBorder: Color to separate two editors from each other when shown side by side in an editor group from left to right.
            /// </summary>
            public static readonly Unit SideBySideEditorVerticalBorder = new("sideBySideEditor.verticalBorder");
        }

        /// <summary>
        /// The most prominent editor colors are the token colors used for syntax highlighting and are based on the language grammar installed.
        /// These colors are defined by the Color Theme but can also be customized with the editor.tokenColorCustomizations setting.
        /// See Customizing a Color Theme for details on updating a Color Theme and the available token types.
        /// </summary>
        public static class Editor
        {
            /// <summary>
            /// editor.background: Editor background color.
            /// </summary>
            public static readonly Unit Background = new("editor.background");

            /// <summary>
            /// editor.foreground: Editor default foreground color.
            /// </summary>
            public static readonly Unit Foreground = new("editor.foreground");

            /// <summary>
            /// editorLineNumber.foreground: Color of editor line numbers.
            /// </summary>
            public static readonly Unit LineNumberForeground = new("editorLineNumber.foreground");

            /// <summary>
            /// editorLineNumber.activeForeground: Color of the active editor line number.
            /// </summary>
            public static readonly Unit LineNumberActiveForeground = new("editorLineNumber.activeForeground");

            /// <summary>
            /// editorLineNumber.dimmedForeground: Color of the final editor line when editor.renderFinalNewline is set to dimmed.
            /// </summary>
            public static readonly Unit LineNumberDimmedForeground = new("editorLineNumber.dimmedForeground");

            /// <summary>
            /// editorCursor.background: The background color of the editor cursor. Allows customizing the color of a character overlapped by a block cursor.
            /// </summary>
            public static readonly Unit CursorBackground = new("editorCursor.background");

            /// <summary>
            /// editorCursor.foreground: Color of the editor cursor.
            /// </summary>
            public static readonly Unit CursorForeground = new("editorCursor.foreground");

            /// <summary>
            /// editor.selectionBackground: Color of the editor selection.
            /// </summary>
            public static readonly Unit SelectionBackground = new("editor.selectionBackground");

            /// <summary>
            /// editor.selectionForeground: Color of the selected text for high contrast.
            /// </summary>
            public static readonly Unit SelectionForeground = new("editor.selectionForeground");

            /// <summary>
            /// editor.inactiveSelectionBackground: Color of the selection in an inactive editor. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit InactiveSelectionBackground = new("editor.inactiveSelectionBackground");

            /// <summary>
            /// editor.selectionHighlightBackground: Color for regions with the same content as the selection. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit SelectionHighlightBackground = new("editor.selectionHighlightBackground");

            /// <summary>
            /// editor.selectionHighlightBorder: Border color for regions with the same content as the selection.
            /// </summary>
            public static readonly Unit SelectionHighlightBorder = new("editor.selectionHighlightBorder");

            /// <summary>
            /// editor.wordHighlightBackground: Background color of a symbol during read-access, for example when reading a variable. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit WordHighlightBackground = new("editor.wordHighlightBackground");

            /// <summary>
            /// editor.wordHighlightBorder: Border color of a symbol during read-access, for example when reading a variable.
            /// </summary>
            public static readonly Unit WordHighlightBorder = new("editor.wordHighlightBorder");

            /// <summary>
            /// editor.wordHighlightStrongBackground: Background color of a symbol during write-access, for example when writing to a variable. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit WordHighlightStrongBackground = new("editor.wordHighlightStrongBackground");

            /// <summary>
            /// editor.wordHighlightStrongBorder: Border color of a symbol during write-access, for example when writing to a variable.
            /// </summary>
            public static readonly Unit WordHighlightStrongBorder = new("editor.wordHighlightStrongBorder");

            /// <summary>
            /// editor.wordHighlightTextBackground: Background color of a textual occurrence for a symbol. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit WordHighlightTextBackground = new("editor.wordHighlightTextBackground");

            /// <summary>
            /// editor.wordHighlightTextBorder: Border color of a textual occurrence for a symbol.
            /// </summary>
            public static readonly Unit WordHighlightTextBorder = new("editor.wordHighlightTextBorder");

            /// <summary>
            /// editor.findMatchBackground: Color of the current search match.
            /// </summary>
            public static readonly Unit FindMatchBackground = new("editor.findMatchBackground");

            /// <summary>
            /// editor.findMatchHighlightBackground: Color of the other search matches. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit FindMatchHighlightBackground = new("editor.findMatchHighlightBackground");

            /// <summary>
            /// editor.findRangeHighlightBackground: Color the range limiting the search (Enable 'Find in Selection' in the find widget). The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit FindRangeHighlightBackground = new("editor.findRangeHighlightBackground");

            /// <summary>
            /// editor.findMatchBorder: Border color of the current search match.
            /// </summary>
            public static readonly Unit FindMatchBorder = new("editor.findMatchBorder");

            /// <summary>
            /// editor.findMatchHighlightBorder: Border color of the other search matches.
            /// </summary>
            public static readonly Unit FindMatchHighlightBorder = new("editor.findMatchHighlightBorder");

            /// <summary>
            /// editor.findRangeHighlightBorder: Border color the range limiting the search (Enable 'Find in Selection' in the find widget).
            /// </summary>
            public static readonly Unit FindRangeHighlightBorder = new("editor.findRangeHighlightBorder");

            /// <summary>
            /// searchEditor.findMatchBackground: Color of the editor's results.
            /// </summary>
            public static readonly Unit SearchEditorFindMatchBackground = new("searchEditor.findMatchBackground");

            /// <summary>
            /// searchEditor.findMatchBorder: Border color of the editor's results.
            /// </summary>
            public static readonly Unit SearchEditorFindMatchBorder = new("searchEditor.findMatchBorder");

            /// <summary>
            /// searchEditor.textInputBorder: Search editor text input box border.
            /// </summary>
            public static readonly Unit SearchEditorTextInputBorder = new("searchEditor.textInputBorder");

            /// <summary>
            /// editor.hoverHighlightBackground: Highlight below the word for which a hover is shown. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit HoverHighlightBackground = new("editor.hoverHighlightBackground");

            /// <summary>
            /// editor.lineHighlightBackground: Background color for the highlight of line at the cursor position.
            /// </summary>
            public static readonly Unit LineHighlightBackground = new("editor.lineHighlightBackground");

            /// <summary>
            /// editor.lineHighlightBorder: Background color for the border around the line at the cursor position.
            /// </summary>
            public static readonly Unit LineHighlightBorder = new("editor.lineHighlightBorder");

            /// <summary>
            /// editorUnicodeHighlight.border: Border color used to highlight unicode characters.
            /// </summary>
            public static readonly Unit UnicodeHighlightBorder = new("editorUnicodeHighlight.border");

            /// <summary>
            /// editorUnicodeHighlight.background: Background color used to highlight unicode characters.
            /// </summary>
            public static readonly Unit UnicodeHighlightBackground = new("editorUnicodeHighlight.background");

            /// <summary>
            /// editorLink.activeForeground: Color of active links.
            /// </summary>
            public static readonly Unit LinkActiveForeground = new("editorLink.activeForeground");

            /// <summary>
            /// editor.rangeHighlightBackground: Background color of highlighted ranges, used by Quick Open, Symbol in File and Find features. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit RangeHighlightBackground = new("editor.rangeHighlightBackground");

            /// <summary>
            /// editor.rangeHighlightBorder: Background color of the border around highlighted ranges.
            /// </summary>
            public static readonly Unit RangeHighlightBorder = new("editor.rangeHighlightBorder");

            /// <summary>
            /// editor.symbolHighlightBackground: Background color of highlighted symbol. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit SymbolHighlightBackground = new("editor.symbolHighlightBackground");

            /// <summary>
            /// editor.symbolHighlightBorder: Background color of the border around highlighted symbols.
            /// </summary>
            public static readonly Unit SymbolHighlightBorder = new("editor.symbolHighlightBorder");

            /// <summary>
            /// editorWhitespace.foreground: Color of whitespace characters in the editor.
            /// </summary>
            public static readonly Unit WhitespaceForeground = new("editorWhitespace.foreground");

            /// <summary>
            /// editorIndentGuide.background: Color of the editor indentation guides.
            /// </summary>
            public static readonly Unit IndentGuideBackground = new("editorIndentGuide.background");

            /// <summary>
            /// editorIndentGuide.activeBackground: Color of the active editor indentation guide.
            /// </summary>
            public static readonly Unit IndentGuideActiveBackground = new("editorIndentGuide.activeBackground");

            /// <summary>
            /// editorInlayHint.background: Background color of inline hints.
            /// </summary>
            public static readonly Unit InlayHintBackground = new("editorInlayHint.background");

            /// <summary>
            /// editorInlayHint.foreground: Foreground color of inline hints.
            /// </summary>
            public static readonly Unit InlayHintForeground = new("editorInlayHint.foreground");

            /// <summary>
            /// editorInlayHint.typeForeground: Foreground color of inline hints for types
            /// </summary>
            public static readonly Unit InlayHintTypeForeground = new("editorInlayHint.typeForeground");

            /// <summary>
            /// editorInlayHint.typeBackground: Background color of inline hints for types
            /// </summary>
            public static readonly Unit InlayHintTypeBackground = new("editorInlayHint.typeBackground");

            /// <summary>
            /// editorInlayHint.parameterForeground: Foreground color of inline hints for parameters
            /// </summary>
            public static readonly Unit InlayHintParameterForeground = new("editorInlayHint.parameterForeground");

            /// <summary>
            /// editorInlayHint.parameterBackground: Background color of inline hints for parameters
            /// </summary>
            public static readonly Unit InlayHintParameterBackground = new("editorInlayHint.parameterBackground");

            /// <summary>
            /// editorRuler.foreground: Color of the editor rulers.
            /// </summary>
            public static readonly Unit RulerForeground = new("editorRuler.foreground");

            /// <summary>
            /// editor.linkedEditingBackground: Background color when the editor is in linked editing mode.
            /// </summary>
            public static readonly Unit LinkedEditingBackground = new("editor.linkedEditingBackground");

            /// <summary>
            /// editorCodeLens.foreground: Foreground color of an editor CodeLens.
            /// </summary>
            public static readonly Unit CodeLensForeground = new("editorCodeLens.foreground");

            /// <summary>
            /// editorLightBulb.foreground: The color used for the lightbulb actions icon.
            /// </summary>
            public static readonly Unit LightBulbForeground = new("editorLightBulb.foreground");

            /// <summary>
            /// editorLightBulbAutoFix.foreground: The color used for the lightbulb auto fix actions icon.
            /// </summary>
            public static readonly Unit LightBulbAutoFixForeground = new("editorLightBulbAutoFix.foreground");

            /// <summary>
            /// editorBracketMatch.background: Background color behind matching brackets.
            /// </summary>
            public static readonly Unit BracketMatchBackground = new("editorBracketMatch.background");

            /// <summary>
            /// editorBracketMatch.border: Color for matching brackets boxes.
            /// </summary>
            public static readonly Unit BracketMatchBorder = new("editorBracketMatch.border");

            /// <summary>
            /// editorBracketHighlight.foreground1: Foreground color of brackets (1). Requires enabling bracket pair colorization.
            /// </summary>
            public static readonly Unit BracketHighlightForeground1 = new("editorBracketHighlight.foreground1");

            /// <summary>
            /// editorBracketHighlight.foreground2: Foreground color of brackets (2). Requires enabling bracket pair colorization.
            /// </summary>
            public static readonly Unit BracketHighlightForeground2 = new("editorBracketHighlight.foreground2");

            /// <summary>
            /// editorBracketHighlight.foreground3: Foreground color of brackets (3). Requires enabling bracket pair colorization.
            /// </summary>
            public static readonly Unit BracketHighlightForeground3 = new("editorBracketHighlight.foreground3");

            /// <summary>
            /// editorBracketHighlight.foreground4: Foreground color of brackets (4). Requires enabling bracket pair colorization.
            /// </summary>
            public static readonly Unit BracketHighlightForeground4 = new("editorBracketHighlight.foreground4");

            /// <summary>
            /// editorBracketHighlight.foreground5: Foreground color of brackets (5). Requires enabling bracket pair colorization.
            /// </summary>
            public static readonly Unit BracketHighlightForeground5 = new("editorBracketHighlight.foreground5");

            /// <summary>
            /// editorBracketHighlight.foreground6: Foreground color of brackets (6). Requires enabling bracket pair colorization.
            /// </summary>
            public static readonly Unit BracketHighlightForeground6 = new("editorBracketHighlight.foreground6");

            /// <summary>
            /// editorBracketHighlight.unexpectedBracket.foreground: Foreground color of unexpected brackets.
            /// </summary>
            public static readonly Unit BracketHighlightUnexpectedBracketForeground
                = new("editorBracketHighlight.unexpectedBracket.foreground");

            /// <summary>
            /// editorBracketPairGuide.activeBackground1: Background color of active bracket pair guides (1). Requires enabling bracket pair guides.
            /// </summary>
            public static readonly Unit BracketPairGuideActiveBackground1
                = new("editorBracketPairGuide.activeBackground1");

            /// <summary>
            /// editorBracketPairGuide.activeBackground2: Background color of active bracket pair guides (2). Requires enabling bracket pair guides.
            /// </summary>
            public static readonly Unit BracketPairGuideActiveBackground2
                = new("editorBracketPairGuide.activeBackground2");

            /// <summary>
            /// editorBracketPairGuide.activeBackground3: Background color of active bracket pair guides (3). Requires enabling bracket pair guides.
            /// </summary>
            public static readonly Unit BracketPairGuideActiveBackground3
                = new("editorBracketPairGuide.activeBackground3");

            /// <summary>
            /// editorBracketPairGuide.activeBackground4: Background color of active bracket pair guides (4). Requires enabling bracket pair guides.
            /// </summary>
            public static readonly Unit BracketPairGuideActiveBackground4
                = new("editorBracketPairGuide.activeBackground4");

            /// <summary>
            /// editorBracketPairGuide.activeBackground5: Background color of active bracket pair guides (5). Requires enabling bracket pair guides.
            /// </summary>
            public static readonly Unit BracketPairGuideActiveBackground5
                = new("editorBracketPairGuide.activeBackground5");

            /// <summary>
            /// editorBracketPairGuide.activeBackground6: Background color of active bracket pair guides (6). Requires enabling bracket pair guides.
            /// </summary>
            public static readonly Unit BracketPairGuideActiveBackground6
                = new("editorBracketPairGuide.activeBackground6");

            /// <summary>
            /// editorBracketPairGuide.background1: Background color of inactive bracket pair guides (1). Requires enabling bracket pair guides.
            /// </summary>
            public static readonly Unit BracketPairGuideBackground1 = new("editorBracketPairGuide.background1");

            /// <summary>
            /// editorBracketPairGuide.background2: Background color of inactive bracket pair guides (2). Requires enabling bracket pair guides.
            /// </summary>
            public static readonly Unit BracketPairGuideBackground2 = new("editorBracketPairGuide.background2");

            /// <summary>
            /// editorBracketPairGuide.background3: Background color of inactive bracket pair guides (3). Requires enabling bracket pair guides.
            /// </summary>
            public static readonly Unit BracketPairGuideBackground3 = new("editorBracketPairGuide.background3");

            /// <summary>
            /// editorBracketPairGuide.background4: Background color of inactive bracket pair guides (4). Requires enabling bracket pair guides.
            /// </summary>
            public static readonly Unit BracketPairGuideBackground4 = new("editorBracketPairGuide.background4");

            /// <summary>
            /// editorBracketPairGuide.background5: Background color of inactive bracket pair guides (5). Requires enabling bracket pair guides.
            /// </summary>
            public static readonly Unit BracketPairGuideBackground5 = new("editorBracketPairGuide.background5");

            /// <summary>
            /// editorBracketPairGuide.background6: Background color of inactive bracket pair guides (6). Requires enabling bracket pair guides.
            /// </summary>
            public static readonly Unit BracketPairGuideBackground6 = new("editorBracketPairGuide.background6");

            /// <summary>
            /// editorOverviewRuler.background: Background color of the editor overview ruler. Only used when the minimap is enabled and placed on the right side of the editor.
            /// </summary>
            public static readonly Unit OverviewRulerBackground = new("editorOverviewRuler.background");

            /// <summary>
            /// editorOverviewRuler.border: Color of the overview ruler border.
            /// </summary>
            public static readonly Unit OverviewRulerBorder = new("editorOverviewRuler.border");

            /// <summary>
            /// editorOverviewRuler.findMatchForeground: Overview ruler marker color for find matches. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit OverviewRulerFindMatchForeground
                = new("editorOverviewRuler.findMatchForeground");

            /// <summary>
            /// editorOverviewRuler.rangeHighlightForeground: Overview ruler marker color for highlighted ranges, like by the Quick Open, Symbol in File and Find features. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit OverviewRulerRangeHighlightForeground
                = new("editorOverviewRuler.rangeHighlightForeground");

            /// <summary>
            /// editorOverviewRuler.selectionHighlightForeground: Overview ruler marker color for selection highlights. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit OverviewRulerSelectionHighlightForeground
                = new("editorOverviewRuler.selectionHighlightForeground");

            /// <summary>
            /// editorOverviewRuler.wordHighlightForeground: Overview ruler marker color for symbol highlights. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit OverviewRulerWordHighlightForeground
                = new("editorOverviewRuler.wordHighlightForeground");

            /// <summary>
            /// editorOverviewRuler.wordHighlightStrongForeground: Overview ruler marker color for write-access symbol highlights. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit OverviewRulerWordHighlightStrongForeground
                = new("editorOverviewRuler.wordHighlightStrongForeground");

            /// <summary>
            /// editorOverviewRuler.wordHighlightTextForeground: Overview ruler marker color of a textual occurrence for a symbol. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit OverviewRulerWordHighlightTextForeground
                = new("editorOverviewRuler.wordHighlightTextForeground");

            /// <summary>
            /// editorOverviewRuler.modifiedForeground: Overview ruler marker color for modified content.
            /// </summary>
            public static readonly Unit OverviewRulerModifiedForeground = new("editorOverviewRuler.modifiedForeground");

            /// <summary>
            /// editorOverviewRuler.addedForeground: Overview ruler marker color for added content.
            /// </summary>
            public static readonly Unit OverviewRulerAddedForeground = new("editorOverviewRuler.addedForeground");

            /// <summary>
            /// editorOverviewRuler.deletedForeground: Overview ruler marker color for deleted content.
            /// </summary>
            public static readonly Unit OverviewRulerDeletedForeground = new("editorOverviewRuler.deletedForeground");

            /// <summary>
            /// editorOverviewRuler.errorForeground: Overview ruler marker color for errors.
            /// </summary>
            public static readonly Unit OverviewRulerErrorForeground = new("editorOverviewRuler.errorForeground");

            /// <summary>
            /// editorOverviewRuler.warningForeground: Overview ruler marker color for warnings.
            /// </summary>
            public static readonly Unit OverviewRulerWarningForeground = new("editorOverviewRuler.warningForeground");

            /// <summary>
            /// editorOverviewRuler.infoForeground: Overview ruler marker color for infos.
            /// </summary>
            public static readonly Unit OverviewRulerInfoForeground = new("editorOverviewRuler.infoForeground");

            /// <summary>
            /// editorOverviewRuler.bracketMatchForeground: Overview ruler marker color for matching brackets.
            /// </summary>
            public static readonly Unit OverviewRulerBracketMatchForeground
                = new("editorOverviewRuler.bracketMatchForeground");

            /// <summary>
            /// editorError.foreground: Foreground color of error squiggles in the editor.
            /// </summary>
            public static readonly Unit ErrorForeground = new("editorError.foreground");

            /// <summary>
            /// editorError.border: Border color of error boxes in the editor.
            /// </summary>
            public static readonly Unit ErrorBorder = new("editorError.border");

            /// <summary>
            /// editorError.background: Background color of error text in the editor. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit ErrorBackground = new("editorError.background");

            /// <summary>
            /// editorWarning.foreground: Foreground color of warning squiggles in the editor.
            /// </summary>
            public static readonly Unit WarningForeground = new("editorWarning.foreground");

            /// <summary>
            /// editorWarning.border: Border color of warning boxes in the editor.
            /// </summary>
            public static readonly Unit WarningBorder = new("editorWarning.border");

            /// <summary>
            /// editorWarning.background: Background color of warning text in the editor. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit WarningBackground = new("editorWarning.background");

            /// <summary>
            /// editorInfo.foreground: Foreground color of info squiggles in the editor.
            /// </summary>
            public static readonly Unit InfoForeground = new("editorInfo.foreground");

            /// <summary>
            /// editorInfo.border: Border color of info boxes in the editor.
            /// </summary>
            public static readonly Unit InfoBorder = new("editorInfo.border");

            /// <summary>
            /// editorInfo.background: Background color of info text in the editor. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit InfoBackground = new("editorInfo.background");

            /// <summary>
            /// editorHint.foreground: Foreground color of hints in the editor.
            /// </summary>
            public static readonly Unit HintForeground = new("editorHint.foreground");

            /// <summary>
            /// editorHint.border: Border color of hint boxes in the editor.
            /// </summary>
            public static readonly Unit HintBorder = new("editorHint.border");

            /// <summary>
            /// problemsErrorIcon.foreground: The color used for the problems error icon.
            /// </summary>
            public static readonly Unit ProblemsErrorIconForeground = new("problemsErrorIcon.foreground");

            /// <summary>
            /// problemsWarningIcon.foreground: The color used for the problems warning icon.
            /// </summary>
            public static readonly Unit ProblemsWarningIconForeground = new("problemsWarningIcon.foreground");

            /// <summary>
            /// problemsInfoIcon.foreground: The color used for the problems info icon.
            /// </summary>
            public static readonly Unit ProblemsInfoIconForeground = new("problemsInfoIcon.foreground");

            /// <summary>
            /// editorUnnecessaryCode.border: Border color of unnecessary (unused) source code in the editor.
            /// </summary>
            public static readonly Unit UnnecessaryCodeBorder = new("editorUnnecessaryCode.border");

            /// <summary>
            /// editorUnnecessaryCode.opacity: Opacity of unnecessary (unused) source code in the editor. For example, `"#000000c0"` will render the code with 75% opacity. For high contrast themes, use the `"editorUnnecessaryCode.border"` theme color to underline unnecessary code instead of fading it out.
            /// </summary>
            public static readonly Unit UnnecessaryCodeOpacity = new("editorUnnecessaryCode.opacity");

            /// <summary>
            /// editorGutter.background: Background color of the editor gutter. The gutter contains the glyph margins and the line numbers.
            /// </summary>
            public static readonly Unit GutterBackground = new("editorGutter.background");

            /// <summary>
            /// editorGutter.modifiedBackground: Editor gutter background color for lines that are modified.
            /// </summary>
            public static readonly Unit GutterModifiedBackground = new("editorGutter.modifiedBackground");

            /// <summary>
            /// editorGutter.addedBackground: Editor gutter background color for lines that are added.
            /// </summary>
            public static readonly Unit GutterAddedBackground = new("editorGutter.addedBackground");

            /// <summary>
            /// editorGutter.deletedBackground: Editor gutter background color for lines that are deleted.
            /// </summary>
            public static readonly Unit GutterDeletedBackground = new("editorGutter.deletedBackground");

            /// <summary>
            /// editorGutter.commentRangeForeground: Editor gutter decoration color for commenting ranges.
            /// </summary>
            public static readonly Unit GutterCommentRangeForeground = new("editorGutter.commentRangeForeground");

            /// <summary>
            /// editorGutter.commentGlyphForground: Editor gutter decoration color for commenting glyphs.
            /// </summary>
            public static readonly Unit GutterCommentGlyphForground = new("editorGutter.commentGlyphForground");

            /// <summary>
            /// editorGutter.foldingControlForeground: Color of the folding control in the editor gutter.
            /// </summary>
            public static readonly Unit GutterFoldingControlForeground = new("editorGutter.foldingControlForeground");

            /// <summary>
            /// editorCommentsWidget.resolvedBorder: Color of borders and arrow for resolved comments.
            /// </summary>
            public static readonly Unit CommentsWidgetResolvedBorder = new("editorCommentsWidget.resolvedBorder");

            /// <summary>
            /// editorCommentsWidget.unresolvedBorder: Color of borders and arrow for unresolved comments.
            /// </summary>
            public static readonly Unit CommentsWidgetUnresolvedBorder = new("editorCommentsWidget.unresolvedBorder");

            /// <summary>
            /// editorCommentsWidget.rangeBackground: Color of background for comment ranges.
            /// </summary>
            public static readonly Unit CommentsWidgetRangeBackground = new("editorCommentsWidget.rangeBackground");

            /// <summary>
            /// editorCommentsWidget.rangeBorder: Color of border for comment ranges.
            /// </summary>
            public static readonly Unit CommentsWidgetRangeBorder = new("editorCommentsWidget.rangeBorder");

            /// <summary>
            /// editorCommentsWidget.rangeActiveBackground: Color of background for currently selected or hovered comment range.
            /// </summary>
            public static readonly Unit CommentsWidgetRangeActiveBackground
                = new("editorCommentsWidget.rangeActiveBackground");

            /// <summary>
            /// editorCommentsWidget.rangeActiveBorder: Color of border for currently selected or hovered comment range.
            /// </summary>
            public static readonly Unit CommentsWidgetRangeActiveBorder = new("editorCommentsWidget.rangeActiveBorder");
        }

        /// <summary>
        /// For coloring inserted and removed text, use either a background or a border color but not both.
        /// </summary>
        public static class DiffEditor
        {
            /// <summary>
            /// diffEditor.insertedTextBackground: Background color for text that got inserted. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit InsertedTextBackground = new("diffEditor.insertedTextBackground");

            /// <summary>
            /// diffEditor.insertedTextBorder: Outline color for the text that got inserted.
            /// </summary>
            public static readonly Unit InsertedTextBorder = new("diffEditor.insertedTextBorder");

            /// <summary>
            /// diffEditor.removedTextBackground: Background color for text that got removed. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit RemovedTextBackground = new("diffEditor.removedTextBackground");

            /// <summary>
            /// diffEditor.removedTextBorder: Outline color for text that got removed.
            /// </summary>
            public static readonly Unit RemovedTextBorder = new("diffEditor.removedTextBorder");

            /// <summary>
            /// diffEditor.border: Border color between the two text editors.
            /// </summary>
            public static readonly Unit Border = new("diffEditor.border");

            /// <summary>
            /// diffEditor.diagonalFill: Color of the diff editor's diagonal fill. The diagonal fill is used in side-by-side diff views.
            /// </summary>
            public static readonly Unit DiagonalFill = new("diffEditor.diagonalFill");

            /// <summary>
            /// diffEditor.insertedLineBackground: Background color for lines that got inserted. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit InsertedLineBackground = new("diffEditor.insertedLineBackground");

            /// <summary>
            /// diffEditor.removedLineBackground: Background color for lines that got removed. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit RemovedLineBackground = new("diffEditor.removedLineBackground");

            /// <summary>
            /// diffEditorGutter.insertedLineBackground: Background color for the margin where lines got inserted.
            /// </summary>
            public static readonly Unit GutterInsertedLineBackground = new("diffEditorGutter.insertedLineBackground");

            /// <summary>
            /// diffEditorGutter.removedLineBackground: Background color for the margin where lines got removed.
            /// </summary>
            public static readonly Unit GutterRemovedLineBackground = new("diffEditorGutter.removedLineBackground");

            /// <summary>
            /// diffEditorOverview.insertedForeground: Diff overview ruler foreground for inserted content.
            /// </summary>
            public static readonly Unit OverviewInsertedForeground = new("diffEditorOverview.insertedForeground");

            /// <summary>
            /// diffEditorOverview.removedForeground: Diff overview ruler foreground for removed content.
            /// </summary>
            public static readonly Unit OverviewRemovedForeground = new("diffEditorOverview.removedForeground");
        }

        /// <summary>
        /// The Editor widget is shown in front of the editor content.
        /// Examples are the Find/Replace dialog, the suggestion widget, and the editor hover.
        /// </summary>
        public static class EditorWidget
        {
            /// <summary>
            /// editorWidget.foreground: Foreground color of editor widgets, such as find/replace.
            /// </summary>
            public static readonly Unit Foreground = new("editorWidget.foreground");

            /// <summary>
            /// editorWidget.background: Background color of editor widgets, such as Find/Replace.
            /// </summary>
            public static readonly Unit Background = new("editorWidget.background");

            /// <summary>
            /// editorWidget.border: Border color of the editor widget unless the widget does not contain a border or defines its own border color.
            /// </summary>
            public static readonly Unit Border = new("editorWidget.border");

            /// <summary>
            /// editorWidget.resizeBorder: Border color of the resize bar of editor widgets. The color is only used if the widget chooses to have a resize border and if the color is not overridden by a widget.
            /// </summary>
            public static readonly Unit ResizeBorder = new("editorWidget.resizeBorder");

            /// <summary>
            /// editorSuggestWidget.background: Background color of the suggestion widget.
            /// </summary>
            public static readonly Unit SuggestBackground = new("editorSuggestWidget.background");

            /// <summary>
            /// editorSuggestWidget.border: Border color of the suggestion widget.
            /// </summary>
            public static readonly Unit SuggestBorder = new("editorSuggestWidget.border");

            /// <summary>
            /// editorSuggestWidget.foreground: Foreground color of the suggestion widget.
            /// </summary>
            public static readonly Unit SuggestForeground = new("editorSuggestWidget.foreground");

            /// <summary>
            /// editorSuggestWidget.focusHighlightForeground: Color of the match highlights in the suggest widget when an item is focused.
            /// </summary>
            public static readonly Unit SuggestFocusHighlightForeground
                = new("editorSuggestWidget.focusHighlightForeground");

            /// <summary>
            /// editorSuggestWidget.highlightForeground: Color of the match highlights in the suggestion widget.
            /// </summary>
            public static readonly Unit SuggestHighlightForeground = new("editorSuggestWidget.highlightForeground");

            /// <summary>
            /// editorSuggestWidget.selectedBackground: Background color of the selected entry in the suggestion widget.
            /// </summary>
            public static readonly Unit SuggestSelectedBackground = new("editorSuggestWidget.selectedBackground");

            /// <summary>
            /// editorSuggestWidget.selectedForeground: Foreground color of the selected entry in the suggest widget.
            /// </summary>
            public static readonly Unit SuggestSelectedForeground = new("editorSuggestWidget.selectedForeground");

            /// <summary>
            /// editorSuggestWidget.selectedIconForeground: Icon foreground color of the selected entry in the suggest widget.
            /// </summary>
            public static readonly Unit SuggestSelectedIconForeground
                = new("editorSuggestWidget.selectedIconForeground");

            /// <summary>
            /// editorSuggestWidgetStatus.foreground: Foreground color of the suggest widget status.
            /// </summary>
            public static readonly Unit SuggestStatusForeground = new("editorSuggestWidgetStatus.foreground");

            /// <summary>
            /// editorHoverWidget.foreground: Foreground color of the editor hover.
            /// </summary>
            public static readonly Unit HoverForeground = new("editorHoverWidget.foreground");

            /// <summary>
            /// editorHoverWidget.background: Background color of the editor hover.
            /// </summary>
            public static readonly Unit HoverBackground = new("editorHoverWidget.background");

            /// <summary>
            /// editorHoverWidget.border: Border color of the editor hover.
            /// </summary>
            public static readonly Unit HoverBorder = new("editorHoverWidget.border");

            /// <summary>
            /// editorHoverWidget.highlightForeground: Foreground color of the active item in the parameter hint.
            /// </summary>
            public static readonly Unit HoverHighlightForeground = new("editorHoverWidget.highlightForeground");

            /// <summary>
            /// editorHoverWidget.statusBarBackground: Background color of the editor hover status bar.
            /// </summary>
            public static readonly Unit HoverStatusBarBackground = new("editorHoverWidget.statusBarBackground");

            /// <summary>
            /// editorGhostText.border: Border color of the ghost text shown by inline completion providers and the suggest preview.
            /// </summary>
            public static readonly Unit GhostTextBorder = new("editorGhostText.border");

            /// <summary>
            /// editorGhostText.background: Background color of the ghost text in the editor.
            /// </summary>
            public static readonly Unit GhostTextBackground = new("editorGhostText.background");

            /// <summary>
            /// editorGhostText.foreground: Foreground color of the ghost text shown by inline completion providers and the suggest preview.
            /// </summary>
            public static readonly Unit GhostTextForeground = new("editorGhostText.foreground");

            /// <summary>
            /// editorStickyScroll.background: Editor sticky scroll background color
            /// </summary>
            public static readonly Unit StickyScrollBackground = new("editorStickyScroll.background");

            /// <summary>
            /// editorStickyScrollHover.background: Editor sticky scroll on hover background color
            /// </summary>
            public static readonly Unit StickyScrollHoverBackground = new("editorStickyScrollHover.background");

            /// <summary>
            /// debugExceptionWidget.background: Exception widget background color.
            /// </summary>
            public static readonly Unit ExceptionWidgetBackground = new("debugExceptionWidget.background");

            /// <summary>
            /// debugExceptionWidget.border: Exception widget border color.
            /// </summary>
            public static readonly Unit ExceptionWidgetBorder = new("debugExceptionWidget.border");

            /// <summary>
            /// editorMarkerNavigation.background: Editor marker navigation widget background.
            /// </summary>
            public static readonly Unit MarkerNavigationBackground = new("editorMarkerNavigation.background");

            /// <summary>
            /// editorMarkerNavigationError.background: Editor marker navigation widget error color.
            /// </summary>
            public static readonly Unit MarkerNavigationErrorBackground = new("editorMarkerNavigationError.background");

            /// <summary>
            /// editorMarkerNavigationWarning.background: Editor marker navigation widget warning color.
            /// </summary>
            public static readonly Unit MarkerNavigationWarningBackground
                = new("editorMarkerNavigationWarning.background");

            /// <summary>
            /// editorMarkerNavigationInfo.background: Editor marker navigation widget info color.
            /// </summary>
            public static readonly Unit MarkerNavigationInfoBackground = new("editorMarkerNavigationInfo.background");

            /// <summary>
            /// editorMarkerNavigationError.headerBackground: Editor marker navigation widget error heading background.
            /// </summary>
            public static readonly Unit MarkerNavigationErrorHeaderBackground
                = new("editorMarkerNavigationError.headerBackground");

            /// <summary>
            /// editorMarkerNavigationWarning.headerBackground: Editor marker navigation widget warning heading background.
            /// </summary>
            public static readonly Unit MarkerNavigationWarningHeaderBackground
                = new("editorMarkerNavigationWarning.headerBackground");

            /// <summary>
            /// editorMarkerNavigationInfo.headerBackground: Editor marker navigation widget info heading background.
            /// </summary>
            public static readonly Unit MarkerNavigationInfoHeaderBackground
                = new("editorMarkerNavigationInfo.headerBackground");
        }

        /// <summary>
        /// Peek views are used to show references and declarations as a view inside the editor.
        /// </summary>
        public static class PeekView
        {
            /// <summary>
            /// peekView.border: Color of the peek view borders and arrow.
            /// </summary>
            public static readonly Unit Border = new("peekView.border");

            /// <summary>
            /// peekViewEditor.background: Background color of the peek view editor.
            /// </summary>
            public static readonly Unit EditorBackground = new("peekViewEditor.background");

            /// <summary>
            /// peekViewEditorGutter.background: Background color of the gutter in the peek view editor.
            /// </summary>
            public static readonly Unit EditorGutterBackground = new("peekViewEditorGutter.background");

            /// <summary>
            /// peekViewEditor.matchHighlightBackground: Match highlight color in the peek view editor.
            /// </summary>
            public static readonly Unit EditorMatchHighlightBackground = new("peekViewEditor.matchHighlightBackground");

            /// <summary>
            /// peekViewEditor.matchHighlightBorder: Match highlight border color in the peek view editor.
            /// </summary>
            public static readonly Unit EditorMatchHighlightBorder = new("peekViewEditor.matchHighlightBorder");

            /// <summary>
            /// peekViewEditorStickyScroll.background: Background color of sticky scroll in the peek view editor
            /// </summary>
            public static readonly Unit EditorStickyScrollBackground = new("peekViewEditorStickyScroll.background");

            /// <summary>
            /// peekViewResult.background: Background color of the peek view result list.
            /// </summary>
            public static readonly Unit ResultBackground = new("peekViewResult.background");

            /// <summary>
            /// peekViewResult.fileForeground: Foreground color for file nodes in the peek view result list.
            /// </summary>
            public static readonly Unit ResultFileForeground = new("peekViewResult.fileForeground");

            /// <summary>
            /// peekViewResult.lineForeground: Foreground color for line nodes in the peek view result list.
            /// </summary>
            public static readonly Unit ResultLineForeground = new("peekViewResult.lineForeground");

            /// <summary>
            /// peekViewResult.matchHighlightBackground: Match highlight color in the peek view result list.
            /// </summary>
            public static readonly Unit ResultMatchHighlightBackground = new("peekViewResult.matchHighlightBackground");

            /// <summary>
            /// peekViewResult.selectionBackground: Background color of the selected entry in the peek view result list.
            /// </summary>
            public static readonly Unit ResultSelectionBackground = new("peekViewResult.selectionBackground");

            /// <summary>
            /// peekViewResult.selectionForeground: Foreground color of the selected entry in the peek view result list.
            /// </summary>
            public static readonly Unit ResultSelectionForeground = new("peekViewResult.selectionForeground");

            /// <summary>
            /// peekViewTitle.background: Background color of the peek view title area.
            /// </summary>
            public static readonly Unit TitleBackground = new("peekViewTitle.background");

            /// <summary>
            /// peekViewTitleDescription.foreground: Color of the peek view title info.
            /// </summary>
            public static readonly Unit TitleDescriptionForeground = new("peekViewTitleDescription.foreground");

            /// <summary>
            /// peekViewTitleLabel.foreground: Color of the peek view title.
            /// </summary>
            public static readonly Unit TitleLabelForeground = new("peekViewTitleLabel.foreground");
        }

        /// <summary>
        /// Merge conflict decorations are shown when the editor contains special diff ranges.
        /// </summary>
        public class Merge
        {
            /// <summary>
            /// merge.currentHeaderBackground: Current header background in inline merge conflicts. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit CurrentHeaderBackground = new("merge.currentHeaderBackground");

            /// <summary>
            /// merge.currentContentBackground: Current content background in inline merge conflicts. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit CurrentContentBackground = new("merge.currentContentBackground");

            /// <summary>
            /// merge.incomingHeaderBackground: Incoming header background in inline merge conflicts. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit IncomingHeaderBackground = new("merge.incomingHeaderBackground");

            /// <summary>
            /// merge.incomingContentBackground: Incoming content background in inline merge conflicts. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit IncomingContentBackground = new("merge.incomingContentBackground");

            /// <summary>
            /// merge.border: Border color on headers and the splitter in inline merge conflicts.
            /// </summary>
            public static readonly Unit Border = new("merge.border");

            /// <summary>
            /// merge.commonContentBackground: Common ancestor content background in inline merge-conflicts. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit CommonContentBackground = new("merge.commonContentBackground");

            /// <summary>
            /// merge.commonHeaderBackground: Common ancestor header background in inline merge-conflicts. The color must not be opaque so as not to hide underlying decorations.
            /// </summary>
            public static readonly Unit CommonHeaderBackground = new("merge.commonHeaderBackground");

            /// <summary>
            /// editorOverviewRuler.currentContentForeground: Current overview ruler foreground for inline merge conflicts.
            /// </summary>
            public static readonly Unit EditorOverviewRulerCurrentContentForeground
                = new("editorOverviewRuler.currentContentForeground");

            /// <summary>
            /// editorOverviewRuler.incomingContentForeground: Incoming overview ruler foreground for inline merge conflicts.
            /// </summary>
            public static readonly Unit EditorOverviewRulerIncomingContentForeground
                = new("editorOverviewRuler.incomingContentForeground");

            /// <summary>
            /// editorOverviewRuler.commonContentForeground: Common ancestor overview ruler foreground for inline merge conflicts.
            /// </summary>
            public static readonly Unit EditorOverviewRulerCommonContentForeground
                = new("editorOverviewRuler.commonContentForeground");

            /// <summary>
            /// mergeEditor.change.background: The background color for changes.
            /// </summary>
            public static readonly Unit EditorChangeBackground = new("mergeEditor.change.background");

            /// <summary>
            /// mergeEditor.change.word.background: The background color for word changes.
            /// </summary>
            public static readonly Unit EditorChangeWordBackground = new("mergeEditor.change.word.background");

            /// <summary>
            /// mergeEditor.conflict.unhandledUnfocused.border: The border color of unhandled unfocused conflicts.
            /// </summary>
            public static readonly Unit EditorConflictUnhandledUnfocusedBorder
                = new("mergeEditor.conflict.unhandledUnfocused.border");

            /// <summary>
            /// mergeEditor.conflict.unhandledFocused.border: The border color of unhandled focused conflicts.
            /// </summary>
            public static readonly Unit EditorConflictUnhandledFocusedBorder
                = new("mergeEditor.conflict.unhandledFocused.border");

            /// <summary>
            /// mergeEditor.conflict.handledUnfocused.border: The border color of handled unfocused conflicts.
            /// </summary>
            public static readonly Unit EditorConflictHandledUnfocusedBorder
                = new("mergeEditor.conflict.handledUnfocused.border");

            /// <summary>
            /// mergeEditor.conflict.handledFocused.border: The border color of handled focused conflicts.
            /// </summary>
            public static readonly Unit EditorConflictHandledFocusedBorder
                = new("mergeEditor.conflict.handledFocused.border");

            /// <summary>
            /// mergeEditor.conflict.handled.minimapOverViewRuler: The foreground color for changes in input 1.
            /// </summary>
            public static readonly Unit EditorConflictHandledMinimapOverViewRuler
                = new("mergeEditor.conflict.handled.minimapOverViewRuler");

            /// <summary>
            /// mergeEditor.conflict.unhandled.minimapOverViewRuler: The foreground color for changes in input 1.
            /// </summary>
            public static readonly Unit EditorConflictUnhandledMinimapOverViewRuler
                = new("mergeEditor.conflict.unhandled.minimapOverViewRuler");

            /// <summary>
            /// mergeEditor.conflictingLines.background: The background of the "Conflicting Lines" text.
            /// </summary>
            public static readonly Unit EditorConflictingLinesBackground
                = new("mergeEditor.conflictingLines.background");

            /// <summary>
            /// mergeEditor.changeBase.background: The background color for changes in base.
            /// </summary>
            public static readonly Unit EditorChangeBaseBackground = new("mergeEditor.changeBase.background");

            /// <summary>
            /// mergeEditor.changeBase.word.background: The background color for word changes in base.
            /// </summary>
            public static readonly Unit EditorChangeBaseWordBackground = new("mergeEditor.changeBase.word.background");

            /// <summary>
            /// mergeEditor.conflict.input1.background: The background color of decorations in input 1.
            /// </summary>
            public static readonly Unit EditorConflictInput1Background = new("mergeEditor.conflict.input1.background");

            /// <summary>
            /// mergeEditor.conflict.input2.background: The background color of decorations in input 2.
            /// </summary>
            public static readonly Unit EditorConflictInput2Background = new("mergeEditor.conflict.input2.background");
        }

        /// <summary>
        /// Panels are shown below the editor area and contain views like Output and Integrated Terminal.
        /// </summary>
        public static class Panel
        {
            /// <summary>
            /// panel.background: Panel background color.
            /// </summary>
            public static readonly Unit Background = new("panel.background");

            /// <summary>
            /// panel.border: Panel border color to separate the panel from the editor.
            /// </summary>
            public static readonly Unit Border = new("panel.border");

            /// <summary>
            /// panel.dropBorder: Drag and drop feedback color for the panel titles. Panels are shown below the editor area and contain views like output and integrated terminal.
            /// </summary>
            public static readonly Unit DropBorder = new("panel.dropBorder");

            /// <summary>
            /// panelTitle.activeBorder: Border color for the active panel title.
            /// </summary>
            public static readonly Unit TitleActiveBorder = new("panelTitle.activeBorder");

            /// <summary>
            /// panelTitle.activeForeground: Title color for the active panel.
            /// </summary>
            public static readonly Unit TitleActiveForeground = new("panelTitle.activeForeground");

            /// <summary>
            /// panelTitle.inactiveForeground: Title color for the inactive panel.
            /// </summary>
            public static readonly Unit TitleInactiveForeground = new("panelTitle.inactiveForeground");

            /// <summary>
            /// panelInput.border: Input box border for inputs in the panel.
            /// </summary>
            public static readonly Unit InputBorder = new("panelInput.border");

            /// <summary>
            /// panelSection.border: Panel section border color used when multiple views are stacked horizontally in the panel. Panels are shown below the editor area and contain views like output and integrated terminal.
            /// </summary>
            public static readonly Unit SectionBorder = new("panelSection.border");

            /// <summary>
            /// panelSection.dropBackground: Drag and drop feedback color for the panel sections. The color should have transparency so that the panel sections can still shine through. Panels are shown below the editor area and contain views like output and integrated terminal.
            /// </summary>
            public static readonly Unit SectionDropBackground = new("panelSection.dropBackground");

            /// <summary>
            /// panelSectionHeader.background: Panel section header background color. Panels are shown below the editor area and contain views like output and integrated terminal.
            /// </summary>
            public static readonly Unit SectionHeaderBackground = new("panelSectionHeader.background");

            /// <summary>
            /// panelSectionHeader.foreground: Panel section header foreground color. Panels are shown below the editor area and contain views like output and integrated terminal.
            /// </summary>
            public static readonly Unit SectionHeaderForeground = new("panelSectionHeader.foreground");

            /// <summary>
            /// panelSectionHeader.border: Panel section header border color used when multiple views are stacked vertically in the panel. Panels are shown below the editor area and contain views like output and integrated terminal.
            /// </summary>
            public static readonly Unit SectionHeaderBorder = new("panelSectionHeader.border");
        }

        /// <summary>
        /// The Status Bar is shown in the bottom of the workbench.
        /// </summary>
        public static class StatusBar
        {
            /// <summary>
            /// statusBar.background: Standard Status Bar background color.
            /// </summary>
            public static readonly Unit Background = new("statusBar.background");

            /// <summary>
            /// statusBar.foreground: Status Bar foreground color.
            /// </summary>
            public static readonly Unit Foreground = new("statusBar.foreground");

            /// <summary>
            /// statusBar.border: Status Bar border color separating the Status Bar and editor.
            /// </summary>
            public static readonly Unit Border = new("statusBar.border");

            /// <summary>
            /// statusBar.debuggingBackground: Status Bar background color when a program is being debugged.
            /// </summary>
            public static readonly Unit DebuggingBackground = new("statusBar.debuggingBackground");

            /// <summary>
            /// statusBar.debuggingForeground: Status Bar foreground color when a program is being debugged.
            /// </summary>
            public static readonly Unit DebuggingForeground = new("statusBar.debuggingForeground");

            /// <summary>
            /// statusBar.debuggingBorder: Status Bar border color separating the Status Bar and editor when a program is being debugged.
            /// </summary>
            public static readonly Unit DebuggingBorder = new("statusBar.debuggingBorder");

            /// <summary>
            /// statusBar.noFolderForeground: Status Bar foreground color when no folder is opened.
            /// </summary>
            public static readonly Unit NoFolderForeground = new("statusBar.noFolderForeground");

            /// <summary>
            /// statusBar.noFolderBackground: Status Bar background color when no folder is opened.
            /// </summary>
            public static readonly Unit NoFolderBackground = new("statusBar.noFolderBackground");

            /// <summary>
            /// statusBar.noFolderBorder: Status Bar border color separating the Status Bar and editor when no folder is opened.
            /// </summary>
            public static readonly Unit NoFolderBorder = new("statusBar.noFolderBorder");

            /// <summary>
            /// statusBarItem.activeBackground: Status Bar item background color when clicking.
            /// </summary>
            public static readonly Unit ItemActiveBackground = new("statusBarItem.activeBackground");

            /// <summary>
            /// statusBarItem.hoverBackground: Status Bar item background color when hovering.
            /// </summary>
            public static readonly Unit ItemHoverBackground = new("statusBarItem.hoverBackground");

            /// <summary>
            /// statusBarItem.prominentForeground: Status Bar prominent items foreground color.
            /// </summary>
            public static readonly Unit ItemProminentForeground = new("statusBarItem.prominentForeground");

            /// <summary>
            /// statusBarItem.prominentBackground: Status Bar prominent items background color.
            /// </summary>
            public static readonly Unit ItemProminentBackground = new("statusBarItem.prominentBackground");

            /// <summary>
            /// statusBarItem.prominentHoverBackground: Status Bar prominent items background color when hovering.
            /// </summary>
            public static readonly Unit ItemProminentHoverBackground = new("statusBarItem.prominentHoverBackground");

            /// <summary>
            /// statusBarItem.remoteBackground: Background color for the remote indicator on the status bar.
            /// </summary>
            public static readonly Unit ItemRemoteBackground = new("statusBarItem.remoteBackground");

            /// <summary>
            /// statusBarItem.remoteForeground: Foreground color for the remote indicator on the status bar.
            /// </summary>
            public static readonly Unit ItemRemoteForeground = new("statusBarItem.remoteForeground");

            /// <summary>
            /// statusBarItem.errorBackground: Status bar error items background color. Error items stand out from other status bar entries to indicate error conditions.
            /// </summary>
            public static readonly Unit ItemErrorBackground = new("statusBarItem.errorBackground");

            /// <summary>
            /// statusBarItem.errorForeground: Status bar error items foreground color. Error items stand out from other status bar entries to indicate error conditions.
            /// </summary>
            public static readonly Unit ItemErrorForeground = new("statusBarItem.errorForeground");

            /// <summary>
            /// statusBarItem.warningBackground: Status bar warning items background color. Warning items stand out from other status bar entries to indicate warning conditions. The status bar is shown in the bottom of the window.
            /// </summary>
            public static readonly Unit ItemWarningBackground = new("statusBarItem.warningBackground");

            /// <summary>
            /// statusBarItem.warningForeground: Status bar warning items foreground color. Warning items stand out from other status bar entries to indicate warning conditions. The status bar is shown in the bottom of the window.
            /// </summary>
            public static readonly Unit ItemWarningForeground = new("statusBarItem.warningForeground");

            /// <summary>
            /// statusBarItem.compactHoverBackground: Status bar item background color when hovering an item that contains two hovers. The status bar is shown in the bottom of the window.
            /// </summary>
            public static readonly Unit ItemCompactHoverBackground = new("statusBarItem.compactHoverBackground");

            /// <summary>
            /// statusBarItem.focusBorder: Status bar item border color when focused on keyboard navigation. The status bar is shown in the bottom of the window.
            /// </summary>
            public static readonly Unit ItemFocusBorder = new("statusBarItem.focusBorder");

            /// <summary>
            /// statusBar.focusBorder: Status bar border color when focused on keyboard navigation. The status bar is shown in the bottom of the window.
            /// </summary>
            public static readonly Unit FocusBorder = new("statusBar.focusBorder");
        }

        /// <summary>
        ///
        /// </summary>
        public static class TitleBar
        {
            /// <summary>
            /// titleBar.activeBackground: Title Bar background when the window is active.
            /// </summary>
            public static readonly Unit ActiveBackground = new("titleBar.activeBackground");

            /// <summary>
            /// titleBar.activeForeground: Title Bar foreground when the window is active.
            /// </summary>
            public static readonly Unit ActiveForeground = new("titleBar.activeForeground");

            /// <summary>
            /// titleBar.inactiveBackground: Title Bar background when the window is inactive.
            /// </summary>
            public static readonly Unit InactiveBackground = new("titleBar.inactiveBackground");

            /// <summary>
            /// titleBar.inactiveForeground: Title Bar foreground when the window is inactive.
            /// </summary>
            public static readonly Unit InactiveForeground = new("titleBar.inactiveForeground");

            /// <summary>
            /// titleBar.border: Title bar border color.
            /// </summary>
            public static readonly Unit Border = new("titleBar.border");
        }

        /// <summary>
        ///
        /// </summary>
        public static class MenuBar
        {
            /// <summary>
            /// menubar.selectionForeground: Foreground color of the selected menu item in the menubar.
            /// </summary>
            public static readonly Unit SelectionForeground = new("menubar.selectionForeground");

            /// <summary>
            /// menubar.selectionBackground: Background color of the selected menu item in the menubar.
            /// </summary>
            public static readonly Unit SelectionBackground = new("menubar.selectionBackground");

            /// <summary>
            /// menubar.selectionBorder: Border color of the selected menu item in the menubar.
            /// </summary>
            public static readonly Unit SelectionBorder = new("menubar.selectionBorder");

            /// <summary>
            /// menu.foreground: Foreground color of menu items.
            /// </summary>
            public static readonly Unit ItemForeground = new("menu.foreground");

            /// <summary>
            /// menu.background: Background color of menu items.
            /// </summary>
            public static readonly Unit ItemBackground = new("menu.background");

            /// <summary>
            /// menu.selectionForeground: Foreground color of the selected menu item in menus.
            /// </summary>
            public static readonly Unit ItemSelectionForeground = new("menu.selectionForeground");

            /// <summary>
            /// menu.selectionBackground: Background color of the selected menu item in menus.
            /// </summary>
            public static readonly Unit ItemSelectionBackground = new("menu.selectionBackground");

            /// <summary>
            /// menu.selectionBorder: Border color of the selected menu item in menus.
            /// </summary>
            public static readonly Unit ItemSelectionBorder = new("menu.selectionBorder");

            /// <summary>
            /// menu.separatorBackground: Color of a separator menu item in menus.
            /// </summary>
            public static readonly Unit ItemSeparatorBackground = new("menu.separatorBackground");

            /// <summary>
            /// menu.border: Border color of menus.
            /// </summary>
            public static readonly Unit ItemBorder = new("menu.border");
        }

        /// <summary>
        ///
        /// </summary>
        public static class CommandCenter
        {
            /// <summary>
            /// commandCenter.foreground: Foreground color of the Command Center.
            /// </summary>
            public static readonly Unit Foreground = new("commandCenter.foreground");

            /// <summary>
            /// commandCenter.activeForeground: Active foreground color of the Command Center.
            /// </summary>
            public static readonly Unit ActiveForeground = new("commandCenter.activeForeground");

            /// <summary>
            /// commandCenter.background: Background color of the Command Center.
            /// </summary>
            public static readonly Unit Background = new("commandCenter.background");

            /// <summary>
            /// commandCenter.activeBackground: Active background color of the Command Center.
            /// </summary>
            public static readonly Unit ActiveBackground = new("commandCenter.activeBackground");

            /// <summary>
            /// commandCenter.border: Border color of the Command Center.
            /// </summary>
            public static readonly Unit Border = new("commandCenter.border");

            /// <summary>
            /// commandCenter.inactiveForeground: Foreground color of the Command Center when the window is inactive.
            /// </summary>
            public static readonly Unit InactiveForeground = new("commandCenter.inactiveForeground");

            /// <summary>
            /// commandCenter.inactiveBorder: Border color of the Command Center when the window is inactive.
            /// </summary>
            public static readonly Unit InactiveBorder = new("commandCenter.inactiveBorder");

            /// <summary>
            /// commandCenter.activeBorder: Active border color of the command center.
            /// </summary>
            public static readonly Unit ActiveBorder = new("commandCenter.activeBorder");
        }

        /// <summary>
        /// Notification toasts slide up from the bottom-right of the workbench.
        /// </summary>
        public static class Notification
        {
            /// <summary>
            /// notificationCenter.border: Notification Center border color.
            /// </summary>
            public static readonly Unit CenterBorder = new("notificationCenter.border");

            /// <summary>
            /// notificationCenterHeader.foreground: Notification Center header foreground color.
            /// </summary>
            public static readonly Unit CenterHeaderForeground = new("notificationCenterHeader.foreground");

            /// <summary>
            /// notificationCenterHeader.background: Notification Center header background color.
            /// </summary>
            public static readonly Unit CenterHeaderBackground = new("notificationCenterHeader.background");

            /// <summary>
            /// notificationToast.border: Notification toast border color.
            /// </summary>
            public static readonly Unit ToastBorder = new("notificationToast.border");

            /// <summary>
            /// notifications.foreground: Notification foreground color.
            /// </summary>
            public static readonly Unit Foreground = new("notifications.foreground");

            /// <summary>
            /// notifications.background: Notification background color.
            /// </summary>
            public static readonly Unit Background = new("notifications.background");

            /// <summary>
            /// notifications.border: Notification border color separating from other notifications in the Notification Center.
            /// </summary>
            public static readonly Unit Border = new("notifications.border");

            /// <summary>
            /// notificationLink.foreground: Notification links foreground color.
            /// </summary>
            public static readonly Unit LinkForeground = new("notificationLink.foreground");

            /// <summary>
            /// notificationsErrorIcon.foreground: The color used for the notification error icon.
            /// </summary>
            public static readonly Unit ErrorIconForeground = new("notificationsErrorIcon.foreground");

            /// <summary>
            /// notificationsWarningIcon.foreground: The color used for the notification warning icon.
            /// </summary>
            public static readonly Unit WarningIconForeground = new("notificationsWarningIcon.foreground");

            /// <summary>
            /// notificationsInfoIcon.foreground: The color used for the notification info icon.
            /// </summary>
            public static readonly Unit InfoIconForeground = new("notificationsInfoIcon.foreground");
        }

        /// <summary>
        /// The banner appears below the title bar and spans the entire width of the workbench when visible.
        /// </summary>
        public static class Banner
        {
            /// <summary>
            /// banner.background: Banner background color.
            /// </summary>
            public static readonly Unit Background = new("banner.background");

            /// <summary>
            /// banner.foreground: Banner foreground color.
            /// </summary>
            public static readonly Unit Foreground = new("banner.foreground");

            /// <summary>
            /// banner.iconForeground: Color for the icon in front of the banner text.
            /// </summary>
            public static readonly Unit IconForeground = new("banner.iconForeground");
        }

        /// <summary>
        ///
        /// </summary>
        public static class Extension
        {
            /// <summary>
            /// extensionButton.prominentForeground: Extension view button foreground color (for example **Install** button).
            /// </summary>
            public static readonly Unit ButtonProminentForeground = new("extensionButton.prominentForeground");

            /// <summary>
            /// extensionButton.prominentBackground: Extension view button background color.
            /// </summary>
            public static readonly Unit ButtonProminentBackground = new("extensionButton.prominentBackground");

            /// <summary>
            /// extensionButton.prominentHoverBackground: Extension view button background hover color.
            /// </summary>
            public static readonly Unit ButtonProminentHoverBackground
                = new("extensionButton.prominentHoverBackground");

            /// <summary>
            /// extensionButton.background: Button background color for extension actions.
            /// </summary>
            public static readonly Unit ButtonBackground = new("extensionButton.background");

            /// <summary>
            /// extensionButton.foreground: Button foreground color for extension actions.
            /// </summary>
            public static readonly Unit ButtonForeground = new("extensionButton.foreground");

            /// <summary>
            /// extensionButton.hoverBackground: Button background hover color for extension actions.
            /// </summary>
            public static readonly Unit ButtonHoverBackground = new("extensionButton.hoverBackground");

            /// <summary>
            /// extensionButton.separator: Button separator color for extension actions.
            /// </summary>
            public static readonly Unit ButtonSeparator = new("extensionButton.separator");

            /// <summary>
            /// extensionBadge.remoteBackground: Background color for the remote badge in the extensions view.
            /// </summary>
            public static readonly Unit BadgeRemoteBackground = new("extensionBadge.remoteBackground");

            /// <summary>
            /// extensionBadge.remoteForeground: Foreground color for the remote badge in the extensions view.
            /// </summary>
            public static readonly Unit BadgeRemoteForeground = new("extensionBadge.remoteForeground");

            /// <summary>
            /// extensionIcon.starForeground: The icon color for extension ratings.
            /// </summary>
            public static readonly Unit IconStarForeground = new("extensionIcon.starForeground");

            /// <summary>
            /// extensionIcon.verifiedForeground: The icon color for extension verified publisher.
            /// </summary>
            public static readonly Unit IconVerifiedForeground = new("extensionIcon.verifiedForeground");

            /// <summary>
            /// extensionIcon.preReleaseForeground: The icon color for pre-release extension.
            /// </summary>
            public static readonly Unit IconPreReleaseForeground = new("extensionIcon.preReleaseForeground");

            /// <summary>
            /// extensionIcon.sponsorForeground: The icon color for extension sponsor.
            /// </summary>
            public static readonly Unit IconSponsorForeground = new("extensionIcon.sponsorForeground");
        }

        /// <summary>
        ///
        /// </summary>
        public static class QuickPicker
        {
            /// <summary>
            /// pickerGroup.border: Quick picker (Quick Open) color for grouping borders.
            /// </summary>
            public static readonly Unit PickerGroupBorder = new("pickerGroup.border");

            /// <summary>
            /// pickerGroup.foreground: Quick picker (Quick Open) color for grouping labels.
            /// </summary>
            public static readonly Unit PickerGroupForeground = new("pickerGroup.foreground");

            /// <summary>
            /// quickInput.background: Quick input background color. The quick input widget is the container for views like the color theme picker.
            /// </summary>
            public static readonly Unit QuickInputBackground = new("quickInput.background");

            /// <summary>
            /// quickInput.foreground: Quick input foreground color. The quick input widget is the container for views like the color theme picker.
            /// </summary>
            public static readonly Unit QuickInputForeground = new("quickInput.foreground");

            /// <summary>
            /// quickInputList.focusBackground: Quick picker background color for the focused item.
            /// </summary>
            public static readonly Unit QuickInputListFocusBackground = new("quickInputList.focusBackground");

            /// <summary>
            /// quickInputList.focusForeground: Quick picker foreground color for the focused item.
            /// </summary>
            public static readonly Unit QuickInputListFocusForeground = new("quickInputList.focusForeground");

            /// <summary>
            /// quickInputList.focusIconForeground: Quick picker icon foreground color for the focused item.
            /// </summary>
            public static readonly Unit QuickInputListFocusIconForeground = new("quickInputList.focusIconForeground");

            /// <summary>
            /// quickInputTitle.background: Quick picker title background color. The quick picker widget is the container for pickers like the Command Palette.
            /// </summary>
            public static readonly Unit QuickInputTitleBackground = new("quickInputTitle.background");
        }

        /// <summary>
        /// Keybinding labels are shown when there is a keybinding associated with a command. An example of the keybinding label can be seen in the Command Palette:
        /// </summary>
        public static class KeybindingLabel
        {
            /// <summary>
            /// keybindingLabel.background: Keybinding label background color. The keybinding label is used to represent a keyboard shortcut.
            /// </summary>
            public static readonly Unit Background = new("keybindingLabel.background");

            /// <summary>
            /// keybindingLabel.foreground: Keybinding label foreground color. The keybinding label is used to represent a keyboard shortcut.
            /// </summary>
            public static readonly Unit Foreground = new("keybindingLabel.foreground");

            /// <summary>
            /// keybindingLabel.border: Keybinding label border color. The keybinding label is used to represent a keyboard shortcut.
            /// </summary>
            public static readonly Unit Border = new("keybindingLabel.border");

            /// <summary>
            /// keybindingLabel.bottomBorder: Keybinding label border bottom color. The keybinding label is used to represent a keyboard shortcut.
            /// </summary>
            public static readonly Unit BottomBorder = new("keybindingLabel.bottomBorder");
        }

        /// <summary>
        ///
        /// </summary>
        public static class KeybindingTable
        {
            /// <summary>
            /// keybindingTable.headerBackground: Background color for the keyboard shortcuts table header.
            /// </summary>
            public static readonly Unit HeaderBackground = new("keybindingTable.headerBackground");

            /// <summary>
            /// keybindingTable.rowsBackground: Background color for the keyboard shortcuts table alternating rows.
            /// </summary>
            public static readonly Unit RowsBackground = new("keybindingTable.rowsBackground");
        }

        /// <summary>
        ///
        /// </summary>
        public static class Terminal
        {
            /// <summary>
            /// terminal.background: The background of the Integrated Terminal's viewport.
            /// </summary>
            public static readonly Unit Background = new("terminal.background");

            /// <summary>
            /// terminal.border: The color of the border that separates split panes within the terminal. This defaults to panel.border.
            /// </summary>
            public static readonly Unit Border = new("terminal.border");

            /// <summary>
            /// terminal.foreground: The default foreground color of the Integrated Terminal.
            /// </summary>
            public static readonly Unit Foreground = new("terminal.foreground");

            /// <summary>
            /// terminal.ansiBlack: 'Black' ANSI color in the terminal.
            /// </summary>
            public static readonly Unit AnsiBlack = new("terminal.ansiBlack");

            /// <summary>
            /// terminal.ansiBlue: 'Blue' ANSI color in the terminal.
            /// </summary>
            public static readonly Unit AnsiBlue = new("terminal.ansiBlue");

            /// <summary>
            /// terminal.ansiBrightBlack: 'BrightBlack' ANSI color in the terminal.
            /// </summary>
            public static readonly Unit AnsiBrightBlack = new("terminal.ansiBrightBlack");

            /// <summary>
            /// terminal.ansiBrightBlue: 'BrightBlue' ANSI color in the terminal.
            /// </summary>
            public static readonly Unit AnsiBrightBlue = new("terminal.ansiBrightBlue");

            /// <summary>
            /// terminal.ansiBrightCyan: 'BrightCyan' ANSI color in the terminal.
            /// </summary>
            public static readonly Unit AnsiBrightCyan = new("terminal.ansiBrightCyan");

            /// <summary>
            /// terminal.ansiBrightGreen: 'BrightGreen' ANSI color in the terminal.
            /// </summary>
            public static readonly Unit AnsiBrightGreen = new("terminal.ansiBrightGreen");

            /// <summary>
            /// terminal.ansiBrightMagenta: 'BrightMagenta' ANSI color in the terminal.
            /// </summary>
            public static readonly Unit AnsiBrightMagenta = new("terminal.ansiBrightMagenta");

            /// <summary>
            /// terminal.ansiBrightRed: 'BrightRed' ANSI color in the terminal.
            /// </summary>
            public static readonly Unit AnsiBrightRed = new("terminal.ansiBrightRed");

            /// <summary>
            /// terminal.ansiBrightWhite: 'BrightWhite' ANSI color in the terminal.
            /// </summary>
            public static readonly Unit AnsiBrightWhite = new("terminal.ansiBrightWhite");

            /// <summary>
            /// terminal.ansiBrightYellow: 'BrightYellow' ANSI color in the terminal.
            /// </summary>
            public static readonly Unit AnsiBrightYellow = new("terminal.ansiBrightYellow");

            /// <summary>
            /// terminal.ansiCyan: 'Cyan' ANSI color in the terminal.
            /// </summary>
            public static readonly Unit AnsiCyan = new("terminal.ansiCyan");

            /// <summary>
            /// terminal.ansiGreen: 'Green' ANSI color in the terminal.
            /// </summary>
            public static readonly Unit AnsiGreen = new("terminal.ansiGreen");

            /// <summary>
            /// terminal.ansiMagenta: 'Magenta' ANSI color in the terminal.
            /// </summary>
            public static readonly Unit AnsiMagenta = new("terminal.ansiMagenta");

            /// <summary>
            /// terminal.ansiRed: 'Red' ANSI color in the terminal.
            /// </summary>
            public static readonly Unit AnsiRed = new("terminal.ansiRed");

            /// <summary>
            /// terminal.ansiWhite: 'White' ANSI color in the terminal.
            /// </summary>
            public static readonly Unit AnsiWhite = new("terminal.ansiWhite");

            /// <summary>
            /// terminal.ansiYellow: 'Yellow' ANSI color in the terminal.
            /// </summary>
            public static readonly Unit AnsiYellow = new("terminal.ansiYellow");

            /// <summary>
            /// terminal.selectionBackground: The selection background color of the terminal.
            /// </summary>
            public static readonly Unit SelectionBackground = new("terminal.selectionBackground");

            /// <summary>
            /// terminal.selectionForeground: The selection foreground color of the terminal. When this is null the selection foreground will be retained and have the minimum contrast ratio feature applied.
            /// </summary>
            public static readonly Unit SelectionForeground = new("terminal.selectionForeground");

            /// <summary>
            /// terminal.inactiveSelectionBackground: The selection background color of the terminal when it does not have focus.
            /// </summary>
            public static readonly Unit InactiveSelectionBackground = new("terminal.inactiveSelectionBackground");

            /// <summary>
            /// terminal.findMatchBackground: Color of the current search match in the terminal. The color must not be opaque so as not to hide underlying terminal content.
            /// </summary>
            public static readonly Unit FindMatchBackground = new("terminal.findMatchBackground");

            /// <summary>
            /// terminal.findMatchBorder: Border color of the current search match in the terminal.
            /// </summary>
            public static readonly Unit FindMatchBorder = new("terminal.findMatchBorder");

            /// <summary>
            /// terminal.findMatchHighlightBackground: Color of the other search matches in the terminal. The color must not be opaque so as not to hide underlying terminal content.
            /// </summary>
            public static readonly Unit FindMatchHighlightBackground = new("terminal.findMatchHighlightBackground");

            /// <summary>
            /// terminal.findMatchHighlightBorder: Border color of the other search matches in the terminal.
            /// </summary>
            public static readonly Unit FindMatchHighlightBorder = new("terminal.findMatchHighlightBorder");

            /// <summary>
            /// terminalCursor.background: The background color of the terminal cursor. Allows customizing the color of a character overlapped by a block cursor.
            /// </summary>
            public static readonly Unit CursorBackground = new("terminalCursor.background");

            /// <summary>
            /// terminalCursor.foreground: The foreground color of the terminal cursor.
            /// </summary>
            public static readonly Unit CursorForeground = new("terminalCursor.foreground");

            /// <summary>
            /// terminal.dropBackground: The background color when dragging on top of terminals. The color should have transparency so that the terminal contents can still shine through.
            /// </summary>
            public static readonly Unit DropBackground = new("terminal.dropBackground");

            /// <summary>
            /// terminal.tab.activeBorder: Border on the side of the terminal tab in the panel. This defaults to `tab.activeBorder`.
            /// </summary>
            public static readonly Unit TabActiveBorder = new("terminal.tab.activeBorder");

            /// <summary>
            /// terminalCommandDecoration.defaultBackground: The default terminal command decoration background color.
            /// </summary>
            public static readonly Unit DefaultBackground = new("terminalCommandDecoration.defaultBackground");

            /// <summary>
            /// terminalCommandDecoration.successBackground: The terminal command decoration background color for successful commands.
            /// </summary>
            public static readonly Unit CommandDecorationSuccessBackground
                = new("terminalCommandDecoration.successBackground");

            /// <summary>
            /// terminalCommandDecoration.errorBackground: The terminal command decoration background color for error commands.
            /// </summary>
            public static readonly Unit CommandDecorationErrorBackground
                = new("terminalCommandDecoration.errorBackground");

            /// <summary>
            /// terminalOverviewRuler.cursorForeground: The overview ruler cursor color.
            /// </summary>
            public static readonly Unit OverviewRulerCursorForeground = new("terminalOverviewRuler.cursorForeground");

            /// <summary>
            /// terminalOverviewRuler.findMatchForeground: Overview ruler marker color for find matches in the terminal.
            /// </summary>
            public static readonly Unit OverviewRulerFindMatchForeground
                = new("terminalOverviewRuler.findMatchForeground");
        }

        /// <summary>
        ///
        /// </summary>
        public static class Debug
        {
            /// <summary>
            /// debugToolBar.background: Debug toolbar background color.
            /// </summary>
            public static readonly Unit ToolBarBackground = new("debugToolBar.background");

            /// <summary>
            /// debugToolBar.border: Debug toolbar border color.
            /// </summary>
            public static readonly Unit ToolBarBorder = new("debugToolBar.border");

            /// <summary>
            /// editor.stackFrameHighlightBackground: Background color of the top stack frame highlight in the editor.
            /// </summary>
            public static readonly Unit EditorStackFrameHighlightBackground
                = new("editor.stackFrameHighlightBackground");

            /// <summary>
            /// editor.focusedStackFrameHighlightBackground: Background color of the focused stack frame highlight in the editor.
            /// </summary>
            public static readonly Unit EditorFocusedStackFrameHighlightBackground
                = new("editor.focusedStackFrameHighlightBackground");

            /// <summary>
            /// editor.inlineValuesForeground: Color for the debug inline value text.
            /// </summary>
            public static readonly Unit EditorInlineValuesForeground = new("editor.inlineValuesForeground");

            /// <summary>
            /// editor.inlineValuesBackground: Color for the debug inline value background.
            /// </summary>
            public static readonly Unit EditorInlineValuesBackground = new("editor.inlineValuesBackground");

            /// <summary>
            /// debugView.exceptionLabelForeground: Foreground color for a label shown in the CALL STACK view when the debugger breaks on an exception
            /// </summary>
            public static readonly Unit ViewExceptionLabelForeground = new("debugView.exceptionLabelForeground");

            /// <summary>
            /// debugView.exceptionLabelBackground: Background color for a label shown in the CALL STACK view when the debugger breaks on an exception
            /// </summary>
            public static readonly Unit ViewExceptionLabelBackground = new("debugView.exceptionLabelBackground");

            /// <summary>
            /// debugView.stateLabelForeground: Foreground color for a label in the CALL STACK view showing the current session's or thread's state
            /// </summary>
            public static readonly Unit ViewStateLabelForeground = new("debugView.stateLabelForeground");

            /// <summary>
            /// debugView.stateLabelBackground: Background color for a label in the CALL STACK view showing the current session's or thread's state
            /// </summary>
            public static readonly Unit ViewStateLabelBackground = new("debugView.stateLabelBackground");

            /// <summary>
            /// debugView.valueChangedHighlight: Color used to highlight value changes in the debug views (ie. in the Variables view)
            /// </summary>
            public static readonly Unit ViewValueChangedHighlight = new("debugView.valueChangedHighlight");

            /// <summary>
            /// debugTokenExpression.name: Foreground color for the token names shown in debug views (ie. the Variables or Watch view)
            /// </summary>
            public static readonly Unit TokenExpressionName = new("debugTokenExpression.name");

            /// <summary>
            /// debugTokenExpression.value: Foreground color for the token values shown in debug views
            /// </summary>
            public static readonly Unit TokenExpressionValue = new("debugTokenExpression.value");

            /// <summary>
            /// debugTokenExpression.string: Foreground color for strings in debug views
            /// </summary>
            public static readonly Unit TokenExpressionString = new("debugTokenExpression.string");

            /// <summary>
            /// debugTokenExpression.boolean: Foreground color for booleans in debug views
            /// </summary>
            public static readonly Unit TokenExpressionBoolean = new("debugTokenExpression.boolean");

            /// <summary>
            /// debugTokenExpression.number: Foreground color for numbers in debug views
            /// </summary>
            public static readonly Unit TokenExpressionNumber = new("debugTokenExpression.number");

            /// <summary>
            /// debugTokenExpression.error: Foreground color for expression errors in debug views
            /// </summary>
            public static readonly Unit TokenExpressionError = new("debugTokenExpression.error");
        }

        /// <summary>
        ///
        /// </summary>
        public static class Testing
        {
            /// <summary>
            /// testing.iconFailed: Color for the 'failed' icon in the test explorer.
            /// </summary>
            public static readonly Unit IconFailed = new("testing.iconFailed");

            /// <summary>
            /// testing.iconErrored: Color for the 'Errored' icon in the test explorer.
            /// </summary>
            public static readonly Unit IconErrored = new("testing.iconErrored");

            /// <summary>
            /// testing.iconPassed: Color for the 'passed' icon in the test explorer.
            /// </summary>
            public static readonly Unit IconPassed = new("testing.iconPassed");

            /// <summary>
            /// testing.runAction: Color for 'run' icons in the editor.
            /// </summary>
            public static readonly Unit RunAction = new("testing.runAction");

            /// <summary>
            /// testing.iconQueued: Color for the 'Queued' icon in the test explorer.
            /// </summary>
            public static readonly Unit IconQueued = new("testing.iconQueued");

            /// <summary>
            /// testing.iconUnset: Color for the 'Unset' icon in the test explorer.
            /// </summary>
            public static readonly Unit IconUnset = new("testing.iconUnset");

            /// <summary>
            /// testing.iconSkipped: Color for the 'Skipped' icon in the test explorer.
            /// </summary>
            public static readonly Unit IconSkipped = new("testing.iconSkipped");

            /// <summary>
            /// testing.peekBorder: Color of the peek view borders and arrow.
            /// </summary>
            public static readonly Unit PeekBorder = new("testing.peekBorder");

            /// <summary>
            /// testing.peekHeaderBackground: Color of the peek view borders and arrow.
            /// </summary>
            public static readonly Unit PeekHeaderBackground = new("testing.peekHeaderBackground");

            /// <summary>
            /// testing.message.error.decorationForeground: Text color of test error messages shown inline in the editor.
            /// </summary>
            public static readonly Unit MessageErrorDecorationForeground
                = new("testing.message.error.decorationForeground");

            /// <summary>
            /// testing.message.error.lineBackground: Margin color beside error messages shown inline in the editor.
            /// </summary>
            public static readonly Unit MessageErrorLineBackground = new("testing.message.error.lineBackground");

            /// <summary>
            /// testing.message.info.decorationForeground: Text color of test info messages shown inline in the editor.
            /// </summary>
            public static readonly Unit MessageInfoDecorationForeground
                = new("testing.message.info.decorationForeground");

            /// <summary>
            /// testing.message.info.lineBackground: Margin color beside info messages shown inline in the editor.
            /// </summary>
            public static readonly Unit MessageInfoLineBackground = new("testing.message.info.lineBackground");
        }

        /// <summary>
        ///
        /// </summary>
        public static class WelcomePage
        {
            /// <summary>
            /// welcomePage.background: Background color for the Welcome page.
            /// </summary>
            public static readonly Unit Background = new("welcomePage.background");

            /// <summary>
            /// welcomePage.progress.background: Foreground color for the Welcome page progress bars.
            /// </summary>
            public static readonly Unit ProgressBackground = new("welcomePage.progress.background");

            /// <summary>
            /// welcomePage.progress.foreground: Background color for the Welcome page progress bars.
            /// </summary>
            public static readonly Unit ProgressForeground = new("welcomePage.progress.foreground");

            /// <summary>
            /// welcomePage.tileBackground: Background color for the tiles on the Get Started page.
            /// </summary>
            public static readonly Unit TileBackground = new("welcomePage.tileBackground");

            /// <summary>
            /// welcomePage.tileHoverBackground: Hover background color for the tiles on the Get Started.
            /// </summary>
            public static readonly Unit TileHoverBackground = new("welcomePage.tileHoverBackground");

            /// <summary>
            /// welcomePage.tileBorder: Border color for the tiles on the Get Started page.
            /// </summary>
            public static readonly Unit TileBorder = new("welcomePage.tileBorder");

            /// <summary>
            /// walkThrough.embeddedEditorBackground: Background color for the embedded editors on the Interactive Playground.
            /// </summary>
            public static readonly Unit WalkThroughEmbeddedEditorBackground
                = new("walkThrough.embeddedEditorBackground");

            /// <summary>
            /// walkthrough.stepTitle.foreground: Foreground color of the heading of each walkthrough step.
            /// </summary>
            public static readonly Unit WalkthroughStepTitleForeground = new("walkthrough.stepTitle.foreground");

            /// <summary>
            /// welcomeOverlay.background: welcomeOverlay Background color.
            /// </summary>
            public static readonly Unit WelcomeOverlayBackground = new("welcomeOverlay.background");
        }

        /// <summary>
        ///
        /// </summary>
        public static class SourceControl
        {
            /// <summary>
            /// scm.providerBorder: SCM Provider separator border.
            /// </summary>
            public static readonly Unit ScmProviderBorder = new("scm.providerBorder");
        }

        /// <summary>
        ///
        /// </summary>
        public static class GitDecoration
        {
            /// <summary>
            /// gitDecoration.addedResourceForeground: Color for added Git resources. Used for file labels and the SCM viewlet.
            /// </summary>
            public static readonly Unit AddedResourceForeground = new("gitDecoration.addedResourceForeground");

            /// <summary>
            /// gitDecoration.modifiedResourceForeground: Color for modified Git resources. Used for file labels and the SCM viewlet.
            /// </summary>
            public static readonly Unit ModifiedResourceForeground = new("gitDecoration.modifiedResourceForeground");

            /// <summary>
            /// gitDecoration.deletedResourceForeground: Color for deleted Git resources. Used for file labels and the SCM viewlet.
            /// </summary>
            public static readonly Unit DeletedResourceForeground = new("gitDecoration.deletedResourceForeground");

            /// <summary>
            /// gitDecoration.renamedResourceForeground: Color for renamed or copied Git resources. Used for file labels and the SCM viewlet.
            /// </summary>
            public static readonly Unit RenamedResourceForeground = new("gitDecoration.renamedResourceForeground");

            /// <summary>
            /// gitDecoration.stageModifiedResourceForeground: Color for staged modifications git decorations.  Used for file labels and the SCM viewlet.
            /// </summary>
            public static readonly Unit StageModifiedResourceForeground
                = new("gitDecoration.stageModifiedResourceForeground");

            /// <summary>
            /// gitDecoration.stageDeletedResourceForeground: Color for staged deletions git decorations.  Used for file labels and the SCM viewlet.
            /// </summary>
            public static readonly Unit StageDeletedResourceForeground
                = new("gitDecoration.stageDeletedResourceForeground");

            /// <summary>
            /// gitDecoration.untrackedResourceForeground: Color for untracked Git resources. Used for file labels and the SCM viewlet.
            /// </summary>
            public static readonly Unit UntrackedResourceForeground = new("gitDecoration.untrackedResourceForeground");

            /// <summary>
            /// gitDecoration.ignoredResourceForeground: Color for ignored Git resources. Used for file labels and the SCM viewlet.
            /// </summary>
            public static readonly Unit IgnoredResourceForeground = new("gitDecoration.ignoredResourceForeground");

            /// <summary>
            /// gitDecoration.conflictingResourceForeground: Color for conflicting Git resources. Used for file labels and the SCM viewlet.
            /// </summary>
            public static readonly Unit ConflictingResourceForeground
                = new("gitDecoration.conflictingResourceForeground");

            /// <summary>
            /// gitDecoration.submoduleResourceForeground: Color for submodule resources.
            /// </summary>
            public static readonly Unit SubmoduleResourceForeground = new("gitDecoration.submoduleResourceForeground");
        }

        /// <summary>
        /// Note: These colors are for the GUI settings editor which can be opened with the Preferences: Open Settings (UI) command.
        /// </summary>
        public static class Settings
        {

            /// <summary>
            /// settings.headerForeground: The foreground color for a section header or active title.
            /// </summary>
            public static readonly Unit HeaderForeground = new("settings.headerForeground");

            /// <summary>
            /// settings.modifiedItemIndicator: The line that indicates a modified setting.
            /// </summary>
            public static readonly Unit ModifiedItemIndicator = new("settings.modifiedItemIndicator");

            /// <summary>
            /// settings.dropdownBackground: Dropdown background.
            /// </summary>
            public static readonly Unit DropdownBackground = new("settings.dropdownBackground");

            /// <summary>
            /// settings.dropdownForeground: Dropdown foreground.
            /// </summary>
            public static readonly Unit DropdownForeground = new("settings.dropdownForeground");

            /// <summary>
            /// settings.dropdownBorder: Dropdown border.
            /// </summary>
            public static readonly Unit DropdownBorder = new("settings.dropdownBorder");

            /// <summary>
            /// settings.dropdownListBorder: Dropdown list border.
            /// </summary>
            public static readonly Unit DropdownListBorder = new("settings.dropdownListBorder");

            /// <summary>
            /// settings.checkboxBackground: Checkbox background.
            /// </summary>
            public static readonly Unit CheckboxBackground = new("settings.checkboxBackground");

            /// <summary>
            /// settings.checkboxForeground: Checkbox foreground.
            /// </summary>
            public static readonly Unit CheckboxForeground = new("settings.checkboxForeground");

            /// <summary>
            /// settings.checkboxBorder: Checkbox border.
            /// </summary>
            public static readonly Unit CheckboxBorder = new("settings.checkboxBorder");

            /// <summary>
            /// settings.rowHoverBackground: The background color of a settings row when hovered.
            /// </summary>
            public static readonly Unit RowHoverBackground = new("settings.rowHoverBackground");

            /// <summary>
            /// settings.textInputBackground: Text input box background.
            /// </summary>
            public static readonly Unit TextInputBackground = new("settings.textInputBackground");

            /// <summary>
            /// settings.textInputForeground: Text input box foreground.
            /// </summary>
            public static readonly Unit TextInputForeground = new("settings.textInputForeground");

            /// <summary>
            /// settings.textInputBorder: Text input box border.
            /// </summary>
            public static readonly Unit TextInputBorder = new("settings.textInputBorder");

            /// <summary>
            /// settings.numberInputBackground: Number input box background.
            /// </summary>
            public static readonly Unit NumberInputBackground = new("settings.numberInputBackground");

            /// <summary>
            /// settings.numberInputForeground: Number input box foreground.
            /// </summary>
            public static readonly Unit NumberInputForeground = new("settings.numberInputForeground");

            /// <summary>
            /// settings.numberInputBorder: Number input box border.
            /// </summary>
            public static readonly Unit NumberInputBorder = new("settings.numberInputBorder");

            /// <summary>
            /// settings.focusedRowBackground: Background color of a focused setting row.
            /// </summary>
            public static readonly Unit FocusedRowBackground = new("settings.focusedRowBackground");

            /// <summary>
            /// settings.focusedRowBorder: The color of the row's top and bottom border when the row is focused.
            /// </summary>
            public static readonly Unit FocusedRowBorder = new("settings.focusedRowBorder");

            /// <summary>
            /// settings.headerBorder: The color of the header container border.
            /// </summary>
            public static readonly Unit HeaderBorder = new("settings.headerBorder");

            /// <summary>
            /// settings.sashBorder: The color of the Settings editor splitview sash border.
            /// </summary>
            public static readonly Unit SashBorder = new("settings.sashBorder");

            /// <summary>
            /// settings.settingsHeaderHoverForeground: The foreground color for a section header or hovered title.
            /// </summary>
            public static readonly Unit SettingsHeaderHoverForeground = new("settings.settingsHeaderHoverForeground");
        }

        /// <summary>
        /// The theme colors for breadcrumbs navigation:
        /// </summary>
        public static class Breadcrumb
        {
            /// <summary>
            /// breadcrumb.foreground: Color of breadcrumb items.
            /// </summary>
            public static readonly Unit Foreground = new("breadcrumb.foreground");

            /// <summary>
            /// breadcrumb.background: Background color of breadcrumb items.
            /// </summary>
            public static readonly Unit Background = new("breadcrumb.background");

            /// <summary>
            /// breadcrumb.focusForeground: Color of focused breadcrumb items.
            /// </summary>
            public static readonly Unit FocusForeground = new("breadcrumb.focusForeground");

            /// <summary>
            /// breadcrumb.activeSelectionForeground: Color of selected breadcrumb items.
            /// </summary>
            public static readonly Unit ActiveSelectionForeground = new("breadcrumb.activeSelectionForeground");

            /// <summary>
            /// breadcrumbPicker.background: Background color of breadcrumb item picker.
            /// </summary>
            public static readonly Unit PickerBackground = new("breadcrumbPicker.background");
        }

        /// <summary>
        /// The theme colors for snippets:
        /// </summary>
        public static class Snippets
        {
            /// <summary>
            /// editor.snippetTabstopHighlightBackground: Highlight background color of a snippet tabstop.
            /// </summary>
            public static readonly Unit SnippetTabstopHighlightBackground
                = new("editor.snippetTabstopHighlightBackground");

            /// <summary>
            /// editor.snippetTabstopHighlightBorder: Highlight border color of a snippet tabstop.
            /// </summary>
            public static readonly Unit SnippetTabstopHighlightBorder = new("editor.snippetTabstopHighlightBorder");

            /// <summary>
            /// editor.snippetFinalTabstopHighlightBackground: Highlight background color of the final tabstop of a snippet.
            /// </summary>
            public static readonly Unit SnippetFinalTabstopHighlightBackground
                = new("editor.snippetFinalTabstopHighlightBackground");

            /// <summary>
            /// editor.snippetFinalTabstopHighlightBorder: Highlight border color of the final tabstop of a snippet.
            /// </summary>
            public static readonly Unit SnippetFinalTabstopHighlightBorder
                = new("editor.snippetFinalTabstopHighlightBorder");
        }

        /// <summary>
        /// The theme colors for symbol icons that appears in the Outline view, breadcrumb navigation, and suggest widget:
        /// </summary>
        public static class SymbolIcon
        {
            /// <summary>
            /// symbolIcon.arrayForeground: The foreground color for array symbols.
            /// </summary>
            public static readonly Unit ArrayForeground = new("symbolIcon.arrayForeground");

            /// <summary>
            /// symbolIcon.booleanForeground: The foreground color for boolean symbols.
            /// </summary>
            public static readonly Unit BooleanForeground = new("symbolIcon.booleanForeground");

            /// <summary>
            /// symbolIcon.classForeground: The foreground color for class symbols.
            /// </summary>
            public static readonly Unit ClassForeground = new("symbolIcon.classForeground");

            /// <summary>
            /// symbolIcon.colorForeground: The foreground color for color symbols.
            /// </summary>
            public static readonly Unit ColorForeground = new("symbolIcon.colorForeground");

            /// <summary>
            /// symbolIcon.constantForeground: The foreground color for constant symbols.
            /// </summary>
            public static readonly Unit ConstantForeground = new("symbolIcon.constantForeground");

            /// <summary>
            /// symbolIcon.constructorForeground: The foreground color for constructor symbols.
            /// </summary>
            public static readonly Unit ConstructorForeground = new("symbolIcon.constructorForeground");

            /// <summary>
            /// symbolIcon.enumeratorForeground: The foreground color for enumerator symbols.
            /// </summary>
            public static readonly Unit EnumeratorForeground = new("symbolIcon.enumeratorForeground");

            /// <summary>
            /// symbolIcon.enumeratorMemberForeground: The foreground color for enumerator member symbols.
            /// </summary>
            public static readonly Unit EnumeratorMemberForeground = new("symbolIcon.enumeratorMemberForeground");

            /// <summary>
            /// symbolIcon.eventForeground: The foreground color for event symbols.
            /// </summary>
            public static readonly Unit EventForeground = new("symbolIcon.eventForeground");

            /// <summary>
            /// symbolIcon.fieldForeground: The foreground color for field symbols.
            /// </summary>
            public static readonly Unit FieldForeground = new("symbolIcon.fieldForeground");

            /// <summary>
            /// symbolIcon.fileForeground: The foreground color for file symbols.
            /// </summary>
            public static readonly Unit FileForeground = new("symbolIcon.fileForeground");

            /// <summary>
            /// symbolIcon.folderForeground: The foreground color for folder symbols.
            /// </summary>
            public static readonly Unit FolderForeground = new("symbolIcon.folderForeground");

            /// <summary>
            /// symbolIcon.functionForeground: The foreground color for function symbols.
            /// </summary>
            public static readonly Unit FunctionForeground = new("symbolIcon.functionForeground");

            /// <summary>
            /// symbolIcon.interfaceForeground: The foreground color for interface symbols.
            /// </summary>
            public static readonly Unit InterfaceForeground = new("symbolIcon.interfaceForeground");

            /// <summary>
            /// symbolIcon.keyForeground: The foreground color for key symbols.
            /// </summary>
            public static readonly Unit KeyForeground = new("symbolIcon.keyForeground");

            /// <summary>
            /// symbolIcon.keywordForeground: The foreground color for keyword symbols.
            /// </summary>
            public static readonly Unit KeywordForeground = new("symbolIcon.keywordForeground");

            /// <summary>
            /// symbolIcon.methodForeground: The foreground color for method symbols.
            /// </summary>
            public static readonly Unit MethodForeground = new("symbolIcon.methodForeground");

            /// <summary>
            /// symbolIcon.moduleForeground: The foreground color for module symbols.
            /// </summary>
            public static readonly Unit ModuleForeground = new("symbolIcon.moduleForeground");

            /// <summary>
            /// symbolIcon.namespaceForeground: The foreground color for namespace symbols.
            /// </summary>
            public static readonly Unit NamespaceForeground = new("symbolIcon.namespaceForeground");

            /// <summary>
            /// symbolIcon.nullForeground: The foreground color for null symbols.
            /// </summary>
            public static readonly Unit NullForeground = new("symbolIcon.nullForeground");

            /// <summary>
            /// symbolIcon.numberForeground: The foreground color for number symbols.
            /// </summary>
            public static readonly Unit NumberForeground = new("symbolIcon.numberForeground");

            /// <summary>
            /// symbolIcon.objectForeground: The foreground color for object symbols.
            /// </summary>
            public static readonly Unit ObjectForeground = new("symbolIcon.objectForeground");

            /// <summary>
            /// symbolIcon.operatorForeground: The foreground color for operator symbols.
            /// </summary>
            public static readonly Unit OperatorForeground = new("symbolIcon.operatorForeground");

            /// <summary>
            /// symbolIcon.packageForeground: The foreground color for package symbols.
            /// </summary>
            public static readonly Unit PackageForeground = new("symbolIcon.packageForeground");

            /// <summary>
            /// symbolIcon.propertyForeground: The foreground color for property symbols.
            /// </summary>
            public static readonly Unit PropertyForeground = new("symbolIcon.propertyForeground");

            /// <summary>
            /// symbolIcon.referenceForeground: The foreground color for reference symbols.
            /// </summary>
            public static readonly Unit ReferenceForeground = new("symbolIcon.referenceForeground");

            /// <summary>
            /// symbolIcon.snippetForeground: The foreground color for snippet symbols.
            /// </summary>
            public static readonly Unit SnippetForeground = new("symbolIcon.snippetForeground");

            /// <summary>
            /// symbolIcon.stringForeground: The foreground color for string symbols.
            /// </summary>
            public static readonly Unit StringForeground = new("symbolIcon.stringForeground");

            /// <summary>
            /// symbolIcon.structForeground: The foreground color for struct symbols.
            /// </summary>
            public static readonly Unit StructForeground = new("symbolIcon.structForeground");

            /// <summary>
            /// symbolIcon.textForeground: The foreground color for text symbols.
            /// </summary>
            public static readonly Unit TextForeground = new("symbolIcon.textForeground");

            /// <summary>
            /// symbolIcon.typeParameterForeground: The foreground color for type parameter symbols.
            /// </summary>
            public static readonly Unit TypeParameterForeground = new("symbolIcon.typeParameterForeground");

            /// <summary>
            /// symbolIcon.unitForeground: The foreground color for unit symbols.
            /// </summary>
            public static readonly Unit UnitForeground = new("symbolIcon.unitForeground");

            /// <summary>
            /// symbolIcon.variableForeground: The foreground color for variable symbols.
            /// </summary>
            public static readonly Unit VariableForeground = new("symbolIcon.variableForeground");
        }

        /// <summary>
        ///
        /// </summary>
        public static class DebugIcon
        {
            /// <summary>
            /// debugIcon.breakpointForeground: Icon color for breakpoints.
            /// </summary>
            public static readonly Unit BreakpointForeground = new("debugIcon.breakpointForeground");

            /// <summary>
            /// debugIcon.breakpointDisabledForeground: Icon color for disabled breakpoints.
            /// </summary>
            public static readonly Unit BreakpointDisabledForeground = new("debugIcon.breakpointDisabledForeground");

            /// <summary>
            /// debugIcon.breakpointUnverifiedForeground: Icon color for unverified breakpoints.
            /// </summary>
            public static readonly Unit BreakpointUnverifiedForeground
                = new("debugIcon.breakpointUnverifiedForeground");

            /// <summary>
            /// debugIcon.breakpointCurrentStackframeForeground: Icon color for the current breakpoint stack frame.
            /// </summary>
            public static readonly Unit BreakpointCurrentStackframeForeground
                = new("debugIcon.breakpointCurrentStackframeForeground");

            /// <summary>
            /// debugIcon.breakpointStackframeForeground: Icon color for all breakpoint stack frames.
            /// </summary>
            public static readonly Unit BreakpointStackframeForeground
                = new("debugIcon.breakpointStackframeForeground");

            /// <summary>
            /// debugIcon.startForeground: Debug toolbar icon for start debugging.
            /// </summary>
            public static readonly Unit StartForeground = new("debugIcon.startForeground");

            /// <summary>
            /// debugIcon.pauseForeground: Debug toolbar icon for pause.
            /// </summary>
            public static readonly Unit PauseForeground = new("debugIcon.pauseForeground");

            /// <summary>
            /// debugIcon.stopForeground: Debug toolbar icon for stop.
            /// </summary>
            public static readonly Unit StopForeground = new("debugIcon.stopForeground");

            /// <summary>
            /// debugIcon.disconnectForeground: Debug toolbar icon for disconnect.
            /// </summary>
            public static readonly Unit DisconnectForeground = new("debugIcon.disconnectForeground");

            /// <summary>
            /// debugIcon.restartForeground: Debug toolbar icon for restart.
            /// </summary>
            public static readonly Unit RestartForeground = new("debugIcon.restartForeground");

            /// <summary>
            /// debugIcon.stepOverForeground: Debug toolbar icon for step over.
            /// </summary>
            public static readonly Unit StepOverForeground = new("debugIcon.stepOverForeground");

            /// <summary>
            /// debugIcon.stepIntoForeground: Debug toolbar icon for step into.
            /// </summary>
            public static readonly Unit StepIntoForeground = new("debugIcon.stepIntoForeground");

            /// <summary>
            /// debugIcon.stepOutForeground: Debug toolbar icon for step over.
            /// </summary>
            public static readonly Unit StepOutForeground = new("debugIcon.stepOutForeground");

            /// <summary>
            /// debugIcon.continueForeground: Debug toolbar icon for continue.
            /// </summary>
            public static readonly Unit ContinueForeground = new("debugIcon.continueForeground");

            /// <summary>
            /// debugIcon.stepBackForeground: Debug toolbar icon for step back.
            /// </summary>
            public static readonly Unit StepBackForeground = new("debugIcon.stepBackForeground");

            /// <summary>
            /// debugConsole.infoForeground: Foreground color for info messages in debug REPL console.
            /// </summary>
            public static readonly Unit ConsoleInfoForeground = new("debugConsole.infoForeground");

            /// <summary>
            /// debugConsole.warningForeground: Foreground color for warning messages in debug REPL console.
            /// </summary>
            public static readonly Unit ConsoleWarningForeground = new("debugConsole.warningForeground");

            /// <summary>
            /// debugConsole.errorForeground: Foreground color for error messages in debug REPL console.
            /// </summary>
            public static readonly Unit ConsoleErrorForeground = new("debugConsole.errorForeground");

            /// <summary>
            /// debugConsole.sourceForeground: Foreground color for source filenames in debug REPL console.
            /// </summary>
            public static readonly Unit ConsoleSourceForeground = new("debugConsole.sourceForeground");

            /// <summary>
            /// debugConsoleInputIcon.foreground: Foreground color for debug console input marker icon.
            /// </summary>
            public static readonly Unit ConsoleInputIconForeground = new("debugConsoleInputIcon.foreground");
        }

        /// <summary>
        ///
        /// </summary>
        public static class Notebook
        {
            /// <summary>
            /// notebook.editorBackground: Notebook background color.
            /// </summary>
            public static readonly Unit EditorBackground = new("notebook.editorBackground");

            /// <summary>
            /// notebook.cellBorderColor: The border color for notebook cells.
            /// </summary>
            public static readonly Unit CellBorderColor = new("notebook.cellBorderColor");

            /// <summary>
            /// notebook.cellHoverBackground: The background color of a cell when the cell is hovered.
            /// </summary>
            public static readonly Unit CellHoverBackground = new("notebook.cellHoverBackground");

            /// <summary>
            /// notebook.cellInsertionIndicator: The color of the notebook cell insertion indicator.
            /// </summary>
            public static readonly Unit CellInsertionIndicator = new("notebook.cellInsertionIndicator");

            /// <summary>
            /// notebook.cellStatusBarItemHoverBackground: The background color of notebook cell status bar items.
            /// </summary>
            public static readonly Unit CellStatusBarItemHoverBackground
                = new("notebook.cellStatusBarItemHoverBackground");

            /// <summary>
            /// notebook.cellToolbarSeparator: The color of the separator in the cell bottom toolbar
            /// </summary>
            public static readonly Unit CellToolbarSeparator = new("notebook.cellToolbarSeparator");

            /// <summary>
            /// notebook.cellEditorBackground: The color of the notebook cell editor background
            /// </summary>
            public static readonly Unit CellEditorBackground = new("notebook.cellEditorBackground");

            /// <summary>
            /// notebook.focusedCellBackground: The background color of a cell when the cell is focused.
            /// </summary>
            public static readonly Unit FocusedCellBackground = new("notebook.focusedCellBackground");

            /// <summary>
            /// notebook.focusedCellBorder: The color of the cell's focus indicator borders when the cell is focused..
            /// </summary>
            public static readonly Unit FocusedCellBorder = new("notebook.focusedCellBorder");

            /// <summary>
            /// notebook.focusedEditorBorder: The color of the notebook cell editor border.
            /// </summary>
            public static readonly Unit FocusedEditorBorder = new("notebook.focusedEditorBorder");

            /// <summary>
            /// notebook.inactiveFocusedCellBorder: The color of the cell's top and bottom border when a cell is focused while the primary focus is outside of the editor.
            /// </summary>
            public static readonly Unit InactiveFocusedCellBorder = new("notebook.inactiveFocusedCellBorder");

            /// <summary>
            /// notebook.inactiveSelectedCellBorder: The color of the cell's borders when multiple cells are selected.
            /// </summary>
            public static readonly Unit InactiveSelectedCellBorder = new("notebook.inactiveSelectedCellBorder");

            /// <summary>
            /// notebook.outputContainerBackgroundColor: The Color of the notebook output container background.
            /// </summary>
            public static readonly Unit OutputContainerBackgroundColor = new("notebook.outputContainerBackgroundColor");

            /// <summary>
            /// notebook.outputContainerBorderColor: The border color of the notebook output container.
            /// </summary>
            public static readonly Unit OutputContainerBorderColor = new("notebook.outputContainerBorderColor");

            /// <summary>
            /// notebook.selectedCellBackground: The background color of a cell when the cell is selected.
            /// </summary>
            public static readonly Unit SelectedCellBackground = new("notebook.selectedCellBackground");

            /// <summary>
            /// notebook.selectedCellBorder: The color of the cell's top and bottom border when the cell is selected but not focused.
            /// </summary>
            public static readonly Unit SelectedCellBorder = new("notebook.selectedCellBorder");

            /// <summary>
            /// notebook.symbolHighlightBackground: Background color of highlighted cell
            /// </summary>
            public static readonly Unit SymbolHighlightBackground = new("notebook.symbolHighlightBackground");

            /// <summary>
            /// notebookScrollbarSlider.activeBackground: Notebook scrollbar slider background color when clicked on.
            /// </summary>
            public static readonly Unit ScrollbarSliderActiveBackground
                = new("notebookScrollbarSlider.activeBackground");

            /// <summary>
            /// notebookScrollbarSlider.background: Notebook scrollbar slider background color.
            /// </summary>
            public static readonly Unit ScrollbarSliderBackground = new("notebookScrollbarSlider.background");

            /// <summary>
            /// notebookScrollbarSlider.hoverBackground: Notebook scrollbar slider background color when hovering.
            /// </summary>
            public static readonly Unit ScrollbarSliderHoverBackground
                = new("notebookScrollbarSlider.hoverBackground");

            /// <summary>
            /// notebookStatusErrorIcon.foreground: The error icon color of notebook cells in the cell status bar.
            /// </summary>
            public static readonly Unit StatusErrorIconForeground = new("notebookStatusErrorIcon.foreground");

            /// <summary>
            /// notebookStatusRunningIcon.foreground: The running icon color of notebook cells in the cell status bar.
            /// </summary>
            public static readonly Unit StatusRunningIconForeground
                = new("notebookStatusRunningIcon.foreground");

            /// <summary>
            /// notebookStatusSuccessIcon.foreground: The success icon color of notebook cells in the cell status bar.
            /// </summary>
            public static readonly Unit StatusSuccessIconForeground
                = new("notebookStatusSuccessIcon.foreground");

            /// <summary>
            /// notebookEditorOverviewRuler.runningCellForeground: The color of the running cell decoration in the notebook editor overview ruler.
            /// </summary>
            public static readonly Unit EditorOverviewRulerRunningCellForeground
                = new("notebookEditorOverviewRuler.runningCellForeground");
        }

        /// <summary>
        ///
        /// </summary>
        public static class Chart
        {
            /// <summary>
            /// charts.foreground: Contrast color for text in charts.
            /// </summary>
            public static readonly Unit Foreground = new("charts.foreground");

            /// <summary>
            /// charts.lines: Color for lines in charts.
            /// </summary>
            public static readonly Unit Lines = new("charts.lines");

            /// <summary>
            /// charts.red: Color for red elements in charts.
            /// </summary>
            public static readonly Unit Red = new("charts.red");

            /// <summary>
            /// charts.blue: Color for blue elements in charts.
            /// </summary>
            public static readonly Unit Blue = new("charts.blue");

            /// <summary>
            /// charts.yellow: Color for yellow elements in charts.
            /// </summary>
            public static readonly Unit Yellow = new("charts.yellow");

            /// <summary>
            /// charts.orange: Color for orange elements in charts.
            /// </summary>
            public static readonly Unit Orange = new("charts.orange");

            /// <summary>
            /// charts.green: Color for green elements in charts.
            /// </summary>
            public static readonly Unit Green = new("charts.green");

            /// <summary>
            /// charts.purple: Color for purple elements in charts.
            /// </summary>
            public static readonly Unit Purple = new("charts.purple");
        }

        /// <summary>
        ///
        /// </summary>
        public static class Port
        {
            /// <summary>
            /// ports.iconRunningProcessForeground: The color of the icon for a port that has an associated running process.
            /// </summary>
            public static readonly Unit IconRunningProcessForeground = new("ports.iconRunningProcessForeground");
        }
    }
}