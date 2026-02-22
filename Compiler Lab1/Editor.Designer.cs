namespace Compiler_Lab1
{
    partial class textEditor
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
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(textEditor));
            toolStrip1 = new ToolStrip();
            toolStripDropDownButton1 = new ToolStripDropDownButton();
            createFile = new ToolStripMenuItem();
            btnOpenFile = new ToolStripMenuItem();
            saveFile = new ToolStripMenuItem();
            saveFileLike = new ToolStripMenuItem();
            btnCloseTab = new ToolStripMenuItem();
            exitBtn = new ToolStripMenuItem();
            toolStripDropDownButton2 = new ToolStripDropDownButton();
            btnBack = new ToolStripMenuItem();
            btnForward = new ToolStripMenuItem();
            btnCut = new ToolStripMenuItem();
            btnCopy = new ToolStripMenuItem();
            btnPaste = new ToolStripMenuItem();
            btnDelete = new ToolStripMenuItem();
            btnSelectAll = new ToolStripMenuItem();
            toolStripDropDownButton3 = new ToolStripDropDownButton();
            постановкаЗадачиToolStripMenuItem = new ToolStripMenuItem();
            грамматикаToolStripMenuItem = new ToolStripMenuItem();
            классификацияГрамматикиToolStripMenuItem = new ToolStripMenuItem();
            методАнализаToolStripMenuItem = new ToolStripMenuItem();
            тестовыйПримерToolStripMenuItem = new ToolStripMenuItem();
            списокЛитературыToolStripMenuItem = new ToolStripMenuItem();
            исходныйКодПрограммыToolStripMenuItem = new ToolStripMenuItem();
            toolStripButton1 = new ToolStripButton();
            toolStripDropDownButton4 = new ToolStripDropDownButton();
            btnHelp = new ToolStripMenuItem();
            btnAbout = new ToolStripMenuItem();
            viewDropDownBtn = new ToolStripDropDownButton();
            toolStripTextBox1 = new ToolStripTextBox();
            FontSizeCmb = new ToolStripComboBox();
            toolStrip2 = new ToolStrip();
            createFileQuick = new ToolStripButton();
            openFileQuick = new ToolStripButton();
            saveFileQuick = new ToolStripButton();
            btnCloseTabQuick = new ToolStripButton();
            btnBackQuick = new ToolStripButton();
            btnForwardQuick = new ToolStripButton();
            btnCopyQuick = new ToolStripButton();
            btnCutQuick = new ToolStripButton();
            btnPasteQuick = new ToolStripButton();
            toolStripButton14 = new ToolStripButton();
            btnHelpQuick = new ToolStripButton();
            btnAboutQuick = new ToolStripButton();
            tabControlEditor = new TabControl();
            splitContainer1 = new SplitContainer();
            dgvOutput = new DataGridView();
            FilePath = new DataGridViewTextBoxColumn();
            Line = new DataGridViewTextBoxColumn();
            Column = new DataGridViewTextBoxColumn();
            Message = new DataGridViewTextBoxColumn();
            toolStrip1.SuspendLayout();
            toolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvOutput).BeginInit();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new Size(20, 20);
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripDropDownButton1, toolStripDropDownButton2, toolStripDropDownButton3, toolStripButton1, toolStripDropDownButton4, viewDropDownBtn });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(782, 27);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripDropDownButton1.DropDownItems.AddRange(new ToolStripItem[] { createFile, btnOpenFile, saveFile, saveFileLike, btnCloseTab, exitBtn });
            toolStripDropDownButton1.Image = (Image)resources.GetObject("toolStripDropDownButton1.Image");
            toolStripDropDownButton1.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new Size(59, 24);
            toolStripDropDownButton1.Text = "Файл";
            // 
            // createFile
            // 
            createFile.Name = "createFile";
            createFile.Size = new Size(206, 26);
            createFile.Text = "Создать";
            createFile.Click += createFile_Click;
            // 
            // btnOpenFile
            // 
            btnOpenFile.Name = "btnOpenFile";
            btnOpenFile.Size = new Size(206, 26);
            btnOpenFile.Text = "Открыть";
            btnOpenFile.Click += OpenFile_Click;
            // 
            // saveFile
            // 
            saveFile.Name = "saveFile";
            saveFile.Size = new Size(206, 26);
            saveFile.Text = "Сохранить";
            saveFile.Click += saveFile_Click;
            // 
            // saveFileLike
            // 
            saveFileLike.Name = "saveFileLike";
            saveFileLike.Size = new Size(206, 26);
            saveFileLike.Text = "Сохранить как";
            saveFileLike.Click += saveFileLike_Click;
            // 
            // btnCloseTab
            // 
            btnCloseTab.Name = "btnCloseTab";
            btnCloseTab.Size = new Size(206, 26);
            btnCloseTab.Text = "Закрыть вкладку";
            btnCloseTab.Click += btnCloseTab_Click;
            // 
            // exitBtn
            // 
            exitBtn.Name = "exitBtn";
            exitBtn.Size = new Size(206, 26);
            exitBtn.Text = "Выход";
            exitBtn.Click += exitBtn_Click;
            // 
            // toolStripDropDownButton2
            // 
            toolStripDropDownButton2.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripDropDownButton2.DropDownItems.AddRange(new ToolStripItem[] { btnBack, btnForward, btnCut, btnCopy, btnPaste, btnDelete, btnSelectAll });
            toolStripDropDownButton2.Image = (Image)resources.GetObject("toolStripDropDownButton2.Image");
            toolStripDropDownButton2.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            toolStripDropDownButton2.Size = new Size(74, 24);
            toolStripDropDownButton2.Text = "Правка";
            // 
            // btnBack
            // 
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(186, 26);
            btnBack.Text = "Отменить";
            btnBack.Click += btnBack_Click;
            // 
            // btnForward
            // 
            btnForward.Name = "btnForward";
            btnForward.Size = new Size(186, 26);
            btnForward.Text = "Вернуть";
            btnForward.Click += btnForward_Click;
            // 
            // btnCut
            // 
            btnCut.Name = "btnCut";
            btnCut.Size = new Size(186, 26);
            btnCut.Text = "Вырезать";
            btnCut.Click += btnCut_Click;
            // 
            // btnCopy
            // 
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(186, 26);
            btnCopy.Text = "Копировать";
            btnCopy.Click += btnCopy_Click;
            // 
            // btnPaste
            // 
            btnPaste.Name = "btnPaste";
            btnPaste.Size = new Size(186, 26);
            btnPaste.Text = "Вставить";
            btnPaste.Click += btnPaste_Click;
            // 
            // btnDelete
            // 
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(186, 26);
            btnDelete.Text = "Удалить";
            btnDelete.Click += btnDelete_Click;
            // 
            // btnSelectAll
            // 
            btnSelectAll.Name = "btnSelectAll";
            btnSelectAll.Size = new Size(186, 26);
            btnSelectAll.Text = "Выделить все";
            btnSelectAll.Click += btnSelectAll_Click;
            // 
            // toolStripDropDownButton3
            // 
            toolStripDropDownButton3.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripDropDownButton3.DropDownItems.AddRange(new ToolStripItem[] { постановкаЗадачиToolStripMenuItem, грамматикаToolStripMenuItem, классификацияГрамматикиToolStripMenuItem, методАнализаToolStripMenuItem, тестовыйПримерToolStripMenuItem, списокЛитературыToolStripMenuItem, исходныйКодПрограммыToolStripMenuItem });
            toolStripDropDownButton3.Image = (Image)resources.GetObject("toolStripDropDownButton3.Image");
            toolStripDropDownButton3.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButton3.Name = "toolStripDropDownButton3";
            toolStripDropDownButton3.Size = new Size(59, 24);
            toolStripDropDownButton3.Text = "Текст";
            // 
            // постановкаЗадачиToolStripMenuItem
            // 
            постановкаЗадачиToolStripMenuItem.Name = "постановкаЗадачиToolStripMenuItem";
            постановкаЗадачиToolStripMenuItem.Size = new Size(288, 26);
            постановкаЗадачиToolStripMenuItem.Text = "Постановка задачи";
            // 
            // грамматикаToolStripMenuItem
            // 
            грамматикаToolStripMenuItem.Name = "грамматикаToolStripMenuItem";
            грамматикаToolStripMenuItem.Size = new Size(288, 26);
            грамматикаToolStripMenuItem.Text = "Грамматика";
            // 
            // классификацияГрамматикиToolStripMenuItem
            // 
            классификацияГрамматикиToolStripMenuItem.Name = "классификацияГрамматикиToolStripMenuItem";
            классификацияГрамматикиToolStripMenuItem.Size = new Size(288, 26);
            классификацияГрамматикиToolStripMenuItem.Text = "Классификация грамматики";
            // 
            // методАнализаToolStripMenuItem
            // 
            методАнализаToolStripMenuItem.Name = "методАнализаToolStripMenuItem";
            методАнализаToolStripMenuItem.Size = new Size(288, 26);
            методАнализаToolStripMenuItem.Text = "Метод анализа";
            // 
            // тестовыйПримерToolStripMenuItem
            // 
            тестовыйПримерToolStripMenuItem.Name = "тестовыйПримерToolStripMenuItem";
            тестовыйПримерToolStripMenuItem.Size = new Size(288, 26);
            тестовыйПримерToolStripMenuItem.Text = "Тестовый пример";
            // 
            // списокЛитературыToolStripMenuItem
            // 
            списокЛитературыToolStripMenuItem.Name = "списокЛитературыToolStripMenuItem";
            списокЛитературыToolStripMenuItem.Size = new Size(288, 26);
            списокЛитературыToolStripMenuItem.Text = "Список литературы";
            // 
            // исходныйКодПрограммыToolStripMenuItem
            // 
            исходныйКодПрограммыToolStripMenuItem.Name = "исходныйКодПрограммыToolStripMenuItem";
            исходныйКодПрограммыToolStripMenuItem.Size = new Size(288, 26);
            исходныйКодПрограммыToolStripMenuItem.Text = "Исходный код программы";
            // 
            // toolStripButton1
            // 
            toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
            toolStripButton1.ImageTransparentColor = Color.Magenta;
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new Size(45, 24);
            toolStripButton1.Text = "Пуск";
            // 
            // toolStripDropDownButton4
            // 
            toolStripDropDownButton4.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripDropDownButton4.DropDownItems.AddRange(new ToolStripItem[] { btnHelp, btnAbout });
            toolStripDropDownButton4.Image = (Image)resources.GetObject("toolStripDropDownButton4.Image");
            toolStripDropDownButton4.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButton4.Name = "toolStripDropDownButton4";
            toolStripDropDownButton4.Size = new Size(81, 24);
            toolStripDropDownButton4.Text = "Справка";
            // 
            // btnHelp
            // 
            btnHelp.Name = "btnHelp";
            btnHelp.Size = new Size(197, 26);
            btnHelp.Text = "Вызов справки";
            btnHelp.Click += btnHelp_Click;
            // 
            // btnAbout
            // 
            btnAbout.Name = "btnAbout";
            btnAbout.Size = new Size(197, 26);
            btnAbout.Text = "О программе";
            btnAbout.Click += btnAbout_Click;
            // 
            // viewDropDownBtn
            // 
            viewDropDownBtn.DisplayStyle = ToolStripItemDisplayStyle.Text;
            viewDropDownBtn.DropDownItems.AddRange(new ToolStripItem[] { toolStripTextBox1, FontSizeCmb });
            viewDropDownBtn.Image = (Image)resources.GetObject("viewDropDownBtn.Image");
            viewDropDownBtn.ImageTransparentColor = Color.Magenta;
            viewDropDownBtn.Name = "viewDropDownBtn";
            viewDropDownBtn.Size = new Size(49, 24);
            viewDropDownBtn.Text = "Вид";
            // 
            // toolStripTextBox1
            // 
            toolStripTextBox1.Name = "toolStripTextBox1";
            toolStripTextBox1.ReadOnly = true;
            toolStripTextBox1.Size = new Size(200, 27);
            toolStripTextBox1.Text = "Выберите размер шрифта";
            // 
            // FontSizeCmb
            // 
            FontSizeCmb.BackColor = SystemColors.Window;
            FontSizeCmb.DropDownStyle = ComboBoxStyle.Simple;
            FontSizeCmb.Items.AddRange(new object[] { "8", "9", "10", "11", "12", "14", "16", "18", "20", "22", "24", "26", "28", "36", "48", "72" });
            FontSizeCmb.Name = "FontSizeCmb";
            FontSizeCmb.Size = new Size(150, 150);
            FontSizeCmb.Text = "Размер шрифта";
            FontSizeCmb.SelectedIndexChanged += FontSizeCmb_SelectedIndexChanged;
            // 
            // toolStrip2
            // 
            toolStrip2.ImageScalingSize = new Size(30, 30);
            toolStrip2.Items.AddRange(new ToolStripItem[] { createFileQuick, openFileQuick, saveFileQuick, btnCloseTabQuick, btnBackQuick, btnForwardQuick, btnCopyQuick, btnCutQuick, btnPasteQuick, toolStripButton14, btnHelpQuick, btnAboutQuick });
            toolStrip2.Location = new Point(0, 27);
            toolStrip2.Name = "toolStrip2";
            toolStrip2.Size = new Size(782, 37);
            toolStrip2.TabIndex = 1;
            toolStrip2.Text = "toolStrip2";
            // 
            // createFileQuick
            // 
            createFileQuick.DisplayStyle = ToolStripItemDisplayStyle.Image;
            createFileQuick.Image = Properties.Resources.icons8_файл_26;
            createFileQuick.ImageTransparentColor = Color.Magenta;
            createFileQuick.Name = "createFileQuick";
            createFileQuick.Size = new Size(34, 34);
            createFileQuick.Text = "Создать";
            createFileQuick.Click += createFile_Click;
            // 
            // openFileQuick
            // 
            openFileQuick.DisplayStyle = ToolStripItemDisplayStyle.Image;
            openFileQuick.Image = Properties.Resources.icons8_файл_50;
            openFileQuick.ImageTransparentColor = Color.Magenta;
            openFileQuick.Name = "openFileQuick";
            openFileQuick.Size = new Size(34, 34);
            openFileQuick.Text = "Открыть";
            openFileQuick.Click += OpenFile_Click;
            // 
            // saveFileQuick
            // 
            saveFileQuick.DisplayStyle = ToolStripItemDisplayStyle.Image;
            saveFileQuick.Image = Properties.Resources.icons8_сохранить_48;
            saveFileQuick.ImageTransparentColor = Color.Magenta;
            saveFileQuick.Name = "saveFileQuick";
            saveFileQuick.Size = new Size(34, 34);
            saveFileQuick.Text = "Сохранить";
            saveFileQuick.Click += saveFile_Click;
            // 
            // btnCloseTabQuick
            // 
            btnCloseTabQuick.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnCloseTabQuick.Image = Properties.Resources.free_icon_close_3926643;
            btnCloseTabQuick.ImageTransparentColor = Color.Magenta;
            btnCloseTabQuick.Name = "btnCloseTabQuick";
            btnCloseTabQuick.Size = new Size(34, 34);
            btnCloseTabQuick.Text = "toolStripButton2";
            btnCloseTabQuick.Click += btnCloseTabQuick_Click;
            // 
            // btnBackQuick
            // 
            btnBackQuick.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnBackQuick.Image = Properties.Resources.icons8_влево_2_50;
            btnBackQuick.ImageTransparentColor = Color.Magenta;
            btnBackQuick.Name = "btnBackQuick";
            btnBackQuick.Size = new Size(34, 34);
            btnBackQuick.Text = "Отменить";
            btnBackQuick.Click += btnBack_Click;
            // 
            // btnForwardQuick
            // 
            btnForwardQuick.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnForwardQuick.Image = Properties.Resources.icons8_вправо_2_50;
            btnForwardQuick.ImageTransparentColor = Color.Magenta;
            btnForwardQuick.Name = "btnForwardQuick";
            btnForwardQuick.Size = new Size(34, 34);
            btnForwardQuick.Text = "Вернуть";
            btnForwardQuick.Click += btnForward_Click;
            // 
            // btnCopyQuick
            // 
            btnCopyQuick.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnCopyQuick.Image = Properties.Resources.icons8_копировать_24;
            btnCopyQuick.ImageTransparentColor = Color.Magenta;
            btnCopyQuick.Name = "btnCopyQuick";
            btnCopyQuick.Size = new Size(34, 34);
            btnCopyQuick.Text = "Копировать";
            btnCopyQuick.Click += btnCopy_Click;
            // 
            // btnCutQuick
            // 
            btnCutQuick.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnCutQuick.Image = Properties.Resources.icons8_вырезать_50;
            btnCutQuick.ImageTransparentColor = Color.Magenta;
            btnCutQuick.Name = "btnCutQuick";
            btnCutQuick.Size = new Size(34, 34);
            btnCutQuick.Text = "Вырезать";
            btnCutQuick.Click += btnCut_Click;
            // 
            // btnPasteQuick
            // 
            btnPasteQuick.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnPasteQuick.Image = Properties.Resources.icons8_вставить_48;
            btnPasteQuick.ImageTransparentColor = Color.Magenta;
            btnPasteQuick.Name = "btnPasteQuick";
            btnPasteQuick.Size = new Size(34, 34);
            btnPasteQuick.Text = "Вставить";
            btnPasteQuick.Click += btnPaste_Click;
            // 
            // toolStripButton14
            // 
            toolStripButton14.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton14.Image = Properties.Resources.icons8_старт_48;
            toolStripButton14.ImageTransparentColor = Color.Magenta;
            toolStripButton14.Name = "toolStripButton14";
            toolStripButton14.Size = new Size(34, 34);
            toolStripButton14.Text = "Компиляция";
            // 
            // btnHelpQuick
            // 
            btnHelpQuick.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnHelpQuick.Image = Properties.Resources.icons8_вопрос_50;
            btnHelpQuick.ImageTransparentColor = Color.Magenta;
            btnHelpQuick.Name = "btnHelpQuick";
            btnHelpQuick.Size = new Size(34, 34);
            btnHelpQuick.Text = "Руководство пользователя";
            btnHelpQuick.Click += btnHelp_Click;
            // 
            // btnAboutQuick
            // 
            btnAboutQuick.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnAboutQuick.Image = Properties.Resources.icons8_восклицательный_знак_48;
            btnAboutQuick.ImageTransparentColor = Color.Magenta;
            btnAboutQuick.Name = "btnAboutQuick";
            btnAboutQuick.Size = new Size(34, 34);
            btnAboutQuick.Text = "О программе";
            btnAboutQuick.Click += btnAbout_Click;
            // 
            // tabControlEditor
            // 
            tabControlEditor.Dock = DockStyle.Fill;
            tabControlEditor.Location = new Point(0, 0);
            tabControlEditor.Name = "tabControlEditor";
            tabControlEditor.SelectedIndex = 0;
            tabControlEditor.Size = new Size(782, 242);
            tabControlEditor.TabIndex = 2;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 64);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tabControlEditor);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(dgvOutput);
            splitContainer1.Size = new Size(782, 489);
            splitContainer1.SplitterDistance = 242;
            splitContainer1.TabIndex = 3;
            // 
            // dgvOutput
            // 
            dgvOutput.AllowUserToAddRows = false;
            dgvOutput.AllowUserToDeleteRows = false;
            dgvOutput.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvOutput.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvOutput.Columns.AddRange(new DataGridViewColumn[] { FilePath, Line, Column, Message });
            dgvOutput.Dock = DockStyle.Fill;
            dgvOutput.Location = new Point(0, 0);
            dgvOutput.Name = "dgvOutput";
            dgvOutput.ReadOnly = true;
            dgvOutput.RowHeadersWidth = 51;
            dgvOutput.Size = new Size(782, 243);
            dgvOutput.TabIndex = 0;
            // 
            // FilePath
            // 
            FilePath.HeaderText = "FilePath";
            FilePath.MinimumWidth = 6;
            FilePath.Name = "FilePath";
            FilePath.ReadOnly = true;
            // 
            // Line
            // 
            Line.HeaderText = "Line";
            Line.MinimumWidth = 6;
            Line.Name = "Line";
            Line.ReadOnly = true;
            // 
            // Column
            // 
            Column.HeaderText = "Column";
            Column.MinimumWidth = 6;
            Column.Name = "Column";
            Column.ReadOnly = true;
            // 
            // Message
            // 
            Message.HeaderText = "Message";
            Message.MinimumWidth = 6;
            Message.Name = "Message";
            Message.ReadOnly = true;
            // 
            // textEditor
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(782, 553);
            Controls.Add(splitContainer1);
            Controls.Add(toolStrip2);
            Controls.Add(toolStrip1);
            MaximumSize = new Size(1920, 1080);
            MinimumSize = new Size(800, 600);
            Name = "textEditor";
            Text = "Текстовый редактор";
            FormClosing += textEditor_FormClosing;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            toolStrip2.ResumeLayout(false);
            toolStrip2.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvOutput).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip toolStrip1;
        private ToolStrip toolStrip2;
        private ToolStripButton createFileQuick;
        private ToolStripButton openFileQuick;
        private ToolStripButton saveFileQuick;
        private ToolStripButton btnBackQuick;
        private ToolStripButton btnForwardQuick;
        private ToolStripButton btnCopyQuick;
        private ToolStripButton btnCutQuick;
        private ToolStripButton btnPasteQuick;
        private ToolStripButton toolStripButton14;
        private ToolStripButton btnHelpQuick;
        private ToolStripButton btnAboutQuick;
        private ToolStripDropDownButton toolStripDropDownButton1;
        private ToolStripMenuItem createFile;
        private ToolStripMenuItem btnOpenFile;
        private ToolStripMenuItem saveFile;
        private ToolStripMenuItem saveFileLike;
        private ToolStripMenuItem exitBtn;
        private ToolStripDropDownButton toolStripDropDownButton2;
        private ToolStripMenuItem btnBack;
        private ToolStripMenuItem btnForward;
        private ToolStripMenuItem btnCut;
        private ToolStripMenuItem btnCopy;
        private ToolStripMenuItem btnPaste;
        private ToolStripMenuItem btnDelete;
        private ToolStripMenuItem btnSelectAll;
        private ToolStripDropDownButton toolStripDropDownButton3;
        private ToolStripMenuItem постановкаЗадачиToolStripMenuItem;
        private ToolStripMenuItem грамматикаToolStripMenuItem;
        private ToolStripMenuItem классификацияГрамматикиToolStripMenuItem;
        private ToolStripMenuItem методАнализаToolStripMenuItem;
        private ToolStripMenuItem тестовыйПримерToolStripMenuItem;
        private ToolStripMenuItem списокЛитературыToolStripMenuItem;
        private ToolStripMenuItem исходныйКодПрограммыToolStripMenuItem;
        private ToolStripButton toolStripButton1;
        private ToolStripDropDownButton toolStripDropDownButton4;
        private ToolStripMenuItem btnHelp;
        private ToolStripMenuItem btnAbout;
        private ToolStripDropDownButton viewDropDownBtn;
        private ToolStripComboBox FontSizeCmb;
        private ToolStripTextBox toolStripTextBox1;
        private TabControl tabControlEditor;
        private SplitContainer splitContainer1;
        private DataGridView dgvOutput;
        private DataGridViewTextBoxColumn FilePath;
        private DataGridViewTextBoxColumn Line;
        private DataGridViewTextBoxColumn Column;
        private DataGridViewTextBoxColumn Message;
        private ToolStripButton btnCloseTabQuick;
        private ToolStripMenuItem btnCloseTab;
    }
}
