using CourseWebsiteDotNet.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.DependencyResolver;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace CourseWebsiteDotNet.Controllers
{
    public class LecturerController : Controller
    {
        public IActionResult Index()
        {
            ViewData["chosenItem"] = 2;
            int? userId = HttpContext.Session.GetInt32("user_id");
            int? role = HttpContext.Session.GetInt32("role");
            int? roleId = HttpContext.Session.GetInt32("role_id");

            var dataTable = SQLExecutor.ExecuteQuery(
                    "SELECT ad.ho_ten, users.anh_dai_dien FROM users INNER JOIN ad ON users.id_ad = ad.id_ad WHERE users.id_user = " + userId
                );

            if (dataTable.Rows.Count > 0)
            {
                // Retrieve data from DataTable
                string? username = dataTable.Rows[0]["ho_ten"].ToString();
                byte[]? avatarData = (byte[])dataTable.Rows[0]["anh_dai_dien"];

                // Set data to ViewData
                ViewData["username"] = username;
                ViewData["role"] = "Administrator";
                ViewData["avatar_data"] = avatarData;

                // Mainsection
                ViewData["teachers"] = SQLExecutor.ExecuteQuery(
                    "SELECT giang_vien.id_giang_vien, giang_vien.ho_ten, giang_vien.email " +
                    "FROM giang_vien"
                );

                return View("AdministratorLecturersList");
            }
            ViewData["ExceptionMessage"] = "Đã có lỗi xảy ra";
            return View("~/Views/Shared/ExeptionPage");
        }
        public IActionResult getInsertForm()
        {
            return View("InsertTeacherForm");
        }

        public IActionResult insertTeacher([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            var teacherModel = new GiangVienModel();

            teacherModel.ho_ten = Convert.ToString(obj.ho_ten);
            teacherModel.ngay_sinh = Convert.ToDateTime(obj.ngay_sinh);
            teacherModel.gioi_tinh = Convert.ToInt32(obj.gioi_tinh);
            teacherModel.email = Convert.ToString(obj.email);
            var courseRepo = new GiangVienRepository();
            var processResult = courseRepo.InsertGiangVien(teacherModel);
            dynamic response = new
            {
                state = processResult.state,
                message = processResult.message,
                auto_increment_id = processResult.insertedId
            };
            return Ok(JsonConvert.SerializeObject(response));
        }

        public IActionResult deleteTeacher([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            var teachers = obj.teachers;
            dynamic responses = new List<dynamic>();
            for (int i = 0; i < teachers.Count; i++)
            {
                var model = new GiangVienRepository();
                var success = model.DeleteGiangVien(Convert.ToInt32(teachers[i]));
                dynamic response = new
                {
                    id_giang_vien = teachers[i],
                    processState = success
                };
                responses.Add(response);
            }

            return Ok(JsonConvert.SerializeObject(responses));
        }

        public IActionResult getUpdateForm([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            var teacherID = Convert.ToInt32(obj.teacherID);
        
            ViewData["lecturer"] = SQLExecutor.ExecuteQuery(
                    "SELECT * " +
                    "FROM giang_vien " +
                    $"WHERE id_giang_vien = {teacherID}"
                );
            ViewData["phancongs"] = SQLExecutor.ExecuteQuery(
                    "SELECT lh.id_lop_hoc, mh.id_mon_hoc, mh.ten_mon_hoc, lh.ngay_bat_dau, lh.ngay_ket_thuc " +
                    "FROM phan_cong_giang_vien pc, lop_hoc lh, mon_hoc mh " +
                    $"WHERE pc.id_giang_vien = {teacherID} " +
                    "AND pc.id_lop_hoc = lh.id_lop_hoc " +
                    "AND mh.id_mon_hoc = lh.id_mon_hoc"
                );
            ViewData["notphancongs"] = SQLExecutor.ExecuteQuery(
                    "SELECT DISTINCT lh.id_lop_hoc, mh.id_mon_hoc, mh.ten_mon_hoc, lh.ngay_bat_dau, lh.ngay_ket_thuc " +
                    "FROM phan_cong_giang_vien pc, lop_hoc lh, mon_hoc mh " +
                    $"WHERE pc.id_giang_vien <> {teacherID} " +
                    "AND pc.id_lop_hoc = lh.id_lop_hoc " +
                    "AND mh.id_mon_hoc = lh.id_mon_hoc " +
                    "AND pc.id_lop_hoc NOT IN(SELECT id_lop_hoc " +
                                            "FROM phan_cong_giang_vien " +
                                            $"WHERE id_giang_vien = {teacherID})"
                );
            return View("UpdateTeacherForm");
        }

        public IActionResult updateTeacher([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            var teacherModel = new GiangVienModel();

            teacherModel.ho_ten = Convert.ToString(obj.ho_ten);
            teacherModel.ngay_sinh = Convert.ToDateTime(obj.ngay_sinh);
            teacherModel.gioi_tinh = Convert.ToInt32(obj.gioi_tinh);
            teacherModel.email = Convert.ToString(obj.email);
            var courseRepo = new GiangVienRepository();
            var processResult = courseRepo.UpdateGiangVien(teacherModel);
            dynamic response = new
            {
                state = processResult.state,
                message = processResult.message,
                auto_increment_id = processResult.insertedId
            };
            return Ok(JsonConvert.SerializeObject(response));
        }

        public IActionResult addClassesIntoListOfTeachingCourses([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            var id_giang_vien = Convert.ToInt32(obj.id_giang_vien);
            var list_id_lop_hoc = obj.list_id_lop_hoc;
            dynamic responses = new List<dynamic>();
            foreach (var item in list_id_lop_hoc)
            {
                var phanCongModel = new PhanCongGiangVienModel();
                var phanCongRepo = new PhanCongGiangVienRepository();
                phanCongModel.id_giang_vien = id_giang_vien;
                phanCongModel.id_lop_hoc = Convert.ToInt32(item.idLopHoc);
                var success = phanCongRepo.InsertPhanCongGiangVien(phanCongModel);
                dynamic response = new
                {
                    course = item.tenLopHoc + " - " + item.idLopHoc,
                    processState = success
                };
                responses.Add(response);
            }

            return Ok(JsonConvert.SerializeObject(responses));
        }

        public IActionResult deleteClassesFromListOfTeachingCourses([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            var id_giang_vien = Convert.ToInt32(obj.id_giang_vien);
            var list_id_lop_hoc = obj.list_id_lop_hoc;
            dynamic responses = new List<dynamic>();
            foreach (var item in list_id_lop_hoc)
            {
                var phanCongModel = new PhanCongGiangVienModel();
                var phanCongRepo = new PhanCongGiangVienRepository();
                phanCongModel.id_giang_vien = id_giang_vien;
                phanCongModel.id_lop_hoc = Convert.ToInt32(item.idLopHoc);
                var success = phanCongRepo.DeletePhanCongGiangVien(phanCongModel);
                dynamic response = new
                {
                    course = item.tenLopHoc + " - " + item.idLopHoc,
                    processState = success
                };
                responses.Add(response);
            }

            return Ok(JsonConvert.SerializeObject(responses));
        }

        public IActionResult liveSearch([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            var model = new GiangVienModel();
            var key = obj.input;
            // $teachers_list_section_layout_data = array();
            var teachers = SQLExecutor.ExecuteQuery(
                    "SELECT id_giang_vien, ho_ten, email " +
                    "FROM giang_vien " +
                    $"WHERE id_giang_vien LIKE('{key}%') " +
                    $"OR ho_ten LIKE('{key}%') " +
                    $"OR email LIKE('{key}%')"
                );
            teachers = teachers as DataTable;
            var list = "";
            for (int i = 0; i < teachers.Rows.Count; i++)
            {
                list = list + $"<div class='col-6 mb-3 teacherCard' teacherid='{teachers.Rows[i]["id_giang_vien"]}'>" +
                                    "<div class='p-3 card shadow-sm'>" +
                                        "<div class='card-body'> " +
                                            $"<h3 class='card-title fs-4'><b>{teachers.Rows[i]["ho_ten"]}</b> - {teachers.Rows[i]["id_giang_vien"]}</h3>" +
                                            "<div class= 'my-5'></div>" +
                                                $"<p class='card-subtitle fs-5'><b> Email:</b> {teachers.Rows[i]["email"]}</p>" +
                                            "</div> " +
                                        $"<input type='checkbox' class='delete-checkbox' value='{teachers.Rows[i]["id_giang_vien"]}'>" +
                                    "</div>" +
                                "</div>";
            }
            HtmlString htmlContent = new HtmlString(list);
            return Content(htmlContent.ToString(), "text/html");
        }
    }
}
