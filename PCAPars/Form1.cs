namespace PCAPars
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using PacketsPars;

    /// <summary>
    /// Основное окно формы.
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// При нажатие на кнопку Открыть.
        /// </summary>
        /// <param name="sender">Кнопка.</param>
        /// <param name="e">Нажатие на кнопку.</param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            this.InitializeComponent();

            openFileDialog1.ShowDialog();
            string fileName = openFileDialog1.FileName;

            FileOpen.ClearReturnsItems();

            FileOpen file = new FileOpen(fileName);
            file.Open(fileName);
       
            Dictionary<int, Dictionary<string, string>> returnsItems = new Dictionary<int, Dictionary<string, string>>();

            returnsItems = FileOpen.GetReturnsItems();

            int num = 1;
            int iterationCount = FileOpen.GetCount();

            for (int i = 1; i <= iterationCount - 1; i++)
            {           
                try
                {
                    Dictionary<string, string> thisItem = returnsItems[i];

                    Label number = LabelCreation.CreateNumberLabel();
                    number.Text = num.ToString();

                    Label time = LabelCreation.CreateTimeLabel();
                    time.Text = returnsItems[i]["Date"];

                    Label source = LabelCreation.CreateSourceLabel();
                    source.Text = returnsItems[i]["Source"];

                    Label destination = LabelCreation.CreateDestinationLabel();
                    destination.Text = returnsItems[i]["Destination"];

                    Label protocol = LabelCreation.CreateProtocolLabel();
                    protocol.Text = returnsItems[i]["Protocol"];

                    Label length = LabelCreation.CreateLengthLabel();
                    length.Text = returnsItems[i]["Length"];

                    Label info = LabelCreation.CreateInfoLabel();
                    info.Text = returnsItems[i]["Info"];

                    tableLayoutPanel1.RowCount++;
                    tableLayoutPanel1.Controls.Add(number);   
                    tableLayoutPanel1.Controls.Add(time);
                    tableLayoutPanel1.Controls.Add(source);
                    tableLayoutPanel1.Controls.Add(destination);
                    tableLayoutPanel1.Controls.Add(protocol);
                    tableLayoutPanel1.Controls.Add(length);
                    tableLayoutPanel1.Controls.Add(info);

                    num++;
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Создание ярлыков.
        /// </summary>
        private class LabelCreation
        {
            /// <summary>
            /// Создание ярлыка для порядкогого номера.
            /// </summary>
            /// <returns></returns>
            public static Label CreateNumberLabel()
            {
                Label newLabel = new Label();

                newLabel.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
               | System.Windows.Forms.AnchorStyles.Left)
               | System.Windows.Forms.AnchorStyles.Right);
                newLabel.BackColor = System.Drawing.Color.Transparent;
                newLabel.Location = new System.Drawing.Point(4, 1);
                newLabel.Size = new System.Drawing.Size(60, 35);
                newLabel.TabIndex = 0;
                newLabel.Name = "label";
                newLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                return newLabel;
            }

            /// <summary>
            /// Создание ярлыка для времени.
            /// </summary>
            /// <returns></returns>
            public static Label CreateTimeLabel()
            {
                Label newLabel = new Label();

                newLabel.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
                newLabel.BackColor = System.Drawing.Color.Transparent;
                newLabel.Location = new System.Drawing.Point(71, 1);
                newLabel.Size = new System.Drawing.Size(193, 35);
                newLabel.TabIndex = 1;
                newLabel.Name = "label";
                newLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                return newLabel;
            }

            /// <summary>
            /// Создание ярлыка для источника.
            /// </summary>
            /// <returns></returns>
            public static Label CreateSourceLabel()
            {
                Label newLabel = new Label();

                newLabel.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
                newLabel.BackColor = System.Drawing.Color.Transparent;
                newLabel.Location = new System.Drawing.Point(271, 1);
                newLabel.Size = new System.Drawing.Size(220, 35);
                newLabel.TabIndex = 2;
                newLabel.Name = "label";
                newLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                return newLabel;
            }

            /// <summary>
            /// Создание ярлыка для пункта назначения.
            /// </summary>
            /// <returns></returns>
            public static Label CreateDestinationLabel()
            {
                Label newLabel = new Label();

                newLabel.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
                newLabel.BackColor = System.Drawing.Color.Transparent;
                newLabel.Location = new System.Drawing.Point(498, 1);
                newLabel.Size = new System.Drawing.Size(220, 35);
                newLabel.TabIndex = 3;
                newLabel.Name = "label";
                newLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                return newLabel;
            }

            /// <summary>
            /// Создание ярлыка для протокола.
            /// </summary>
            /// <returns></returns>
            public static Label CreateProtocolLabel()
            {
                Label newLabel = new Label();

                newLabel.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
                newLabel.BackColor = System.Drawing.Color.Transparent;
                newLabel.Location = new System.Drawing.Point(725, 1);
                newLabel.Size = new System.Drawing.Size(100, 35);
                newLabel.TabIndex = 4;
                newLabel.Name = "label";
                newLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                return newLabel;
            }

            /// <summary>
            /// Создание ярлыка для длины.
            /// </summary>
            /// <returns></returns>
            public static Label CreateLengthLabel()
            {
                Label newLabel = new Label();

                newLabel.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
                newLabel.BackColor = System.Drawing.Color.Transparent;
                newLabel.Location = new System.Drawing.Point(832, 1);
                newLabel.Size = new System.Drawing.Size(87, 35);
                newLabel.TabIndex = 5;
                newLabel.Name = "label";
                newLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                return newLabel;
            }

            /// <summary>
            /// Создание ярлыка для доп. информации.
            /// </summary>
            /// <returns></returns>
            public static Label CreateInfoLabel()
            {
                Label newLabel = new Label();

                newLabel.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
                newLabel.AutoSize = true;
                newLabel.BackColor = System.Drawing.Color.Transparent;
                newLabel.Location = new System.Drawing.Point(927, 1);
                newLabel.Size = new System.Drawing.Size(411, 35);
                newLabel.TabIndex = 6;
                newLabel.Name = "label";
                newLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                return newLabel;
            }
        }
    }
}