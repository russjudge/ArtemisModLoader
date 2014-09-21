namespace ArtemisModLoader
{
    partial class ProjectInstaller
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // ProjectInstaller
            // 
            this.Committed += new System.Configuration.Install.InstallEventHandler(this.AfterCommitted_Event);
            this.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.AfterInstall_Event);
            this.AfterUninstall += new System.Configuration.Install.InstallEventHandler(this.AfterUninstall_Event);

        }

        #endregion
    }
}