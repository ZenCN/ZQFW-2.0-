using iTextSharp.text;
using iTextSharp.text.pdf;
using Svg;
using System;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web.Mvc;



namespace ZQFW.Controllers
{
    [Description("图表导出控制器")]
    public class HightchartsController : Controller
    {
        //
        // GET: /Hightchats/

        //<span style="background-color: #ff6600;">[ValidateInput(false)]</span>
        [HttpPost]
        [ValidateInput(false)]
        public void GetImgFromHightcharts()
        {
            if (Request.Form["type"] != null && Request.Form["svg"] != null && Request.Form["filename"] != null)
            {
                string tType = Request.Form["type"].ToString();
                string tSvg = Request.Form["svg"].ToString();
                string tFileName = Request.Form["filename"].ToString();
                if (tFileName == "")
                {
                    tFileName = "chart";
                }
                MemoryStream tData = new MemoryStream(Encoding.UTF8.GetBytes(tSvg));
                MemoryStream tStream = new MemoryStream();
                string tTmp = new Random().Next().ToString();

                string tExt = "";
                string tTypeString = "";

                switch (tType)
                {
                    case "image/png":
                        tTypeString = "-m image/png";
                        tExt = "png";
                        break;
                    case "image/jpeg":
                        tTypeString = "-m image/jpeg";
                        tExt = "jpg";
                        break;
                    case "application/pdf":
                        tTypeString = "-m application/pdf";
                        tExt = "pdf";
                        break;
                    case "image/svg+xml":
                        tTypeString = "-m image/svg+xml";
                        tExt = "svg";
                        break;
                }

                if (tTypeString != "")
                {
                    string tWidth = Request.Form["width"].ToString();
                    Svg.SvgDocument tSvgObj = SvgDocument.Open(tData);
                    switch (tExt)
                    {
                        case "jpg":
                            tSvgObj.Draw().Save(tStream, ImageFormat.Jpeg);
                            break;
                        case "png":
                            tSvgObj.Draw().Save(tStream, ImageFormat.Png);
                            break;
                        case "pdf":
                            PdfWriter tWriter = null;
                            Document tDocumentPdf = null;
                            try
                            {
                                tSvgObj.Draw().Save(tStream, ImageFormat.Png);
                                tDocumentPdf = new Document(new Rectangle((float)tSvgObj.Width, (float)tSvgObj.Height));
                                tDocumentPdf.SetMargins(0.0f, 0.0f, 0.0f, 0.0f);
                                iTextSharp.text.Image tGraph = iTextSharp.text.Image.GetInstance(tStream.ToArray());
                                tGraph.ScaleToFit((float)tSvgObj.Width, (float)tSvgObj.Height);

                                tStream = new MemoryStream();
                                tWriter = PdfWriter.GetInstance(tDocumentPdf, tStream);
                                tDocumentPdf.Open();
                                tDocumentPdf.NewPage();
                                tDocumentPdf.Add(tGraph);
                                tDocumentPdf.CloseDocument();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            finally
                            {
                                tDocumentPdf.Close();
                                tDocumentPdf.Dispose();
                                tWriter.Close();
                                tWriter.Dispose();
                                tData.Dispose();
                                tData.Close();

                            }
                            break;

                        case "svg":
                            tStream = tData;
                            break;
                    }

                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.ContentType = tType;
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + tFileName + "." + tExt + "");
                    Response.BinaryWrite(tStream.ToArray());
                    Response.End();
                }
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public void saveImgFromHightcharts(string svg)
        {
            MemoryStream tData = new MemoryStream(Encoding.UTF8.GetBytes(svg));
            MemoryStream tStream = new MemoryStream();
            Svg.SvgDocument tSvgObj = SvgDocument.Open(tData);
            tSvgObj.Draw().Save(tStream, ImageFormat.Png);
            DateTime nowTime = DateTime.Now;
            Random random = new Random();
            int RandKey = random.Next(100);
            string fileName = "chart" + nowTime.Year.ToString() + nowTime.Month.ToString() + nowTime.Day.ToString() 
                + nowTime.Hour.ToString() + nowTime.Minute.ToString() + nowTime.Second.ToString()
                + nowTime.Millisecond.ToString() + RandKey.ToString();
            string dir = Server.MapPath("../ExportPicture/");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string savePath = dir + fileName + ".png";
            System.IO.File.WriteAllBytes(savePath, tStream.ToArray());
            Response.Write(savePath);
        }
    }
}