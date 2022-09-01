using System;
using System.Collections.Generic;
using SkyCarsWebAPI.Models.Common;
using SkyCars.Core;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using SkyCarsWebAPI.Models.Common;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.Net.Http.Headers;
using ClosedXML.Excel;
using SkyCars.Core.DomainEntity.Grid;

namespace SkyCarsWebAPI.Extensions
{
    public static class CommonExtensions
    {
        public static HttpContext HttpContextAccessor => new HttpContextAccessor().HttpContext;
        public static GridResponseModel ToGrid<T>(this IPagedList<T> GridParam)
        {
            try
            {
                return new GridResponseModel() { Data = GridParam, RecordsFiltered = GridParam.TotalCount, RecordsTotal = GridParam.TotalCount };
            }
            catch (Exception)
            {
                return new GridResponseModel();
            }
        }
        public static IList<SelectListItem> ToDropDown<T>(this IList<T> drpList, string dropValColName = "Id", string dropDisColName = "Name",string drpGrpColName="")
        {
            //return drpList.Select(x => new DropDownClass() { Value = (typeof(T).GetProperty(dropValColName).GetValue(x)).ToString(), Text = typeof(T).GetProperty(dropDisColName).GetValue(x).ToString() }).ToList();
            if (dropValColName.Split('|').Length == 1)
                return drpList.Select(x => new SelectListItem() { Value = (typeof(T).GetProperty(dropValColName).GetValue(x)).ToString(), Text = typeof(T).GetProperty(dropDisColName).GetValue(x).ToString(),Group = new SelectListGroup() { Name = !string.IsNullOrEmpty(drpGrpColName) ? typeof(T).GetProperty(drpGrpColName).GetValue(x).ToString() : "" }}).ToList();
            else
                return drpList.Select(x => new SelectListItem() { Value = (typeof(T).GetProperty(dropValColName.Split('|')[0]).GetValue(x)).ToString() + "|" + (typeof(T).GetProperty(dropValColName.Split('|')[1]).GetValue(x)).ToString(), Text = typeof(T).GetProperty(dropDisColName).GetValue(x).ToString(),Group =  new SelectListGroup() { Name = !string.IsNullOrEmpty(drpGrpColName) ? typeof(T).GetProperty(drpGrpColName).GetValue(x).ToString() : "" } }).ToList();
        }
        /// <summary>
        /// This method is used to return status based on data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns>
        /// if data null then not found otherwise 200ok
        /// </returns>
        public static ApiStatusCode GetApiResponseStatusCodeByData<T>(this T data)
        {
            return data is null ? ApiStatusCode.Status404NotFound : ApiStatusCode.Status200OK;
        }

        //#region :: Export  ::
        //public static IActionResult ExportToGrid<T>(this IPagedList<T> GridParam,ExportTypeEnum ExportType, string FileName, string TimeZone = "+05:30")
        //{
        //    FileName=FileName+(ExportType==ExportTypeEnum.Excel? ".xlsx" :".pdf");                        
        //    return  new FileStreamResult(
        //            ExportType==ExportTypeEnum.Excel ? ExportToExcel(GridParam,FileName,"+05:30") : ExportToPdf(GridParam,"+05:30"),
        //            new MediaTypeHeaderValue(
        //            ExportType==ExportTypeEnum.Excel ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet": "application/pdf")) 
        //        {
        //            FileDownloadName = FileName
        //        };
        //}
        //private static System.IO.Stream ExportToExcel<T>(IPagedList<T> GridParam, string FileName, string TimeZone = "+05:30")
        //{
        //    var PropList=GridParam[0].GetType().GetProperties();
        //    #region :: Get Custom Attribute ::
        //    List<string> _dict = new List<string>();            
        //    foreach (var prop in PropList)
        //    {
        //        foreach (object attr in prop.GetCustomAttributes(true))
        //        {
        //           if(attr.GetType()==typeof(NoColumnExport))
        //                _dict.Add(prop.Name);
        //        }
        //    }
        //    #endregion
        //    #region worksheet
        //    using var excel = new XLWorkbook();
        //    IXLWorksheet workSheet = excel.Worksheets.Add(FileName?.Length == 0 ? "Sheet1" : FileName);// set sheet name
        //    int  col=1;
        //    foreach (var propInfo in PropList)
        //    {
        //        if(!_dict.Contains(propInfo.Name.ToString()))
        //        {
        //            workSheet.Cell(1,col).Value = propInfo.Name;
        //            workSheet.Cell(1,col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;                  
        //            col++;
        //        }
        //    }

