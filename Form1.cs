using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;

namespace GrimDawnCnvQstToTXT
{
    public partial class Form1 : Form
    {
        private string currentLanguage = "cn";
        private string outputChoice = "custom1";
        private BackgroundWorker backgroundWorker;
        private const string VERSION = "v0.9";

        // 语言资源字典
        private Dictionary<string, Dictionary<string, string>> language = new Dictionary<string, Dictionary<string, string>>()
        {
            { "topTitle", new Dictionary<string, string>()
                {
                    { "cn", "恐怖黎明任务和对话文本提取工具" },
                    { "en", "Grim Dawn Quests and Conversations Extraction Tool"}
                }
            },
            { "bigTitle", new Dictionary<string, string>()
                {
                    { "cn", "恐怖黎明任务和对话文本提取工具" },
                    { "en", "Grim Dawn Quests and Conversations Extraction Tool"}
                }
            },
            { "inDir", new Dictionary<string, string>()
                {
                    { "cn", "请输入目录:" },
                    { "en", "Enter directory:" }
                }
            },
            { "inButton", new Dictionary<string, string>()
                {
                    { "cn", "浏览" },
                    { "en", "Browse" }
                }
            },
            { "startProcessing", new Dictionary<string, string>()
                {
                    { "cn", "处理开始时间: {0} 处理文件如下：" },
                    { "en", "Processing start time: {0} Processing the following files:" }
                }
            },
            { "errorFile", new Dictionary<string, string>()
                {
                    { "cn", "处理文件 {0} 时发生错误: {1}" },
                    { "en", "Error occurred while processing file {0}: {1}" }
                }
            },
            { "endTime", new Dictionary<string, string>()
                {
                    { "cn", "处理结束时间: {0} 共处理 {1} 个文件，用时 {2:F2} 秒" },
                    { "en", "Processing end time: {0} Total processed {1} files, time taken {2:F2} seconds" }
                }
            },
            { "mouseCopy", new Dictionary<string, string>()
                {
                    { "cn", "复制" },
                    { "en", "Copy" }
                }
            },
            { "mousePaste", new Dictionary<string, string>()
                {
                    { "cn", "粘贴" },
                    { "en", "Paste" }
                }
            },
            { "mouseCut", new Dictionary<string, string>()
                {
                    { "cn", "剪切" },
                    { "en", "Cut" }
                }
            },
            { "mouseSelectAll", new Dictionary<string, string>()
                {
                    { "cn", "全选" },
                    { "en", "Select all" }
                }
            },
            { "choiceButtonOriginal", new Dictionary<string, string>()
                {
                    { "cn", "输出至原目录下同名txt" },
                    { "en", "Output to the same directory as the source files" }
                }
            },
            { "choiceButtonCustom1", new Dictionary<string, string>()
                {
                    { "cn", "输出至所选目录下CnvQstToTxtNoLevel目录（所有txt在同一目录）" },
                    { "en", "Output to the CnvQstToTxtNoLevel directory under the selected folder (all txt files in a single directory)" }
                }
            },
            { "choiceButtonCustom2", new Dictionary<string, string>()
                {
                    { "cn", "输出至所选目录下CnvQstToTxtKeepLevel目录（保留目录结构）" },
                    { "en", "Output to the CnvQstToTxtKeepLevel directory under the selected folder (preserve original directory structure)" }
                }
            },
            { "startButton", new Dictionary<string, string>()
                {
                    { "cn", "开始" },
                    { "en", "Start" }
                }
            },
            { "resultUI", new Dictionary<string, string>()
                {
                    { "cn", "处理结果:" },
                    { "en", "Processing result:" }
                }
            },
            { "resultFinished", new Dictionary<string, string>()
                {
                    { "cn", "处理完成" },
                    { "en", "Processing complete" }
                }
            },
            { "resultOriginal", new Dictionary<string, string>()
                {
                    { "cn", "已经将处理后的txt文件保存至各cnv、qst同级目录中，请自行检查" },
                    { "en", "Processed txt files have been saved in the same directory as the corresponding cnv and qst files. Please check them manually." }
                }
            },
            { "resultCustom1", new Dictionary<string, string>()
                {
                    { "cn", "已经将处理后的txt文件保存至 {0} 目录中，请自行检查" },
                    { "en", "Processed txt files have been saved in the {0} directory. Please check them manually." }
                }
            },
            { "resultCustom2", new Dictionary<string, string>()
                {
                    { "cn", "已经将处理后的txt文件保存至 {0} 目录中，并保留原有目录结构，请自行检查" },
                    { "en", "Processed txt files have been saved in the {0} directory while preserving the original directory structure. Please check them manually." }
                }
            },
            { "by", new Dictionary<string, string>()
                {
                    { "cn", "by:老张allif" },
                    { "en", "by:laozhangggg" }
                }
            },
            { "introduction", new Dictionary<string, string>()
                {
                    { "cn", "本工具可以提取恐怖黎明任务文本、对话文本；\n浏览、粘贴、拖放目录后，会自动寻找该目录及不限级子目录下的.qst和.cnv文件；\n工具将自动转换2种文件为同名的txt文件，至对应目录下，请自行选择存放目录；\n将Conversations.arc和quests.arc解包后，可以直接把resources的完整路径粘贴过来，点击开始即可同时处理2种文件；" },
                    { "en", "This tool extracts quests and conversations from Grim Dawn.\nAfter browsing, pasting, dragging and dropping the directory, it will automatically search for .qst and .cnv files within that directory and its subdirectories.\nThe tool will convert these two file types into txt files with the same names and save them in the corresponding directories. Please choose your preferred output location.\nAfter extracting Conversations.arc and Quests.arc, you can directly paste the full path of the 'resources' folder and Click Start to process 2 types of files simultaneously" }
                }
            },
            { "failed", new Dictionary<string, string>()
                {
                    { "cn", "处理失败" },
                    { "en", "Processing failed" }
                }
            },
            { "error", new Dictionary<string, string>()
                {
                    { "cn", "错误" },
                    { "en", "Error" }
                }
            },
            { "noFiles", new Dictionary<string, string>()
                {
                    { "cn", "无可处理文件，请检查所选目录及子目录下是否有.cnv或qst文件。" },
                    { "en", "No files to process. Please check if the selected directory and its subdirectories contain .cnv or qst files." }
                }
            }
        };

