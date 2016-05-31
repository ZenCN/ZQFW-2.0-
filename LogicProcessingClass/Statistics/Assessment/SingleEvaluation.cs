using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace LogicProcessingClass.Statistics
{
    public class SingleEvaluation
    {
        Dictionary<string, EvaluationParameter> m_Parameters;  //场次灾情评估参数
        IList<Threshold> m_ParameterChoices;  //场次洪涝灾情评估参数取值
        IList<Threshold> m_EvaluationGrades;     //场次洪涝灾情评估值与对应洪涝灾害等级
        IList<Threshold> m_EvaluationGrades_s;     //死亡人口与对应洪涝灾害等级
        IList<Threshold> m_EvaluationGrades_z;     //直接经济损失与对应洪涝灾害等级

        /// <summary>场次灾情评估构造函数
        /// 
        /// </summary>
        public SingleEvaluation()
        {
            Evaluation evaluation = new Evaluation();
            m_Parameters = evaluation.getEvalParams(0);
            m_ParameterChoices = evaluation.getParameterChoices(0);
            m_EvaluationGrades = evaluation.getEvaluationGrades(0, 0);
            m_EvaluationGrades_s = evaluation.getEvaluationGrades(0, 1);
            m_EvaluationGrades_z = evaluation.getEvaluationGrades(0, 2);
        }

        /// <summary>使用直接判定法和指标权重法综合计算灾情等级
        /// 
        /// </summary>
        /// <param name="swrk">死亡人口</param>
        /// <param name="szrk">受灾人口</param>
        /// <param name="shmjxj">受灾面积</param>
        /// <param name="zjjjzss">直接经济损失</param>
        /// <param name="slsszjjjss">水里经济损失占直接经济损失比例</param>
        /// <param name="dtfw">倒塌房屋</param>
        /// <param name="smxjt">生命线交通中断历时</param>
        /// <param name="gcymls">城市受淹历时</param>
        /// <param name="smxgs">生命线供水中断历时</param>
        /// <param name="smxgd">生命线供电中断历时</param>
        /// <param name="smxgq">生命线供气中断历时</param>
        /// <param name="polulation">人口</param>
        /// <param name="landArea">耕地面积</param>
        /// <returns>灾情等级</returns>
        public int getGrade(double swrk, double szrk, double shmjxj, double zjjjzss, double slsszjjjss, double dtfw,
                        double smxjt, double gcymls, double smxgs, double smxgd, double smxgq, double polulation, double landArea)
        {
            int grade1 = getGradeBySigFactor(swrk, "SWRK"); //使用死亡人口判断灾情等级
            int grade2 = getGradeBySigFactor(zjjjzss / 100000000, "ZJJJZSS");  //使用直接经济损失判断灾情等级
            int grade = Math.Max(grade1, grade2);
            if (grade == 4)   //如果灾情等级已经判断为最高级别（特别重大灾害），返回灾情等级
            {
                return grade;
            }
            double D = Evaluation.getParamValue(m_Parameters["D"].Factors["SWRK"], swrk, m_ParameterChoices);// —— 死亡人口指标的参数取值
            double P1 = Evaluation.getParamValue(m_Parameters["P"].Factors["SZRK"], szrk, m_ParameterChoices);  //受灾人口指标参数取值
            double P2 = (polulation == 0) ? 0 : Evaluation.getParamValue(m_Parameters["P"].Factors["SZRK_P"], szrk / polulation * 100, m_ParameterChoices);//受灾人口占区域人口比例指标参数取值
            double P = Math.Max(P1, P2); //受灾人口指标的参数取值
            double A1 = Evaluation.getParamValue(m_Parameters["A"].Factors["SHMJXJ"], shmjxj, m_ParameterChoices); //农作物受灾面积指标参数取值
            double A2 = (landArea == 0) ? 0 : Evaluation.getParamValue(m_Parameters["A"].Factors["SHMJXJ_P"], shmjxj / landArea * 100, m_ParameterChoices);  //农作物受灾面积占区域耕地面积比例指标参数取值
            double A = Math.Max(A1, A2); //农作物受灾面积指标的参数取值
            double L1 = Evaluation.getParamValue(m_Parameters["L"].Factors["ZJJJZSS"], zjjjzss, m_ParameterChoices); //直接经济损失指标参数取值
            double L2 = 0; //直接经济损失占上一年区域GDP比例指标参数取值
            double L = Math.Max(L1, L2); //直接经济损失指标的参数取值
            double F = (zjjjzss == 0) ? 0 : Evaluation.getParamValue(m_Parameters["F"].Factors["SLSSZJJJSS_P"], slsszjjjss / zjjjzss * 100, m_ParameterChoices); //水利设施经济损失占直接经济损失比例指标的参数取值
            double H = Evaluation.getParamValue(m_Parameters["H"].Factors["DTFW"], dtfw, m_ParameterChoices); //倒塌房屋指标的参数取值
            double R1 = 0; //骨干交通(铁路、公路干线、主要航道中断)中断历时指标的参数取值
            double R2 = Evaluation.getParamValue(m_Parameters["R2"].Factors["GGJT_R2"], smxjt, m_ParameterChoices); //骨干交通(城市主要街道)中断历时指标的参数取值
            double S = Evaluation.getParamValue(m_Parameters["S"].Factors["GCYMLS"], gcymls, m_ParameterChoices); //城市受淹历时指标的参数取值
            double smxg = Math.Max(Math.Max(smxgs, smxgd), smxgq);     //生命线工程中断历时(取供水、供电、供气中断历时最大值)
            double T = Evaluation.getParamValue(m_Parameters["T"].Factors["SMXG"], smxg, m_ParameterChoices); //生命线工程中断历时指标的参数取值
            double C = D * m_Parameters["D"].Weight + P * m_Parameters["P"].Weight + A * m_Parameters["A"].Weight
                + L * m_Parameters["L"].Weight + F * m_Parameters["F"].Weight + H * m_Parameters["H"].Weight
                + R1 * m_Parameters["R1"].Weight + R2 * m_Parameters["R2"].Weight + S * m_Parameters["S"].Weight
                + T * m_Parameters["T"].Weight; //洪涝灾情评估值
            int grade3 = Evaluation.getGrade(m_EvaluationGrades, C);  //获取洪涝灾情评估等级
            return Math.Max(grade, grade3);
        }

        /// <summary>根据单一指标判断灾情等级
        /// 
        /// </summary>
        /// <param name="factorValue">指标值</param>
        /// <param name="factor">指标名称</param>
        /// <returns>灾情等级</returns>
        int getGradeBySigFactor(double factorValue, string factor)
        {
            int grade = 1;
            IList<Threshold> evaluationGrades = new List<Threshold>();
            switch (factor)
            {
                case "SWRK":   //死亡人口
                    evaluationGrades = m_EvaluationGrades_s;
                    break;
                case "ZJJJZSS": //直接经济损失
                    evaluationGrades = m_EvaluationGrades_z;
                    break;
                default:
                    break;
            }
            for (int i = 0; i < evaluationGrades.Count; i++)
            {
                Threshold evaluationGrade = evaluationGrades[i];
                if (evaluationGrade.HigherValue == null)
                {
                    if (factorValue >= (double)evaluationGrade.LowerValue)
                    {
                        grade = evaluationGrade.Grade;
                        break;
                    }
                }
                else if (factorValue >= (double)evaluationGrade.LowerValue && factorValue < (double)evaluationGrade.HigherValue)
                {
                    grade = evaluationGrade.Grade;
                    break;
                }
            }
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
            double[][] gradeArr = da_content.Get_YL_GradeData();//获得灾情等级划分
            int maxGrade = 1;
            for (int i = 0; i < disasterDatas.Length; i++)
            {
                int grade = da_content.GetDataGrade(disasterDatas[i]/measureUnitArr[i], gradeArr[i]);
                if (maxGrade < grade)
                {
                    maxGrade = grade;
                }
            }
            return maxGrade;
        }
    }
}
