using DelftTools.Controls.Swf.Charting.Customized;

namespace DelftTools.Controls.Swf.Charting
{
    partial class ChartView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components;

        #region Component Designer generated code

        /// <summary> 
        ///   Required method for Designer support - do not modify the contents of 
        ///   this method with the code editor.
        /// UPDATE:
        ///  All chart related properties are moved to IChart and Chart.SetDefaultValues
        ///  InitializeComponent
        ///  Important change: the TChart is encapsulated inside Chart class and
        ///  this.teeChart = new Steema.TeeChart.TChart() removed from ChartViewInitializeComponent
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.teeChart = new DeltaShellTChart();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // teeChart
            // 
            this.teeChart.Aspect.View3D = false;
            this.teeChart.Aspect.ZOffset = 0D;
            this.teeChart.Axes.Bottom.Title.Transparent = true;
            this.teeChart.Axes.Depth.Title.Transparent = true;
            this.teeChart.Axes.DepthTop.Title.Transparent = true;
            this.teeChart.Axes.Left.Title.Transparent = true;
            this.teeChart.Axes.Right.Title.Transparent = true;
            this.teeChart.Axes.Top.Title.Transparent = true;
            this.teeChart.BackColor = System.Drawing.Color.Transparent;
            this.teeChart.Cursor = System.Windows.Forms.Cursors.Default;
            this.teeChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.teeChart.Header.Lines = new string[] {""};
            this.teeChart.Location = new System.Drawing.Point(0, 0);
            this.teeChart.Name = "Chart";
            this.teeChart.Size = new System.Drawing.Size(542, 368);
            this.teeChart.TabIndex = 0;
            this.teeChart.Walls.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.Timer1Tick);
            // 
            // ChartView
            // 
            this.Controls.Add(this.teeChart);
            this.Name = "ChartView";
            this.Size = new System.Drawing.Size(542, 368);
            this.ResumeLayout(false);

        }
        #endregion

        private DeltaShellTChart teeChart;
        private System.Windows.Forms.Timer timer1;
    }
}