        //    var headerRwo = workSheet.Row(1);
        //    headerRwo.Style.Font.Bold = true;

        //    workSheet.Row(1).Height = 30;
        //    int j = 2; //from 2 bcs row one is header
            
        //    foreach (var item in GridParam)
        //    {
        //        int i=0;
        //        foreach (var propInfo in item.GetType().GetProperties())
        //        {   
        //            if(!_dict.Contains(propInfo.Name.ToString()))
        //            {                 
        //                if (propInfo.PropertyType == typeof(System.DateTime))
        //                {
        //                    if(propInfo.GetValue(item, null) != null)
        //                    {
        //                        DateTime Coldatetime = Convert.ToDateTime(propInfo.GetValue(item)).ToLocalDateTime(TimeZone);
        //                        propInfo.SetValue(item,Coldatetime.ToString(),null);
        //                    }
        //                }                    
        //                workSheet.Cell(j, i + 1).Value = propInfo.GetValue(item);
        //                workSheet.Cell(j, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;                       
        //                i++;
        //            }
        //        }
        //        workSheet.Row(j).Height = 30;
        //        j++;
        //    }

        //    System.IO.Stream spreadsheetStream = new System.IO.MemoryStream();
        //    excel.SaveAs(spreadsheetStream);
        //    spreadsheetStream.Position = 0;
        //    return spreadsheetStream;            
        //    #endregion            
        //}
        //private static System.IO.MemoryStream ExportToPdf<T>(IPagedList<T> GridParam, string TimeZone = "+05:30")        
        //{
        //     string html = "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>" +
        //        "<head><meta http-equiv='Content-Type' content='text/html; charset=UTF-8'><meta name='viewport' content='width=device-width'><meta http-equiv='X-UA-Compatible' content='IE=edge'>" +
        //        "<title>SamplePdf</title></head><body><table cellpadding='5' border='1' style='border-spacing:0px;'>";
        //     var PropList=GridParam[0].GetType().GetProperties();
        //     #region :: Get Custom Attribute ::
        //     List<string> _dict = new List<string>();            
        //     foreach (var prop in PropList)
        //     {
        //         foreach (object attr in prop.GetCustomAttributes(true))
        //         {
        //         if(attr.GetType()==typeof(NoColumnExport))
        //                 _dict.Add(prop.Name);
        //         }
        //     }
        //     #endregion
        //     //add header row
        //     html += "<tr>";
        //      foreach (var propInfo in PropList)
        //     {
        //         if(!_dict.Contains(propInfo.Name.ToString()))
        //         {
        //             html += "<th>" + propInfo.Name + "</th>";                     
        //         }
        //     }                    
        //     html += "</tr>";
        //     //add rows
        //     int j = 2; //from 2 bcs row one is header            
        //     foreach (var item in GridParam)
        //     {
        //         int i=0;
        //         html += "<tr>";
        //         foreach (var propInfo in item.GetType().GetProperties())
        //         {   
        //             if(!_dict.Contains(propInfo.Name.ToString()))
        //             {                 
        //                 if (propInfo.PropertyType == typeof(System.DateTime))
        //                 {
        //                     if(propInfo.GetValue(item, null) != null)
        //                     {
        //                         DateTime Coldatetime = Convert.ToDateTime(propInfo.GetValue(item)).ToLocalDateTime(TimeZone);
        //                         propInfo.SetValue(item,Coldatetime.ToString(),null);
        //                     }
        //                 }  
        //                 html += "<td>" + propInfo.GetValue(item)+ "</td>";                  
        //                 i++;
        //             }
        //         }
        //         html += "</tr>";
        //         j++;
        //     }
        //    html += "</table></body></html>";             
        //    HtmlConverter htmlConverter = new HtmlConverter(html);
        //    var outputByte = htmlConverter.Convert();
        //    MemoryStream ms = new(outputByte);
        //    ms.Position = 0;
        //    return  ms;
        //}
        //#endregion
        //public static DateTime ToLocalDateTime(this DateTime OnDate, string TimeZone)
        //{
        //    TimeSpan ts = string.IsNullOrEmpty(TimeZone) ? new TimeSpan() : TimeSpan.Parse(TimeZone.Replace("+", ""));
        //    return OnDate.Add(ts);
        //}
    }
}