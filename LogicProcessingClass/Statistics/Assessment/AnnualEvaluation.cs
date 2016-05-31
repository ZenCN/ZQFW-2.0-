using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace LogicProcessingClass.Statistics
{
    public class AnnualEvaluation
    {
        Dictionary<string, EvaluationParameter> m_Parameters;  //年度灾情评估参数
        //IList<EvaluationParameter> m_Parameters;  //年度灾情评估参数
        IList<Threshold> m_ParameterChoices;  //年度洪涝灾情评估参数取值
        IList<Threshold> m_EvaluationGrades;     //洪涝灾情评估值与对应洪涝灾害等级

        /// <summary>年度灾情评估构造函数
        /// 
        /// </summary>
        public AnnualEvaluation()
        {
            Evaluation evaluation = new Evaluation();
            m_Parameters = evaluation.getEvalParams(1);
            m_ParameterChoices = evaluation.getParameterChoices(1);
            m_EvaluationGrades = evaluation.getEvaluationGrades(1, 0);
        }

        /// <summary>根据指标值获取灾情对应等级
        /// 
        /// </summary>
        /// <param name="swrk">死亡人口</param>
        /// <param name="szrk">受灾人口</param>
        /// <param name="shmjxj">农作物经济损失</param>
        /// <param name="zjjjzss">直接经济损失</param>
        /// <param name="slsszjjjss">水利设施经济损失</param>
        /// <param name="dtfw">倒塌房屋</param>
        /// <returns></returns>
        public int getGrade(double swrk, double szrk, double shmjxj, double zjjjzss, double slsszjjjss, double dtfw)
        {
            double D = Evaluation.getParamValue(m_Parameters["D"].Factors["SWRK"], swrk, m_ParameterChoices); //死亡人口指标的参数取值；
            double P = Evaluation.getParamValue(m_Parameters["P"].Factors["SZRK"], szrk, m_ParameterChoices);//受灾人口指标的参数取值；
            double A = Evaluation.getParamValue(m_Parameters["A"].Factors["SHMJXJ"], shmjxj, m_ParameterChoices);//农作物受灾面积指标的参数取值；
            double L = Evaluation.getParamValue(m_Parameters["L"].Factors["ZJJJZSS"], zjjjzss, m_ParameterChoices);//直接经济损失指标的参数取值；
            double F = (zjjjzss == 0) ? 0 : Evaluation.getParamValue(m_Parameters["F"].Factors["SLSSZJJJSS_P"], slsszjjjss / zjjjzss, m_ParameterChoices);//水利设施经济损失占直接经济损失比例指标的参数取值；
            double H = Evaluation.getParamValue(m_Parameters["H"].Factors["DTFW"], dtfw, m_ParameterChoices);//倒塌房屋指标的参数取值。

            double C = D * m_Parameters["D"].Weight + P * m_Parameters["P"].Weight + A * m_Parameters["A"].Weight
                + L * m_Parameters["L"].Weight + F * m_Parameters["F"].Weight + H * m_Parameters["H"].Weight; //洪涝灾情评估值
            int grade = Evaluation.getGrade(m_EvaluationGrades, C);
            return grade;
        }

        /// <summary>根据灾情值获取灾情对应等级
        /// 
        /// </summary>
        /// <param name="disasterDatas">灾情值</param>
        /// <returns>灾情等级</returns>
        public int getGrade(double[] disasterDatas,int level)
        {
            DisasterAssessment_Content da_content = new DisasterAssessment_Content(level);
            int[] measureUnitArr = da_content.Get_YL_UnitData();  //获得数据单位
            double[][] gradeArr = da_content.Get_YL_Year_GradeData();//获得灾情等级划分
            int maxGrade = 1;
            for (int i = 0; i < disasterDatas.Length; i++)
            {
                int grade = da_content.GetDataGrade(disasterDatas[i] / measureUnitArr[i], gradeArr[i]);
                if (maxGrade < grade)
                {
                    maxGrade = grade;
                }
            }
            return maxGrade;
        }
    }
}
