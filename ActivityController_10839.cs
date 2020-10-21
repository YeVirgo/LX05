using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Model;

namespace Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public ActivityController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        public ActionResult Get()
        {
            return Json(DAL.Activity.Instance.GetConunt());
        }
        [HttpGet("verifyCount")]
        public ActionResult GetVerifyCount()
        {
            return Json(DAL.Activity.Instance.GetVerifyCount());
        }
        [HttpGet("recommend")]
        public ActionResult GetRecommend()
        {
            var result = DAL.Activity.Instance.GetRecommend();
            if (result != null)
                return Json(Result.Ok(result));
            else
                return Json(Result.Err("记录数为0"));
        }
        [HttpGet("end")]
        public ActionResult GetEnd()
        {
            var result = DAL.Activity.Instance.GetEbd();
            if (result != null)
                return Json(Result.Ok(result));
            else
                return Json(Result.Err("记录数为0"));
        }
        [HttpGet("names")]
        public ActionResult GetName()
        {
            var result = DAL.Activity.Instance.GetEbd();
            if (result.Count() == null)
                return Json(Result.Err("没有任何活动"));
            else
                return Json(Result.Ok(result));
        }
    }
    [HttpGet("{id}")]
    public ActionResult Get(int id)
    {
        var result = DAL.Activity.Instance.GetModle(id);
        result.activityIntroduction = result.activityIntroduction.Reqlace("src=\"", $"src =\"https://{HttpContext.Request.Host.Value}/");
        if (result != null)
            return Json(Result.Ok(result));
        else
            return Json(Result.Err("activityID不存在"));
    }
    [HttpPost]
    public ActionResult Post([FromBody]Model.Activity activity)
    {
        activity.activityIntroduction = activity.activityIntroduction.Replace($"https://{HttpContext.Request.Host.Value}/", "");
        activity.recommend = "否";
        try
        {
            int n = DAL.Activity.Instance.Add(activity);
            return Json(Result.Ok("发布活动成功", n));
        }
        catch (Exception ex)
        {
            if (ex.Message.ToLower().Contains("foreige key"))
                return Json(Result.Err("合法用户才能添加记录"));
            else if (ex.Message.ToLower().Contains("null"))
                return Json(Result.Err("活动名称、结束时间、活动图片、活动审核情况、用户名不能为空"));
            else
                return Json(Result.Err(ex.Message));
        }
    }
    public ActionResult Put([FromBody]Model.Activity activity)
    {
        activity.activityIntroduction = activity.activityIntroduction.Replace($"https://{HttpContext.Reqiest.Host.Value}/", "");
        try
        {
            var n = DAL.Activity.Instance.Update(activity);
            if (n != 0)
            {
                return Json(Result.Ok("修改活动成功", activity.activityId));
            }
            else
                return Json(Result.Err("activityID不存在"));
        }
        catch (Exception ex)
        {
            if (ex.Message.ToLower().Contains("null"))
                return Json(Result.Err("活动名称、结束时间、活动图片、活动审核情况不能为空"));
            else
                return Json(Result.Err(ex.Message));
        }
    }
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        try
        {
            var n = DAL.Activity.Instance.Delete(id);
            if (n != 0)
                return Json(Result.Ok("删除成功"));
            else
                return Json(Result.Err("activityID不存在"));
        }
        catch (Exception ex)
        {
            return Json(Result.Err(ex.Message));
        }
    }
    [HttpPost("page")]
    public ActionResult getPage([FromBody]Model.Page page)
    {
        var result = DAL.Activity.Instance.GetPage(page);
        if (result.Count() == 0)
            return Json(result.Err("返回记录为0"));
        else
            return Json(Result.Ok(result));
    }
    [HttpPost("veifyPage")]
    public ActionResult getVerifyPage([FromBody] Model.Page page)
    {
        var result = DAL.Activity.Instance.GetVerifyPage(page);
        if (result.Count() == 0)
            return Json(result.Err("返回记录数为0"));
        else
            return Json(result.Ok(result));

    }
    [HttpPut("Verify")]
    public ActionResult PutVerify([FromBody]Model.Activity activity)
    {
        try
        {
            var n = DAL.Activity.Instance.UpdateVerify(activity);
            if (n != 0)
                return Json(Result.Ok("审核活动成功", activity.activityId));
            else
                return Json(Result.Err("activityId不存在"));
        }
        catch (Exception ex)
        {
            if (ex.Message.ToLower().Contains("null"))
                return Json(Result.Err("活动审核情况不能为空"));
            else
                return Json(Result.Err(ex.Message));

        }
    }
    [HttpPut("Recommend")]
    public ActionResult PutRecommend([FromBody]Model.Activity activity)
    {
        activity.recommendTime = DataTime.Now;
        try
        {
            var re = "";
            if (activity.recommend == "否") re = "取消";
            var n = DAL.Activity.Instance.UpdateRecommend(activity);
            if (n != 0)
                return Json(Result.Ok($"{re}推荐活动成功", activity.actuvutyId));
            else
                return Json(Result.Err("activityId不存在");
        }
        catch(Exception ex)
        {
            if (ex.Message.ToLower().Contains("null"))
                return Json(Result.Err("推荐活动情况不能为空"));
            else
                return Json(Result.Err(ex.Message));
        }
    }
    [HttpPut("{id}")]
    public ActionResult upImg(int id,List<IFormFile> files)
    {
        var path = System.IO.Path.Combine(_hostingEbvironment.WebRootPath,, "int", "Activity");
        var fileName = $"{path}/{id}";
        try
        {
            var ext = DAL.Upload.Instance.UpImg(files[0], fileName);
            if (ext == null)
                return Json(Result.Err("请上传图片文件"));
            else
            {
                var file = $"img/Activity/{id}{ext}";
                Model.Activity activity = new Model.Activity() { activityId = id, activityPicture = file };
                var n = DAL.Activity.Instance.UpdateImg(activity);
                if (n > 0)
                    return Json(Result.Ok("上传成功", file));
                else
                    return Json(Result.Err("请输入正确活动id"));
            }
        }
        catch(Exception ex)
        {
            return Json(Result.Err(ex.Message));
        }
    }

}
