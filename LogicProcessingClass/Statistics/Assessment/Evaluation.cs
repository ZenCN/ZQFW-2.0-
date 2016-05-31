using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using EntityModel;

namespace LogicProcessingClass.Statistics
{
    public class Evaluation
    {
        FXDICTEntities m_FXDictEntities;   //字典库实体类

        public Evaluation()
        {
            m_FXDictEntities = new FXDICTEntities();
        }

        /// <summary>获取评估参数以及相关数据
        /// 
        /// </summary>
        /// <param name="evaluationType">评估类型（0为场次，1为年度）</param>
        /// <returns></returns>
        public Dictionary<string, EvaluationParameter> getEvalParams(int evaluationType)
        {
            var parameters = m_FXDictEntities.TB57_EvaluationParameter.Where(t => t.EvaluationType == evaluationType).ToDictionary(
                k => k.Parameter, v => new EvaluationParameter
                {
                    Name = v.Parameter,
                    Weight = (double)v.Weight,
                    Factors = getEvalFactors(evaluationType, v.Parameter)
                });
            return parameters;
        }

        /// <summary>获取评估指标及相关数据
        /// 
        /// </summary>
        /// <param name="evaluationType">评估类型</param>
        /// <param name="parameter">评估参数</param>
        /// <returns></returns>
        public Dictionary<string, EvaluationFactor> getEvalFactors(int evaluationType, string parameter)
        {
            var evalFactors = m_FXDictEntities.TB58_EvaluationFactor.ToList().Where(t => t.EvaluationType == evaluationType &&
                t.Parameter == parameter).ToDictionary(k => k.Factor, v => new EvaluationFactor
                {
                    Name = v.Factor,
                    MeasureValue = (double)v.MeasureValue,
                    Thresholds = getThresholds(evaluationType, v.Factor)
                });
            return evalFactors;
        }

        /// <summary>获取指标阈值
        /// 
        /// </summary>
        /// <param name="evaluationType">评估类型</param>
        /// <param name="factor">指标</param>
        /// <returns></returns>
        public IList<Threshold> getThresholds(int evaluationType, string factor)
        {
            var listThreshold = m_FXDictEntities.TB59_EvalFactorThreshold.Where(t => t.EvaluationType == evaluationType &&
                                t.Factor == factor).OrderBy(t => t.LowerValue).Select(t => new Threshold
                                    {
                                        Grade = (int)t.Grade,
                                        LowerValue = (double)t.LowerValue,
                                        HigherValue = (double?)t.HigherValue
                                    }).ToList();
            return listThreshold;
        }

        /// <summary>获取对应参数等级阈值
        /// 
        /// </summary>
        /// <param name="evaluationType">评估类型</param>
        /// <returns></returns>
        public IList<Threshold> getParameterChoices(int evaluationType)
        {
            return getThresholds(evaluationType, "ParameterChoice");
        }

        /// <summary>获取洪涝灾情评估值与对应洪涝灾害等级
        /// 
        /// </summary>
        /// <param name="evaluaitonType">评估类型（场次或年度）</param>
        /// <param name="evaluationMethod">评估方法</param>
        /// <returns></returns>
        public IList<Threshold> getEvaluationGrades(int evaluationType, int evaluationMethod)
        {
            var evaluationGrades = m_FXDictEntities.TB60_EvaluationGrade.Where(t => t.EvaluationType == evaluationType &&
                t.EvaluationMethod == evaluationMethod).OrderBy(t => t.LowerValue).Select(t => new Threshold
                {
                    Grade = (int)t.Grade,
                    LowerValue = (double)t.LowerValue,
                    HigherValue =(double?)t.HigherValue
                }).ToList();
            return evaluationGrades;
        }

