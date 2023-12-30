using CourseWebsiteDotNet.Models;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Text.Json;

namespace CourseWebsiteDotNet.Controllers
{
    public class AssignmentController : Controller
    {
        private bool checkPreviledge(int courseid)
        {
            int? userId = HttpContext.Session.GetInt32("user_id");
            int? role = HttpContext.Session.GetInt32("role");
            int? roleId = HttpContext.Session.GetInt32("role_id");

            if (role == 1)
            {
                return true;
            }
            else if (role == 2)
            {
                var rs = SQLExecutor.ExecuteQuery($@"SELECT * FROM phan_cong_giang_vien WHERE phan_cong_giang_vien.id_giang_vien = {roleId} AND phan_cong_giang_vien.id_lop_hoc = {courseid}");
                return rs.Rows.Count == 0 ? false : true;

            }
            else if (role == 3)
            {
                var rs = SQLExecutor.ExecuteQuery($@"SELECT * FROM hoc_vien_tham_gia WHERE hoc_vien_tham_gia.id_hoc_vien = {roleId} AND hoc_vien_tham_gia.id_lop_hoc = {courseid}");
                return rs.Rows.Count == 0 ? false : true;
            }
            return false;
        }
        private void LoadNavbar()
		{
			int? userId = HttpContext.Session.GetInt32("user_id");
			int? role = HttpContext.Session.GetInt32("role");
			int? roleId = HttpContext.Session.GetInt32("role_id");

			if (role == 1)
			{
				var dataTable = SQLExecutor.ExecuteQuery(
				   "SELECT ad.ho_ten, users.anh_dai_dien FROM users INNER JOIN ad ON users.id_ad = ad.id_ad WHERE users.id_user = " + userId
				);
				string? username = dataTable.Rows[0]["ho_ten"].ToString();
				byte[]? avatarData = dataTable.Rows[0]["anh_dai_dien"] != DBNull.Value ? dataTable.Rows[0]["anh_dai_dien"] as byte[] : null;

				// Set data to ViewData
				// Navbar data
				ViewData["username"] = username;
				ViewData["role"] = "Administrator";
				ViewData["avatar_data"] = avatarData;
			}
			else if (role == 2)
			{
				var dataTable = SQLExecutor.ExecuteQuery(
				   $@"SELECT giang_vien.ho_ten, giang_vien.id_giang_vien, users.anh_dai_dien
                    FROM users
                    INNER JOIN giang_vien ON users.id_giang_vien = giang_vien.id_giang_vien
                    WHERE users.id_user = {userId}"
				);
				string? username = dataTable.Rows[0]["ho_ten"].ToString();
				byte[]? avatarData = dataTable.Rows[0]["anh_dai_dien"] != DBNull.Value ? dataTable.Rows[0]["anh_dai_dien"] as byte[] : null;

				// Set data to ViewData
				// Navbar data
				ViewData["username"] = username;
				ViewData["role"] = "Giảng viên";
				ViewData["avatar_data"] = avatarData;
			}
			else if (role == 3)
			{
				var dataTable = SQLExecutor.ExecuteQuery(
				   $@"SELECT hoc_vien.ho_ten, hoc_vien.id_hoc_vien, users.anh_dai_dien
                FROM users
                INNER JOIN hoc_vien ON users.id_hoc_vien = hoc_vien.id_hoc_vien
                WHERE users.id_user = {userId}"
				);
				string? username = dataTable.Rows[0]["ho_ten"].ToString();
				byte[]? avatarData = dataTable.Rows[0]["anh_dai_dien"] != DBNull.Value ? dataTable.Rows[0]["anh_dai_dien"] as byte[] : null;

				// Set data to ViewData
				// Navbar data
				ViewData["username"] = username;
				ViewData["role"] = "Học viên";
				ViewData["avatar_data"] = avatarData;
			}

		}
		public IActionResult Assignment(int? assignmentid)
        {
			if (assignmentid == null || assignmentid == 0)
			{
				LoadNavbar();
				ViewData["ExceptionMessage"] = "Bài tập không tồn tại";
				return View("ExceptionPage");
			}

			// Assignment có tồn tại hay không
			BaiTapModel bt = new BaiTapRepository().GetBaiTapById((int)assignmentid);
			
			if (bt == null)
			{
				LoadNavbar();
				ViewData["ExceptionMessage"] = "Bài tập không tồn tại";
				return View("ExceptionPage");
			}

			int courseid = new MucRepository().GetMucById((int)bt.id_muc).id_lop_hoc;
            // User có quyền truy cập vào lớp hay không


            int? role = HttpContext.Session.GetInt32("role");

            if (!checkPreviledge((int)courseid))
            {
                LoadNavbar();
                ViewData["ExceptionMessage"] = "Bạn không có quyền truy cập lớp học này";
                return View("ExceptionPage");
            }

            DataTable courses = SQLExecutor.ExecuteQuery(
				" SELECT lop_hoc.id_lop_hoc,  DATE_FORMAT(lop_hoc.ngay_bat_dau, '%d/%m/%Y') as ngay_bat_dau,  DATE_FORMAT(lop_hoc.ngay_ket_thuc, '%d/%m/%Y') as ngay_ket_thuc, mon_hoc.id_mon_hoc, mon_hoc.ten_mon_hoc, COUNT(hoc_vien_tham_gia.id_hoc_vien) as so_luong_hoc_vien "
				+ " FROM lop_hoc"
				+ " INNER JOIN mon_hoc ON lop_hoc.id_mon_hoc = mon_hoc.id_mon_hoc " +
				" LEFT JOIN hoc_vien_tham_gia ON lop_hoc.id_lop_hoc = hoc_vien_tham_gia.id_lop_hoc " +
				$" WHERE lop_hoc.id_lop_hoc = {courseid} " +
				" GROUP BY lop_hoc.id_lop_hoc, lop_hoc.ngay_bat_dau, lop_hoc.ngay_ket_thuc, lop_hoc.id_mon_hoc, mon_hoc.ten_mon_hoc; ");

            HttpContext.Session.SetInt32("courseid", courseid);
            HttpContext.Session.SetInt32("assignmentid", (int)assignmentid);


            LoadNavbar();
			if (role == 1)
			{
                // Main section


                // Layout data
                ViewData["class_name"] = $"{courses.Rows[0]["ten_mon_hoc"]} {courses.Rows[0]["id_mon_hoc"].ToString().PadLeft(3, '0')}.{courses.Rows[0]["id_lop_hoc"].ToString().PadLeft(6, '0')}";
                ViewData["state"] = KiemTraTinhTrang((string)courses.Rows[0]["ngay_bat_dau"], (string)courses.Rows[0]["ngay_ket_thuc"]);
                ViewData["student_quantity"] = Convert.ToInt32(courses.Rows[0]["so_luong_hoc_vien"]);
                ViewData["courseid"] = Convert.ToInt32(courses.Rows[0]["id_lop_hoc"]);

                return View("AdministratorAssignmentPage");
			}
			else if (role == 2)
			{
				// Main section


				// Layout data
				ViewData["class_name"] = $"{courses.Rows[0]["ten_mon_hoc"]} {courses.Rows[0]["id_mon_hoc"].ToString().PadLeft(3, '0')}.{courses.Rows[0]["id_lop_hoc"].ToString().PadLeft(6, '0')}";
				ViewData["state"] = KiemTraTinhTrang((string)courses.Rows[0]["ngay_bat_dau"], (string)courses.Rows[0]["ngay_ket_thuc"]);
				ViewData["student_quantity"] = Convert.ToInt32(courses.Rows[0]["so_luong_hoc_vien"]);
				ViewData["courseid"] = Convert.ToInt32(courses.Rows[0]["id_lop_hoc"]);

				return View("LecturerAssignmentPage");
			}
			else if (role== 3)
			{
                // Main section


                // Layout data
                ViewData["class_name"] = $"{courses.Rows[0]["ten_mon_hoc"]} {courses.Rows[0]["id_mon_hoc"].ToString().PadLeft(3, '0')}.{courses.Rows[0]["id_lop_hoc"].ToString().PadLeft(6, '0')}";
                ViewData["state"] = KiemTraTinhTrang((string)courses.Rows[0]["ngay_bat_dau"], (string)courses.Rows[0]["ngay_ket_thuc"]);
                ViewData["student_quantity"] = Convert.ToInt32(courses.Rows[0]["so_luong_hoc_vien"]);
                ViewData["courseid"] = Convert.ToInt32(courses.Rows[0]["id_lop_hoc"]);
            }
			return View("StudentAssignmentPage");

		}
		[HttpPost]
		public IActionResult updateAssignment([FromBody] JsonDocument dataReceived)
		{
			//				th: normalizeString($(`.th`).val()),
			//			thn: normalizeString($(`.thn`).val()),
			//			ten: $(`.assignment - title - input`).val(),
			//			noi_dung: $(`.content - textarea`).val()

			dynamic assignment = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
			int? role = HttpContext.Session.GetInt32("role");
			int? assignmentid = HttpContext.Session.GetInt32("assignmentid");

			if (role == 1 || role == 2) {
				BaiTapModel bt = new BaiTapRepository().GetBaiTapById((int)assignmentid);
				bt.ten = assignment.ten;
				bt.noi_dung = assignment.noi_dung;
				bt.thoi_han = CreateDateTimeFromString(assignment.th);
				bt.thoi_han_nop = CreateDateTimeFromString(assignment.thn);
				return Ok(JsonConvert.SerializeObject(new BaiTapRepository().UpdateBaiTap(bt)));

			} else
			{
				var notFoundResponse = new
				{
					state = false,
					message = "Bạn không có quyền truy cập tài nguyên này"
				};
				return Json(notFoundResponse);
			}
		}
		static DateTime CreateDateTimeFromString(string dateTimeString)
		{
			// Định dạng của chuỗi ngày giờ (ví dụ: "yyyy-MM-dd HH:mm")
			string format = "yyyy-MM-dd HH:mm";

			try
			{
				// Thử tạo đối tượng DateTime từ chuỗi với định dạng cụ thể
				DateTime result = DateTime.ParseExact(dateTimeString, format, System.Globalization.CultureInfo.InvariantCulture);
				return result;
			}
			catch (FormatException)
			{
				Console.WriteLine("Invalid date format");
				return DateTime.MinValue; // Trả về giá trị mặc định nếu có lỗi
			}
		}
        [HttpGet]
        public IActionResult getAssignmentInformation()
        {
            int assignmentid = (int)HttpContext.Session.GetInt32("assignmentid");
            DataTable assignment = SQLExecutor.ExecuteQuery($@"SELECT id_bai_tap, thoi_han_nop, ten, noi_dung, thoi_han, id_giang_vien, id_muc, ngay_dang FROM bai_tap WHERE bai_tap.id_bai_tap = {assignmentid}");
            if (assignment.Rows.Count == 0)
            {
                return BadRequest("Bài tập không tồn tại");
            }
            MucModel muc = new MucRepository().GetMucById((int)assignment.Rows[0]["id_muc"]);
            assignment.Columns.Add("submits", typeof(DataTable));
            assignment.Rows[0]["submits"] = SQLExecutor.ExecuteQuery(
                $@"SELECT bai_nop.id_bai_nop, 
                bai_nop.thoi_gian_nop,
                bai_nop.id_bai_tap, hoc_vien.id_hoc_vien, hoc_vien.ho_ten FROM hoc_vien inner join hoc_vien_tham_gia on hoc_vien.id_hoc_vien = hoc_vien_tham_gia.id_hoc_vien
                LEFT JOIN bai_nop ON bai_nop.id_hoc_vien = hoc_vien.id_hoc_vien and bai_nop.id_bai_tap = {assignmentid} WHERE hoc_vien_tham_gia.id_lop_hoc = {muc.id_lop_hoc};"
                );
            ((DataTable)assignment.Rows[0]["submits"]).Columns.Add("files", typeof(DataTable));


            for (int i = 0; i < ((DataTable)assignment.Rows[0]["submits"]).Rows.Count; i++)
            {
                if (((DataTable)assignment.Rows[0]["submits"]).Rows[i]["id_bai_nop"] != DBNull.Value)
                {
                    ((DataTable)assignment.Rows[0]["submits"]).Rows[i]["files"] = SQLExecutor.ExecuteQuery(
                        $@"SELECT tep_tin_tai_len.* FROM chi_tiet_bai_nop INNER JOIN tep_tin_tai_len on chi_tiet_bai_nop.id_tep_tin_tai_len = tep_tin_tai_len.id_tep_tin_tai_len 
						WHERE chi_tiet_bai_nop.id_bai_nop = {((DataTable)assignment.Rows[0]["submits"]).Rows[i]["id_bai_nop"]}
"
                    );

                }
                else
                {
                    ((DataTable)assignment.Rows[0]["submits"]).Rows[i]["files"] = new DataTable();
                }
            }

            return Ok(JsonConvert.SerializeObject(assignment));
        }
        public IActionResult getAssignmentInformationForStudent()
		{
            int assignmentid = (int)HttpContext.Session.GetInt32("assignmentid");
            int id_hoc_vien = (int)HttpContext.Session.GetInt32("role_id");

            DataTable assignment = SQLExecutor.ExecuteQuery($@"SELECT id_bai_tap, thoi_han_nop, ten, noi_dung, thoi_han, id_giang_vien, id_muc, ngay_dang FROM bai_tap WHERE bai_tap.id_bai_tap = {assignmentid}");

            assignment.Columns.Add("student_submit", typeof(DataRow));

            DataTable studentSubmit = SQLExecutor.ExecuteQuery($@"SELECT * FROM bai_nop WHERE bai_nop.id_hoc_vien = {id_hoc_vien} and  bai_nop.id_bai_tap = {assignmentid};");

            studentSubmit.Columns.Add("files", typeof(DataTable));


            if (studentSubmit.Rows.Count == 0)
            {
                assignment.Rows[0]["student_submit"] = null;
            } else
            {
                assignment.Rows[0]["student_submit"] = studentSubmit.Rows[0];
                int id_bai_nop = (int)studentSubmit.Rows[0]["id_bai_nop"];
                ((DataRow)assignment.Rows[0]["student_submit"])["files"] = SQLExecutor.ExecuteQuery($@"
                    SELECT tep_tin_tai_len.* FROM chi_tiet_bai_nop 
                    inner join tep_tin_tai_len on chi_tiet_bai_nop.id_tep_tin_tai_len = tep_tin_tai_len.id_tep_tin_tai_len where chi_tiet_bai_nop.id_bai_nop = {id_bai_nop}");
            }

            return Ok(JsonConvert.SerializeObject(assignment));
        }
        //public function getAssignmentInformationForStudent()
        //{
        //    if (!session()->has('id_user'))
        //    {
        //        return redirect()->to('/');
        //    }
        //$id_hoc_vien = session()->get("id_role");
        
        //// assignmentid: assignmentid * 1
        //$assignmentid = null;
        //    if (isset($_GET))
        //    {
        //    $assignmentid = $_GET['assignmentid'];
        //    }
        //    else
        //    {
        //        return redirect()->to('/courses');
        //    }
        //    if (!is_numeric($assignmentid))
        //    {
        //        return redirect()->to('/courses');
        //    }
        //$assignmentModel = new BaiTapModel();
        //$assignment = $assignmentModel->executeCustomQuery(
        //    "SELECT id_bai_tap, thoi_han_nop, ten, noi_dung, thoi_han, id_giang_vien, id_muc, ngay_dang FROM bai_tap WHERE bai_tap.id_bai_tap = $assignmentid"
        //);
        //    if (count($assignment) == 0)
        //    {
        //    $result = ["state" => false, "message" => "Đã có lỗi xảy ra!"];
        //        return $this->response->setJSON($result);
        //    }
        //$model = new BaiNopModel();
        //$assignment[0]["student_submit"] = ($model->executeCustomQuery(
        //    "SELECT * FROM bai_nop WHERE bai_nop.id_hoc_vien = $id_hoc_vien and  bai_nop.id_bai_tap = $assignmentid;
        //"));
        //if (count($assignment[0]["student_submit"]) > 0)
        //    {
        //    $assignment[0]["student_submit"] = $assignment[0]["student_submit"][0];
        //    // tep
        //    $model = new chi_tiet_bai_nopModel();
        //    $id_bai_nop = $assignment[0]['student_submit']['id_bai_nop'];
        //    $assignment[0]["student_submit"]["files"] = $model->executeCustomQuery(
        //        "SELECT tep_tin_tai_len.* FROM chi_tiet_bai_nop inner join tep_tin_tai_len on chi_tiet_bai_nop.id_tep_tin_tai_len = tep_tin_tai_len.id_tep_tin_tai_len where chi_tiet_bai_nop.id_bai_nop = $id_bai_nop
        //    ");
        //}
        //    else
        //    {
        //    $assignment[0]["student_submit"] = null;
        //    }


        //    return $this->response->setJSON($assignment[0]);

        //}
        

        //public function getAssignmentInformation()
        //{
        //    if (!session()->has('id_user'))
        //    {
        //        return redirect()->to('/');
        //    }
        //// assignmentid: assignmentid * 1
        //$assignmentid = null;
        //    if (isset($_GET))
        //    {
        //    $assignmentid = $_GET['assignmentid'];
        //    }
        //    else
        //    {
        //        return redirect()->to('/courses');
        //    }
        //    if (!is_numeric($assignmentid))
        //    {
        //        return redirect()->to('/courses');
        //    }
        //$assignmentModel = new BaiTapModel();
        //$assignment = $assignmentModel->executeCustomQuery(
        //    "SELECT id_bai_tap, thoi_han_nop, ten, noi_dung, thoi_han, id_giang_vien, id_muc, ngay_dang FROM bai_tap WHERE bai_tap.id_bai_tap = $assignmentid"
        //);
        //    if (count($assignment) == 0)
        //    {
        //    $result = ["state" => false, "message" => "Đã có lỗi xảy ra!"];
        //        return $this->response->setJSON($result);
        //    }
        //$muc = (new MucModel())->getMucById($assignment[0]["id_muc"]);
        //$model = new BaiNopModel();
        //$assignment[0]["submits"] = $model->executeCustomQuery(
        //    "SELECT bai_nop.id_bai_nop, 
        //        bai_nop.thoi_gian_nop,
        //        bai_nop.id_bai_tap, hoc_vien.id_hoc_vien, hoc_vien.ho_ten FROM hoc_vien inner join hoc_vien_tham_gia on hoc_vien.id_hoc_vien = hoc_vien_tham_gia.id_hoc_vien
        //        LEFT JOIN bai_nop ON bai_nop.id_hoc_vien = hoc_vien.id_hoc_vien and bai_nop.id_bai_tap = $assignmentid WHERE hoc_vien_tham_gia.id_lop_hoc = $muc->id_lop_hoc; "
        //);

        //$model = new chi_tiet_bai_nopModel();
        //    for ($i = 0; $i < count($assignment[0]["submits"]); $i++) {
        //        if ($assignment[0]['submits'][$i]['id_bai_nop'] != null) {
        //        $assignment[0]["submits"][$i]["files"] = $model->executeCustomQuery(
        //            "SELECT tep_tin_tai_len.* FROM chi_tiet_bai_nop INNER JOIN tep_tin_tai_len on chi_tiet_bai_nop.id_tep_tin_tai_len = tep_tin_tai_len.id_tep_tin_tai_len WHERE chi_tiet_bai_nop.id_bai_nop = {$assignment[0]['submits'][$i]['id_bai_nop']}
        //                "
        //        );
        //        } else
        //        {
        //        $assignment[0]["submits"][$i]["files"] = [];
        //        }
        //    }
        //    return $this->response->setJSON($assignment[0]);

        //}

        public string KiemTraTinhTrang(string ngayBatDau, string ngayKetThuc)
		{
			DateTime datetimeBatDau = DateTime.ParseExact(ngayBatDau, "d/M/yyyy", CultureInfo.InvariantCulture);
			DateTime datetimeKetThuc = DateTime.ParseExact(ngayKetThuc, "d/M/yyyy", CultureInfo.InvariantCulture);
			DateTime datetimeHienTai = DateTime.Now;

			datetimeBatDau = datetimeBatDau.Date;
			datetimeKetThuc = datetimeKetThuc.Date.Add(new TimeSpan(23, 59, 59)); // Set time to end of the day

			if (datetimeBatDau <= datetimeHienTai && datetimeKetThuc >= datetimeHienTai)
			{
				return "<span class=\"class__item--inprocess\">Đang diễn ra</span>";
			}
			else if (datetimeKetThuc < datetimeHienTai)
			{
				return "<span class=\"class__item--over\">Đã kết thúc</span>";
			}
			else
			{
				return "<span class=\"class__item--upcoming\">Sắp diễn ra</span>";
			}
		}
	}
}
