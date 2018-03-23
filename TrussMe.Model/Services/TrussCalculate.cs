using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Math;
using TrussMe.Model.Entities;
using TrussMe.Model.Interfaces;
using WP = DocumentFormat.OpenXml.Wordprocessing;
using Bar = TrussMe.Model.Entities.Bar;


namespace TrussMe.Model.Services
{
    public static class TrussCalculate
    {
        private const float E = 206000;
        private const double TOLERANCE = 0.0001;

        public static IEnumerable<WP.Paragraph> Calculate(ProjectTruss projectTrussToCalculate, ISteelRepository steelRepository)
        {
            var barsCalculationParagraph = new List<WP.Paragraph>();
            var trussToCalculate = projectTrussToCalculate.Truss;
            var projectToCalculate = projectTrussToCalculate.Project;
            barsCalculationParagraph.Add(FormulaCreator.CenterBigTextParagraph($"Расчет фермы"));
            barsCalculationParagraph.Add(FormulaCreator.TextParagraph($"Проект: {projectToCalculate.Code} - {projectToCalculate.Description}"));
            barsCalculationParagraph.Add(FormulaCreator.TextParagraph($"Ферма: {projectTrussToCalculate.TrussName}."));
            barsCalculationParagraph.Add(FormulaCreator.TextParagraph($"Шаг ферм: {projectTrussToCalculate.TrussSpacing} мм, нагрузка на 1м2: {projectTrussToCalculate.Load} кгс/м2."));
            barsCalculationParagraph.Add(FormulaCreator.CenterBigTextParagraph($"Информация о ферме"));
            barsCalculationParagraph.Add(FormulaCreator.TextParagraph($"Пролет: {trussToCalculate.Span} мм"));
            barsCalculationParagraph.Add(FormulaCreator.TextParagraph($"Уклон: {trussToCalculate.Slope}"));
            barsCalculationParagraph.Add(FormulaCreator.TextParagraph($"Высота на опоре: {trussToCalculate.SupportDepth} мм"));
            barsCalculationParagraph.Add(FormulaCreator.TextParagraph($"Количество панелей: {trussToCalculate.PanelAmount}"));
            barsCalculationParagraph.Add(FormulaCreator.TextParagraph($"Тип нагрузки: {trussToCalculate.LoadType}"));




            var barsToCalc = trussToCalculate.Bar.OrderBy(bar=>bar.BarNumber);

            foreach (var bar in barsToCalc)
            {
                barsCalculationParagraph.AddRange(FormulaCreator.GenerateParagraphWithBarText(bar));

                var Ry = steelRepository.GetStrength(bar.Steel, bar.Section.Thickness);
                barsCalculationParagraph.Add(FormulaCreator.TextParagraph($"Расчетное сопротивления стержня по пределу текучести: {Ry} кгс/м2"));
                if (bar.ActualForce < 0)
                {

                    var ecc = bar.ActualMoment / Math.Abs(bar.ActualForce) * 100;
                    var m = ecc * bar.Section.Area / bar.Section.SectionModulusX;

                    if (Math.Abs(bar.Moment) < TOLERANCE || m <= 0.1)
                    {
                        barsCalculationParagraph.Add(bar.ActualMoment == 0
                            ? FormulaCreator.TextParagraph($"Стержень рассчитывается на сжатие")
                            : FormulaCreator.TextParagraph($"Влияние момента незначительно, стержень рассчитывается на сжатие"));
                        barsCalculationParagraph.AddRange(CalculateForce(bar, Ry));
                    }
                    else
                    {
                        barsCalculationParagraph.Add(FormulaCreator.TextParagraph($"Стержень рассчитывается на сжатие с изгибом"));
                        barsCalculationParagraph.AddRange(CalculateForceMoment(bar, Ry));
                    }
                }
                else
                {
                    barsCalculationParagraph.Add(FormulaCreator.TextParagraph($"Стержень рассчитывается на растяжение"));
                    bar.CalcType = "расчет на растяжение";
                    bar.CalcResult = bar.ActualForce / bar.Section.Area / Ry * 100;
                    barsCalculationParagraph.Add(FormulaCreator.GenerateParagraphWithAxialTensionFormula(bar, Ry));
                }
            }

            return barsCalculationParagraph;
        }

