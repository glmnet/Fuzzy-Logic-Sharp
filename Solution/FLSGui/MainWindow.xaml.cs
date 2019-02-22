using System;
using System.Windows;
using FLS;
using FLS.MembershipFunctions;
using FLS.Rules;

namespace FLSGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IFuzzyEngine fuzzySteering;
        private IFuzzyEngine fuzzySpeed;

        public MainWindow()
        {
            InitializeComponent();

            //Arrange
            var trackError = new LinguisticVariable("TrackError");
            var teNL = trackError.MembershipFunctions.AddTriangle("TeNL", -1, -1, -.5);
            var teNM = trackError.MembershipFunctions.AddTriangle("TeNM", -1, -.5, -.1);
            var teNS = trackError.MembershipFunctions.AddTriangle("TeNS", -.5, -.1, 0);
            var teZ = trackError.MembershipFunctions.AddTriangle("TeZ", -.1, 0, .1);
            var tePS = trackError.MembershipFunctions.AddTriangle("TePS", 0, .1, .5);
            var tePM = trackError.MembershipFunctions.AddTriangle("TePM", .1, .5, 1);
            var tePL = trackError.MembershipFunctions.AddTriangle("TePL", .5, 1, 1);

            var estimatedTrackError = new LinguisticVariable("EstimatedTrackError");
            var eteNL = estimatedTrackError.MembershipFunctions.AddTriangle("EteNL", -1, -1, -.5);
            var eteNM = estimatedTrackError.MembershipFunctions.AddTriangle("EteNM", -1, -.5, -.3);
            var eteNS = estimatedTrackError.MembershipFunctions.AddTriangle("EteNS", -.5, -.3, 0);
            var eteZ = estimatedTrackError.MembershipFunctions.AddTriangle("EteZ", -.3, 0, .3);
            var etePS = estimatedTrackError.MembershipFunctions.AddTriangle("EtePS", 0, .3, .5);
            var etePM = estimatedTrackError.MembershipFunctions.AddTriangle("EtePM", .3, .5, 1);
            var etePL = estimatedTrackError.MembershipFunctions.AddTriangle("EtePL", .5, 1, 1);

            var steering = new LinguisticVariable("Steering");
            var sNL = steering.MembershipFunctions.AddTriangle("sNL", -1, -1, -.6);
            var sNM = steering.MembershipFunctions.AddTriangle("sNM", -1, -.6, -.3);
            var sNS = steering.MembershipFunctions.AddTriangle("sNS", -.6, -.3, 0);
            var sZ_ = steering.MembershipFunctions.AddTriangle("sZ", -.3, 0, .3);
            var sPS = steering.MembershipFunctions.AddTriangle("sPS", 0, .3, .6);
            var sPM = steering.MembershipFunctions.AddTriangle("sPM", .3, .6, 1);
            var sPL = steering.MembershipFunctions.AddTriangle("sPL", .6, 1, 1);

            var speed = new LinguisticVariable("Speed");
            var spdZ = speed.MembershipFunctions.AddTriangle("spdZ", 0, 0, .1);
            var spdS = speed.MembershipFunctions.AddTriangle("spdS", 0, .1, .9);
            var spdM = speed.MembershipFunctions.AddTriangle("spdM", .1, .9, 1);
            var spdL = speed.MembershipFunctions.AddTriangle("spdL", .9, 1, 1);

            fuzzySteering = new FuzzyEngineFactory().Default();fuzzySpeed = new FuzzyEngineFactory().Default();
            

            var teCols = new[] { teNL, teNM, teNS, teZ, tePS, tePM, tePL };
            var eteRows = new[] { eteNL, eteNM, eteNS, eteZ, etePS, etePM, etePL };
            var steerRulesMatrix = new[]
            {
                sPM, sPS, sZ_, sZ_, sNL, sNL, sNL,
                sPL, sPS, sPS, sPS, sPS, sZ_, sNL,
                sPL, sPM, sPS, sPS, sZ_, sZ_, sNL,
                sPL, sPM, sPS, sZ_, sNS, sNM, sNL,
                sPL, sZ_, sZ_, sNS, sNS, sNM, sNL,
                sPL, sZ_, sNS, sNS, sNS, sNS, sNL,
                sPL, sPL, sPL, sZ_, sZ_, sNS, sNM
            };
            var speedRulesMatrix = new[]
            {
                spdM, spdS, spdS, spdS, spdS, spdZ, spdZ,
                spdS, spdM, spdM, spdM, spdM, spdM, spdS,
                spdZ, spdS, spdL, spdL, spdL, spdM, spdS,
                spdS, spdM, spdL, spdL, spdL, spdM, spdS,
                spdS, spdM, spdL, spdL, spdL, spdS, spdZ,
                spdS, spdM, spdM, spdM, spdM, spdM, spdS,
                spdZ, spdZ, spdS, spdS, spdS, spdS, spdM
            };


            for (int i = 0; i < eteRows.Length; i++)
                for (int j = 0; j < teCols.Length; j++)
                {
                    fuzzySteering.Rules.Add(
                        Rule.If($"Steering rule {i} {j} ({i * 7 + j})", trackError.Is(teCols[j]).And(estimatedTrackError.Is(eteRows[i]))).Then(
                            steering.Is(steerRulesMatrix[i * 7 + j])));
                    fuzzySpeed.Rules.Add(
                        Rule.If($"Speed rule {i} {j} ({i * 7 + j})", trackError.Is(teCols[j]).And(estimatedTrackError.Is(eteRows[i]))).Then(
                            speed.Is(speedRulesMatrix[i * 7 + j])));
                }

            //fuzzySteering = new FuzzyEngineFactory().Default();

            //fuzzySteering.Rules.Add(new FuzzyRule[] {
            //    Rule.If(trackError.Is(tePM).And(estimatedTrackError.Is(eteNL))).Then(steering.Is(sNL)),
            //    Rule.If(trackError.Is(teNL).And(estimatedTrackError.Is(eteNM))).Then(steering.Is(sPL)),
            //    Rule.If(trackError.Is(teZ).And(estimatedTrackError.Is(eteNM))).Then(steering.Is(sPS)),
            //    Rule.If(trackError.Is(tePM).And(estimatedTrackError.Is(eteNM))).Then(steering.Is(sPS)),
            //    Rule.If(trackError.Is(tePL).And(estimatedTrackError.Is(eteNM))).Then(steering.Is(sNL)),
            //    Rule.If(trackError.Is(teNM).And(estimatedTrackError.Is(eteZ))).Then(steering.Is(sPL)),
            //    Rule.If(trackError.Is(tePM).And(estimatedTrackError.Is(eteZ))).Then(steering.Is(sNL)),
            //    Rule.If(trackError.Is(teNL).And(estimatedTrackError.Is(etePM))).Then(steering.Is(sPL)),
            //    Rule.If(trackError.Is(teNM).And(estimatedTrackError.Is(etePM))).Then(steering.Is(sNS)),
            //    Rule.If(trackError.Is(teZ).And(estimatedTrackError.Is(etePM))).Then(steering.Is(sNS)),
            //    Rule.If(trackError.Is(tePL).And(estimatedTrackError.Is(etePM))).Then(steering.Is(sNL)),
            //    Rule.If(trackError.Is(teNM).And(estimatedTrackError.Is(etePL))).Then(steering.Is(sPL)) });

            //fuzzySpeed = new FuzzyEngineFactory().Default();
            //fuzzySpeed.Rules.Add(new FuzzyRule[] {
            //    Rule.If(trackError.Is(tePL).And(estimatedTrackError.Is(eteNL))).Then(speed.Is(spdZ)),
            //    Rule.If(trackError.Is(teNL).And(estimatedTrackError.Is(eteZ))).Then(speed.Is(spdS)),
            //    Rule.If(trackError.Is(teZ).And(estimatedTrackError.Is(eteZ))).Then(speed.Is(spdL)),
            //    Rule.If(trackError.Is(tePL).And(estimatedTrackError.Is(eteZ))).Then(speed.Is(spdS)),
            //    Rule.If(trackError.Is(teNL).And(estimatedTrackError.Is(etePL))).Then(speed.Is(spdZ)),
            //    });

        }
        public double Map(double value, double fromSource, double toSource, double fromTarget, double toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }

        private void WaterSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (fuzzySpeed == null || fuzzySteering == null) return;

            TapeLine.X1 = Slider0.Margin.Left + Slider0.ActualWidth * (Slider0.Value + (Slider0.Maximum - Slider0.Minimum) / 2) / (Slider0.Maximum - Slider0.Minimum);
            TapeLine.Y1 = Slider0.Margin.Top + Slider0.ActualHeight / 2;

            Slider1.Margin = new Thickness(Slider1.Margin.Left, Slider0.Margin.Top + SliderDt.Value * SliderDt.Height / SliderDt.Maximum, Slider1.Margin.Right, Slider1.Margin.Bottom);

            TapeLine.X2 = Slider1.Margin.Left + Slider1.ActualWidth * (Slider1.Value + (Slider1.Maximum - Slider1.Minimum) / 2) / (Slider1.Maximum - Slider1.Minimum);
            TapeLine.Y2 = Slider1.Margin.Top + Slider1.ActualHeight / 2;

            var e0 = (int)Slider0.Value;
            var e1 = (int)Slider1.Value;
            var dt = SliderDt.Value;

            var h = 170;
            var eb = e1 - e0;

            double d;
            if (e0 == e1)
            {
                d = e0;
            }
            else
            {
                double alpha = Math.Atan2(e1 - e0, dt);
                d = Math.Sin(alpha) * (h + Math.Tan(Math.PI / 2 - alpha) * e0);
            }

            DeltaFromWheelCenter.Content = d.ToString();

            var data = new
            {
                trackError = Math.Min(1, Math.Max(-1, Map(e0, -80, 80, -1, 1))),
                EstimatedTrackError = Math.Min(1, Math.Max(-1, Map(d, -120, 120, -1, 1))),
            };

            Console.WriteLine("*****************************");
            var fStr = fuzzySteering.Defuzzify(data);
            var fSpd = fuzzySpeed.Defuzzify(data);
            LabelData.Content = $"trackError: {data.trackError} estimated error: {data.EstimatedTrackError} str: {fStr} fSpd: {fSpd}";

            Console.WriteLine(LabelData.Content.ToString());

            LabelSpeed.Content = SliderSpeed.Value = fSpd;
            LabelSteering.Content = SliderSteering.Value = fStr;
        }

        private void TestSyntax()
        {
            //var fl = new FuzzyLogic()
            //    .AddLinguisticVariable(out var steering)
            //        .AddTriangle(out var sPS, 1, 2, 3)
            //        .AddTriangle(out var sMS, 2, 3, 4)
            //    .AddLinguisticVariable(out var speed)
            //        .AddTriangle(out var sPSx, 1, 2, 3)
            //        .AddTriangle(out var sMSx, 2, 3, 4)
            //    .FuzzyLogic
            //        .Rules.If(speed.Is(sPs)).And(;

        }
    }

    public class FuzzySet
    {
        private readonly FuzzyLogic _fuzzyLogic;

        public FuzzySet(FuzzyLogic fuzzyLogic)
        {
            _fuzzyLogic = fuzzyLogic;
        }

        public FuzzySet AddTriangle(out FuzzyThing thing, double a, double b, double c)
        {
            thing = new FuzzyThing();
            return this;
        }

        public FuzzyLogic FuzzyLogic { get { return _fuzzyLogic; } }

        public FuzzySet AddLinguisticVariable(out FuzzySet fuzzySet)
        {
            return _fuzzyLogic.AddLinguisticVariable(out fuzzySet);
        }
    }

    public class FuzzyThing
    {

    }
    public class FuzzyRules
    {
        public FuzzyRules If(FuzzySet fs)
        {
            return null;
        }
    }
    public class FuzzyLogic
    {
        public FuzzySet AddLinguisticVariable(out FuzzySet fuzzySet)
        {
            fuzzySet = new FuzzySet(this);
            return fuzzySet;
        }

        public FuzzyRules Rules
        {
            get
            {
                return new FuzzyRules();
            }
        }

    }


}
