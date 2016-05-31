using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel
{
    public class ZqzsBean
    {
        public string StartDateTime;//开始时间
        public string EndDateTime;//结束时间
        public string DisasterTypeName;//气象因素
        public string JYQDDX;//全省出现XX降雨强度
        public string JYFW;//降雨范围
        public string PJYL;//平均雨量
        public string CPJYL;//超过降雨量
        public string CPJYLZS;//
        public string FGMJ;
        public string GCZDDZYL;
        public string RZDDZYL;
        public string YQZDDZYL;
        public string JMC;
        public string HMC;
        public string CLSZS;
        public string CBZZS;
        public string CJJZS;
        public string ZQSM;
        public string ZQJZRQ;
        public string SZDSMC;
        public string SZFWDSS;
        public string SZFWX;
        public string SZFWZ;
        public string SZRK;
        public string SWRK;
        public string SZRKR;
        public string ZYRK;
        public string DTFW;
        public string ZJJJZSS;
        public string SHMJXJ;
        public string CZMJXJ;
        public string JSMJXJ;
        public string YZJCLS;
        public string SWDSC;
        public string SCYZMJ;
        public string SCYZSL;
        public string NLMYZJJJSS;
        public string TCGKQY;
        public string TLZD;
        public string GLZD;
        public string JCGKGT;
        public string GDZD;
        public string TXZD;
        public string GJYSZJJJSS;
        public string SHSKD;
        public string SKKBD;
        public string SHDFCS;
        public string SHDFCD;
        public string SHHAC;
        public string SHSZ;
        public string CHTB;
        public string SHGGSS;
        public string SHSWCZ;
        public string SHJDBZ;
        public string SHSDZ;
        public string SLSSZJJJSS;
        public string SYCS;
        public string YMFWMJ;
        public string YMFWBL;
        public string GCYMLS;
        public string GCHSWRKR;
        public string GCJJZYRK;
        public string ZYZJZDSS;
        public string SMXGS;
        public string SMXGD;
        public string SMXGQ;
        public string SMXJT;
        public string JZWSYFW;
        public string JZWSYDX;
        public string CQZJJJSS;
        public string ZQTD;
        public string ZQDQ;
        public string KZJZDX;
        public string ZYRYSWRK;
        public string GCHSWKRK;
        public string SimpleUnitName;//所有的单位  UnitData
        public string Limit;//单位级别
        public string UnitName;//单位名称

        /*public string UnitName
        {
            get { return unitName; }
            set { unitName = value; }
        }

        public string Limit
        {
            get { return limit; }
            set { limit = value; }
        }

        public string getUnitData()
        {
            return unitData;
        }
        public void setUnitData(string unitData)
        {
            this.unitData = unitData;
        }


        public string getGchswkrk()
        {
            return gchswkrk;
        }
        public void setGchswkrk(string gchswkrk)
        {
            this.gchswkrk = gchswkrk;
        }

        public string getZyrkswrk()
        {
            return zyrkswrk;
        }
        public void setZyrkswrk(string zyrkswrk)
        {
            this.zyrkswrk = zyrkswrk;
        }

        public string getStartTime()
        {
            return startTime;
        }
        public void setStartTime(string startTime)
        {
            this.startTime = startTime;
        }
        public string getEndTime()
        {
            return endTime;
        }
        public void setEndTime(string endTime)
        {
            this.endTime = endTime;
        }
        public string getDisastertypename()
        {
            return disastertypename;
        }
        public void setDisastertypename(string disastertypename)
        {
            this.disastertypename = disastertypename;
        }
        public string getJyqddx()
        {
            return jyqddx;
        }
        public void setJyqddx(string jyqddx)
        {
            this.jyqddx = jyqddx;
        }
        public string getJyfw()
        {
            return jyfw;
        }
        public void setJyfw(string jyfw)
        {
            this.jyfw = jyfw;
        }
        public string getPjyl()
        {
            return pjyl;
        }
        public void setPjyl(string pjyl)
        {
            this.pjyl = pjyl;
        }
        public string getCpjyl()
        {
            return cpjyl;
        }
        public void setCpjyl(string cpjyl)
        {
            this.cpjyl = cpjyl;
        }
        public string getCpjylzs()
        {
            return cpjylzs;
        }
        public void setCpjylzs(string cpjylzs)
        {
            this.cpjylzs = cpjylzs;
        }
        public string getFgmj()
        {
            return fgmj;
        }
        public void setFgmj(string fgmj)
        {
            this.fgmj = fgmj;
        }
        public string getGczddzyl()
        {
            return gczddzyl;
        }
        public void setGczddzyl(string gczddzyl)
        {
            this.gczddzyl = gczddzyl;
        }
        public string getRzddzyl()
        {
            return rzddzyl;
        }
        public void setRzddzyl(string rzddzyl)
        {
            this.rzddzyl = rzddzyl;
        }
        public string getYqzddzyl()
        {
            return yqzddzyl;
        }
        public void setYqzddzyl(string yqzddzyl)
        {
            this.yqzddzyl = yqzddzyl;
        }
        public string getJmc()
        {
            return jmc;
        }
        public void setJmc(string jmc)
        {
            this.jmc = jmc;
        }
        public string getHmc()
        {
            return hmc;
        }
        public void setHmc(string hmc)
        {
            this.hmc = hmc;
        }
        public string getClszs()
        {
            return clszs;
        }
        public void setClszs(string clszs)
        {
            this.clszs = clszs;
        }
        public string getCbzzs()
        {
            return cbzzs;
        }
        public void setCbzzs(string cbzzs)
        {
            this.cbzzs = cbzzs;
        }
        public string getZqsm()
        {
            return zqsm;
        }
        public void setZqsm(string zqsm)
        {
            this.zqsm = zqsm;
        }
        public string getZqjzrq()
        {
            return zqjzrq;
        }
        public void setZqjzrq(string zqjzrq)
        {
            this.zqjzrq = zqjzrq;
        }
        public string getSzdsmc()
        {
            return szdsmc;
        }
        public void setSzdsmc(string szdsmc)
        {
            this.szdsmc = szdsmc;
        }
        public string getSzfwdss()
        {
            return szfwdss;
        }
        public void setSzfwdss(string szfwdss)
        {
            this.szfwdss = szfwdss;
        }
        public string getSzfwx()
        {
            return szfwx;
        }
        public void setSzfwx(string szfwx)
        {
            this.szfwx = szfwx;
        }
        public string getSzfwz()
        {
            return szfwz;
        }
        public void setSzfwz(string szfwz)
        {
            this.szfwz = szfwz;
        }
        public string getSzrk()
        {
            return szrk;
        }
        public void setSzrk(string szrk)
        {
            this.szrk = szrk;
        }
        public string getSwrk()
        {
            return swrk;
        }
        public void setSwrk(string swrk)
        {
            this.swrk = swrk;
        }
        public string getZyryswrk()
        {
            return zyryswrk;
        }
        public void setZyryswrk(string zyryswrk)
        {
            this.zyryswrk = zyryswrk;
        }
        public string getSzrkr()
        {
            return szrkr;
        }
        public void setSzrkr(string szrkr)
        {
            this.szrkr = szrkr;
        }
        public string getZyrk()
        {
            return zyrk;
        }
        public void setZyrk(string zyrk)
        {
            this.zyrk = zyrk;
        }
        public string getDtfw()
        {
            return dtfw;
        }
        public void setDtfw(string dtfw)
        {
            this.dtfw = dtfw;
        }
        public string getZjjjzss()
        {
            return zjjjzss;
        }
        public void setZjjjzss(string zjjjzss)
        {
            this.zjjjzss = zjjjzss;
        }
        public string getShmjxj()
        {
            return shmjxj;
        }
        public void setShmjxj(string shmjxj)
        {
            this.shmjxj = shmjxj;
        }
        public string getCzmjxj()
        {
            return czmjxj;
        }
        public void setCzmjxj(string czmjxj)
        {
            this.czmjxj = czmjxj;
        }
        public string getJsmjxj()
        {
            return jsmjxj;
        }
        public void setJsmjxj(string jsmjxj)
        {
            this.jsmjxj = jsmjxj;
        }
        public string getYzjcls()
        {
            return yzjcls;
        }
        public void setYzjcls(string yzjcls)
        {
            this.yzjcls = yzjcls;
        }
        public string getSwdsc()
        {
            return swdsc;
        }
        public void setSwdsc(string swdsc)
        {
            this.swdsc = swdsc;
        }
        public string getScyzmj()
        {
            return scyzmj;
        }
        public void setScyzmj(string scyzmj)
        {
            this.scyzmj = scyzmj;
        }
        public string getScyzsl()
        {
            return scyzsl;
        }
        public void setScyzsl(string scyzsl)
        {
            this.scyzsl = scyzsl;
        }
        public string getNlmyzjjjss()
        {
            return nlmyzjjjss;
        }
        public void setNlmyzjjjss(string nlmyzjjjss)
        {
            this.nlmyzjjjss = nlmyzjjjss;
        }
        public string getTcgkqy()
        {
            return tcgkqy;
        }
        public void setTcgkqy(string tcgkqy)
        {
            this.tcgkqy = tcgkqy;
        }
        public string getTlzd()
        {
            return tlzd;
        }
        public void setTlzd(string tlzd)
        {
            this.tlzd = tlzd;
        }
        public string getGlzd()
        {
            return glzd;
        }
        public void setGlzd(string glzd)
        {
            this.glzd = glzd;
        }
        public string getJcgkgt()
        {
            return jcgkgt;
        }
        public void setJcgkgt(string jcgkgt)
        {
            this.jcgkgt = jcgkgt;
        }
        public string getTxzd()
        {
            return txzd;
        }
        public void setTxzd(string txzd)
        {
            this.txzd = txzd;
        }
        public string getGjyszjjjss()
        {
            return gjyszjjjss;
        }
        public void setGjyszjjjss(string gjyszjjjss)
        {
            this.gjyszjjjss = gjyszjjjss;
        }
        public string getShskd()
        {
            return shskd;
        }
        public void setShskd(string shskd)
        {
            this.shskd = shskd;
        }
        public string getSkkbd()
        {
            return skkbd;
        }
        public void setSkkbd(string skkbd)
        {
            this.skkbd = skkbd;
        }
        public string getShdfcs()
        {
            return shdfcs;
        }
        public void setShdfcs(string shdfcs)
        {
            this.shdfcs = shdfcs;
        }
        public string getShdfcd()
        {
            return shdfcd;
        }
        public void setShdfcd(string shdfcd)
        {
            this.shdfcd = shdfcd;
        }
        public string getShhac()
        {
            return shhac;
        }
        public void setShhac(string shhac)
        {
            this.shhac = shhac;
        }
        public string getShsz()
        {
            return shsz;
        }
        public void setShsz(string shsz)
        {
            this.shsz = shsz;
        }
        public string getChtb()
        {
            return chtb;
        }
        public void setChtb(string chtb)
        {
            this.chtb = chtb;
        }
        public string getShswcz()
        {
            return shswcz;
        }
        public void setShswcz(string shswcz)
        {
            this.shswcz = shswcz;
        }
        public string getShjdbz()
        {
            return shjdbz;
        }
        public void setShjdbz(string shjdbz)
        {
            this.shjdbz = shjdbz;
        }
        public string getShsdz()
        {
            return shsdz;
        }
        public void setShsdz(string shsdz)
        {
            this.shsdz = shsdz;
        }
        public string getSlsszjjjss()
        {
            return slsszjjjss;
        }
        public void setSlsszjjjss(string slsszjjjss)
        {
            this.slsszjjjss = slsszjjjss;
        }
        public string getSycss()
        {
            return sycss;
        }
        public void setSycss(string sycss)
        {
            this.sycss = sycss;
        }
        public string getYmfwmj()
        {
            return ymfwmj;
        }
        public void setYmfwmj(string ymfwmj)
        {
            this.ymfwmj = ymfwmj;
        }
        public string getYmfwbl()
        {
            return ymfwbl;
        }
        public void setYmfwbl(string ymfwbl)
        {
            this.ymfwbl = ymfwbl;
        }
        public string getGcymls()
        {
            return gcymls;
        }
        public void setGcymls(string gcymls)
        {
            this.gcymls = gcymls;
        }
        public string getGchswrkr()
        {
            return gchswrkr;
        }
        this.Age = "XX";
        public void Age(string age)
        {
            if (typeof (age) == int)
            {
                this.age = age;
            }
            else
            {
                this.age = "";
            }

            this.gchswrkr = gchswrkr;
        }
        public string getGcjjzyrk()
        {
            return gcjjzyrk;
        }
        public void setGcjjzyrk(string gcjjzyrk)
        {
            this.gcjjzyrk = gcjjzyrk;
        }
        public string getZyzjzdss()
        {
            return zyzjzdss;
        }
        public void setZyzjzdss(string zyzjzdss)
        {
            this.zyzjzdss = zyzjzdss;
        }
        public string getSmxgs()
        {
            return smxgs;
        }
        public void setSmxgs(string smxgs)
        {
            this.smxgs = smxgs;
        }
        public string getSmxgd()
        {
            return smxgd;
        }
        public void setSmxgd(string smxgd)
        {
            this.smxgd = smxgd;
        }
        public string getSmxgq()
        {
            return smxgq;
        }
        public void setSmxgq(string smxgq)
        {
            this.smxgq = smxgq;
        }
        public string getSmxjt()
        {
            return smxjt;
        }
        public void setSmxjt(string smxjt)
        {
            this.smxjt = smxjt;
        }
        public string getJzwsyfw()
        {
            return jzwsyfw;
        }
        public void setJzwsyfw(string jzwsyfw)
        {
            this.jzwsyfw = jzwsyfw;
        }
        public string getJzwsydx()
        {
            return jzwsydx;
        }
        public void setJzwsydx(string jzwsydx)
        {
            this.jzwsydx = jzwsydx;
        }
        public string getCqzjjjss()
        {
            return cqzjjjss;
        }
        public void setCqzjjjss(string cqzjjjss)
        {
            this.cqzjjjss = cqzjjjss;
        }
        public string getCjjzs()
        {
            return cjjzs;
        }
        public void setCjjzs(String cjjzs)
        {
            this.cjjzs = cjjzs;
        }
        public string getGdzd()
        {
            return gdzd;
        }
        public void setGdzd(string gdzd)
        {
            this.gdzd = gdzd;
        }

        public string getShggss()
        {
            return shggss;
        }
        public void setShggss(string shggss)
        {
            this.shggss = shggss;
        }
        public string getZqtd()
        {
            return zqtd;
        }
        public void setZqtd(string zqtd)
        {
            this.zqtd = zqtd;
        }
        public string getZqdq()
        {
            return zqdq;
        }
        public void setZqdq(string zqdq)
        {
            this.zqdq = zqdq;
        }
        public string getKzjzdx()
        {
            return kzjzdx;
        }
        public void setKzjzdx(string kzjzdx)
        {
            this.kzjzdx = kzjzdx;
        }*/

    }

    public class ZqzsBean_15
    {
        //雨情
        public string UnitName;  //单位名称
        public string StartDateTime;  //开始时间
        public string EndDateTime;  //结束时间
        /*ppublic string ZGJMS;
        public string JMS;
        public string JQX;
        public string QMJYL;
        public string GCJYLMax;
        public string GCJYL;
        public string RJYLMax;
        public string RJYL;*/
        //水情
        /*public string JMC;
        public string HMC;
        public string CLSZS;
        public string CBZZS;
        public string CJJZS;*/
        //灾情
        public string ZQKSRQ;
        public string ZQJZRQ;
        public string SZFWDSS;
        public string ZYRYSWRK;
        public string GCHSWKRK;
        public string SZFWX;
        public string SZFWZ;
        public string SZRK;
        public string SWRK;
        public string SZRKR;
        public string ZYRK;
        public string DTFW;
        public string ZJJJZSS;
        public string SHMJXJ;
        public string CZMJXJ;
        public string JSMJXJ;
        public string YZJCLS;
        public string SWDSC;
        public string SCYZMJ;
        public string SCYZSL;
        public string NLMYZJJJSS;
        public string TCGKQY;
        public string TLZD;
        public string GLZD;
        public string JCGKGT;
        public string GDZD;
        public string TXZD;
        public string GJYSZJJJSS;
        public string SHSKD;
        public string SKKBD;
        public string SHDFCS;
        public string SHDFCD;
        public string SHHAC;
        public string SHSZ;
        public string CHTB;
        public string SHGGSS;
        public string SHSWCZ;
        public string SHJDBZ;
        public string SHSDZ;
        public string SLSSZJJJSS;
        public string SYCS;
        public string YMFWMJ;
        public string YMFWBL;
        public string GCYMLS;
        public string GCJJZYRK;
        public string ZYZJZDSS;
        public string SMXGS;
        public string SMXGD;
        public string SMXGQ;
        public string SMXJT;
        public string JZWSYFW;
        public string JZWSYDX;
        public string CQZJJJSS;

        //HL014 "QXHJ", "QXBDGB", "QXDFRY", "QXFXJD", "ZJXJ", "ZJZY", "ZJSJ", "ZJSJYS", "ZJQZ"
        public string QXHJ;
        public string QXBDGB;
        public string QXDFRY;
        public string QXFXJD;
        public string ZJXJ;
        public string ZJZY;
        public string ZJSJ;
        public string ZJSJYS;
        public string ZJQZ;
        //抢先投入
        /*public string QXTR;*/
    }

    public class ZqzsBean_45
    {
        //雨情
        public string UnitName;  //单位名称
        public string TimeNow;  //开始时间

        public string StartDateTime;
        public string EndDateTime;
        public string SZDQ;
        public string SZRK;
        public string GCHSWKRK;
        public string GCJJZYRK;
        public string DTFW;
        public string SHMJXJ;
        public string CZMJXJ;
        public string JSMJXJ;
        public string YZJCLS;
        public string JJZWSS;
        public string SWDSC;
        public string SCYZSL;
        public string TCGKQY;
        public string GLZD;
        public string GDZD;
        public string TXZD;
        public string SHDFCS;
        public string SHDFCD;
        public string SHHAC;
        public string SHSZ;
        public string SHJDJ;
        public string SHJDBZ;
        public string SHSWCZ;
        public string ZJJJZSS;
        public string NLMYZJJJSS;
        public string GJYSZJJJSS;
        public string SLSSZJJJSS;
        public string QXHJ;
        public string SBJX;
        public string WZBZD;
        public string WZBZB;
        public string WZSSL;
        public string WZMC;
        public string WZGC;
        public string WZY;
        public string WZD;
        public string WZZXH;
        public string XYJYGD;
        public string XYJMLSJS;
        public string XYJSSZRK;
        public string XYJJQZ;
        public string XYJZJJXY;
    }
}