        public static IEnumerable<WP.Paragraph> CalculateForce(Bar bar, float Ry)
        {
            var report = new List<WP.Paragraph>();
            var lambda = bar.Length / bar.Section.RadiusOfGyrationX / 10;
            report.Add(FormulaCreator.GenerateParagraphWithText("Гибкость стержня: "));
            report.Add(FormulaCreator.LambdaParagraph(bar, lambda));

            var lambdaRef = lambda * (float)Math.Sqrt(Ry / E);
            report.Add(FormulaCreator.GenerateParagraphWithText("Условная гибкость стержня: "));
            report.Add(FormulaCreator.LambdaRefParagraph(lambda, lambdaRef, Ry, E));

            float fi;
            if (lambdaRef > 0 && lambdaRef <= 2.5)
            {
                fi = (float)(1 - (0.073 - 5.53 * Ry / E) * (float)Math.Pow(lambdaRef, 1.5));
            }
            else if (lambdaRef > 2.5 && lambdaRef <= 4.5)
            {
                fi = (float)(1.47 - 13 * Ry / E - (0.371 - 27.3 * Ry / E) * lambdaRef +
                             (0.0275 - 5.53 * Ry / E) * lambdaRef * lambdaRef);
            }
            else if (lambdaRef > 4.5)
            {
                fi = 332 / (lambdaRef * lambdaRef * (51 - lambdaRef));
            }
            else
            {
                throw new ArgumentException($"Incorrect lambda ref coefficient = {lambdaRef} in bar {bar.ElementType} - {bar.BarNumber}");
            }
            report.Add(FormulaCreator.GenerateParagraphWithText($"По таблице 72 СНиП II-23-81* определяем коэффициент φ: {fi}"));

            report.Add(FormulaCreator.GenerateParagraphWithText($"Проверка стержня на потерю устойчивости:"));
            bar.CalcResult = Math.Abs(Math.Abs(bar.ActualForce) / bar.Section.Area / fi / Ry * 100);
            report.Add(FormulaCreator.GenerateParagraphWithAxialCompressionFormula(bar, Ry, fi, false));
            bar.CalcType = "расчет на сжатие";
            return report;
        }

        public static IEnumerable<WP.Paragraph> CalculateForceMoment(Bar bar, float Ry)
        {
            var report = new List<WP.Paragraph>();

            var section = bar.Section;
            var ecc = bar.ActualMoment / Math.Abs(bar.ActualForce)*100;
            report.Add(FormulaCreator.GenerateParagraphWithText($"Эксцентриситет:"));
            report.Add(FormulaCreator.EccentricityParagraph(bar, ecc));

            var m = ecc * section.Area / section.SectionModulusX;
            report.Add(FormulaCreator.GenerateParagraphWithText($"Относительный эксцентриситет:"));
            report.Add(FormulaCreator.ReffEccentricityParagraph(bar, ecc, m));

            var lambda_x = bar.Length / section.RadiusOfGyrationX / 10;
            report.Add(FormulaCreator.GenerateParagraphWithText("Гибкость стержня: "));
            report.Add(FormulaCreator.LambdaParagraph(bar, lambda_x));

            var lambda_x_ref = lambda_x * (float)Math.Sqrt(Ry / E);
            report.Add(FormulaCreator.GenerateParagraphWithText("Условная гибкость стержня: "));
            report.Add(FormulaCreator.LambdaRefParagraph(lambda_x, lambda_x_ref, Ry, E));

            var af_aw = section.Width / 2F / section.Height;
            report.Add(FormulaCreator.GenerateParagraphWithText("Коэффициент η определяем по таблице 73 в зависимости от относительного эксцентриситета m, условной гибкости стержня и отношении площади полок к площади стенок сечения:"));
            report.Add(FormulaCreator.AreaFractionParagraph(bar, af_aw));

            var etta = Etta(af_aw, lambda_x_ref, m, bar);

            report.Add(FormulaCreator.GenerateParagraphWithText($"η = {etta}"));

            var mef = m * etta;
            report.Add(FormulaCreator.GenerateParagraphWithText("Приведенный относительный коэффициент: "));
            report.Add(FormulaCreator.MefParagraph(m, etta, mef));

            var fie = GetFie(lambda_x_ref, mef);
            report.Add(FormulaCreator.GenerateParagraphWithText($"По таблице 74 СНиП II-23-81* определяем коэффициент φe: {fie}"));



            bar.CalcResult = Math.Abs(bar.ActualForce) / bar.Section.Area / Ry * 100 / fie;
            report.Add(FormulaCreator.GenerateParagraphWithText($"Проверка стержня на потерю устойчивости в плоскости:"));
            report.Add(FormulaCreator.GenerateParagraphWithAxialCompressionFormula(bar, Ry, fie, true));
            bar.CalcType = "расчет на в/ц сжатие";
            return report;
        }

