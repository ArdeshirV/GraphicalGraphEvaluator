#region Header
// Assembly Information
// FormMain.cs : Main form of GraphicalGraph application
// Copyright© 2002-2021 ArdeshirV@protonmail.com, Licensed under GPLv3+

using System;
using ArdeshirV.Tools;
using ArdeshirV.Forms;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using ArdeshirV.Applications.GraphicalGraphEvaluator;

#endregion	
//-----------------------------------------------------------------------------
namespace ArdeshirV.Applications.GraphicalGraphEvaluator
{
	/// <summary>
	/// Startup form.
	/// </summary>
	public class FormMain: FormBase
	{
		#region Variables

        private bool m_blnBadInput;
        private Button m_btnClearAll;
        private ToolTip m_tltToolTip;
        private IContainer components;
        private Button m_btnExit;
        private GroupBox m_grpIn;
        private System.Windows.Forms.NumericUpDown m_numInput;
        private Button m_btnRemove;
        private Button m_btnAdd;
        private ListBox m_lstList;
        private GroupBox m_grpOut;
        private Button m_btnSave;
        private ListBox m_lstResult;
        private TextBox m_txtOutput;
        private System.Windows.Forms.Button m_btnCalculate;
        private System.Windows.Forms.Button m_btnAboutApp;

		#endregion
		//---------------------------------------------------------------------
		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public FormMain()
		{
			InitializeComponent();
            m_btnSave.Enabled = false;
            
		}

		#endregion
		//---------------------------------------------------------------------
		#region Event handlers

        private void m_btnSave_Click(object sender, EventArgs e)
        {
            SaveToFile();
        }

		/// <summary>
		/// Occured whenever m_btnAdd button has been clicked.
		/// </summary>
		/// <param name="sender">Add button</param>
		/// <param name="e">Event argument</param>
		private void m_btnAdd_Click(object sender, System.EventArgs e)
		{
			try
			{
				if(m_numInput.Text != "")
					m_lstList.Items.Add((int)m_numInput.Value);

				m_numInput.ResetText();
				m_numInput.Select(0, 1);
                m_numInput.Focus();
			}
			catch(Exception exp)
			{
				FormErrorHandler.Show(this, exp);
				m_numInput.Value = 0;
				m_numInput.Focus();
			}
		}

		/// <summary>
		/// Occured whenever m_btnRemove button has been clicked.
		/// </summary>
		/// <param name="sender">Remove button</param>
		/// <param name="e">Event argument</param>
		private void m_btnRemove_Click(object sender, System.EventArgs e)
		{
			if(m_lstList.SelectedIndex != -1)
				m_lstList.Items.RemoveAt(m_lstList.SelectedIndex);

            if(m_lstList.Items.Count > 0)
                m_lstList.SelectedIndex = 0;
		}

		/// <summary>
		/// Occured whenever m_btnCalculate button has been clicked.
		/// </summary>
		/// <param name="sender">Calculate button</param>
		/// <param name="e">Event argument</param>
		private void m_btnCalculate_Click(object sender, System.EventArgs e)
		{
            if (m_lstList.Items.Count == 0)
                return;

			ArrayList l_arlItems = new ArrayList(m_lstList.Items);

            m_blnBadInput = false;
            bool l_blnResult = Calculate(l_arlItems);

            if(!m_blnBadInput)
                if (l_blnResult)
                    ShowArrayList(l_arlItems, "<--" + (m_txtOutput.Text = "Graphical Graph"));
                else
                    ShowArrayList(l_arlItems, "<--" + (m_txtOutput.Text = "None Graphical Graph"));

            m_btnSave.Enabled = true;
		}

        private void m_btnClearAll_Click(object sender, EventArgs e)
        {
            m_txtOutput.Text = "";
            m_lstList.Items.Clear();
            m_lstResult.Items.Clear();
            m_btnSave.Enabled = false;
        }
        
