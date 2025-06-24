﻿using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PixelWallE.GUI;

namespace PixelWallE.GUI;

public class PixelPictureBox : PictureBox
{
    protected override void OnPaint(PaintEventArgs pe)
    {
        pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        base.OnPaint(pe);
    }
}


partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>

    private System.Windows.Forms.RichTextBox rtbCode;
    private System.Windows.Forms.NumericUpDown nudCanvasSize;
    private System.Windows.Forms.Button btnResize;
    private System.Windows.Forms.Button btnLoad;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnRun;
    private PixelPictureBox picCanvas;

    private void InitializeComponent()
    {
        this.rtbCode = new System.Windows.Forms.RichTextBox();
        this.nudCanvasSize = new System.Windows.Forms.NumericUpDown();
        this.btnResize = new System.Windows.Forms.Button();
        this.btnLoad = new System.Windows.Forms.Button();
        this.btnSave = new System.Windows.Forms.Button();
        this.btnRun = new System.Windows.Forms.Button();
        this.picCanvas = new PixelWallE.GUI.PixelPictureBox();

        ((System.ComponentModel.ISupportInitialize)(this.nudCanvasSize)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.picCanvas)).BeginInit();
        this.SuspendLayout();

        // 
        // rtbCode
        // 
        this.rtbCode.Location = new System.Drawing.Point(0, 0);
        this.rtbCode.Name = "rtbCode";
        this.rtbCode.Size = new System.Drawing.Size(800, 200);
        this.rtbCode.TabIndex = 0;
        this.rtbCode.Text = "";

        // 
        // nudCanvasSize
        // 
        this.nudCanvasSize.Location = new System.Drawing.Point(10, 210);
        this.nudCanvasSize.Minimum = 10;
        this.nudCanvasSize.Maximum = 500;
        this.nudCanvasSize.Value = 100;
        this.nudCanvasSize.Name = "nudCanvasSize";
        this.nudCanvasSize.Size = new System.Drawing.Size(80, 20);
        this.nudCanvasSize.TabIndex = 1;

        // 
        // btnResize
        // 
        this.btnResize.Location = new System.Drawing.Point(100, 208);
        this.btnResize.Name = "btnResize";
        this.btnResize.Size = new System.Drawing.Size(80, 23);
        this.btnResize.Text = "Ajustar";
        this.btnResize.TabIndex = 2;
        this.btnResize.UseVisualStyleBackColor = true;
        this.btnResize.Click += new System.EventHandler(this.btnResize_Click);

        // 
        // btnLoad
        // 
        this.btnLoad.Location = new System.Drawing.Point(10, 240);
        this.btnLoad.Name = "btnLoad";
        this.btnLoad.Size = new System.Drawing.Size(80, 23);
        this.btnLoad.Text = "Cargar .pw";
        this.btnLoad.TabIndex = 3;
        this.btnLoad.UseVisualStyleBackColor = true;
        this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);

        // 
        // btnSave
        // 
        this.btnSave.Location = new System.Drawing.Point(100, 240);
        this.btnSave.Name = "btnSave";
        this.btnSave.Size = new System.Drawing.Size(80, 23);
        this.btnSave.Text = "Guardar .pw";
        this.btnSave.TabIndex = 4;
        this.btnSave.UseVisualStyleBackColor = true;
        this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

        // 
        // btnRun
        // 
        this.btnRun.Location = new System.Drawing.Point(190, 240);
        this.btnRun.Name = "btnRun";
        this.btnRun.Size = new System.Drawing.Size(80, 23);
        this.btnRun.Text = "Ejecutar";
        this.btnRun.TabIndex = 5;
        this.btnRun.UseVisualStyleBackColor = true;
        this.btnRun.Click += new System.EventHandler(this.btnRun_Click);

        // 
        // picCanvas
        // 
        this.picCanvas.Location = new System.Drawing.Point(0, 270);
        this.picCanvas.Name = "picCanvas";
        this.picCanvas.Size = new System.Drawing.Size(800, 430);
        this.picCanvas.SizeMode = PictureBoxSizeMode.AutoSize;
        this.picCanvas.TabIndex = 6;
        this.picCanvas.TabStop = false;

        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 700);
        this.Name = "Form1";
        this.Text = "Pixel Wall-E GUI";

        ((System.ComponentModel.ISupportInitialize)(this.nudCanvasSize)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.picCanvas)).EndInit();
        this.ResumeLayout(false);
    }

    #endregion
}