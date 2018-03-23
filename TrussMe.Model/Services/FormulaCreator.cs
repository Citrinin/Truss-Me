using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Math;
using WP = DocumentFormat.OpenXml.Wordprocessing;
using Bar = TrussMe.Model.Entities.Bar;

namespace TrussMe.Model.Services
{
    public static class FormulaCreator
    {

        public static WP.Paragraph GenerateParagraphWithText(string content)
        {
            var p = new WP.Paragraph();
            var r = new WP.Run();
            var text = new WP.Text(content);
            r.AppendChild(text);
            p.AppendChild(r);
            return p;
        }

        public static WP.Run GenerateRunWithText(string content)
        {


            RunProperties runProperties = new RunProperties();
            WP.RunFonts runFonts = new WP.RunFonts() { Ascii = "Cambria Math", HighAnsi = "Cambria Math" };
            WP.Italic italic = new WP.Italic();

            runProperties.Append(runFonts);
            runProperties.Append(italic);
            return new WP.Run(runProperties,new WP.Text(content));
        }

        public static IEnumerable<WP.Paragraph> GenerateParagraphWithBarText(Bar bar)
        {
            return new List<WP.Paragraph>
            {
                CenterBigTextParagraph($"Расчет стержня №{bar.BarNumber} {bar.ElementType}"),
                TextParagraph($"Длина стержня: {bar.Length} мм"),
                TextParagraph($"Усилие в стержне: {bar.ActualForce} тс"),
                TextParagraph($"Момент в стержне: {bar.ActualMoment} тс*м")
            };
        }

        public static WP.Paragraph GenerateParagraphWithAxialTensionFormula(Bar bar, float Ry)
        {
            var p = new WP.Paragraph();
            var officeMath = new OfficeMath();

            var fractionFormula = CreateFraction(
                new List<OpenXmlElement> { GenerateRunWithText("N") },
                new List<OpenXmlElement> { GenerateRunWithText("A∙"), Index("R", "y") });

            var fraction = CreateFraction(
                new List<OpenXmlElement> { GenerateRunWithText($"{Math.Abs(bar.ActualForce)}") },
                new List<OpenXmlElement> { GenerateRunWithText($"{bar.Section.Area}∙{Ry}∙"), Power("10", "2") });


            officeMath.Append(
                fractionFormula,
                new Run(new Text(" = ")),
                fraction,
                new Run(new Text($" = {bar.CalcResult} " + (bar.CalcResult <= 1 ? "≤" : ">") + " 1")));
            p.AppendChild(officeMath);
            return p;
        }

        public static WP.Paragraph GenerateParagraphWithAxialCompressionFormula(Bar bar, float Ry, float fi, bool moment)
        {
            var p = new WP.Paragraph();
            var officeMath = new OfficeMath();
            OpenXmlElement fitext;
            if (moment)
            {
                fitext = Index("φ", "e");
            }
            else
            {
                fitext = GenerateRunWithText("φ");
            }


            var fractionFormula = CreateFraction(
                new List<OpenXmlElement> { GenerateRunWithText("N") },
                new List<OpenXmlElement> { GenerateRunWithText("A∙"), fitext, GenerateRunWithText("∙"), Index("R", "y") });

            var fraction = CreateFraction(
                new List<OpenXmlElement> { GenerateRunWithText($"{Math.Abs(bar.ActualForce)}") },
                new List<OpenXmlElement> { GenerateRunWithText($"{bar.Section.Area}∙{fi}∙{Ry}∙"), Power("10", "2"), });


            officeMath.Append(
                fractionFormula,
                new Run(new Text(" = ")),
                fraction,
                new Run(new Text($" = {bar.CalcResult} " + (bar.CalcResult <= 1 ? "≤" : ">") + " 1")));
            p.AppendChild(officeMath);
            return p;
        }

        public static WP.Paragraph MefParagraph(float m, float etta, float mef)
        {
            var p = new WP.Paragraph();
            var officeMath = new OfficeMath();

            officeMath.Append(
                Index("m","ef"),
                GenerateRunWithText($" = m ∙ η = {m} ∙ {etta} = {mef}"));
            p.AppendChild(officeMath);
            return p;
        }

        public static WP.Paragraph LambdaParagraph(Bar bar, float lambda)
        {
            var p = new WP.Paragraph();
            var officeMath = new OfficeMath();

            officeMath.Append(
                GenerateRunWithText($"λ = "),
                CreateFraction(
                    new List<OpenXmlElement> { Index("l", "x"), GenerateRunWithText($"∙"), Index("μ", "x") },
                    new List<OpenXmlElement> { Index("i", "x") }),
                GenerateRunWithText(" = "),
                CreateFraction(
                    new List<OpenXmlElement> { GenerateRunWithText($"{bar.Length} ∙ 1") },
                    new List<OpenXmlElement> { GenerateRunWithText($"{bar.Section.RadiusOfGyrationX} ∙ 10") }),
                GenerateRunWithText($" = {lambda}")
                );
            p.AppendChild(officeMath);
            return p;
        }

        public static WP.Paragraph EccentricityParagraph(Bar bar, float ecc)
        {
            var p = new WP.Paragraph();
            var officeMath = new OfficeMath();

            officeMath.Append(
                GenerateRunWithText($"e = "),
                CreateFraction(
                    new List<OpenXmlElement> { GenerateRunWithText($"M") },
                    new List<OpenXmlElement> { GenerateRunWithText($"N") }),
                GenerateRunWithText(" = "),
                CreateFraction(
                    new List<OpenXmlElement> { GenerateRunWithText($"{bar.ActualMoment}∙ 100") },
                    new List<OpenXmlElement> { GenerateRunWithText($"{Math.Abs(bar.ActualForce)}") }),
                GenerateRunWithText($" = {ecc * 100} cм")
            );
            p.AppendChild(officeMath);
            return p;
        }

