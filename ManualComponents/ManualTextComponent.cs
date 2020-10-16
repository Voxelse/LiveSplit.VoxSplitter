﻿using LiveSplit.Model;
using LiveSplit.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LiveSplit.VoxSplitter {
    public class ManualTextComponent : UI.Components.IComponent {
        protected FontTextComponent InternalComponent { get; set; }
        public ManualTextSettings Settings { get; set; }

        public float PaddingTop => InternalComponent.PaddingTop;
        public float PaddingLeft => InternalComponent.PaddingLeft;
        public float PaddingBottom => InternalComponent.PaddingBottom;
        public float PaddingRight => InternalComponent.PaddingRight;
        public float VerticalHeight => InternalComponent.VerticalHeight;
        public float MinimumWidth => InternalComponent.MinimumWidth;
        public float HorizontalWidth => InternalComponent.HorizontalWidth;
        public float MinimumHeight => InternalComponent.MinimumHeight;
        public IDictionary<string, Action> ContextMenuControls => null;

        public string ComponentName { get; set; }

        public string Name { get; set; }
        private readonly bool freeName;
        public string Value { get; set; }

        public ManualTextComponent(LiveSplitState state, string componentName, bool freeName = false) {
            ComponentName = Name = componentName;
            this.freeName = freeName;
            Settings = new ManualTextSettings(componentName, freeName) { CurrentState = state };
            InternalComponent = new FontTextComponent(Settings);
        }

        private void PrepareDraw(LiveSplitState state, LayoutMode mode) {
            InternalComponent.DisplayTwoRows = Settings.Display2Rows;

            InternalComponent.NameLabel.HasShadow = InternalComponent.ValueLabel.HasShadow = state.LayoutSettings.DropShadows;

            if(String.IsNullOrEmpty(freeName ? Settings.Text1 : Name) || String.IsNullOrEmpty(Value)) {
                InternalComponent.NameLabel.HorizontalAlignment = StringAlignment.Center;
                InternalComponent.ValueLabel.HorizontalAlignment = StringAlignment.Center;
                InternalComponent.NameLabel.VerticalAlignment = StringAlignment.Center;
                InternalComponent.ValueLabel.VerticalAlignment = StringAlignment.Center;
            } else {
                InternalComponent.NameLabel.HorizontalAlignment = StringAlignment.Near;
                InternalComponent.ValueLabel.HorizontalAlignment = StringAlignment.Far;
                bool isAcross = mode == LayoutMode.Horizontal || Settings.Display2Rows;
                InternalComponent.NameLabel.VerticalAlignment = isAcross ? StringAlignment.Near : StringAlignment.Center;
                InternalComponent.ValueLabel.VerticalAlignment = isAcross ? StringAlignment.Far : StringAlignment.Center;
            }

            InternalComponent.NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
            InternalComponent.ValueLabel.ForeColor = Settings.OverrideTimeColor ? Settings.TimeColor : state.LayoutSettings.TextColor;
        }

        private void DrawBackground(Graphics g, float width, float height) {
            if(Settings.BackgroundColor.A > 0 || Settings.BackgroundGradient != GradientType.Plain && Settings.BackgroundColor2.A > 0) {
                LinearGradientBrush gradientBrush = new LinearGradientBrush(
                    new PointF(0, 0),
                    Settings.BackgroundGradient == GradientType.Horizontal ? new PointF(width, 0) : new PointF(0, height),
                    Settings.BackgroundColor,
                    Settings.BackgroundGradient == GradientType.Plain ? Settings.BackgroundColor : Settings.BackgroundColor2);
                g.FillRectangle(gradientBrush, 0, 0, width, height);
            }
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion) {
            DrawBackground(g, width, VerticalHeight);
            PrepareDraw(state, LayoutMode.Vertical);
            InternalComponent.DrawVertical(g, state, width, clipRegion);
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) {
            DrawBackground(g, HorizontalWidth, height);
            PrepareDraw(state, LayoutMode.Horizontal);
            InternalComponent.DrawHorizontal(g, state, height, clipRegion);
        }

        public Control GetSettingsControl(LayoutMode mode) {
            Settings.Mode = mode;
            return Settings;
        }

        public void SetSettings(System.Xml.XmlNode settings) => Settings.SetSettings(settings);

        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document) => Settings.GetSettings(document);

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) {
            InternalComponent.InformationName = freeName ? Settings.Text1 : Name;
            InternalComponent.InformationValue = Value;
            InternalComponent.LongestString = Name.Length > Value.Length ? Name : Value;

            InternalComponent.Update(invalidator, state, width, height, mode);
        }

        public void Dispose() { }

        public int GetSettingsHashCode() => Settings.GetSettingsHashCode();
    }
}