        public static float Etta(float af_aw, float lambda_ref, float m, Bar bar)
        {
            if (lambda_ref <= 5 && lambda_ref >= 0)
            {
                if (m <= 5 && m >= 0.1)
                {
                    var af_aw_025 = (1.45F - 0.05F * m) - 0.01F * (5 - m) * lambda_ref;
                    var af_aw_05 = (1.75F - 0.1F * m) - 0.02F * (5 - m) * lambda_ref;
                    var af_aw_10 = (1.9F - 0.1F * m) - 0.02F * (6 - m) * lambda_ref;
                    float k, b;
                    if (af_aw <= 0.5)
                    {
                        GetCoefficients(0.25F, af_aw_025, 0.5F, af_aw_05, out k, out b);
                        return k * af_aw + b;
                    }
                    GetCoefficients(0.5F, af_aw_05, 1F, af_aw_10, out k, out b);
                    return k * af_aw + b;
                }
                if (m > 5 && m <= 20)
                {
                    var af_aw_025 = 1.2F;
                    var af_aw_05 = 1.25F;
                    var af_aw_10 = 1.4F - 0.02F * lambda_ref;
                    float k, b;
                    if (af_aw <= 0.5)
                    {
                        GetCoefficients(0.25F, af_aw_025, 0.5F, af_aw_05, out k, out b);
                        return k * af_aw + b;
                    }
                    GetCoefficients(0.5F, af_aw_05, 1F, af_aw_10, out k, out b);
                    return k * af_aw + b;
                }
                throw new ArithmeticException($"m coefficient is suspicious  (m = {m} in bar {bar.BarNumber}). Change initial data");
            }
            if (lambda_ref > 5)
            {
                var af_aw_025 = 1.2F;
                var af_aw_05 = 1.25F;
                var af_aw_10 = 1.3F;
                float k, b;
                if (af_aw <= 0.5)
                {
                    GetCoefficients(0.25F, af_aw_025, 0.5F, af_aw_05, out k, out b);
                    return k * af_aw + b;
                }
                GetCoefficients(0.5F, af_aw_05, 1F, af_aw_10, out k, out b);
                return k * af_aw + b;
            }
            throw new ArithmeticException($"Incorrect Labmda refference coefficient (l_ref = {lambda_ref}). Change initial data");
        }

        private static void GetCoefficients(float x1, float y1, float x2, float y2, out float k, out float b)
        {
            k = (y1 - y2) / (x1 - x2);
            b = (y1 * x2 - y2 * x1) / (x2 - x1);
        }