        /// <summary>获取评估指标参数值
        /// 
        /// </summary>
        /// <param name="factor">评估指标名称</param>
        /// <param name="factorValue">指标值</param>
        /// <returns></returns>
        public static double getParamValue(EvaluationFactor factor, double factorValue, IList<Threshold> parameterChoices)
        {
            factorValue = factorValue / factor.MeasureValue;
            double param = 0;       //参数取值
            for (int i = 0; i < factor.Thresholds.Count; i++)
            {
                if (i == factor.Thresholds.Count - 1)
                {
                    Threshold factorThreshold = factor.Thresholds[i - 1]; //评估指标阈值
                    Threshold paramThreshold = parameterChoices[i - 1];  //参数取值范围
                    param = paramThreshold.LowerValue + (factorValue - factorThreshold.LowerValue)
                        *((double)paramThreshold.HigherValue - paramThreshold.LowerValue)
                        / ((double)factorThreshold.HigherValue - factorThreshold.LowerValue);
                    break;
                }
                else
                {
                    Threshold factorThreshold = factor.Thresholds[i]; //评估指标阈值
                    if (factorValue >= factorThreshold.LowerValue && factorValue <= factorThreshold.HigherValue)
                    {
                        int grade = factorThreshold.Grade;  //指标值所在阈值范围对应等级
                        Threshold paramThreshold = parameterChoices[grade - 1];  //参数取值范围
                        param = paramThreshold.LowerValue + (factorValue - factorThreshold.LowerValue)
                            * ((double)paramThreshold.HigherValue - paramThreshold.LowerValue)
                            / ((double)factorThreshold.HigherValue - factorThreshold.LowerValue);
                        break;
                    }
                }
            }
            if (param > 100) param = 100;
            return param;
        }

        /// <summary>根据洪涝灾情评估值和评估等级阈值获取洪涝灾情评估等级
        /// 
        /// </summary>
        /// <param name="evaluationGrades">评估等级阈值</param>
        /// <param name="C">洪涝灾情评估值</param>
        /// <returns></returns>
        public static int getGrade(IList<Threshold> evaluationGrades, double C)
        {
            int grade = 1;
            for (int i = 0; i < evaluationGrades.Count; i++)
            {
                Threshold evaluationGrade = evaluationGrades[i];
                if (evaluationGrade.HigherValue == null)
                {
                    if (evaluationGrade.LowerValue >= C)
                    {
                        grade = evaluationGrade.Grade;
                        break;
                    }
                }
                else if (C >= evaluationGrade.LowerValue && C < evaluationGrade.HigherValue)
                {
                    grade = evaluationGrade.Grade;
                    break;
                }
            }
            return grade;
        }
    }

    /// <summary>评估参数
    /// 
    /// </summary>
    public class EvaluationParameter
    {
        string _name;   //参数名称
        Dictionary<string, EvaluationFactor> _factors;  //评估指标<名称，指标阈值>
        double _weight;   //参数权重

        public EvaluationParameter() { }

        public EvaluationParameter(string name, Dictionary<string, EvaluationFactor> factors, double weight)
        {
            this._name = name;
            this._factors = factors;
            this._weight = weight;
        }

        /// <summary>
        /// //参数名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// //评估指标<名称，指标阈值>
        /// </summary>
        public Dictionary<string, EvaluationFactor> Factors
        {
            get { return _factors; }
            set { _factors = value; }
        }

        /// <summary>
        /// //参数权重
        /// </summary>
        public double Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }
    }

    /// <summary>//评估因子
    /// 
    /// </summary>
    public class EvaluationFactor
    {
        public EvaluationFactor() { }

        public EvaluationFactor(string name, IList<Threshold> thresholds)
        {
            this.Name = name;
            this.Thresholds = thresholds;
        }

        public string Name{get ;set ;} //指标名称
        public double MeasureValue{ get; set ;}//指标值单位换算参数
        public IList<Threshold> Thresholds { get; set; }//指标参数取值的阈值
    }

    /// <summary>阈值，临界值上下限及对应等级
    /// 
    /// </summary>
    public class Threshold
    {
        public int Grade { get; set; }  //临界值对应等级
        public double LowerValue { get; set; } //临界值下限
        public double? HigherValue { get; set; }//临界值上限
    }
}