        public static WP.Paragraph ReffEccentricityParagraph(Bar bar, float e, float m)
        {
            var p = new WP.Paragraph();
            var officeMath = new OfficeMath();

            officeMath.Append(
                GenerateRunWithText("m = e ∙ "),
                    CreateFraction(
                        new List<OpenXmlElement> { GenerateRunWithText("A") },
                        new List<OpenXmlElement> { GenerateRunWithText("W") }),
                GenerateRunWithText($" = {e} ∙ "),
                    CreateFraction(
                        new List<OpenXmlElement> { GenerateRunWithText($"{bar.Section.Area}") },
                        new List<OpenXmlElement> { GenerateRunWithText($"{bar.Section.SectionModulusX}") }),
                GenerateRunWithText($" = {m}")
            );
            p.AppendChild(officeMath);
            return p;
        }

        public static WP.Paragraph AreaFractionParagraph(Bar bar, float afaw)
        {
            var p = new WP.Paragraph();
            var officeMath = new OfficeMath();

            officeMath.Append(
                    CreateFraction(
                        new List<OpenXmlElement> { Index("A", "f") },
                        new List<OpenXmlElement> { Index("A", "w") }),
                GenerateRunWithText(" = "),

                    CreateFraction(
                        new List<OpenXmlElement> { Index("b", "f"), GenerateRunWithText(" ∙ "), Index("t", "f") },
                        new List<OpenXmlElement> { GenerateRunWithText("2 ∙ "), Index("h", "w"), GenerateRunWithText(" ∙ "), Index("t", "w") }),
                GenerateRunWithText(" = "),

                    CreateFraction(
                        new List<OpenXmlElement> { GenerateRunWithText($"{bar.Section.Width} ∙{bar.Section.Thickness}") },
                        new List<OpenXmlElement> { GenerateRunWithText($"2 ∙ {bar.Section.Height} ∙ {bar.Section.Thickness}") }),
                GenerateRunWithText($" = {afaw}")
            );
            p.AppendChild(officeMath);
            return p;
        }

        public static Radical CreateRadical(OpenXmlElement argument)
        {
            var rad = new Radical();
            var radprop = new RadicalProperties();
            var hideDegree = new HideDegree { Val = BooleanValues.One };
            var degree = new Degree();
            radprop.AppendChild(hideDegree);
            var bas = new Base();
            bas.AppendChild(argument);

            rad.Append(radprop, degree, bas);
            return rad;
        }

        public static WP.Paragraph LambdaRefParagraph(float lambda, float lambdaRef, float Ry, float E)
        {
            var p = new WP.Paragraph();
            var officeMath = new OfficeMath();

            var bar = new DocumentFormat.OpenXml.Math.Bar();
            var barProperties = new BarProperties();
            var position = new Position() { Val = VerticalJustificationValues.Top };
            barProperties.Append(position);

            var bas = new Base(new Run(new Text("λ")));
            bar.Append(barProperties, bas);

            officeMath.Append(
                bar,
                GenerateRunWithText(" = λ ∙ "),
                CreateRadical(
                    CreateFraction(
                        new List<OpenXmlElement> { Index("R", "y") },
                        new List<OpenXmlElement> { GenerateRunWithText("E") })),
                GenerateRunWithText($" = {lambda} "),

                CreateRadical(
                    CreateFraction(
                        new List<OpenXmlElement> { GenerateRunWithText($"{Ry}") },
                        new List<OpenXmlElement> { GenerateRunWithText($"{E}") })),
                GenerateRunWithText(" = "),
                GenerateRunWithText($" = {lambdaRef}")
            );
            p.AppendChild(officeMath);
            return p;
        }

        public static Fraction CreateFraction(IEnumerable<OpenXmlElement> upper, IEnumerable<OpenXmlElement> lower)
        {
            var fraction = new Fraction();

            var numerator = new Numerator();
            numerator.Append(upper);
            var denominator = new Denominator();
            denominator.Append(lower);
            fraction.AppendChild(numerator);
            fraction.AppendChild(denominator);
            return fraction;
        }

        public static Superscript Power(string bas, string power)
        {
            Superscript superscript = new Superscript();
            Base b = new Base(new Run(new Text(bas)));
            SuperArgument superArgument = new SuperArgument(new Run(new Text(power)));

            superscript.AppendChild(b);
            superscript.AppendChild(superArgument);
            return superscript;
        }

        public static Subscript Index(string bas, string index)
        {
            var subscript = new Subscript();
            Base b = new Base(new Run(new Text(bas)));
            var subArgument = new SubArgument(new Run(new Text(index)));

            subscript.AppendChild(b);
            subscript.AppendChild(subArgument);
            return subscript;
        }

        public static WP.Paragraph TextParagraph(string text)
        {
            return new WP.Paragraph(new WP.Run(new WP.Text(text)));
        }

        public static WP.Paragraph CenterBigTextParagraph(string text)
        {
            var pp = new WP.ParagraphProperties(new WP.Justification { Val = WP.JustificationValues.Center });
            var rp = new WP.RunProperties(new WP.FontSize() { Val = "32" }, new WP.Bold());
            return new WP.Paragraph(pp, new WP.Run(rp, new WP.Text(text)));
        }


    }
}