        private static float GetFie(float lambdaRef, float mef)
        {
            var mef_lower = mef <= MefList[0] ? MefList[0] : MefList.Last(m => m <= mef);
            float mef_upper;
            if (Math.Abs(mef_lower - 0.1F) < TOLERANCE)
            {
                mef_upper = MefList[1];
            }
            else
            {
                mef_upper = mef >= MefList[MefList.Length - 1] ? MefList[MefList.Length - 1] : MefList.First(m => m >= mef);
                if (Math.Abs(mef_upper - MefList[MefList.Length - 1]) < TOLERANCE)
                {
                    mef_lower = MefList[MefList.Length - 2];
                }
            }

            var lambdaRef_lower = lambdaRef <= LambdaRefList[0] ? LambdaRefList[0] : LambdaRefList.Last(lm => lm <= lambdaRef);
            float lambdaRef_upper;
            if (Math.Abs(mef_lower - 0.1F) < TOLERANCE)
            {
                lambdaRef_upper = LambdaRefList[1];
            }
            else
            {
                lambdaRef_upper = lambdaRef >= LambdaRefList[LambdaRefList.Length - 1] ? LambdaRefList[LambdaRefList.Length - 1] : LambdaRefList.First(lm => lm >= lambdaRef);
                if (Math.Abs(mef_upper - 14F) < TOLERANCE)
                {
                    lambdaRef_lower = LambdaRefList[LambdaRefList.Length - 2];
                }
            }


            var fie_lamlower_meflower = Fie[lambdaRef_lower][mef_lower];
            var fie_lamlower_mefupper = Fie[lambdaRef_lower][mef_upper];
            var fie_lamupper_meflower = Fie[lambdaRef_upper][mef_lower];
            var fie_lamupper_mefupper = Fie[lambdaRef_upper][mef_upper];

            GetCoefficients(mef_lower, fie_lamlower_meflower, mef_upper, fie_lamlower_mefupper, out var kl, out var bl);
            var fie_lamlower = kl * mef + bl;

            GetCoefficients(mef_lower, fie_lamupper_meflower, mef_upper, fie_lamupper_mefupper, out var ku, out var bu);
            var fie_lamupper = ku * mef + bu;

            GetCoefficients(lambdaRef_lower, fie_lamlower, lambdaRef_upper, fie_lamupper, out var k, out var b);
            return (k * lambdaRef + b) / 1000;
        }