		void M_numInputKeyUp(object sender, KeyEventArgs e)
		{
			if(e.KeyData == Keys.Enter) {
				this.m_btnAdd_Click(sender, e);
			}
		}

		/// <summary>
		/// Occured whenever m_btnExit button has been clicked 
		/// </summary>
		/// <param name="sender">Exit button</param>
		/// <param name="e">Event argument</param>
		private void m_btnExit_Click(object sender, System.EventArgs e)
		{
			Close();
		}
		
		void M_btnAboutAppClick(object sender, EventArgs e)
		{
			AssemblyAttributeAccessors aaa = new AssemblyAttributeAccessors(this);
			string name = aaa.AssemblyProduct;
			Donation[] _donations = DefaultDonationList.Items;
			
			FormAboutData data = new FormAboutData(this,
				new Copyright[] { new Copyright((object)this, Icon.ToBitmap()) },
				new Credits[] { new Credits(name,
					new Credit[] { new Credit("ArdeshirV", "'Graphical Graph Evalulator' is developed by ArdeshirV about 2002 and then presented in github in 2021 with few changes about modern interface and a new license.", 
						GlobalResouces.AuthorsPhotos.ArdeshirV) }
				)},
				new ArdeshirV.Forms.License[] { 
					new ArdeshirV.Forms.License(name, GlobalResouces.Licenses.GPLLicense, GlobalResouces.Licenses.GPLLicenseLogo)
				},
				new Donations[] { new Donations(name, _donations) },
				"https://ArdeshirV.github.io/GraphicalGraphEvaluator/", "ArdeshirV@protonmail.com"
			);
			FormAbout.Show(data);
		}

		#endregion
		//---------------------------------------------------------------------
		#region Utility Functions

		/// <summary>
		/// Caculater.
		/// </summary>
		/// <param name="arl">Array List</param>
		internal bool Calculate(ArrayList arl)
		{
            int l_intTemp;
            int l_intIndexer;
            IComparer l_cmp = new BigToLow();
            ArrayList l_lst = new ArrayList(arl.Count);

            m_lstResult.Items.Clear();
            l_lst = (ArrayList)arl.Clone();

            if (l_lst.Count == 1)
                if ((int)l_lst[0] == 0)
                    return true;
                else
                {
                    m_blnBadInput = true;
                    m_txtOutput.Text = "Not a simple-graph";

                    return false;
                }

            foreach (int i in l_lst)
                if (i <= 0 || i > l_lst.Count)
                {
                    m_blnBadInput = true;
                    m_txtOutput.Text = "Not a simple-graph";

                    return false;
                }

            try
            {
                ShowArrayList(l_lst, "<--Nodes.");

                while (IsThereAnyItemBiggerThanZero(l_lst))
                {
                    l_lst.Sort(l_cmp);
                    ShowArrayList(l_lst, "<--Sort Nodes.");
                    l_intIndexer = (int)l_lst[0];

                    for (; l_intIndexer > 0; l_intIndexer--)
                    {
                        l_intTemp = ((int)l_lst[l_intIndexer]) - 1;
                        l_lst[l_intIndexer] = l_intTemp;
                    }

                    l_lst.RemoveAt(0);
                    ShowArrayList(l_lst, "<--Remove first item and decrease others");
                }
            }
            catch
            {
                return false;
            }

            ShowArrayList(l_lst, "<--Result");

            foreach (int i in l_lst)
                if (i != 0)
                    return false;

            return true;
		}
		//---------------------------------------------------------------------
        public static bool IsThereAnyItemBiggerThanZero(ArrayList arl)
        {
            foreach (int i in arl)
                if (i > 0)
                    return true;

            return false;
        }
        //---------------------------------------------------------------------
        private void ShowArrayList(ArrayList l_lst, string str)
        {
            string l_strText = string.Empty;

            foreach (int i in l_lst)
                l_strText += i.ToString() + ", ";

            m_lstResult.Items.Add(l_strText + "       " + str);
        }
        //---------------------------------------------------------------------
        public void SaveToFile()
        {
            SaveFileDialog l_dlgSave = new SaveFileDialog();

            l_dlgSave.DefaultExt = "txt";

            if (l_dlgSave.ShowDialog(this) == DialogResult.OK)
            {
                System.IO.StreamWriter w = 
                    System.IO.File.CreateText(l_dlgSave.FileName);

                foreach(string str in m_lstResult.Items)
                    w.WriteLine(str);

                w.Close();
            }

            l_dlgSave.Dispose();
        }

