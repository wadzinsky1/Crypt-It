﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Crypt_It
{
    /// <summary>
    /// Booleans 
    /// </summary>
    public static class B
    {
        public static bool Set_Cores = false; // override automatic core detection for threading
        public static bool Timer = false; // set to true to show time it took to complete the encrypt/decrypt process 
        public static bool TestFile = false; // set to true to set the output file to C:\testfile.(bin/crypt)
        public static bool Reverse = false; // don't mess with this (just use the "decrypt" button)
        public static bool DelSource = false;    // WARNING!! Set to true if you want the source file(s) deleted after encrypt/decrypt process
        /// ------- Personal Preference --------
        public static readonly bool Hide_Files = false;  // set to true to hide output file(s) during encrypt/decrypt process
        public static bool LastChance = false;
        public static bool Working = false;
        public static bool Cancel = false;
        public static bool MultiThread = false;
        public static bool Overwrite = false;
        public static bool YesClick = false;
        public static bool Overwrite_Checked = false;
        public static bool msDCHK = false;
        public static bool Thread_Cancel = false;
        public static bool DryRun = false;
        public static bool Drop = false;
    }
    /// <summary>
    /// Variables for File list
    /// </summary>
    public static class File  // Global variables used by both (Iwad) and (Dwad) functions
    {
        public static string[] NewFile = new string[0];
        public static string[] OutFile = new string[0];
        public static long[] FileSize = new long[0];
        public static long l_tot;
        public static int i_TotalFiles;
        public static bool cryptfile;
        public static int FileNum;

        public static void Enumerate()
        {
            i_TotalFiles = NewFile.Length;
            l_tot = 0;
            for (int i = 0; i < FileSize.Length; i++)
            {
                l_tot += FileSize[i];
            }
        }
    }
    public class MyRender : ToolStripProfessionalRenderer
    {
        public MyRender() : base(new ColorTable()) { }
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!e.Item.Selected)
            {
                base.OnRenderMenuItemBackground(e);
                e.Item.ForeColor = Color.Silver;
            }
            else
            {
                Rectangle menuRec = new Rectangle(Point.Empty, e.Item.Size);
                var grbrush = new LinearGradientBrush(new Point(0, 10), new Point(menuRec.Width, 10)
                    , Color.FromArgb(62, 228, 228, 228), Color.FromArgb(255, 0, 0, 0));
                e.Graphics.FillRectangle(grbrush, menuRec);
                e.Graphics.DrawRectangle(Pens.Black, 0, 0, menuRec.Width - 1, menuRec.Height - 1);
                e.Item.ForeColor = Color.FromArgb(144, 255, 144);
            }
        }
    }
    /// <summary>
    /// Menu Strip color override
    /// </summary>
    public class ColorTable : ProfessionalColorTable
    {
        public override Color MenuItemPressedGradientBegin => Color.Black;
        public override Color MenuItemPressedGradientEnd => Color.DimGray;
        public override Color MenuItemBorder => Color.Black;
        public override Color MenuItemSelectedGradientBegin => Color.Black;
        public override Color MenuItemSelectedGradientEnd => Color.DimGray;
        public override Color ToolStripDropDownBackground => Color.FromArgb(215, 4, 4, 4);
        public override Color ImageMarginGradientBegin => Color.FromArgb(255, 80, 80, 80);
        public override Color ImageMarginGradientMiddle => Color.FromArgb(255, 80, 80, 80);
        public override Color ImageMarginGradientEnd => Color.FromArgb(255, 80, 80, 80);
        public override Color SeparatorDark => Color.Silver;
        public override Color MenuBorder => Color.DimGray;
    }
    /// <summary>
    /// Progress bar class
    /// </summary>
    public enum ProgressBarDisplayText { Percentage, CustomText }
    public class CustomProgressBar : ProgressBar
    {
        public ProgressBarDisplayText DisplayStyle { get; set; }
        public String CustomText { get; set; }
        public CustomProgressBar()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = ClientRectangle;
            Graphics g = e.Graphics;
            ProgressBarRenderer.DrawHorizontalBar(g, rect);
            rect.Inflate(-1, -1);
            var brush = new LinearGradientBrush(new Point(0, 10), new Point(rect.Width + 2, 10)
                    , Color.FromArgb(224, 32, 32, 32), Color.FromArgb(224, 0, 0, 0));
            e.Graphics.FillRectangle(brush, 1, 1, rect.Width + 0, rect.Height + 0);
            if (Value > 0)
            {
                Rectangle clip = new Rectangle(rect.X, rect.Y, (int)Math.Round(((float)Value / Maximum) * rect.Width), rect.Height);
                var fillbrush = new LinearGradientBrush(new Point(0, 10), new Point(rect.Width + 2, 10)
                        , Color.FromArgb(255, 0, 168, 0), Color.FromArgb(255, 0, 64, 168));
                ColorBlend c = new ColorBlend(3)
                {
                    Colors = new Color[3] { Color.FromArgb(255, 0, 196, 44), Color.FromArgb(255, 0, 255, 64), Color.FromArgb(255, 0, 32, 0) },
                    Positions = new float[3] { 0f, 0.5f, 1f }
                };
                fillbrush.InterpolationColors = c;
                ProgressBarRenderer.DrawHorizontalChunks(g, clip);
                e.Graphics.FillRectangle(fillbrush, 1, 1, clip.Width + 0, clip.Height + 0);
            }
            int percent = (int)(((double)this.Value / (double)this.Maximum) * 100);
            string text = DisplayStyle == ProgressBarDisplayText.Percentage ? percent.ToString() + '%' : CustomText;
            using (Font f = new Font(FontFamily.GenericSerif, 9))
            {
                SizeF len = g.MeasureString(text, f);
                Point location = new Point(Convert.ToInt32((Width - 15) - len.Width / 2), Convert.ToInt32((Height / 2) - len.Height / 2));
                g.DrawString(text, f, Brushes.Silver, location);
            }
        }
    }
    /// <summary>
    /// Prompt window class
    /// </summary>
    public class Prompt // Prompt window configuration
    {
        public static (int, bool) ShowDialog(string text, string caption, int cv)
        {
            bool t = false;
            Form prompt = new Form
            {
                Width = 200,
                Height = 120,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
                ControlBox = false
            };
            Label textLabel = new Label()
            {
                Left = 35,
                Top = 18,
                Text = text
            };
            NumericUpDown textBox = new NumericUpDown
            {
                Left = 75,
                Top = 15,
                Width = 50,
                Value = cv,
                Maximum = 255,
                Minimum = 1
            };
            Button confirmation = new Button()
            {
                Text = "Ok",
                Left = 30,
                Width = 50,
                Top = 40,
                DialogResult = DialogResult.OK
            };
            Button cancel = new Button()
            {
                Text = "Cancel",
                Left = 80,
                Width = 65,
                Top = 40,
                DialogResult = DialogResult.Cancel
            };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;
            var c = 0;
            var dr = (prompt.ShowDialog());
            if (dr == DialogResult.OK) c = (int)textBox.Value; else c = cv;
            if (dr == DialogResult.Cancel) t = true;
            return (c, t);
        }
    }
}