        private static readonly Dictionary<float, Dictionary<float, float>> Fie = new Dictionary<float, Dictionary<float, float>>
        {
            {
                0.5F, new Dictionary<float, float>
                {
                    {0.1F, 967F},
                    {0.25F, 922F},
                    {0.5F, 850F},
                    {0.75F, 782F},
                    {1F, 722F},
                    {1.25F, 669F},
                    {1.5F, 620F},
                    {1.75F, 577F},
                    {2F, 538F},
                    {2.5F, 469F},
                    {3F, 417F},
                    {3.5F, 370F},
                    {4F, 337F},
                    {4.5F, 307F},
                    {5F, 280F},
                    {5.5F, 260F},
                    {6F, 237F},
                    {6.5F, 222F},
                    {7F, 210F},
                    {8F, 183F},
                    {9F, 164F},
                    {10F, 150F},
                    {12F, 125F},
                    {14F, 106F},
                    {17F, 90F},
                    {20F, 77F}
                }
            },
            {
                1.0F, new Dictionary<float, float>
                {
                    {0.1F, 925F},
                    {0.25F, 854F},
                    {0.5F, 778F},
                    {0.75F, 711F},
                    {1F, 653F},
                    {1.25F, 600F},
                    {1.5F, 563F},
                    {1.75F, 520F},
                    {2F, 484F},
                    {2.5F, 427F},
                    {3F, 382F},
                    {3.5F, 341F},
                    {4F, 307F},
                    {4.5F, 283F},
                    {5F, 259F},
                    {5.5F, 240F},
                    {6F, 225F},
                    {6.5F, 209F},
                    {7F, 196F},
                    {8F, 175F},
                    {9F, 157F},
                    {10F, 142F},
                    {12F, 121F},
                    {14F, 103F},
                    {17F, 86F},
                    {20F, 74F}
                }
            },
            {
                1.5F, new Dictionary<float, float>
                {
                    {0.1F, 875F},
                    {0.25F, 804F},
                    {0.5F, 716F},
                    {0.75F, 647F},
                    {1F, 593F},
                    {1.25F, 548F},
                    {1.5F, 507F},
                    {1.75F, 470F},
                    {2F, 439F},
                    {2.5F, 388F},
                    {3F, 347F},
                    {3.5F, 312F},
                    {4F, 283F},
                    {4.5F, 262F},
                    {5F, 240F},
                    {5.5F, 223F},
                    {6F, 207F},
                    {6.5F, 195F},
                    {7F, 182F},
                    {8F, 163F},
                    {9F, 148F},
                    {10F, 134F},
                    {12F, 114F},
                    {14F, 99F},
                    {17F, 82F},
                    {20F, 70F}
                }
            },
            {
                2.0F, new Dictionary<float, float>
                {
                    {0.1F, 813F},
                    {0.25F, 742F},
                    {0.5F, 653F},
                    {0.75F, 587F},
                    {1F, 536F},
                    {1.25F, 496F},
                    {1.5F, 457F},
                    {1.75F, 425F},
                    {2F, 397F},
                    {2.5F, 352F},
                    {3F, 315F},
                    {3.5F, 286F},
                    {4F, 260F},
                    {4.5F, 240F},
                    {5F, 222F},
                    {5.5F, 206F},
                    {6F, 193F},
                    {6.5F, 182F},
                    {7F, 170F},
                    {8F, 153F},
                    {9F, 138F},
                    {10F, 125F},
                    {12F, 107F},
                    {14F, 94F},
                    {17F, 79F},
                    {20F, 67F}
                }
            },
            {
                2.5F,new Dictionary<float, float>
                {
                    {0.1F, 742F},
                    {0.25F, 672F},
                    {0.5F, 587F},
                    {0.75F, 526F},
                    {1F, 480F},
                    {1.25F, 442F},
                    {1.5F, 410F},
                    {1.75F, 383F},
                    {2F, 357F},
                    {2.5F, 317F},
                    {3F, 287F},
                    {3.5F, 262F},
                    {4F, 238F},
                    {4.5F, 220F},
                    {5F, 204F},
                    {5.5F, 190F},
                    {6F, 178F},
                    {6.5F, 168F},
                    {7F, 158F},
                    {8F, 144F},
                    {9F, 130F},
                    {10F, 118F},
                    {12F, 101F},
                    {14F, 90F},
                    {17F, 76F},
                    {20F, 65F}
                }
            },
            {
                3.0F,new Dictionary<float, float>
                {
                    {0.1F, 667F},
                    {0.25F, 597F},
                    {0.5F, 520F},
                    {0.75F, 465F},
                    {1F, 425F},
                    {1.25F, 395F},
                    {1.5F, 365F},
                    {1.75F, 342F},
                    {2F, 320F},
                    {2.5F, 287F},
                    {3F, 260F},
                    {3.5F, 238F},
                    {4F, 217F},
                    {4.5F, 202F},
                    {5F, 187F},
                    {5.5F, 175F},
                    {6F, 166F},
                    {6.5F, 156F},
                    {7F, 147F},
                    {8F, 135F},
                    {9F, 123F},
                    {10F, 112F},
                    {12F, 97F},
                    {14F, 86F},
                    {17F, 73F},
                    {20F, 63F}
                }
            },
            {
                3.5F, new Dictionary<float, float>
                {
                    {0.1F, 587F},
                    {0.25F, 522F},
                    {0.5F, 455F},
                    {0.75F, 408F},
                    {1F, 375F},
                    {1.25F, 350F},
                    {1.5F, 325F},
                    {1.75F, 303F},
                    {2F, 287F},
                    {2.5F, 258F},
                    {3F, 233F},
                    {3.5F, 216F},
                    {4F, 198F},
                    {4.5F, 183F},
                    {5F, 172F},
                    {5.5F, 162F},
                    {6F, 153F},
                    {6.5F, 145F},
                    {7F, 137F},
                    {8F, 125F},
                    {9F, 115F},
                    {10F, 106F},
                    {12F, 92F},
                    {14F, 82F},
                    {17F, 69F},
                    {20F, 60F}
                }
            },
            {
                4.0F, new Dictionary<float, float>
                {
                    {0.1F, 505F},
                    {0.25F, 447F},
                    {0.5F, 394F},
                    {0.75F, 356F},
                    {1F, 330F},
                    {1.25F, 309F},
                    {1.5F, 289F},
                    {1.75F, 270F},
                    {2F, 256F},
                    {2.5F, 232F},
                    {3F, 212F},
                    {3.5F, 197F},
                    {4F, 181F},
                    {4.5F, 168F},
                    {5F, 158F},
                    {5.5F, 149F},
                    {6F, 140F},
                    {6.5F, 135F},
                    {7F, 127F},
                    {8F, 118F},
                    {9F, 108F},
                    {10F, 98F},
                    {12F, 88F},
                    {14F, 78F},
                    {17F, 66F},
                    {20F, 57F}
                }
            },
            {
                4.5F, new Dictionary<float, float>
                {
                    {0.1F, 418F},
                    {0.25F, 382F},
                    {0.5F, 342F},
                    {0.75F, 310F},
                    {1F, 288F},
                    {1.25F, 272F},
                    {1.5F, 257F},
                    {1.75F, 242F},
                    {2F, 229F},
                    {2.5F, 208F},
                    {3F, 192F},
                    {3.5F, 178F},
                    {4F, 165F},
                    {4.5F, 155F},
                    {5F, 146F},
                    {5.5F, 137F},
                    {6F, 130F},
                    {6.5F, 125F},
                    {7F, 118F},
                    {8F, 110F},
                    {9F, 101F},
                    {10F, 93F},
                    {12F, 83F},
                    {14F, 75F},
                    {17F, 64F},
                    {20F, 55F}
                }
            },
            {
                5.0F, new Dictionary<float, float>
                {
                    {0.1F, 354F},
                    {0.25F, 326F},
                    {0.5F, 295F},
                    {0.75F, 273F},
                    {1F, 253F},
                    {1.25F, 239F},
                    {1.5F, 225F},
                    {1.75F, 215F},
                    {2F, 205F},
                    {2.5F, 188F},
                    {3F, 175F},
                    {3.5F, 162F},
                    {4F, 150F},
                    {4.5F, 143F},
                    {5F, 135F},
                    {5.5F, 126F},
                    {6F, 120F},
                    {6.5F, 117F},
                    {7F, 111F},
                    {8F, 103F},
                    {9F, 95F},
                    {10F, 88F},
                    {12F, 79F},
                    {14F, 72F},
                    {17F, 62F},
                    {20F, 53F}
                }
            },
            {
                5.5F, new Dictionary<float, float>
                {
                    {0.1F, 302F},
                    {0.25F, 280F},
                    {0.5F, 256F},
                    {0.75F, 240F},
                    {1F, 224F},
                    {1.25F, 212F},
                    {1.5F, 200F},
                    {1.75F, 192F},
                    {2F, 184F},
                    {2.5F, 170F},
                    {3F, 158F},
                    {3.5F, 148F},
                    {4F, 138F},
                    {4.5F, 132F},
                    {5F, 124F},
                    {5.5F, 117F},
                    {6F, 112F},
                    {6.5F, 108F},
                    {7F, 104F},
                    {8F, 95F},
                    {9F, 89F},
                    {10F, 84F},
                    {12F, 75F},
                    {14F, 69F},
                    {17F, 60F},
                    {20F, 51F}
                }
            },
            {
                6.0F, new Dictionary<float, float>
                {
                    {0.1F, 258F},
                    {0.25F, 244F},
                    {0.5F, 223F},
                    {0.75F, 210F},
                    {1F, 198F},
                    {1.25F, 190F},
                    {1.5F, 178F},
                    {1.75F, 172F},
                    {2F, 166F},
                    {2.5F, 153F},
                    {3F, 145F},
                    {3.5F, 137F},
                    {4F, 128F},
                    {4.5F, 120F},
                    {5F, 115F},
                    {5.5F, 109F},
                    {6F, 104F},
                    {6.5F, 100F},
                    {7F, 96F},
                    {8F, 89F},
                    {9F, 84F},
                    {10F, 79F},
                    {12F, 72F},
                    {14F, 66F},
                    {17F, 57F},
                    {20F, 49F},

                }
            },
            {
                6.5F,new Dictionary<float, float>
                {
                    {0.1F, 223F},
                    {0.25F, 213F},
                    {0.5F, 196F},
                    {0.75F, 185F},
                    {1F, 176F},
                    {1.25F, 170F},
                    {1.5F, 160F},
                    {1.75F, 155F},
                    {2F, 149F},
                    {2.5F, 140F},
                    {3F, 132F},
                    {3.5F, 125F},
                    {4F, 117F},
                    {4.5F, 112F},
                    {5F, 106F},
                    {5.5F, 101F},
                    {6F, 97F},
                    {6.5F, 94F},
                    {7F, 89F},
                    {8F, 83F},
                    {9F, 80F},
                    {10F, 74F},
                    {12F, 68F},
                    {14F, 62F},
                    {17F, 54F},
                    {20F, 47F}
                }
            },
            {
                7.0F,new Dictionary<float, float>
                {
                    {0.1F, 194F},
                    {0.25F, 186F},
                    {0.5F, 173F},
                    {0.75F, 163F},
                    {1F, 157F},
                    {1.25F, 152F},
                    {1.5F, 145F},
                    {1.75F, 141F},
                    {2F, 136F},
                    {2.5F, 127F},
                    {3F, 121F},
                    {3.5F, 115F},
                    {4F, 108F},
                    {4.5F, 102F},
                    {5F, 98F},
                    {5.5F, 94F},
                    {6F, 91F},
                    {6.5F, 87F},
                    {7F, 83F},
                    {8F, 78F},
                    {9F, 74F},
                    {10F, 70F},
                    {12F, 64F},
                    {14F, 59F},
                    {17F, 52F},
                    {20F, 45F}
                }
            },
            {
                8.0F,new Dictionary<float, float>
                {
                    {0.1F, 152F},
                    {0.25F, 146F},
                    {0.5F, 138F},
                    {0.75F, 133F},
                    {1F, 128F},
                    {1.25F, 121F},
                    {1.5F, 117F},
                    {1.75F, 115F},
                    {2F, 113F},
                    {2.5F, 106F},
                    {3F, 100F},
                    {3.5F, 95F},
                    {4F, 91F},
                    {4.5F, 87F},
                    {5F, 83F},
                    {5.5F, 81F},
                    {6F, 78F},
                    {6.5F, 76F},
                    {7F, 74F},
                    {8F, 68F},
                    {9F, 65F},
                    {10F, 62F},
                    {12F, 57F},
                    {14F, 53F},
                    {17F, 47F},
                    {20F, 41F}
                }
            },
            {
                9.0F,new Dictionary<float, float>
                {
                    {0.1F, 122F},
                    {0.25F, 117F},
                    {0.5F, 112F},
                    {0.75F, 107F},
                    {1F, 103F},
                    {1.25F, 100F},
                    {1.5F, 98F},
                    {1.75F, 96F},
                    {2F, 93F},
                    {2.5F, 88F},
                    {3F, 85F},
                    {3.5F, 82F},
                    {4F, 79F},
                    {4.5F, 75F},
                    {5F, 72F},
                    {5.5F, 69F},
                    {6F, 66F},
                    {6.5F, 65F},
                    {7F, 64F},
                    {8F, 61F},
                    {9F, 58F},
                    {10F, 55F},
                    {12F, 51F},
                    {14F, 48F},
                    {17F, 43F},
                    {20F, 38F}
                }
            },
            {
                10.0F,new Dictionary<float, float>
                {
                    {0.1F, 100F},
                    {0.25F, 97F},
                    {0.5F, 93F},
                    {0.75F, 91F},
                    {1F, 90F},
                    {1.25F, 85F},
                    {1.5F, 81F},
                    {1.75F, 80F},
                    {2F, 79F},
                    {2.5F, 75F},
                    {3F, 72F},
                    {3.5F, 70F},
                    {4F, 69F},
                    {4.5F, 65F},
                    {5F, 62F},
                    {5.5F, 60F},
                    {6F, 59F},
                    {6.5F, 58F},
                    {7F, 57F},
                    {8F, 55F},
                    {9F, 52F},
                    {10F, 49F},
                    {12F, 46F},
                    {14F, 43F},
                    {17F, 39F},
                    {20F, 35F}
                }
            },
            {
                11.0F,new Dictionary<float, float>
                {
                    {0.1F, 83F},
                    {0.25F, 79F},
                    {0.5F, 77F},
                    {0.75F, 76F},
                    {1F, 75F},
                    {1.25F, 73F},
                    {1.5F, 71F},
                    {1.75F, 69F},
                    {2F, 68F},
                    {2.5F, 63F},
                    {3F, 62F},
                    {3.5F, 61F},
                    {4F, 60F},
                    {4.5F, 57F},
                    {5F, 55F},
                    {5.5F, 53F},
                    {6F, 52F},
                    {6.5F, 51F},
                    {7F, 50F},
                    {8F, 48F},
                    {9F, 46F},
                    {10F, 44F},
                    {12F, 40F},
                    {14F, 38F},
                    {17F, 35F},
                    {20F, 32F}
                }
            },
            {
                12.0F,new Dictionary<float, float>
                {
                    {0.1F, 69F},
                    {0.25F, 67F},
                    {0.5F, 64F},
                    {0.75F, 63F},
                    {1F, 62F},
                    {1.25F, 60F},
                    {1.5F, 59F},
                    {1.75F, 59F},
                    {2F, 58F},
                    {2.5F, 55F},
                    {3F, 54F},
                    {3.5F, 53F},
                    {4F, 52F},
                    {4.5F, 51F},
                    {5F, 50F},
                    {5.5F, 49F},
                    {6F, 48F},
                    {6.5F, 47F},
                    {7F, 46F},
                    {8F, 44F},
                    {9F, 42F},
                    {10F, 40F},
                    {12F, 37F},
                    {14F, 35F},
                    {17F, 32F},
                    {20F, 29F}
                }
            },
            {
                13.0F,new Dictionary<float, float>
                {
                    {0.1F, 62F},
                    {0.25F, 61F},
                    {0.5F, 54F},
                    {0.75F, 53F},
                    {1F, 52F},
                    {1.25F, 51F},
                    {1.5F, 51F},
                    {1.75F, 50F},
                    {2F, 49F},
                    {2.5F, 49F},
                    {3F, 48F},
                    {3.5F, 48F},
                    {4F, 47F},
                    {4.5F, 45F},
                    {5F, 44F},
                    {5.5F, 43F},
                    {6F, 42F},
                    {6.5F, 41F},
                    {7F, 41F},
                    {8F, 39F},
                    {9F, 38F},
                    {10F, 37F},
                    {12F, 35F},
                    {14F, 33F},
                    {17F, 30F},
                    {20F, 27F}
                }
            },
            {
                14.0F,new Dictionary<float, float>
                {
                    {0.1F, 52F},
                    {0.25F, 49F},
                    {0.5F, 49F},
                    {0.75F, 48F},
                    {1F, 48F},
                    {1.25F, 47F},
                    {1.5F, 47F},
                    {1.75F, 46F},
                    {2F, 45F},
                    {2.5F, 44F},
                    {3F, 43F},
                    {3.5F, 43F},
                    {4F, 42F},
                    {4.5F, 41F},
                    {5F, 40F},
                    {5.5F, 40F},
                    {6F, 39F},
                    {6.5F, 39F},
                    {7F, 38F},
                    {8F, 37F},
                    {9F, 36F},
                    {10F, 36F},
                    {12F, 34F},
                    {14F, 32F},
                    {17F, 29F},
                    {20F, 26F}
                }
            }
        };
        private static readonly float[] MefList = {
            0.1F,
            0.25F,
            0.5F,
            0.75F,
            1F,
            1.25F,
            1.5F,
            1.75F,
            2F,
            2.5F,
            3F,
            3.5F,
            4F,
            4.5F,
            5F,
            5.5F,
            6F,
            6.5F,
            7F,
            8F,
            9F,
            10F,
            12F,
            14F,
            17F,
            20F
        };
        private static readonly float[] LambdaRefList =
        {
            0.5F,
            1F,
            1.5F,
            2F,
            2.5F,
            3F,
            3.5F,
            4F,
            4.5F,
            5F,
            5.5F,
            6F,
            6.5F,
            7F,
            8F,
            9F,
            10F,
            11F,
            12F,
            13F,
            14F
        };
    }

}