		#endregion
		//---------------------------------------------------------------------
		#region Overrided functions

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
				if(components != null)
					components.Dispose();

			base.Dispose(disposing);
		}

		#endregion
		//---------------------------------------------------------------------
		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.m_btnCalculate = new System.Windows.Forms.Button();
			this.m_btnExit = new System.Windows.Forms.Button();
			this.m_btnClearAll = new System.Windows.Forms.Button();
			this.m_tltToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.m_btnRemove = new System.Windows.Forms.Button();
			this.m_btnAdd = new System.Windows.Forms.Button();
			this.m_lstList = new System.Windows.Forms.ListBox();
			this.m_btnSave = new System.Windows.Forms.Button();
			this.m_lstResult = new System.Windows.Forms.ListBox();
			this.m_txtOutput = new System.Windows.Forms.TextBox();
			this.m_btnAboutApp = new System.Windows.Forms.Button();
			this.m_grpIn = new System.Windows.Forms.GroupBox();
			this.m_numInput = new System.Windows.Forms.NumericUpDown();
			this.m_grpOut = new System.Windows.Forms.GroupBox();
			this.m_grpIn.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_numInput)).BeginInit();
			this.m_grpOut.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_btnCalculate
			// 
			this.m_btnCalculate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_btnCalculate.BackColor = System.Drawing.Color.Transparent;
			this.m_btnCalculate.Location = new System.Drawing.Point(520, 16);
			this.m_btnCalculate.Name = "m_btnCalculate";
			this.m_btnCalculate.Size = new System.Drawing.Size(92, 29);
			this.m_btnCalculate.TabIndex = 2;
			this.m_btnCalculate.Text = "&Process";
			this.m_tltToolTip.SetToolTip(this.m_btnCalculate, "Process node degrees for graph.");
			this.m_btnCalculate.UseVisualStyleBackColor = false;
			this.m_btnCalculate.Click += new System.EventHandler(this.m_btnCalculate_Click);
			// 
			// m_btnExit
			// 
			this.m_btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_btnExit.BackColor = System.Drawing.Color.Transparent;
			this.m_btnExit.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnExit.Location = new System.Drawing.Point(520, 121);
			this.m_btnExit.Name = "m_btnExit";
			this.m_btnExit.Size = new System.Drawing.Size(92, 29);
			this.m_btnExit.TabIndex = 5;
			this.m_btnExit.Text = "E&xit";
			this.m_tltToolTip.SetToolTip(this.m_btnExit, "Exit graph processor program.");
			this.m_btnExit.UseVisualStyleBackColor = false;
			this.m_btnExit.Click += new System.EventHandler(this.m_btnExit_Click);
			// 
			// m_btnClearAll
			// 
			this.m_btnClearAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_btnClearAll.BackColor = System.Drawing.Color.Transparent;
			this.m_btnClearAll.Location = new System.Drawing.Point(520, 51);
			this.m_btnClearAll.Name = "m_btnClearAll";
			this.m_btnClearAll.Size = new System.Drawing.Size(92, 29);
			this.m_btnClearAll.TabIndex = 3;
			this.m_btnClearAll.Text = "Clear &All";
			this.m_tltToolTip.SetToolTip(this.m_btnClearAll, "Clear all lists.");
			this.m_btnClearAll.UseVisualStyleBackColor = false;
			this.m_btnClearAll.Click += new System.EventHandler(this.m_btnClearAll_Click);
			// 
			// m_btnRemove
			// 
			this.m_btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.m_btnRemove.Location = new System.Drawing.Point(6, 308);
			this.m_btnRemove.Name = "m_btnRemove";
			this.m_btnRemove.Size = new System.Drawing.Size(148, 23);
			this.m_btnRemove.TabIndex = 3;
			this.m_btnRemove.Text = "&Remove";
			this.m_tltToolTip.SetToolTip(this.m_btnRemove, "Remove selected node degree from degree list.");
			this.m_btnRemove.Click += new System.EventHandler(this.m_btnRemove_Click);
			// 
			// m_btnAdd
			// 
			this.m_btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_btnAdd.Location = new System.Drawing.Point(88, 19);
			this.m_btnAdd.Name = "m_btnAdd";
			this.m_btnAdd.Size = new System.Drawing.Size(66, 26);
			this.m_btnAdd.TabIndex = 1;
			this.m_btnAdd.Text = "&Add";
			this.m_tltToolTip.SetToolTip(this.m_btnAdd, "Add node degree to list.");
			this.m_btnAdd.Click += new System.EventHandler(this.m_btnAdd_Click);
			// 
			// m_lstList
			// 
			this.m_lstList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.m_lstList.BackColor = System.Drawing.Color.Black;
			this.m_lstList.ForeColor = System.Drawing.Color.LightGreen;
			this.m_lstList.Location = new System.Drawing.Point(6, 51);
			this.m_lstList.Name = "m_lstList";
			this.m_lstList.Size = new System.Drawing.Size(148, 251);
			this.m_lstList.TabIndex = 2;
			this.m_tltToolTip.SetToolTip(this.m_lstList, "degree of graph node");
			// 
			// m_btnSave
			// 
			this.m_btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.m_btnSave.Location = new System.Drawing.Point(6, 303);
			this.m_btnSave.Name = "m_btnSave";
			this.m_btnSave.Size = new System.Drawing.Size(324, 28);
			this.m_btnSave.TabIndex = 2;
			this.m_btnSave.Text = "&Save Result...";
			this.m_tltToolTip.SetToolTip(this.m_btnSave, "Save result list to file");
			this.m_btnSave.UseVisualStyleBackColor = true;
			this.m_btnSave.Click += new System.EventHandler(this.m_btnSave_Click);
			// 
			// m_lstResult
			// 
			this.m_lstResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.m_lstResult.BackColor = System.Drawing.Color.Black;
			this.m_lstResult.ForeColor = System.Drawing.Color.Goldenrod;
			this.m_lstResult.FormattingEnabled = true;
			this.m_lstResult.HorizontalScrollbar = true;
			this.m_lstResult.Location = new System.Drawing.Point(6, 49);
			this.m_lstResult.Name = "m_lstResult";
			this.m_lstResult.ScrollAlwaysVisible = true;
			this.m_lstResult.SelectionMode = System.Windows.Forms.SelectionMode.None;
			this.m_lstResult.Size = new System.Drawing.Size(324, 251);
			this.m_lstResult.TabIndex = 0;
			this.m_lstResult.TabStop = false;
			this.m_tltToolTip.SetToolTip(this.m_lstResult, "Result of processing.");
			// 
			// m_txtOutput
			// 
			this.m_txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.m_txtOutput.BackColor = System.Drawing.Color.Black;
			this.m_txtOutput.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_txtOutput.ForeColor = System.Drawing.Color.Red;
			this.m_txtOutput.Location = new System.Drawing.Point(6, 19);
			this.m_txtOutput.Name = "m_txtOutput";
			this.m_txtOutput.ReadOnly = true;
			this.m_txtOutput.Size = new System.Drawing.Size(324, 26);
			this.m_txtOutput.TabIndex = 1;
			this.m_txtOutput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.m_tltToolTip.SetToolTip(this.m_txtOutput, "Last final result of processing.");
			// 
			// m_btnAboutApp
			// 
			this.m_btnAboutApp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_btnAboutApp.BackColor = System.Drawing.Color.Transparent;
			this.m_btnAboutApp.Location = new System.Drawing.Point(520, 86);
			this.m_btnAboutApp.Name = "m_btnAboutApp";
			this.m_btnAboutApp.Size = new System.Drawing.Size(92, 29);
			this.m_btnAboutApp.TabIndex = 4;
			this.m_btnAboutApp.Text = "&About...";
			this.m_tltToolTip.SetToolTip(this.m_btnAboutApp, "Exit graph processor program.");
			this.m_btnAboutApp.UseVisualStyleBackColor = false;
			this.m_btnAboutApp.Click += new System.EventHandler(this.M_btnAboutAppClick);
			// 
			// m_grpIn
			// 
			this.m_grpIn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left)));
			this.m_grpIn.BackColor = System.Drawing.Color.Transparent;
			this.m_grpIn.Controls.Add(this.m_numInput);
			this.m_grpIn.Controls.Add(this.m_btnRemove);
			this.m_grpIn.Controls.Add(this.m_btnAdd);
			this.m_grpIn.Controls.Add(this.m_lstList);
			this.m_grpIn.Location = new System.Drawing.Point(12, 12);
			this.m_grpIn.Name = "m_grpIn";
			this.m_grpIn.Size = new System.Drawing.Size(160, 337);
			this.m_grpIn.TabIndex = 0;
			this.m_grpIn.TabStop = false;
			this.m_grpIn.Text = "&Input";
			// 
			// m_numInput
			// 
			this.m_numInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.m_numInput.BackColor = System.Drawing.Color.Black;
			this.m_numInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_numInput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
			this.m_numInput.Location = new System.Drawing.Point(6, 19);
			this.m_numInput.Name = "m_numInput";
			this.m_numInput.Size = new System.Drawing.Size(76, 26);
			this.m_numInput.TabIndex = 0;
			this.m_numInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.M_numInputKeyUp);
			// 
			// m_grpOut
			// 
			this.m_grpOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.m_grpOut.BackColor = System.Drawing.Color.Transparent;
			this.m_grpOut.Controls.Add(this.m_btnSave);
			this.m_grpOut.Controls.Add(this.m_txtOutput);
			this.m_grpOut.Controls.Add(this.m_lstResult);
			this.m_grpOut.Location = new System.Drawing.Point(178, 12);
			this.m_grpOut.Name = "m_grpOut";
			this.m_grpOut.Size = new System.Drawing.Size(336, 337);
			this.m_grpOut.TabIndex = 1;
			this.m_grpOut.TabStop = false;
			this.m_grpOut.Text = "&Output";
			// 
			// FormMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.Control;
			this.BackgoundEndGradientColor = System.Drawing.Color.Lime;
			this.BackgoundInactiveEndGradientColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.CancelButton = this.m_btnExit;
			this.ClientSize = new System.Drawing.Size(624, 361);
			this.Controls.Add(this.m_btnAboutApp);
			this.Controls.Add(this.m_grpOut);
			this.Controls.Add(this.m_btnClearAll);
			this.Controls.Add(this.m_btnExit);
			this.Controls.Add(this.m_btnCalculate);
			this.Controls.Add(this.m_grpIn);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(500, 250);
			this.Name = "FormMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Graphical Graph Evaluator";
			this.m_grpIn.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.m_numInput)).EndInit();
			this.m_grpOut.ResumeLayout(false);
			this.m_grpOut.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		//---------------------------------------------------------------------
		#region Entry point

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main()
		{
			try
			{
				Application.Run(new FormMain());
			}
			catch(Exception exp)
			{
				MessageBox.Show(exp.Message, "Error!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		#endregion
	}
	//-------------------------------------------------------------------------
    public class BigToLow : IComparer
    {
        public virtual int Compare(object First, object Second)
        {
            return ((int)Second - (int)First);
        }
    }
}

