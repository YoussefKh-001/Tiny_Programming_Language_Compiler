
namespace Tiny_Langauge_Compiler
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.CodeTextBox = new System.Windows.Forms.TextBox();
            this.TokensGridView = new System.Windows.Forms.DataGridView();
            this.CompileBtn = new System.Windows.Forms.Button();
            this.ClearTokensBtn = new System.Windows.Forms.Button();
            this.ErrorListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.TokensGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // CodeTextBox
            // 
            this.CodeTextBox.Location = new System.Drawing.Point(8, 7);
            this.CodeTextBox.Multiline = true;
            this.CodeTextBox.Name = "CodeTextBox";
            this.CodeTextBox.Size = new System.Drawing.Size(615, 455);
            this.CodeTextBox.TabIndex = 0;
            // 
            // TokensGridView
            // 
            this.TokensGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TokensGridView.Location = new System.Drawing.Point(638, 9);
            this.TokensGridView.Name = "TokensGridView";
            this.TokensGridView.RowHeadersWidth = 51;
            this.TokensGridView.Size = new System.Drawing.Size(408, 361);
            this.TokensGridView.TabIndex = 1;
            // 
            // CompileBtn
            // 
            this.CompileBtn.Location = new System.Drawing.Point(638, 387);
            this.CompileBtn.Name = "CompileBtn";
            this.CompileBtn.Size = new System.Drawing.Size(198, 75);
            this.CompileBtn.TabIndex = 2;
            this.CompileBtn.Text = "Compile";
            this.CompileBtn.UseVisualStyleBackColor = true;
            this.CompileBtn.Click += new System.EventHandler(this.CompileBtn_Click);
            // 
            // ClearTokensBtn
            // 
            this.ClearTokensBtn.Location = new System.Drawing.Point(848, 387);
            this.ClearTokensBtn.Name = "ClearTokensBtn";
            this.ClearTokensBtn.Size = new System.Drawing.Size(198, 75);
            this.ClearTokensBtn.TabIndex = 3;
            this.ClearTokensBtn.Text = "Clear Tokens";
            this.ClearTokensBtn.UseVisualStyleBackColor = true;
            // 
            // ErrorListBox
            // 
            this.ErrorListBox.FormattingEnabled = true;
            this.ErrorListBox.Location = new System.Drawing.Point(8, 496);
            this.ErrorListBox.Name = "ErrorListBox";
            this.ErrorListBox.Size = new System.Drawing.Size(1037, 147);
            this.ErrorListBox.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 480);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "Error List";
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1053, 655);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ErrorListBox);
            this.Controls.Add(this.ClearTokensBtn);
            this.Controls.Add(this.CompileBtn);
            this.Controls.Add(this.TokensGridView);
            this.Controls.Add(this.CodeTextBox);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.TokensGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox CodeTextBox;
        private System.Windows.Forms.DataGridView TokensGridView;
        private System.Windows.Forms.Button CompileBtn;
        private System.Windows.Forms.Button ClearTokensBtn;
        private System.Windows.Forms.ListBox ErrorListBox;
        private System.Windows.Forms.Label label1;
    }
}