        public Form1()
        {
            // 移除InitializeComponent()调用，因为我们是手动创建UI
            InitializeBackgroundWorker();
            // 设置窗口图标 - 使用更可靠的方式，避免单文件模式下的问题
            // 设置图标（需要先添加图标文件）
            try
            {
                // 直接使用Icon.ExtractAssociatedIcon方法从可执行文件中提取图标
                // 这是最可靠的方式，特别是在单文件发布模式下
                this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch (Exception ex)
            {
                // 如果上面的方法失败，尝试从嵌入资源中加载
                try
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    // 使用项目命名空间 + 文件名作为资源路径
                    string resourceName = "GrimDawnCnvQstToTXT.app.ico";
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream != null)
                        {
                            this.Icon = new Icon(stream);
                        }
                    }
                }
                catch (Exception innerEx)
                {
                    Console.WriteLine("Failed to load icon from resources: " + innerEx.Message);
                }
            }
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = language["topTitle"][currentLanguage];
            this.Size = new Size(1000, 750);
            this.StartPosition = FormStartPosition.CenterScreen;

            // 设置字体，使用字体回退机制确保在没有SimSun字体的系统上也能正常显示
            // 首先尝试使用SimSun（宋体），如果不存在则使用系统默认字体
            Font defaultFont = CreateFontWithFallback("SimSun", 9);
            Font titleFont = CreateFontWithFallback("SimSun", 16, FontStyle.Bold);
            Font languageFont = CreateFontWithFallback("SimSun", 12);
            Font comboBoxFont = CreateFontWithFallback("SimSun", 12);
            Font directoryFont = CreateFontWithFallback("SimSun", 12);

            // 创建主面板
            Panel mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Margin = new Padding(20);
            mainPanel.Padding = new Padding(20);
            this.Controls.Add(mainPanel);

            // 使用TableLayoutPanel作为主布局
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 1;
            mainLayout.RowCount = 9;
            mainLayout.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            mainLayout.Padding = new Padding(10);
            mainLayout.AutoScroll = true;
            mainPanel.Controls.Add(mainLayout);

