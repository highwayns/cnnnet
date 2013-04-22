namespace CnnNetLib
{
    partial class FormNetworkControl
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
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.radioButtonRunInfinity = new System.Windows.Forms.RadioButton();
            this.buttonReset = new System.Windows.Forms.Button();
            this.radioButtonSteps = new System.Windows.Forms.RadioButton();
            this.nudSteps = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxStepNumber = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonApplyParameters = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.npMaxNeuronMoveDistance = new System.Windows.Forms.NumericUpDown();
            this.npInputNeuronsMoveToHigherDesirability = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.npInputNeuronCount = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.npMinDistanceBetweenNeurons = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.npNeuronDesirabilityPlainRange = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.npPercentActiveNeurons = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.npDesirabilityDecayAmount = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.npMaxNeuronInfluence = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.npNeuronInfluenceRange = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.npNeuronDensity = new System.Windows.Forms.NumericUpDown();
            this.radioButtonStepByStep = new System.Windows.Forms.RadioButton();
            this.buttonNextStepByStep = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudSteps)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.npMaxNeuronMoveDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.npInputNeuronCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.npMinDistanceBetweenNeurons)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.npNeuronDesirabilityPlainRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.npPercentActiveNeurons)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.npDesirabilityDecayAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.npMaxNeuronInfluence)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.npNeuronInfluenceRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.npNeuronDensity)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(12, 32);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.OnButtonStartClick);
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(93, 32);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 2;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.OnButtonStopClick);
            // 
            // radioButtonRunInfinity
            // 
            this.radioButtonRunInfinity.AutoSize = true;
            this.radioButtonRunInfinity.Checked = true;
            this.radioButtonRunInfinity.Location = new System.Drawing.Point(12, 61);
            this.radioButtonRunInfinity.Name = "radioButtonRunInfinity";
            this.radioButtonRunInfinity.Size = new System.Drawing.Size(55, 17);
            this.radioButtonRunInfinity.TabIndex = 3;
            this.radioButtonRunInfinity.TabStop = true;
            this.radioButtonRunInfinity.Text = "Infinity";
            this.radioButtonRunInfinity.UseVisualStyleBackColor = true;
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(184, 32);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(75, 23);
            this.buttonReset.TabIndex = 4;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.OnButtonResetClick);
            // 
            // radioButtonSteps
            // 
            this.radioButtonSteps.AutoSize = true;
            this.radioButtonSteps.Location = new System.Drawing.Point(12, 91);
            this.radioButtonSteps.Name = "radioButtonSteps";
            this.radioButtonSteps.Size = new System.Drawing.Size(52, 17);
            this.radioButtonSteps.TabIndex = 5;
            this.radioButtonSteps.TabStop = true;
            this.radioButtonSteps.Text = "Steps";
            this.radioButtonSteps.UseVisualStyleBackColor = true;
            this.radioButtonSteps.CheckedChanged += new System.EventHandler(this.OnRadioButtonStepsCheckedChanged);
            // 
            // nudSteps
            // 
            this.nudSteps.Enabled = false;
            this.nudSteps.Location = new System.Drawing.Point(105, 92);
            this.nudSteps.Name = "nudSteps";
            this.nudSteps.Size = new System.Drawing.Size(52, 20);
            this.nudSteps.TabIndex = 6;
            this.nudSteps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Step Number";
            // 
            // textBoxStepNumber
            // 
            this.textBoxStepNumber.Location = new System.Drawing.Point(93, 6);
            this.textBoxStepNumber.Name = "textBoxStepNumber";
            this.textBoxStepNumber.ReadOnly = true;
            this.textBoxStepNumber.Size = new System.Drawing.Size(75, 20);
            this.textBoxStepNumber.TabIndex = 8;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonApplyParameters);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.npMaxNeuronMoveDistance);
            this.groupBox1.Controls.Add(this.npInputNeuronsMoveToHigherDesirability);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.npInputNeuronCount);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.npMinDistanceBetweenNeurons);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.npNeuronDesirabilityPlainRange);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.npPercentActiveNeurons);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.npDesirabilityDecayAmount);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.npMaxNeuronInfluence);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.npNeuronInfluenceRange);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.npNeuronDensity);
            this.groupBox1.Location = new System.Drawing.Point(290, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(247, 309);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parameters";
            // 
            // buttonApplyParameters
            // 
            this.buttonApplyParameters.Location = new System.Drawing.Point(157, 276);
            this.buttonApplyParameters.Name = "buttonApplyParameters";
            this.buttonApplyParameters.Size = new System.Drawing.Size(75, 23);
            this.buttonApplyParameters.TabIndex = 19;
            this.buttonApplyParameters.Text = "Apply";
            this.buttonApplyParameters.UseVisualStyleBackColor = true;
            this.buttonApplyParameters.Click += new System.EventHandler(this.OnButtonApplyParametersClick);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 252);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(140, 13);
            this.label10.TabIndex = 18;
            this.label10.Text = "Max Neuron Move Distance";
            // 
            // npMaxNeuronMoveDistance
            // 
            this.npMaxNeuronMoveDistance.Location = new System.Drawing.Point(173, 250);
            this.npMaxNeuronMoveDistance.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.npMaxNeuronMoveDistance.Name = "npMaxNeuronMoveDistance";
            this.npMaxNeuronMoveDistance.Size = new System.Drawing.Size(59, 20);
            this.npMaxNeuronMoveDistance.TabIndex = 17;
            this.npMaxNeuronMoveDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.npMaxNeuronMoveDistance.ThousandsSeparator = true;
            this.npMaxNeuronMoveDistance.Value = new decimal(new int[] {
            999,
            0,
            0,
            0});
            // 
            // npInputNeuronsMoveToHigherDesirability
            // 
            this.npInputNeuronsMoveToHigherDesirability.AutoSize = true;
            this.npInputNeuronsMoveToHigherDesirability.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.npInputNeuronsMoveToHigherDesirability.Location = new System.Drawing.Point(6, 227);
            this.npInputNeuronsMoveToHigherDesirability.Name = "npInputNeuronsMoveToHigherDesirability";
            this.npInputNeuronsMoveToHigherDesirability.Size = new System.Drawing.Size(226, 17);
            this.npInputNeuronsMoveToHigherDesirability.TabIndex = 16;
            this.npInputNeuronsMoveToHigherDesirability.Text = "Input Neurons Move To Higher Desirability";
            this.npInputNeuronsMoveToHigherDesirability.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 203);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "Input Neuron Count";
            // 
            // npInputNeuronCount
            // 
            this.npInputNeuronCount.Location = new System.Drawing.Point(173, 201);
            this.npInputNeuronCount.Name = "npInputNeuronCount";
            this.npInputNeuronCount.Size = new System.Drawing.Size(59, 20);
            this.npInputNeuronCount.TabIndex = 14;
            this.npInputNeuronCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.npInputNeuronCount.ThousandsSeparator = true;
            this.npInputNeuronCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 177);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(157, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "Min Distance Between Neurons";
            // 
            // npMinDistanceBetweenNeurons
            // 
            this.npMinDistanceBetweenNeurons.Location = new System.Drawing.Point(173, 175);
            this.npMinDistanceBetweenNeurons.Name = "npMinDistanceBetweenNeurons";
            this.npMinDistanceBetweenNeurons.Size = new System.Drawing.Size(59, 20);
            this.npMinDistanceBetweenNeurons.TabIndex = 12;
            this.npMinDistanceBetweenNeurons.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.npMinDistanceBetweenNeurons.ThousandsSeparator = true;
            this.npMinDistanceBetweenNeurons.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 151);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(156, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Neuron Desirability Plain Range";
            // 
            // npNeuronDesirabilityPlainRange
            // 
            this.npNeuronDesirabilityPlainRange.Location = new System.Drawing.Point(173, 149);
            this.npNeuronDesirabilityPlainRange.Name = "npNeuronDesirabilityPlainRange";
            this.npNeuronDesirabilityPlainRange.Size = new System.Drawing.Size(59, 20);
            this.npNeuronDesirabilityPlainRange.TabIndex = 10;
            this.npNeuronDesirabilityPlainRange.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.npNeuronDesirabilityPlainRange.ThousandsSeparator = true;
            this.npNeuronDesirabilityPlainRange.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 125);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Percent Active Neurons";
            // 
            // npPercentActiveNeurons
            // 
            this.npPercentActiveNeurons.DecimalPlaces = 2;
            this.npPercentActiveNeurons.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.npPercentActiveNeurons.Location = new System.Drawing.Point(173, 123);
            this.npPercentActiveNeurons.Name = "npPercentActiveNeurons";
            this.npPercentActiveNeurons.Size = new System.Drawing.Size(59, 20);
            this.npPercentActiveNeurons.TabIndex = 8;
            this.npPercentActiveNeurons.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.npPercentActiveNeurons.ThousandsSeparator = true;
            this.npPercentActiveNeurons.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 99);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(130, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Desirability Decay Amount";
            // 
            // npDesirabilityDecayAmount
            // 
            this.npDesirabilityDecayAmount.DecimalPlaces = 3;
            this.npDesirabilityDecayAmount.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.npDesirabilityDecayAmount.Location = new System.Drawing.Point(173, 97);
            this.npDesirabilityDecayAmount.Name = "npDesirabilityDecayAmount";
            this.npDesirabilityDecayAmount.Size = new System.Drawing.Size(59, 20);
            this.npDesirabilityDecayAmount.TabIndex = 6;
            this.npDesirabilityDecayAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.npDesirabilityDecayAmount.ThousandsSeparator = true;
            this.npDesirabilityDecayAmount.Value = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Max Neuron Influence";
            // 
            // npMaxNeuronInfluence
            // 
            this.npMaxNeuronInfluence.DecimalPlaces = 2;
            this.npMaxNeuronInfluence.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.npMaxNeuronInfluence.Location = new System.Drawing.Point(173, 71);
            this.npMaxNeuronInfluence.Name = "npMaxNeuronInfluence";
            this.npMaxNeuronInfluence.Size = new System.Drawing.Size(59, 20);
            this.npMaxNeuronInfluence.TabIndex = 4;
            this.npMaxNeuronInfluence.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.npMaxNeuronInfluence.ThousandsSeparator = true;
            this.npMaxNeuronInfluence.Value = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Neuron Influence Range";
            // 
            // npNeuronInfluenceRange
            // 
            this.npNeuronInfluenceRange.Location = new System.Drawing.Point(173, 45);
            this.npNeuronInfluenceRange.Name = "npNeuronInfluenceRange";
            this.npNeuronInfluenceRange.Size = new System.Drawing.Size(59, 20);
            this.npNeuronInfluenceRange.TabIndex = 2;
            this.npNeuronInfluenceRange.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.npNeuronInfluenceRange.ThousandsSeparator = true;
            this.npNeuronInfluenceRange.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Neuron Density";
            // 
            // npNeuronDensity
            // 
            this.npNeuronDensity.DecimalPlaces = 3;
            this.npNeuronDensity.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.npNeuronDensity.Location = new System.Drawing.Point(173, 19);
            this.npNeuronDensity.Name = "npNeuronDensity";
            this.npNeuronDensity.Size = new System.Drawing.Size(59, 20);
            this.npNeuronDensity.TabIndex = 0;
            this.npNeuronDensity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.npNeuronDensity.ThousandsSeparator = true;
            this.npNeuronDensity.Value = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            // 
            // radioButtonStepByStep
            // 
            this.radioButtonStepByStep.AutoSize = true;
            this.radioButtonStepByStep.Location = new System.Drawing.Point(12, 121);
            this.radioButtonStepByStep.Name = "radioButtonStepByStep";
            this.radioButtonStepByStep.Size = new System.Drawing.Size(87, 17);
            this.radioButtonStepByStep.TabIndex = 10;
            this.radioButtonStepByStep.TabStop = true;
            this.radioButtonStepByStep.Text = "Step By Step";
            this.radioButtonStepByStep.UseVisualStyleBackColor = true;
            this.radioButtonStepByStep.CheckedChanged += new System.EventHandler(this.OnRadioButtonStepByStepCheckedChanged);
            // 
            // buttonNextStepByStep
            // 
            this.buttonNextStepByStep.Location = new System.Drawing.Point(105, 118);
            this.buttonNextStepByStep.Name = "buttonNextStepByStep";
            this.buttonNextStepByStep.Size = new System.Drawing.Size(75, 23);
            this.buttonNextStepByStep.TabIndex = 11;
            this.buttonNextStepByStep.Text = "Next";
            this.buttonNextStepByStep.UseVisualStyleBackColor = true;
            this.buttonNextStepByStep.Click += new System.EventHandler(this.OnButtonNextStepByStepClick);
            // 
            // FormNetworkControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 330);
            this.ControlBox = false;
            this.Controls.Add(this.buttonNextStepByStep);
            this.Controls.Add(this.radioButtonStepByStep);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxStepNumber);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudSteps);
            this.Controls.Add(this.radioButtonSteps);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.radioButtonRunInfinity);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormNetworkControl";
            this.Text = "Controls";
            ((System.ComponentModel.ISupportInitialize)(this.nudSteps)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.npMaxNeuronMoveDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.npInputNeuronCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.npMinDistanceBetweenNeurons)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.npNeuronDesirabilityPlainRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.npPercentActiveNeurons)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.npDesirabilityDecayAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.npMaxNeuronInfluence)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.npNeuronInfluenceRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.npNeuronDensity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.RadioButton radioButtonRunInfinity;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.RadioButton radioButtonSteps;
        private System.Windows.Forms.NumericUpDown nudSteps;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxStepNumber;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown npNeuronDensity;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown npNeuronInfluenceRange;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown npMaxNeuronInfluence;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown npDesirabilityDecayAmount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown npPercentActiveNeurons;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown npNeuronDesirabilityPlainRange;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown npMinDistanceBetweenNeurons;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown npInputNeuronCount;
        private System.Windows.Forms.CheckBox npInputNeuronsMoveToHigherDesirability;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown npMaxNeuronMoveDistance;
        private System.Windows.Forms.Button buttonApplyParameters;
        private System.Windows.Forms.RadioButton radioButtonStepByStep;
        private System.Windows.Forms.Button buttonNextStepByStep;
    }
}