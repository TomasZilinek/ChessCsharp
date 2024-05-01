
namespace ChessPB069
{
    partial class PromoteDialog
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
            this.rookButton = new System.Windows.Forms.Button();
            this.knightButton = new System.Windows.Forms.Button();
            this.bishopButton = new System.Windows.Forms.Button();
            this.queenButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rookButton
            // 
            this.rookButton.Location = new System.Drawing.Point(128, 12);
            this.rookButton.Name = "rookButton";
            this.rookButton.Size = new System.Drawing.Size(100, 100);
            this.rookButton.TabIndex = 0;
            this.rookButton.UseVisualStyleBackColor = true;
            this.rookButton.Click += new System.EventHandler(this.rookButton_Click);
            // 
            // knightButton
            // 
            this.knightButton.Location = new System.Drawing.Point(12, 131);
            this.knightButton.Name = "knightButton";
            this.knightButton.Size = new System.Drawing.Size(100, 100);
            this.knightButton.TabIndex = 1;
            this.knightButton.UseVisualStyleBackColor = true;
            this.knightButton.Click += new System.EventHandler(this.knightButton_Click);
            // 
            // bishopButton
            // 
            this.bishopButton.Location = new System.Drawing.Point(128, 131);
            this.bishopButton.Name = "bishopButton";
            this.bishopButton.Size = new System.Drawing.Size(100, 100);
            this.bishopButton.TabIndex = 2;
            this.bishopButton.UseVisualStyleBackColor = true;
            this.bishopButton.Click += new System.EventHandler(this.bishopButton_Click);
            // 
            // queenButton
            // 
            this.queenButton.Location = new System.Drawing.Point(12, 12);
            this.queenButton.Name = "queenButton";
            this.queenButton.Size = new System.Drawing.Size(100, 100);
            this.queenButton.TabIndex = 0;
            this.queenButton.UseVisualStyleBackColor = true;
            this.queenButton.Click += new System.EventHandler(this.queenButton_Click);
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(254, 12);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(121, 42);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(254, 70);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(121, 42);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // PromoteDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 256);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.bishopButton);
            this.Controls.Add(this.knightButton);
            this.Controls.Add(this.rookButton);
            this.Controls.Add(this.queenButton);
            this.Name = "PromoteDialog";
            this.Text = "promoteDialog";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button queenButton;
        private System.Windows.Forms.Button rookButton;
        private System.Windows.Forms.Button knightButton;
        private System.Windows.Forms.Button bishopButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}