            // 设置行高
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // 语言选择
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F)); // 大标题
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F)); // 目录输入
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F)); // 单选按钮
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F)); // 结果标签
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F)); // 文本区域
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F)); // 进度条
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F)); // 开始按钮
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F)); // 介绍（固定高度，减少与底栏的间距）

            // 1. 语言选择（右侧对齐）
            Panel languagePanel = new Panel();
            languagePanel.Dock = DockStyle.Fill;
            languagePanel.Width = mainLayout.Width - 40;
            mainLayout.Controls.Add(languagePanel, 0, 0);

            Label languageLabel = new Label();
            languageLabel.Text = "Language:";
            languageLabel.Font = languageFont;
            languageLabel.Dock = DockStyle.Right;
            languageLabel.TextAlign = ContentAlignment.MiddleRight;
            languageLabel.Margin = new Padding(10, 0, 10, 0);

            ComboBox languageComboBox = new ComboBox();
            languageComboBox.Name = "languageComboBox";
            languageComboBox.Font = comboBoxFont;
            languageComboBox.Items.Add(new ComboBoxItem("简体中文", "cn"));
            languageComboBox.Items.Add(new ComboBoxItem("English", "en"));
            languageComboBox.DisplayMember = "Text";
            languageComboBox.ValueMember = "Value";
            languageComboBox.SelectedIndex = 0;
            languageComboBox.Width = 150;
            languageComboBox.Dock = DockStyle.Right;
            languageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            languageComboBox.SelectedIndexChanged += LanguageComboBox_SelectedIndexChanged;

            languagePanel.Controls.Add(languageLabel);
            languagePanel.Controls.Add(languageComboBox);

            // 2. 大标题
            Label toolNameLabel = new Label();
            toolNameLabel.Name = "toolNameLabel";
            toolNameLabel.Text = language["bigTitle"][currentLanguage];
            toolNameLabel.Font = titleFont;
            toolNameLabel.Dock = DockStyle.Fill;
            toolNameLabel.TextAlign = ContentAlignment.MiddleCenter;
            mainLayout.Controls.Add(toolNameLabel, 0, 1);

            // 3. 目录输入
            Panel directoryPanel = new Panel();
            directoryPanel.Dock = DockStyle.Fill;
            directoryPanel.Width = mainLayout.Width - 40;
            mainLayout.Controls.Add(directoryPanel, 0, 2);

            // 设置directoryPanel为手动布局模式
            directoryPanel.SizeChanged += DirectoryPanel_SizeChanged;
            
            Label directoryLabel = new Label();
            directoryLabel.Name = "directoryLabel";
            directoryLabel.Text = language["inDir"][currentLanguage];
            directoryLabel.Font = defaultFont;
            directoryLabel.Width = 100;
            directoryLabel.Height = 34;
            directoryLabel.Location = new Point(0, 0);
            directoryLabel.TextAlign = ContentAlignment.MiddleLeft;

            CustomTextBox directoryEntry = new CustomTextBox();
            directoryEntry.Name = "directoryEntry";
            directoryEntry.Text = Environment.CurrentDirectory;
            directoryEntry.Height = 40;
            directoryEntry.Font = directoryFont;
            directoryEntry.Left = 110; // 标签右侧
            directoryEntry.Width = directoryPanel.Width - 200; // 留出按钮的空间
            directoryEntry.Top = 0;
            directoryEntry.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Button browseButton = new Button();
            browseButton.Name = "browseButton";
            browseButton.Text = language["inButton"][currentLanguage];
            browseButton.Width = 80;
            browseButton.Height = 34; // 与文件夹选择框高度保持一致
            browseButton.Left = directoryPanel.Width - 80;
            browseButton.Top = 0;

            browseButton.Click += BrowseButton_Click;
            
            // 按顺序添加控件
            directoryPanel.Controls.Add(directoryLabel);
            directoryPanel.Controls.Add(directoryEntry);
            directoryPanel.Controls.Add(browseButton);
            
            // 设置默认焦点到文件夹选择框
            this.ActiveControl = directoryEntry;

            // 4. 单选按钮
            Panel radioPanel = new Panel();
            radioPanel.Dock = DockStyle.Fill;
            radioPanel.Width = mainLayout.Width - 40;
            mainLayout.Controls.Add(radioPanel, 0, 3);

            RadioButton radioButtonOriginal = new RadioButton();
            radioButtonOriginal.Name = "radioButtonOriginal";
            radioButtonOriginal.Text = language["choiceButtonOriginal"][currentLanguage];
            radioButtonOriginal.Font = defaultFont;
            radioButtonOriginal.Location = new Point(10, 10);
            radioButtonOriginal.AutoSize = true;
            radioButtonOriginal.CheckedChanged += (s, e) => { if (radioButtonOriginal.Checked) outputChoice = "original"; };

            RadioButton radioButtonCustom1 = new RadioButton();
            radioButtonCustom1.Name = "radioButtonCustom1";
            radioButtonCustom1.Text = language["choiceButtonCustom1"][currentLanguage];
            radioButtonCustom1.Font = defaultFont;
            radioButtonCustom1.Location = new Point(10, 35);
            radioButtonCustom1.AutoSize = true;
            radioButtonCustom1.Checked = true;
            radioButtonCustom1.CheckedChanged += (s, e) => { if (radioButtonCustom1.Checked) outputChoice = "custom1"; };

            RadioButton radioButtonCustom2 = new RadioButton();
            radioButtonCustom2.Name = "radioButtonCustom2";
            radioButtonCustom2.Text = language["choiceButtonCustom2"][currentLanguage];
            radioButtonCustom2.Font = defaultFont;
            radioButtonCustom2.Location = new Point(10, 60);
            radioButtonCustom2.AutoSize = true;
            radioButtonCustom2.CheckedChanged += (s, e) => { if (radioButtonCustom2.Checked) outputChoice = "custom2"; };

            radioPanel.Controls.Add(radioButtonOriginal);
            radioPanel.Controls.Add(radioButtonCustom1);
            radioPanel.Controls.Add(radioButtonCustom2);

            // 5. 处理结果标签
            Label resultLabel = new Label();
            resultLabel.Name = "resultLabel";
            resultLabel.Text = language["resultUI"][currentLanguage];
            resultLabel.Font = defaultFont;
            resultLabel.Dock = DockStyle.Fill;
            resultLabel.TextAlign = ContentAlignment.MiddleLeft;
            mainLayout.Controls.Add(resultLabel, 0, 4);

            // 6. 文本区域
            RichTextBox textArea = new RichTextBox();
            textArea.Name = "textArea";
            textArea.Dock = DockStyle.Fill;
            textArea.ReadOnly = true;
            textArea.Margin = new Padding(0, 0, 0, 10);
            mainLayout.Controls.Add(textArea, 0, 5);

            // 7. 进度条
            ProgressBar progressBar = new ProgressBar();
            progressBar.Name = "progressBar";
            progressBar.Dock = DockStyle.Fill;
            progressBar.Value = 0;
            mainLayout.Controls.Add(progressBar, 0, 6);

            // 8. 开始按钮（居中）
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Fill;
            mainLayout.Controls.Add(buttonPanel, 0, 7);

            Button executeButton = new Button();
            executeButton.Name = "executeButton";
            executeButton.Text = language["startButton"][currentLanguage];
            executeButton.Size = new Size(100, 40);
            executeButton.Location = new Point((buttonPanel.Width - 100) / 2, 10);
            executeButton.Click += ExecuteButton_Click;
            buttonPanel.Controls.Add(executeButton);

            // 9. 介绍
            Label introductionLabel = new Label();
            introductionLabel.Name = "introductionLabel";
            introductionLabel.Text = language["introduction"][currentLanguage];
            introductionLabel.Font = defaultFont;
            introductionLabel.Dock = DockStyle.Fill;
            introductionLabel.TextAlign = ContentAlignment.TopLeft;
            introductionLabel.Margin = new Padding(0, 5, 0, 0); // 减小上下边距，特别是下边距
            introductionLabel.AutoSize = false;
            introductionLabel.MaximumSize = new Size(mainLayout.Width - 40, 0);
            // 移除WordWrap属性设置，Label控件没有此属性
            // 使用AutoSize=false和设置合适的大小来实现文本换行
            mainLayout.Controls.Add(introductionLabel, 0, 8);

            // 10. 底部状态栏
            StatusStrip statusStrip = new StatusStrip();
            statusStrip.Name = "statusStrip";
            statusStrip.Dock = DockStyle.Bottom;
            this.Controls.Add(statusStrip);
            
            // 创建左对齐的作者信息标签
            ToolStripStatusLabel authorLabel = new ToolStripStatusLabel();
            authorLabel.Name = "authorLabel";
            authorLabel.Text = language["by"][currentLanguage];
            authorLabel.ForeColor = Color.Gray;
            authorLabel.Spring = false; // 不自动拉伸
            authorLabel.Alignment = ToolStripItemAlignment.Left;
            statusStrip.Items.Add(authorLabel);
            
            // 添加弹簧标签填充中间空间，实现两端对齐
            ToolStripStatusLabel springLabel = new ToolStripStatusLabel();
            springLabel.Spring = true; // 自动拉伸填充剩余空间
            statusStrip.Items.Add(springLabel);
            
            // 创建右对齐的工具名+版本号标签
            ToolStripStatusLabel versionLabel = new ToolStripStatusLabel();
            versionLabel.Name = "versionLabel";
            versionLabel.Text = string.Format("恐怖黎明任务和对话文本提取工具 {0}", VERSION);
            versionLabel.ForeColor = Color.Gray;
            versionLabel.Alignment = ToolStripItemAlignment.Right;
            versionLabel.Spring = false; // 不自动拉伸
            statusStrip.Items.Add(versionLabel);
        }

        private void InitializeBackgroundWorker()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        private void LanguageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox?.SelectedItem is ComboBoxItem item)
            {
                currentLanguage = item.Value as string;
                UpdateUITexts();
            }
        }

        // 创建字体时使用回退机制的辅助方法
        private Font CreateFontWithFallback(string fontName, float size, FontStyle style = FontStyle.Regular)
        {
            try
            {
                // 检查指定的字体是否可用
                using (Font testFont = new Font(fontName, size, style))
                {
                    // 如果字体名称不匹配，说明系统使用了替代字体
                    if (testFont.Name.Equals(fontName, StringComparison.OrdinalIgnoreCase))
                    {
                        return testFont.Clone() as Font;
                    }
                }
            }
            catch
            {
                // 如果创建字体失败，使用默认字体但保持原始字号
            }
            
            // 返回系统默认字体，但保持原始请求的字号和样式
            return new Font(SystemFonts.DefaultFont.FontFamily, size, style);
        }

        private void DirectoryPanel_SizeChanged(object sender, EventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel != null)
            {
                CustomTextBox directoryEntry = panel.Controls.Find("directoryEntry", false).FirstOrDefault() as CustomTextBox;
                Button browseButton = panel.Controls.Find("browseButton", false).FirstOrDefault() as Button;
                
                if (directoryEntry != null && browseButton != null)
                {
                    directoryEntry.Width = panel.Width - 200; // 调整输入框宽度
                    browseButton.Left = panel.Width - 80; // 调整按钮位置
                }
            }
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
            {
                folderBrowser.Description = language["inDir"][currentLanguage];
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    CustomTextBox directoryEntry = this.Controls.Find("directoryEntry", true).FirstOrDefault() as CustomTextBox;
                    if (directoryEntry != null)
                    {
                        directoryEntry.Text = folderBrowser.SelectedPath;
                    }
                }
            }
        }

        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            CustomTextBox directoryEntry = this.Controls.Find("directoryEntry", true).FirstOrDefault() as CustomTextBox;
            RichTextBox textArea = this.Controls.Find("textArea", true).FirstOrDefault() as RichTextBox;
            ProgressBar progressBar = this.Controls.Find("progressBar", true).FirstOrDefault() as ProgressBar;
            Button executeButton = sender as Button;

            if (directoryEntry != null && textArea != null && progressBar != null && executeButton != null)
            {
                string directory = directoryEntry.Text;
                if (!Directory.Exists(directory))
                {
                    MessageBox.Show("目录不存在，请重新选择！", language["error"][currentLanguage], MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 清空文本区域并重置进度条
                textArea.Clear();
                progressBar.Value = 0;

                // 禁用开始按钮
                executeButton.Enabled = false;

                // 开始后台工作
                backgroundWorker.RunWorkerAsync(new WorkerArgs { Directory = directory, OutputChoice = outputChoice });
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is WorkerArgs args)
            {
                string directory = args.Directory;
                string outputChoice = args.OutputChoice;
                int processedCount = 0;
                DateTime startTime = DateTime.Now;

                // 计算总文件数
                var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories)
                    .Where(f => f.EndsWith(".qst", StringComparison.OrdinalIgnoreCase) || 
                                f.EndsWith(".cnv", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                int totalFiles = files.Count;
                
                // 检查是否有文件可处理
                if (totalFiles == 0)
                {
                    // 将错误消息输出到消息区域
                    ReportStatus(language["noFiles"][currentLanguage]);
                    return;
                }

                // 报告开始时间
                ReportStatus(string.Format(language["startProcessing"][currentLanguage], startTime.ToString("yyyy-MM-dd HH:mm:ss")));

                foreach (string fullPath in files)
                {
                    if (backgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    processedCount++;
                    string fileName = Path.GetFileName(fullPath);
                    string fileExtension = Path.GetExtension(fullPath);
                    string fileNameNoExt = Path.GetFileNameWithoutExtension(fullPath);

                    try
                    {
                        // 读取文件内容
                        byte[] binaryContent = File.ReadAllBytes(fullPath); //读取整个文件的所有字节内容
                        string content = Encoding.UTF8.GetString(binaryContent, 0, binaryContent.Length); //将整个字节数组转换为UTF-8字符串

                        // 处理内容
                        if (content.Contains("enUS")) //检查内容是否包含"enUS"字符串
                        {
                            int index = content.IndexOf("enUS"); //如果包含，找到"enUS"的索引位置
                            content = content.Substring(index + 12); //从索引位置+12开始截取后续所有内容（跳过"enUS"及其后12个字符）
                        }

                        // 处理字节数组
                        byte[] processedContent = Encoding.UTF8.GetBytes(content); //将处理后的字符串转换为UTF-8字节数组
                        processedContent = ReplaceBytes(processedContent, Encoding.UTF8.GetBytes("\n"), Encoding.UTF8.GetBytes("{^n}")); //将所有换行符"\n"替换为"{^n}"标记

                        int nullPos = Array.LastIndexOf(processedContent, (byte)0); //从处理后的字节数组中查找最后一个出现的空字符(0x00)的索引位置
                        while (nullPos > 3)
                        {
                            // 替换为换行符,将换行符("\n")的UTF-8字节复制到 nullPos - 3 的位置，长度为1
                            Array.Copy(Encoding.UTF8.GetBytes("\r\n"), 0, processedContent, nullPos - 3, 1);
                            // 填充其他位置为空格
                            for (int i = nullPos - 2; i <= nullPos; i++) 
                            {
                                processedContent[i] = 0x20; // 空格
                            }
                            nullPos = Array.LastIndexOf(processedContent, (byte)0); 
                        }

                        // 清理内容，过滤无效字符
                        string utf8Str = Encoding.UTF8.GetString(processedContent, 0, processedContent.Length);
                        // 过滤掉控制字符（除了换行和回车）
                        utf8Str = Regex.Replace(utf8Str, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", "");
                        // 过滤掉不可打印的Unicode字符
                        utf8Str = Regex.Replace(utf8Str, @"[\uD800-\uDFFF]", ""); // 过滤掉代理对
                        utf8Str = Regex.Replace(utf8Str, @" {2,}", " ");
                        // 过滤掉Unicode替换字符�（\ufffd）
                        utf8Str = Regex.Replace(utf8Str, @"[\ufffd]", "");
                        // 使用更接近Python splitlines()的方式分割行
                        // 先将\r\n替换为\n，再以\n分割为多行存储到lines数组中
                        utf8Str = utf8Str.Replace("\r\n", "\n");
                        utf8Str = utf8Str.Replace("\r", "\n");
                        //以\n分割为多行存储到lines数组中
                        string[] lines = utf8Str.Split(new[] { '\n' }, StringSplitOptions.None);
                        List<string> cleanedLines = new List<string>();

                        foreach (string line in lines)
                        {
                            // 先移除行首和行尾的空白字符
                            string cleanedLine = line.Trim();

                            while (cleanedLine.EndsWith("{^n}"))
                            {
                                cleanedLine = cleanedLine.Substring(0, cleanedLine.Length - 4);
                                cleanedLine = cleanedLine.TrimEnd();
                            }
                            while (cleanedLine.EndsWith("{^n"))
                            {
                                cleanedLine = cleanedLine.Substring(0, cleanedLine.Length - 3);
                                cleanedLine = cleanedLine.TrimEnd();
                            }
                            cleanedLine = cleanedLine.TrimEnd();
                            cleanedLines.Add(cleanedLine);
                        }
                        
                        // 重新组合，使用Windows风格的\r\n作为换行符
                        string utf8StrCleaned = string.Join("\r\n", cleanedLines);
                        if (fileExtension.Equals(".qst", StringComparison.OrdinalIgnoreCase))
                        {
                            utf8StrCleaned += "\r\n";
                        }
                        else
                        {
                            utf8StrCleaned += "\r\n";
                        }

                        // 确定输出路径
                        string outputFilePath;
                        if (outputChoice == "original")
                        {
                            outputFilePath = Path.Combine(Path.GetDirectoryName(fullPath), fileNameNoExt + ".txt");
                        }
                        else if (outputChoice == "custom1")
                        {
                            string targetDir = Path.Combine(directory, "CnvQstToTxtNoLevel");
                            Directory.CreateDirectory(targetDir);
                            outputFilePath = Path.Combine(targetDir, fileNameNoExt + ".txt");
                        }
                        else // custom2
                        {
                            string relativePath = Path.GetRelativePath(directory, fullPath);
                            string targetDir = Path.Combine(directory, "CnvQstToTxtKeepLevel", Path.GetDirectoryName(relativePath));
                            Directory.CreateDirectory(targetDir);
                            outputFilePath = Path.Combine(targetDir, fileNameNoExt + ".txt");
                        }

                        // 保存文件，使用不带BOM的UTF-8编码
                        File.WriteAllText(outputFilePath, utf8StrCleaned, new UTF8Encoding(false));

                        // 报告进度
                        ReportStatus($"{processedCount}:{fullPath}");
                        int progress = (int)((double)processedCount / totalFiles * 100);
                        backgroundWorker.ReportProgress(progress);
                    }
                    catch (Exception ex)
                    {
                        ReportStatus(string.Format(language["errorFile"][currentLanguage], fullPath, ex.Message));
                    }
                }

                // 报告结束时间
                DateTime endTime = DateTime.Now;
                double secondsPassed = (endTime - startTime).TotalSeconds;
                ReportStatus(string.Format(language["endTime"][currentLanguage], 
                    endTime.ToString("yyyy-MM-dd HH:mm:ss"), processedCount, secondsPassed));

                e.Result = processedCount;
            }
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar progressBar = this.Controls.Find("progressBar", true).FirstOrDefault() as ProgressBar;
            if (progressBar != null)
            {
                progressBar.Value = e.ProgressPercentage;
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Button executeButton = this.Controls.Find("executeButton", true).FirstOrDefault() as Button;
            if (executeButton != null)
            {
                executeButton.Enabled = true;
            }

            if (e.Cancelled)
            {
                ReportStatus(language["failed"][currentLanguage]);
            }
            else if (e.Error != null)
            {
                MessageBox.Show($"{language["failed"][currentLanguage]}: {e.Error.Message}", 
                    language["error"][currentLanguage], MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // 只有当有处理文件时才显示完成消息
                if (e.Result != null && (int)e.Result > 0)
                {
                    ReportStatus(language["resultFinished"][currentLanguage]);
                    CustomTextBox directoryEntry = this.Controls.Find("directoryEntry", true).FirstOrDefault() as CustomTextBox;
                    if (directoryEntry != null)
                    {
                        string directory = directoryEntry.Text;
                        if (outputChoice == "original")
                        {
                            ReportStatus(language["resultOriginal"][currentLanguage]);
                        }
                        else if (outputChoice == "custom1")
                        {
                            string targetDir = Path.Combine(directory, "CnvQstToTxtNoLevel");
                            ReportStatus(string.Format(language["resultCustom1"][currentLanguage], targetDir));
                        }
                        else if (outputChoice == "custom2")
                        {
                            string targetDir = Path.Combine(directory, "CnvQstToTxtKeepLevel");
                            ReportStatus(string.Format(language["resultCustom2"][currentLanguage], targetDir));
                        }
                    }
                }
            }
        }

        private void ReportStatus(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action<string>)ReportStatus, message);
            }
            else
            {
                RichTextBox textArea = this.Controls.Find("textArea", true).FirstOrDefault() as RichTextBox;
                if (textArea != null)
                {
                    textArea.AppendText(message + Environment.NewLine);
                    textArea.ScrollToCaret();
                }
            }
        }

        private void UpdateUITexts()
        {
            this.Text = language["topTitle"][currentLanguage];

            Label toolNameLabel = this.Controls.Find("toolNameLabel", true).FirstOrDefault() as Label;
            if (toolNameLabel != null) toolNameLabel.Text = language["bigTitle"][currentLanguage];

            Label directoryLabel = this.Controls.Find("directoryLabel", true).FirstOrDefault() as Label;
            if (directoryLabel != null) directoryLabel.Text = language["inDir"][currentLanguage];

            Button browseButton = this.Controls.Find("browseButton", true).FirstOrDefault() as Button;
            if (browseButton != null) browseButton.Text = language["inButton"][currentLanguage];

            RadioButton radioButtonOriginal = this.Controls.Find("radioButtonOriginal", true).FirstOrDefault() as RadioButton;
            if (radioButtonOriginal != null) radioButtonOriginal.Text = language["choiceButtonOriginal"][currentLanguage];

            RadioButton radioButtonCustom1 = this.Controls.Find("radioButtonCustom1", true).FirstOrDefault() as RadioButton;
            if (radioButtonCustom1 != null) radioButtonCustom1.Text = language["choiceButtonCustom1"][currentLanguage];

            RadioButton radioButtonCustom2 = this.Controls.Find("radioButtonCustom2", true).FirstOrDefault() as RadioButton;
            if (radioButtonCustom2 != null) radioButtonCustom2.Text = language["choiceButtonCustom2"][currentLanguage];

            Label resultLabel = this.Controls.Find("resultLabel", true).FirstOrDefault() as Label;
            if (resultLabel != null) resultLabel.Text = language["resultUI"][currentLanguage];

            Button executeButton = this.Controls.Find("executeButton", true).FirstOrDefault() as Button;
            if (executeButton != null) executeButton.Text = language["startButton"][currentLanguage];

            Label introductionLabel = this.Controls.Find("introductionLabel", true).FirstOrDefault() as Label;
            if (introductionLabel != null) introductionLabel.Text = language["introduction"][currentLanguage];

            // 更新状态栏文本
            StatusStrip statusStrip = this.Controls.Find("statusStrip", true).FirstOrDefault() as StatusStrip;
            if (statusStrip != null)
            {
                // 更新左对齐的作者信息标签
                ToolStripStatusLabel authorLabel = statusStrip.Items.Find("authorLabel", false).FirstOrDefault() as ToolStripStatusLabel;
                if (authorLabel != null)
                {
                    authorLabel.Text = language["by"][currentLanguage];
                }
                
                // 更新右对齐的工具名+版本号标签
                ToolStripStatusLabel versionLabel = statusStrip.Items.Find("versionLabel", false).FirstOrDefault() as ToolStripStatusLabel;
                if (versionLabel != null)
                {
                    string toolName = currentLanguage == "cn" ? "恐怖黎明任务和对话文本提取工具" : "Grim Dawn Quests and Conversations Tool";
                    versionLabel.Text = string.Format("{0} {1}", toolName, VERSION);
                }
            }
        }

        private byte[] ReplaceBytes(byte[] source, byte[] search, byte[] replace)
        {
            if (source == null || search == null || replace == null || search.Length == 0)
                return source;

            List<byte> result = new List<byte>();
            int i = 0;

            while (i <= source.Length - search.Length)
            {
                bool found = true;
                for (int j = 0; j < search.Length; j++)
                {
                    if (source[i + j] != search[j])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    result.AddRange(replace);
                    i += search.Length;
                }
                else
                {
                    result.Add(source[i]);
                    i++;
                }
            }

            // 添加剩余字节
            for (; i < source.Length; i++)
            {
                result.Add(source[i]);
            }

            return result.ToArray();
        }

        private class WorkerArgs
        {
            public string Directory { get; set; } = string.Empty;
            public string OutputChoice { get; set; } = string.Empty;
        }

        private class ComboBoxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public ComboBoxItem(string text, object value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString()
            {
                return Text;
            }
        }
    }

    // 自定义TextBox支持拖放和右键菜单
    public class CustomTextBox : TextBox
    {
        public CustomTextBox()
        {
            this.AllowDrop = true;
            this.ContextMenuStrip = CreateContextMenu();
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            base.OnDragEnter(e);
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    this.Text = files[0];
                }
            }
            base.OnDragDrop(e);
        }

        private ContextMenuStrip CreateContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem copyItem = new ToolStripMenuItem("复制");
            ToolStripMenuItem pasteItem = new ToolStripMenuItem("粘贴");
            ToolStripMenuItem cutItem = new ToolStripMenuItem("剪切");
            ToolStripMenuItem selectAllItem = new ToolStripMenuItem("全选");

            copyItem.Click += (s, e) => this.Copy();
            pasteItem.Click += (s, e) => this.Paste();
            cutItem.Click += (s, e) => this.Cut();
            selectAllItem.Click += (s, e) => this.SelectAll();

            menu.Items.AddRange(new ToolStripItem[] { copyItem, pasteItem, cutItem, selectAllItem });
            return menu;
        }
    }

    // 辅助类用于Worker - 暂时保留但不使用
    public class Worker
    {
        public string Directory { get; set; }
        public string OutputChoice { get; set; }

        public Worker(string directory, string outputChoice)
        {
            Directory = directory;
            OutputChoice = outputChoice;
        }
    }
}