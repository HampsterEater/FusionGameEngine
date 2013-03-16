namespace BinaryPhoenix.Fusion.Engine.Windows
{
    partial class RegisterWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegisterWindow));
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.registerButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.confirmPasswordTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.emailTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.displaynameTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.rocTextBox = new System.Windows.Forms.TextBox();
            this.agreeCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(25, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(487, 43);
            this.label1.TabIndex = 7;
            this.label1.Text = "Please enter all the required details into the following form and then click the " +
                "register button. Once this is done your account will be created on the server an" +
                "d you will be able to login.";
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(440, 485);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // registerButton
            // 
            this.registerButton.Enabled = false;
            this.registerButton.Location = new System.Drawing.Point(359, 485);
            this.registerButton.Name = "registerButton";
            this.registerButton.Size = new System.Drawing.Size(75, 23);
            this.registerButton.TabIndex = 5;
            this.registerButton.Text = "Register";
            this.registerButton.UseVisualStyleBackColor = true;
            this.registerButton.Click += new System.EventHandler(this.registerButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.confirmPasswordTextBox);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.passwordTextBox);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.emailTextBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.displaynameTextBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.usernameTextBox);
            this.groupBox1.Location = new System.Drawing.Point(25, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(487, 270);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Account Details";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(20, 227);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(263, 38);
            this.label10.TabIndex = 25;
            this.label10.Text = "This is used to make sure you don\'t accidently register with the wrong password.";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(20, 214);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(107, 13);
            this.label11.TabIndex = 24;
            this.label11.Text = "Confirm Password";
            // 
            // confirmPasswordTextBox
            // 
            this.confirmPasswordTextBox.Location = new System.Drawing.Point(289, 224);
            this.confirmPasswordTextBox.Name = "confirmPasswordTextBox";
            this.confirmPasswordTextBox.PasswordChar = '*';
            this.confirmPasswordTextBox.Size = new System.Drawing.Size(176, 20);
            this.confirmPasswordTextBox.TabIndex = 23;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(20, 190);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(263, 24);
            this.label8.TabIndex = 22;
            this.label8.Text = "For security this is limited to 6 characters minimum.";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(20, 177);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Password";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(289, 187);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '*';
            this.passwordTextBox.Size = new System.Drawing.Size(176, 20);
            this.passwordTextBox.TabIndex = 20;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(20, 153);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(263, 24);
            this.label6.TabIndex = 19;
            this.label6.Text = "Must be a valid email address to register.";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(20, 140);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Email";
            // 
            // emailTextBox
            // 
            this.emailTextBox.Location = new System.Drawing.Point(289, 150);
            this.emailTextBox.Name = "emailTextBox";
            this.emailTextBox.Size = new System.Drawing.Size(176, 20);
            this.emailTextBox.TabIndex = 17;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(20, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(263, 42);
            this.label4.TabIndex = 16;
            this.label4.Text = "Shown on every message you create to identify yourself to other members, if this " +
                "is left blank your username will be used.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(20, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Display Name";
            // 
            // displaynameTextBox
            // 
            this.displaynameTextBox.Location = new System.Drawing.Point(289, 92);
            this.displaynameTextBox.Name = "displaynameTextBox";
            this.displaynameTextBox.Size = new System.Drawing.Size(176, 20);
            this.displaynameTextBox.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(20, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(263, 35);
            this.label3.TabIndex = 13;
            this.label3.Text = "Used in conjuction with your password to login.\r\nFor security this is limited to " +
                "6 characters minimum.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(20, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Username";
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Location = new System.Drawing.Point(289, 41);
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(176, 20);
            this.usernameTextBox.TabIndex = 11;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(25, 339);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(409, 31);
            this.label12.TabIndex = 12;
            this.label12.Text = "These are our rules of conduct, you must agree to abide by them to register.\r\nIf " +
                "any of these rules are broken we reserve the right to revoke your membership.";
            // 
            // rocTextBox
            // 
            this.rocTextBox.Location = new System.Drawing.Point(28, 373);
            this.rocTextBox.Multiline = true;
            this.rocTextBox.Name = "rocTextBox";
            this.rocTextBox.ReadOnly = true;
            this.rocTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.rocTextBox.Size = new System.Drawing.Size(487, 95);
            this.rocTextBox.TabIndex = 13;
            this.rocTextBox.Text = resources.GetString("rocTextBox.Text");
            // 
            // agreeCheckBox
            // 
            this.agreeCheckBox.AutoSize = true;
            this.agreeCheckBox.Location = new System.Drawing.Point(28, 489);
            this.agreeCheckBox.Name = "agreeCheckBox";
            this.agreeCheckBox.Size = new System.Drawing.Size(214, 17);
            this.agreeCheckBox.TabIndex = 14;
            this.agreeCheckBox.Text = "I agree to abide by the rules of conduct.";
            this.agreeCheckBox.UseVisualStyleBackColor = true;
            this.agreeCheckBox.CheckedChanged += new System.EventHandler(this.agreeCheckBox_CheckedChanged);
            // 
            // RegisterWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 527);
            this.Controls.Add(this.agreeCheckBox);
            this.Controls.Add(this.rocTextBox);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.registerButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RegisterWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Register Account";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button registerButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox emailTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox displaynameTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox confirmPasswordTextBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox rocTextBox;
        private System.Windows.Forms.CheckBox agreeCheckBox;
    }
}